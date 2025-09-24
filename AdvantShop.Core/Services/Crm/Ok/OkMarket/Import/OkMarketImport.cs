using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.ChangeHistories;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Import
{
    public class OkMarketImport
    {
        private readonly OkMarketApiService _apiService;
        private List<Warehouse> _warehouses;
        private const string ModifiedBy = "Ok Import";

        public OkMarketImport()
        {
            _apiService = new OkMarketApiService();
        }

        public void Process()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                OkMarketImportState.Start();
                _process();
                OkMarketImportState.Stop();
            });
        }

        public void _process()
        {
            try
            {
                _warehouses = WarehouseService.GetList();
                var productIds = _apiService.GetProductIdsByGroup();
                OkImportProgress.Start(productIds.Count);
                foreach (var productId in productIds)
                {
                    var product = _apiService.GetProductsByIds(productId.TryParseLong()).FirstOrDefault();
                    if (product == null)
                    {
                        OkMarketImportState.WriteLog("Неудалось получить информацию о продукте - " + productId);
                        break;
                    }
                    var productModel = new OkMarketProductModel(product);

                    var currency = CurrencyService.GetCurrencyByIso3(productModel.CurrencyName) ?? SettingsCatalog.DefaultCurrency;

                    var catalog = _apiService.GetCatalog(productModel.OkCatalogId);
                    int categoryId = 0;
                    if (catalog != null)
                        categoryId = GetOrAddCategory(catalog);
                    else
                        categoryId = CategoryService.DefaultNonCategoryId;
                    var dbProduct = OkMarketService.GetProductByOkId(productModel.OkProductId);
                    if (dbProduct != null)
                    {
                        productModel.ProductId = dbProduct.ProductId;
                        var productToUpdate = ProductService.GetProduct(productModel.ProductId);
                        if (productToUpdate == null)
                        {
                            OkMarketService.DeleteProduct(productModel.ProductId, productModel.OkProductId);
                            AddProduct(productModel, categoryId, productModel.OkCatalogId, currency);
                        }
                        //else update?
                    }
                    else
                    {
                        AddProduct(productModel, categoryId, productModel.OkCatalogId, currency);
                    }
                    OkImportProgress.Inc();
                }
                
                LuceneSearch.CreateAllIndexInBackground();
                ProductService.PreCalcProductParamsMassInBackground();
                CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
            }
            catch (Exception ex)
            {
                OkMarketImportState.WriteLog("Unexpected error on OK import");
                Debug.Log.Error("Unexpected error on OK import", ex);
            }
        }

        public int GetOrAddCategory(OkMarketCatalog catalog)
        {
            var OKCategory = OkMarketService.GetCatalogByOkCatalogId(catalog.OkCatalogId);
            if (OKCategory != null)
            {
                var categories = OkMarketService.GetLinkedCategories(OKCategory.Id).ToList();
                if (categories != null && categories.Count > 0)
                    return categories[0].CategoryId;
            }
            else
            {
                OKCategory = new OkMarketCatalog()
                {
                    Name = catalog.Name,
                    OkCatalogId = catalog.OkCatalogId
                };
                OkMarketService.AddCatalog(OKCategory);
            }

            var category = new Category()
            {
                Name = catalog.Name,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, catalog.Name.Trim()),
                Enabled = true,
                DisplayStyle = ECategoryDisplayStyle.Tile,
                Sorting = ESortOrder.NoSorting,
                SortOrder = 0,
                ModifiedBy = ModifiedBy
            };
            CategoryService.AddCategory(category, true, trackChanges: true, changedBy: new ChangedBy(ModifiedBy));

            OkMarketService.AddCatalogLink(category.ID, OKCategory.Id);
            return category.ID;
        }

        public void AddProduct(OkMarketProductModel product, int categoryId, long OKCatalogId, Currency currency)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                ArtNo = null,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, product.Name),
                CurrencyID = currency.CurrencyId,
                Multiplicity = 1,
                Enabled = product.Enabled,
                Meta = null,
                ModifiedBy = ModifiedBy,
                Description = product.Description
            };

            newProduct.Offers.Add(new Offer
            {
                ArtNo = null,
                // Amount = 1,
                Main = true,
                BasePrice = product.Price
            });

            var productId = ProductService.AddProduct(newProduct, false, true, new ChangedBy(ModifiedBy));

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
                ProductService.SetProductHierarchicallyEnabled(productId);
            }

            OkMarketService.AddProduct(new OkMarketProductModel
            {
                ProductId = productId,
                OkProductId = product.OkProductId,
                OkCatalogId = OKCatalogId,
                OkPhotoIdsList = product.OkPhotoIdsList
            });

            if (product.OkPhotoIdsList != null)
            {
                foreach (var photoId in product.OkPhotoIdsList)
                {
                    var url = _apiService.GetPhotoUrl(photoId);
                    if (url != null)
                    {
                        AddPhoto(productId, url);
                    }
                }
            }
        }

        public void AddPhoto(int productId, string photoUrl)
        {
            try
            {
                var photo = new Photo(0, productId, PhotoType.Product) { OriginName = photoUrl };
                var photoName = PhotoService.AddPhoto(photo);
                var tempImagePath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                try
                {
                    if (!string.IsNullOrWhiteSpace(photoName))
                    {
                        if (FileHelpers.DownloadRemoteImageFile(photoUrl, tempImagePath))
                        {
                            using (var image = Image.FromFile(tempImagePath))
                            {
                                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                                {
                                    photoName += ".jpeg";
                                    photo.PhotoName = photoName;
                                    PhotoService.UpdatePhotoName(photo);
                                }
                                else if (ImageFormat.Png.Equals(image.RawFormat))
                                {
                                    photoName += ".png";
                                    photo.PhotoName = photoName;
                                    PhotoService.UpdatePhotoName(photo);
                                }
                                else
                                {
                                    OkMarketImportState.WriteLog("Неудалось определить тип фотографии продукта - " + productId);
                                    return;
                                }
                                FileHelpers.SaveProductImageUseCompress(photoName, image);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    PhotoService.DeleteProductPhoto(photo.PhotoId);
                }
                finally
                {
                    FileHelpers.DeleteFile(tempImagePath);
                }
            }
            catch (Exception ex)
            {
                OkMarketImportState.WriteLog("Unexpected error on import Photo of product Id: " + productId);
                Debug.Log.Error("Unexpected error on import Photo of product Id: " + productId, ex);
            }
        }
    }
}