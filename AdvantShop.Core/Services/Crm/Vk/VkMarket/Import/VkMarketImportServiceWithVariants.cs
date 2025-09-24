using System;
using System.Collections.Generic;
using System.Drawing;
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
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using JetBrains.Annotations;
using VkNet;
using VkNet.Enums;
using VkNet.Model;
using VkNet.Model.Attachments;
using Image = System.Drawing.Image;
using Photo = AdvantShop.Catalog.Photo;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Import
{
    public class VkMarketImportServiceWithVariants
    {
        private readonly bool _createImVkRelations;
        private readonly VkCategoryService _categoryService;
        private readonly VkProductService _productService;
        private readonly VkMarketApiService _apiService;
        private List<ProductVkItemMap> _productVariantsMap = new List<ProductVkItemMap>();
        private List<string> _productPhotosAdded = new List<string>();
        private List<MarketAlbum> _albums;
        private List<Warehouse> _warehouses;

        private const string ModifiedBy = "Vk Import";

        private class ProductVkItemMap
        {
            public int ProductId { get; }
            public long ItemId { get; }

            public ProductVkItemMap(int productId, long itemId)
            {
                ProductId = productId;
                ItemId = itemId;
            }
        }

        public VkMarketImportServiceWithVariants(bool createImVkRelations)
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
                _albums = _apiService.GetAllAlbums(vk);
                _warehouses = WarehouseService.GetList();

                VkProgress.Set(2);
                
                var groupId = -SettingsVk.Group.Id;
                var currencyId = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3).CurrencyId;
                
                // 1. Get all products with main variant.
                // 2. Save product to db (or get if exist) and store relation new_product_id and additional variants.
                // 3. Get additional variants by itemIds (if it needs).
                // 4. Save them to db (if it needs).

                AddProducts(vk, groupId, currencyId);
                VkProgress.Inc();
                
                var itemIds = _productVariantsMap.Select(x => x.ItemId).Distinct().ToList();

                foreach (var product in _apiService.GetProductsByIds(vk, groupId, itemIds))
                {
                    AddOffer(product);
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

        private void AddProducts(VkApi vk, long groupId, int currencyId, MarketAlbum album = null)
        {
            try
            {
                var sortOrder = 0;

                foreach (var product in _apiService.GetProductsWithVariants(vk, groupId, album?.Id))
                {
                    var productId = AddProduct(product, currencyId, sortOrder);
                    if (productId == 0)
                        continue;

                    sortOrder += 10;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("VkMarket.ImportProducts Альбом: " + album?.Title, ex);
            }
        }

        private int GetOrAddCategory(long albumId, int sortOrder)
        {
            var album = _albums.Find(x => x.Id == albumId);
            if (album == null)
                return 0;
            
            var vkCategoryImport = _categoryService.GetCategoryImport(albumId);
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
                    AddPhoto(category.CategoryId, photoUrl.ToString(), PhotoType.CategorySmall, null);
                }
                else if (album.Photo.Sizes != null && album.Photo.Sizes.Count > 0)
                {
                    var size = album.Photo.Sizes.Where(x => x.Url != null)
                        .OrderByDescending(x => x.Height)
                        .ThenByDescending(x => x.Width)
                        .FirstOrDefault();
                
                    if (size != null && size.Url != null)
                        AddPhoto(category.CategoryId,  size.Url.ToString(), PhotoType.CategorySmall, null);
                }
            }
            
            _categoryService.AddCategoryImport(category.CategoryId, albumId);
            
            return category.CategoryId;
        }

        private int AddProduct(VkMarketProduct market, int currencyId, int sortOrder)
        {
            if (market.Id == null)
                return 0;

            var p = _productService.Get(market.Id.Value);

            if (p != null) // update?
                return p.ProductId;

            var description = market.Description.Replace("\n", "<br/> ");

            var product = new Product()
            {
                Name = market.Title,
                Description = description,
                BriefDescription = description,
                ArtNo = !string.IsNullOrEmpty(market.Sku) && ProductService.GetProductId(market.Sku) == 0
                    ? market.Sku
                    : null,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, market.Title),
                CurrencyID = currencyId,
                Multiplicity = 1,
                Offers = new List<Offer>(),
                Enabled = true,
                Meta = null,
                ModifiedBy = ModifiedBy
            };

            var colorId = TryGetColorSizeId(market.ProductPropertyValues, true);
            var sizeId = TryGetColorSizeId(market.ProductPropertyValues, false);
            
            product.Offers.Add(new Offer()
            {
                ArtNo = !string.IsNullOrEmpty(market.Sku) && OfferService.GetOffer(market.Sku) == null
                    ? market.Sku
                    : null,
                // Amount = 1,
                BasePrice = (market.Price.Amount ?? 0L) / 100f,
                Main = true,
                ColorID = colorId,
                SizeID = sizeId,
                Width = market.Dimensions?.Width,
                Height = market.Dimensions?.Height,
                Length = market.Dimensions?.Length
            });

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

            var index = 0;
            foreach (var marketAlbumId in market.AlbumIds)
            {
                var categoryId = GetOrAddCategory(marketAlbumId, sortOrder);
                if (categoryId == 0)
                    continue;
                
                ProductService.AddProductLink(productId, categoryId, 0, true, index == 0);
                index++;
            }
            
            ProductService.SetProductHierarchicallyEnabled(product.ProductId);

            var photos = market.Photos;
            var hasPhoto = photos != null && photos.Count > 0;
            if (hasPhoto)
            {
                foreach (var photo in photos)
                    AddPhoto(photo, productId, colorId);
            }
            
            // сохраняем в список айдишники других модификаций этого товара
            if (market.Variants != null)
            {
                foreach (var variant in market.Variants.Where(x =>
                             x.Availability == ProductAvailability.Available &&
                             x.ItemId != market.Id))
                {
                    _productVariantsMap.Add(new ProductVkItemMap(productId, variant.ItemId));
                }
            }

            if (_createImVkRelations)
            {
                var offer = OfferService.GetProductOffers(productId).FirstOrDefault();
                _productService.Add(new VkProduct()
                {
                    Id = market.Id ?? 0,
                    ProductId = productId,
                    OfferId = offer?.OfferId ?? 0,
                    AlbumId =  market.AlbumIds.FirstOrDefault(),
                    MainPhotoId = hasPhoto && photos[0].Id != null ? photos[0].Id.Value : 0,
                    PhotoIdsList = hasPhoto ? photos.Where(x => x.Id != null).Select(x => x.Id.Value) : null
                });
            }

            return product.ProductId;
        }

        private void AddPhoto(VkNet.Model.Attachments.Photo photo, int productId, int? colorId)
        {
            string photoLink = null;
            
            var photoUrl = photo.BigPhotoSrc ?? photo.Photo1280 ?? photo.Photo807 ?? photo.Photo130;
            if (photoUrl != null)
            {
                photoLink = photoUrl.ToString();
            }
            else if (photo.Sizes != null && photo.Sizes.Count > 0)
            {
                var size = photo.Sizes.Where(x => x.Url != null)
                    .OrderByDescending(x => x.Height)
                    .ThenByDescending(x => x.Width)
                    .FirstOrDefault();
                
                if (size != null && size.Url != null)
                    photoLink = size.Url.ToString();
            }
            
            if (photoLink == null || _productPhotosAdded.Contains(photoLink))
                return;

            if (AddPhoto(productId, photoLink, PhotoType.Product, colorId))
                _productPhotosAdded.Add(photoLink);
        }

        private bool AddPhoto(int objId, string fileLink, PhotoType type, int? colorId)
        {
            try
            {
                var originName = fileLink.Split('?')[0];
                var photo = new Photo(0, objId, type) { OriginName = originName, ColorID = colorId};
                var photoName = PhotoService.AddPhoto(photo);
                var photoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                if (string.IsNullOrWhiteSpace(photoName))
                    return false;

                if (!FileHelpers.DownloadRemoteImageFile(fileLink, photoFullName))
                    return false;
                
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
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

        private void AddOffer(VkMarketProduct market)
        {
            if (market.Id == null)
                return;

            var map = _productVariantsMap.Find(x => x.ItemId == market.Id);
            if (map == null)
                return;

            var p = ProductService.GetProduct(map.ProductId);
            if (p == null)
                return;

            var colorId = TryGetColorSizeId(market.ProductPropertyValues, true);
            var sizeId = TryGetColorSizeId(market.ProductPropertyValues, false);

            var offer = p.Offers.Find(x => x.ColorID == colorId && x.SizeID == sizeId);
            if (offer != null) // update?
                return;
            
            var productId = p.ProductId;

            offer = new Offer()
            {
                ProductId = productId,
                ArtNo = !string.IsNullOrEmpty(market.Sku) && OfferService.GetOffer(market.Sku) == null
                    ? GetAvailableOfferArto(p, market.Sku)
                    : GetAvailableOfferArto(p, p.ArtNo),
                // Amount = 1,
                BasePrice = (market.Price.Amount ?? 0L) / 100f,
                Main = false,
                ColorID = colorId,
                SizeID = sizeId,
                Width = market.Dimensions?.Width,
                Height = market.Dimensions?.Height,
                Length = market.Dimensions?.Length
            };
            OfferService.AddOffer(offer);

            if (offer.OfferId != 0)
            {
                // поддержание старого поведения, когда не было складов
                if (_warehouses.Count == 1)
                {
                    WarehouseStocksService.AddUpdateStocks(new WarehouseStock
                    {
                        OfferId = offer.OfferId,
                        WarehouseId = _warehouses[0].Id,
                        Quantity = 1,
                    });
                }
            }

            var photos = market.Photos;
            if (photos != null && photos.Count > 0)
            {
                foreach (var photo in photos)
                    AddPhoto(photo, productId, colorId);
            }
        }

        /// <summary>
        /// Получить цвет/размер по названию
        /// </summary>
        /// <param name="productPropertyValues">Свойства (цвет/размер) товара</param>
        /// <param name="isColor">Цвет? Иначе размер</param>
        /// <returns>Nullable colorId/sizeId</returns>
        private int? TryGetColorSizeId(List<VkMarketProductPropertyValue> productPropertyValues, bool isColor)
        {
            if (productPropertyValues == null || productPropertyValues.Count == 0)
                return null;
            
            var propertyName = isColor ? SettingsCatalog.ColorsHeader : SettingsCatalog.SizesHeader;
            
            var property = productPropertyValues.FirstOrDefault(x =>
                x.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            var name = property?.VariantName;
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (isColor)
            {
                var color = ColorService.GetColor(name);
                if (color != null)
                    return color.ColorId;

                return ColorService.AddColor(new AdvantShop.Catalog.Color()
                {
                    ColorName = name,
                    ColorCode = "#ffffff"
                });
            }
            else
            {
                var size = SizeService.GetSize(name);
                if (size != null)
                    return size.SizeId;

                return SizeService.AddSize(new AdvantShop.Catalog.Size()
                {
                    SizeName = name
                });
            }
        }

        private string GetAvailableOfferArto(Product p, string artNo)
        {
            if (OfferService.GetOffer(artNo) == null)
                return artNo;
            
            var count = p.Offers.Count;
            for (int i = 1; i < 10; i++)
            {
                artNo = p.ArtNo + "-" + (count + i);
                if (OfferService.GetOffer(artNo) == null)
                    return artNo;
            }
            return Guid.NewGuid().ToString();
        }
    }
}
