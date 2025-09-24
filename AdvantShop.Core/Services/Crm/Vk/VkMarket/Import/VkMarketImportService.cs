using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using VkNet.Model;
using VkNet.Model.Attachments;
using Image = System.Drawing.Image;
using Photo = AdvantShop.Catalog.Photo;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Import
{
    [Obsolete("use VkMarketImportServiceWithVariants")]
    public class VkMarketImportService
    {
        private readonly bool _createImVkRelations;
        private readonly VkCategoryService _categoryService;
        private readonly VkProductService _productService;
        private readonly VkMarketApiService _apiService;
        private List<Warehouse> _warehouses;

        private const string ModifiedBy = "Vk Import";

        public VkMarketImportService(bool createImVkRelations)
        {
            _createImVkRelations = createImVkRelations;
            _categoryService = new VkCategoryService();
            _productService = new VkProductService();
            _apiService = new VkMarketApiService();
        }

        public void Import()
        {
            System.Threading.Tasks.Task.Run(ImportProducts);
        }

        private void ImportProducts()
        {
            try
            {
                if (VkProgress.IsProcessing())
                    return;
                
                VkProgress.Start();

                var vk = _apiService.Auth();

                var groupId = -SettingsVk.Group.Id;
                var currencyId = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3).CurrencyId;

                var albums = _apiService.GetAllAlbums(vk);
                _warehouses = WarehouseService.GetList();

                VkProgress.Set(albums != null ? albums.Count + 1 : 1);

                if (albums != null && albums.Count > 0)
                {
                    var sortOrder = 0;

                    foreach (var album in albums)
                    {
                        try
                        {
                            var categoryId = GetOrAddCategory(album, sortOrder);

                            foreach (var product in _apiService.GetProducts(vk, groupId, album.Id))
                            {
                                AddProduct(product, categoryId, currencyId, album.Id.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error("VkMarket.ImportProducts Альбом: " + album.Title, ex);
                        }

                        sortOrder += 10;
                        VkProgress.Inc();
                    }
                }

                foreach (var product in _apiService.GetProducts(vk, groupId, null))
                {
                    AddProduct(product, CategoryService.DefaultNonCategoryId, currencyId, 0);
                }

                VkProgress.Inc();

                LuceneSearch.CreateAllIndexInBackground();
                ProductService.PreCalcProductParamsMassInBackground();
                CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            finally
            {
                VkProgress.Stop();
            }
        }

        private int GetOrAddCategory(MarketAlbum album, int sortOrder)
        {
            var vkCategoryImport = _categoryService.GetCategoryImport(album.Id.Value);
            if (vkCategoryImport != null)
                return vkCategoryImport.CategoryId;

            var category = new Category()
            {
                Name = album.Title,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, album.Title.Trim()),
                Enabled = true,
                DisplayStyle = ECategoryDisplayStyle.Tile,
                Sorting = ESortOrder.NoSorting,
                SortOrder = sortOrder,
                ModifiedBy = ModifiedBy
            };
            CategoryService.AddCategory(category, true, trackChanges:true, changedBy:new ChangedBy(ModifiedBy));

            if (album.Photo != null)
            {
                var photoUrl = album.Photo.BigPhotoSrc ?? album.Photo.Photo1280 ?? album.Photo.Photo807 ?? album.Photo.Photo130;
                if (photoUrl != null)
                {
                    AddPhoto(category.CategoryId, photoUrl.ToString(), PhotoType.CategorySmall);
                }
            }
            
            _categoryService.AddCategoryImport(category.CategoryId, album.Id.Value);
            
            return category.CategoryId;
        }

        private void AddProduct(Market market, int categoryId, int currencyId, long albumId)
        {
            if (market.Id == null)
                return;

            var p = _productService.Get(market.Id.Value);

            if (p != null)
            {
                // update?
                return;
            }

            var description = market.Description.Replace("\n", "<br/> ");

            var product = new Product()
            {
                Name = market.Title,
                Description = description,
                BriefDescription = description,
                ArtNo = null,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, market.Title),
                CurrencyID = currencyId,
                Multiplicity = 1,
                Offers = new List<Offer>()
                {
                    new Offer()
                    {
                        ArtNo = null,
                        // Amount = 1,
                        BasePrice = market.Price != null ? (market.Price.Amount ?? 0L)/100f : 0,
                        Main = true,
                    }
                },
                Enabled = true,
                Meta = null,
                ModifiedBy = ModifiedBy
            };

            var productId = ProductService.AddProduct(product, false, true, new ChangedBy(ModifiedBy));

            if (productId != 0)
            {
                // поддержание старого поведения, когда не было складов
                if (_warehouses.Count == 1)
                {
                    foreach (var productOffer in OfferService.GetProductOffers(productId))
                    {
                        WarehouseStocksService.AddUpdateStocks(new WarehouseStock
                        {
                            OfferId = productOffer.OfferId,
                            WarehouseId = _warehouses[0].Id,
                            Quantity = 1,
                        });
                    }
                }
            }

            if (productId != 0 && categoryId != 0 && categoryId != CategoryService.DefaultNonCategoryId)
            {
                ProductService.AddProductLink(productId, categoryId, 0, true);
                ProductService.SetProductHierarchicallyEnabled(product.ProductId);
            }

            var photos = market.Photos;
            var hasPhoto = photos != null && photos.Count > 0;
            if (hasPhoto)
            {
                foreach (var photo in photos)
                {
                    var photoUrl = photo.BigPhotoSrc ?? photo.Photo1280 ?? photo.Photo807 ?? photo.Photo130;
                    if (photoUrl != null)
                    {
                        AddPhoto(productId, photoUrl.ToString(), PhotoType.Product);
                    }
                    else if (photo.Sizes != null && photo.Sizes.Count > 0)
                    {
                        var size = photo.Sizes.Where(x => x.Url != null)
                            .OrderByDescending(x => x.Height)
                            .ThenByDescending(x => x.Width)
                            .FirstOrDefault();
                        
                        if (size != null)
                            AddPhoto(productId, size.Url.ToString(), PhotoType.Product);
                    }
                }
            }

            if (_createImVkRelations)
            {
                _productService.Add(new VkProduct()
                {
                    Id = market.Id ?? 0,
                    ProductId = productId,
                    AlbumId = albumId,
                    MainPhotoId = hasPhoto && photos[0].Id != null ? photos[0].Id.Value : 0,
                    PhotoIdsList = hasPhoto ? photos.Where(x => x.Id != null).Select(x => x.Id.Value) : null
                });
            }
        }

        private bool AddPhoto(int objId, string fileLink, PhotoType type)
        {
            try
            {
                var originName = fileLink.Split('?')[0];
                var photo = new Photo(0, objId, type) { OriginName = originName };
                var photoName = PhotoService.AddPhoto(photo);
                var photoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                if (!string.IsNullOrWhiteSpace(photoName))
                {
                    if (FileHelpers.DownloadRemoteImageFile(fileLink, photoFullName))
                    {
                        using (var image = Image.FromFile(photoFullName))
                        {
                            if (type == PhotoType.Product)
                                FileHelpers.SaveProductImageUseCompress(photoName, image);

                            if (type == PhotoType.CategorySmall)
                                FileHelpers.SaveResizePhotoFile(
                                    FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Small, photoName),
                                    SettingsPictureSize.SmallCategoryImageWidth,
                                    SettingsPictureSize.SmallCategoryImageHeight,
                                    image);
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

    }
}
