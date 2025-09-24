using System;
using System.IO;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class CopyProduct
    {
        private readonly int _productId;
        private readonly string _name;

        public CopyProduct(int productId, string name)
        {
            _productId = productId;
            _name = name;
        }

        public int Execute()
        {
            var sourceProduct = ProductService.GetProduct(_productId);
            if (sourceProduct == null)
                throw new BlException("Товар не найден");

            var name = _name.Replace("#PRODUCT_NAME#", sourceProduct.Name);

            var meta = MetaInfoService.GetMetaInfo(sourceProduct.ProductId, MetaType.Product);
            if (meta != null)
                meta.ObjId = 0;

            var product = new Product()
            {
                ProductId = 0,
                ArtNo = "",
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, StringHelper.Translit(name ?? "")),

                Name = name ?? "",
                BriefDescription = sourceProduct.BriefDescription,
                Description = sourceProduct.Description,

                Discount = new Discount(sourceProduct.Discount.Percent, sourceProduct.Discount.Amount, sourceProduct.Discount.Type),
                DoNotApplyOtherDiscounts = sourceProduct.DoNotApplyOtherDiscounts,
                ShippingPrice = sourceProduct.ShippingPrice,
                UnitId = sourceProduct.UnitId,
                Multiplicity = sourceProduct.Multiplicity,
                MaxAmount = sourceProduct.MaxAmount,
                MinAmount = sourceProduct.MinAmount,

                Enabled = sourceProduct.Enabled,
                AllowPreOrder = sourceProduct.AllowPreOrder,

                BestSeller = sourceProduct.BestSeller,
                Recomended = sourceProduct.Recomended,
                New = sourceProduct.New,
                OnSale = sourceProduct.OnSale,
                BrandId = sourceProduct.BrandId,
                
                ExportOptions = sourceProduct.ExportOptions,

                HasMultiOffer = sourceProduct.HasMultiOffer,

                TaxId = sourceProduct.TaxId,
                PaymentMethodType = sourceProduct.PaymentMethodType,
                PaymentSubjectType = sourceProduct.PaymentSubjectType,

                CurrencyID = sourceProduct.CurrencyID,
                Meta = meta,
                ModifiedBy = CustomerContext.CustomerId.ToString(),
                
                ActiveView360 = sourceProduct.ActiveView360,

                IsMarkingRequired = sourceProduct.IsMarkingRequired,
            };

            if (sourceProduct.IsDigital)
            {
                product.IsDigital = sourceProduct.IsDigital;
                product.DownloadLink = sourceProduct.DownloadLink;
            }

            if (sourceProduct.ManualRatio != null)
            {
                product.ManualRatio = sourceProduct.ManualRatio;
            }

            if (product.ExportOptions != null)
            {
                product.ExportOptions.IsChanged = true;
            }
            
            product.SizeChart = sourceProduct.SizeChart;

            product.ProductId = ProductService.AddProduct(product, true);
            if (product.ProductId == 0)
                throw new BlException("Не удалось добавить товар. " +
                                      (Saas.SaasDataService.IsSaasEnabled
                                          ? "Пожалуйста проверьте лимит товаров на вашем тарифе (" + Saas.SaasDataService.CurrentSaasData.ProductsCount + ")"
                                          : ""));

            var offersCount = sourceProduct.Offers.Count;
            
            var lpService = new LpSiteService();
            var sourceProductLpSite = lpService.GetByAdditionalSalesProductId(sourceProduct.ProductId);
            if (sourceProductLpSite != null)
            {
                lpService.AddAdditionalSalesProduct(product.ProductId, sourceProductLpSite.Id);
            }

            var productGifts = ProductGiftService.GetGifts(sourceProduct.ProductId).ToList();
            
            if (productGifts != null)
            {
                foreach (var gift in productGifts.Where(gift => gift.OfferId == null))
                {
                    ProductGiftService.AddGift(product.ProductId, gift.GiftOfferId, null, gift.ProductCount);
                }
            }
            
            for (int i = 0; i < offersCount; i++)
            {
                var sourceOfferId = sourceProduct.Offers[i].OfferId;
                var offer = sourceProduct.Offers[i];
                offer.ArtNo = product.ArtNo + (offersCount > 1 ? "-" + i : "");
                offer.ProductId = product.ProductId;

                if (OfferService.GetOffer(offer.ArtNo) == null)
                {
                    OfferService.AddOffer(offer);
                }
                else
                {
                    offer.ArtNo += "-" + i;

                    int count = 0;
                    while (count++ < 10)
                    {
                        if (OfferService.GetOffer(offer.ArtNo) == null)
                            break;

                        offer.ArtNo += "_" + i;
                    }
                    OfferService.AddOffer(offer);
                }

                foreach (var sourceStocks in WarehouseStocksService.GetOfferStocks(sourceOfferId))
                    WarehouseStocksService.AddUpdateStocks(new WarehouseStock
                        {
                            OfferId = offer.OfferId,
                            Quantity = sourceStocks.Quantity,
                            WarehouseId = sourceStocks.WarehouseId
                        },
                        calcHasProductsForWarehouse: false);
                
                OfferService.RecalculateOfferAmount(offer.OfferId);
                
                var giftForCurrentOffer = productGifts?.FirstOrDefault(gift => gift.OfferId == sourceOfferId);
                if (giftForCurrentOffer != null) 
                    ProductGiftService.AddGift(product.ProductId, giftForCurrentOffer.GiftOfferId, offer.OfferId, giftForCurrentOffer.ProductCount);
            }

            var categories = ProductService.GetCategoriesByProductId(sourceProduct.ProductId);
            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    ProductService.AddProductLink(product.ProductId, category.CategoryId, 0, true, 
                        mainCat: sourceProduct.MainCategory.CategoryId == category.CategoryId ? true : false,
                        incrementProductsCount: true,
                        useAutomap: false);
                }
                ProductService.SetProductHierarchicallyEnabled(product.ProductId);
                CategoryService.CalculateHasProductsForWarehousesInProductCategories(product.ProductId);
            }

            var propertyValues = PropertyService.GetPropertyValuesByProductId(sourceProduct.ProductId);
            foreach (var propertyValue in propertyValues)
            {
                PropertyService.AddProductProperyValue(propertyValue.PropertyValueId, product.ProductId);
            }

            var photos = PhotoService.GetPhotos(sourceProduct.ProductId, PhotoType.Product).ToList();
            if (photos.Count > 0)
            {
                try
                {
                    foreach (var photo in photos)
                    {
                        if (photo.PhotoName.Contains("http://") || photo.PhotoName.Contains("https://"))
                        {
                            photo.ObjId = product.ProductId;

                            PhotoService.AddPhotoWithOrignName(photo);
                            continue;
                        }


                        var tempPhotoName = PhotoService.AddPhoto(new Photo(0, product.ProductId, PhotoType.Product)
                        {
                            Description = photo.Description,
                            OriginName =
                                !string.IsNullOrWhiteSpace(photo.OriginName) ? photo.OriginName : photo.PhotoName,
                            ColorID = photo.ColorID,
                            Main = photo.Main,
                            PhotoSortOrder = photo.PhotoSortOrder,
                            PhotoNameSize1 = photo.PhotoNameSize1,
                            PhotoNameSize2 = photo.PhotoNameSize2
                        });

                        if (string.IsNullOrWhiteSpace(tempPhotoName)) continue;

                        var photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photo.PhotoName);
                        if (!System.IO.File.Exists(photoPath))
                        {
                            photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photo.PhotoName);
                        }

                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(photoPath))
                        {
                            FileHelpers.SaveProductImageUseCompress(tempPhotoName, image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            
            var photo360 = PhotoService.GetPhotos(sourceProduct.ProductId, PhotoType.Product360).ToList();
            if (photo360.Count > 0)
            {
                var sourceRotateImageDirectoryPath = FoldersHelper.GetRotateImageProductPathAbsolut(sourceProduct.ProductId.ToString());
                var destRotateImageDirectoryPath = FoldersHelper.GetRotateImageProductPathAbsolut(product.ProductId.ToString());

                if (!string.IsNullOrEmpty(destRotateImageDirectoryPath))
                {
                    Directory.CreateDirectory(destRotateImageDirectoryPath);
                    
                    foreach (var photo in photo360)
                    {
                        try
                        {
                            var pathToPhotoSource = sourceRotateImageDirectoryPath + photo.PhotoName;
                            var pathToPhotoDest = destRotateImageDirectoryPath + photo.PhotoName;
                            photo.ObjId = product.ProductId;
                            PhotoService.AddPhoto(photo);
                            File.Copy(pathToPhotoSource, pathToPhotoDest, true);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error(ex);
                        }
                    }
                }
            }

            var videos = ProductVideoService.GetProductVideos(sourceProduct.ProductId);
            foreach (var video in videos)
            {
                video.ProductId = product.ProductId;
                ProductVideoService.AddProductVideo(video);
            }

            var customOptions = CustomOptionsService.GetCustomOptionsByProductId(sourceProduct.ProductId);
            if (customOptions != null)
            {
                foreach (var customOption in customOptions)
                {
                    customOption.ProductId = product.ProductId;
                    CustomOptionsService.AddCustomOption(customOption);
                }
            }

            foreach (var relatedProduct in ProductService.GetAllRelatedProducts(sourceProduct.ProductId, RelatedType.Related))
            {
                ProductService.AddRelatedProduct(product.ProductId, relatedProduct.ProductId, RelatedType.Related);
            }

            foreach (var relatedProduct in ProductService.GetAllRelatedProducts(sourceProduct.ProductId, RelatedType.Alternative))
            {
                ProductService.AddRelatedProduct(product.ProductId, relatedProduct.ProductId, RelatedType.Alternative);
            }

            foreach (var gift in ProductGiftService.GetGiftsByGiftOfferId(sourceProduct.ProductId))
            {
                ProductGiftService.AddGift(gift.ProductId, gift.GiftOfferId, gift.OfferId, gift.ProductCount);
            }

            foreach (var excludeProduct in SalesChannelService.GetExcludedProductSalesChannelList(sourceProduct.ProductId))
            {
                SalesChannelService.SetExcludedProductSalesChannel(excludeProduct, product.ProductId);
            }

            foreach (var tag in TagService.Gets(_productId, ETagType.Product, true))
            {
                TagService.AddMap(product.ProductId, tag.Id, ETagType.Product, tag.SortOrder);
            }

            foreach (var type in AttachedModules.GetModules<IProductCopy>())
            {
                var module = (IProductCopy)Activator.CreateInstance(type);
                module.AfterCopyProduct(sourceProduct, product);
            }

            ProductService.PreCalcProductParams(product.ProductId);
            CategoryService.RecalculateProductsCountInCategories(product.ProductId);
            CategoryService.CalculateHasProductsForWarehousesInProductCategories(product.ProductId);

            var p = ProductService.GetProduct(product.ProductId);
            if (p != null)
                ProductWriter.AddUpdate(p);

            return product.ProductId;
        }
    }
}
