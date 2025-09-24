using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog;
using AdvantShop.Web.Admin.Handlers.Catalog.Products;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Admin.Models.Catalog.Properties;
using AdvantShop.Web.Admin.Models.Catalog.PropertyValues;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class ProductController : BaseAdminController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(string name, int categoryId)
        {
            if (!SettingsCongratulationsDashboard.ProductDone)
            {
                SettingsCongratulationsDashboard.ProductDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_ProductDone);
            }
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ProductCreated);

            return ProcessJsonResult(new AddProduct(name, categoryId));
        }

        public ActionResult Edit(int id)
        {
            var model = new GetProduct(id).Execute();
            if (model == null)
                return Error404();

            SetMetaInformation(T("Admin.Catalog.Index.ProductTitle") + model.Name);
            SetNgController(NgControllers.NgControllersTypes.ProductCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(AdminProductModel model)
        {
            foreach (var key in new List<string>() { "Discount", "Multiplicity", "Weight", "Width", "Length", "Height", "ShippingPrice", "TaxId" })
            {
                if (ModelState.ContainsKey(key))
                    ModelState[key].Errors.Clear();
            }

            if (model.Enabled && SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") > SaasDataService.CurrentSaasData.ProductsCount)
            {
                ModelState.AddModelError("", T("Admin.Catalog.LimitationsOfTariff"));
            }
            else if (ModelState.IsValid)
            {
                var result = new UpdateProduct(model).Execute();
                if (result)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));

                    if (!SettingsCongratulationsDashboard.ProductDone)
                    {
                        SettingsCongratulationsDashboard.ProductDone = true;
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_ProductDone);
                    }

                    // Нельзя использовать RedirectToAction, потому что у модулей вызывается action с POST. При редиректе он теряется.
                    return Edit(model.ProductId);
                }

                ModelState.AddModelError("", T("Admin.Catalog.ErrorWhileSaving"));
            }

            ShowErrorMessages();


            return RedirectToAction("Edit", new { id = model.ProductId });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProduct(int productId)
        {
            ProductService.DeleteProduct(productId, true);
            CategoryService.RecalculateProductsCountManual();

            return Json(new { result = true });
        }

        [HttpGet]
        public JsonResult GetProductLastModified(int productId)
        {
            return Json(new GetProductLastModified(productId).Execute());
        }

        #region Offers

        [HttpGet]
        public JsonResult GetOffers(int productId)
        {
            var countWarehouse = WarehouseService.GetList().Count;
            var offers =
                OfferService.GetProductOffers(productId)
                            .Select(offer =>
                             {
                                 var adminOfferModel = new AdminOfferModel
                                 {
                                     OfferId = offer.OfferId,
                                     ProductId = offer.ProductId,
                                     ArtNo = offer.ArtNo,
                                     BasePrice = offer.BasePrice,
                                     SupplyPrice = offer.SupplyPrice,
                                     Amount = offer.Amount,
                                     ColorId = offer.ColorID,
                                     Color = offer.Color?.ToString() ?? string.Empty,
                                     SizeId = offer.SizeID,
                                     Size = offer.SizeForCategory?.GetFullName() ?? string.Empty,
                                     Main = offer.Main,
                                     Weight = offer.Weight,
                                     Width = offer.Width,
                                     Height = offer.Height,
                                     Length = offer.Length,
                                     BarCode = offer.BarCode,
                                     OfferPriceRules = PriceRuleService.GetOfferPriceRuleModels(offer.OfferId),
                                     UsedWarehouses = WarehouseStocksService.GetOfferStocks(offer.OfferId).Count,
                                     CountWarehouses = countWarehouse
                                 };
                                 return adminOfferModel;
                             });
            return Json(new AdvantShop.Web.Infrastructure.Admin.FilterResult<AdminOfferModel>
                {DataItems = offers.ToList()});
        }

        [HttpGet]
        public JsonResult GetOffer(int offerId)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return Json(null);

            return Json(new AdminOfferModel
            {
                OfferId = offer.OfferId,
                ProductId = offer.ProductId,
                ArtNo = offer.ArtNo,
                BasePrice = offer.BasePrice,
                SupplyPrice = offer.SupplyPrice,
                Amount = offer.Amount,
                ColorId = offer.ColorID,
                SizeId = offer.SizeID,
                Main = offer.Main,
                Weight = offer.Weight,
                Width = offer.Width,
                Height = offer.Height,
                Length = offer.Length,
                BarCode = offer.BarCode,
                OfferPriceRules = PriceRuleService.GetOfferPriceRuleModels(offer.OfferId),
                UsedWarehouses = WarehouseStocksService.GetOfferStocks(offer.OfferId).Count,
                CountWarehouses = WarehouseService.GetList().Count
            });
        }

        [HttpGet]
        public JsonResult GetOffersValidation(int productId)
        {
            var error = "";
            var offers = OfferService.GetProductOffers(productId);

            if (offers.Any(o => o.ColorID != null) && offers.Any(o => o.ColorID == null))
            {
                error = T("Admin.Product.UpdateOffer.AllColorIsNotNull");
            }

            if (offers.Any(o => o.SizeID != null) && offers.Any(o => o.SizeID == null))
            {
                error = T("Admin.Product.UpdateOffer.AllSizeIsNotNull");
            }

            if (offers.GroupBy(x => new { x.SizeID, x.ColorID }).Any(x => x.Count() > 1))
            {
                error = T("Admin.Product.UpdateOffer.Duplicate");
            }

            return Json(new CommandResult() { Result = error == "", Error = error });
        }

        [HttpGet]
        public JsonResult GetAvailableArtNo(int productId)
        {
            var p = ProductService.GetProduct(productId);
            if (p == null)
                return Json(Guid.NewGuid().ToString());

            var lastOffer = p.Offers.OrderBy(x => x.OfferId).LastOrDefault();
            if (lastOffer != null)
            {
                var arr = lastOffer.ArtNo.Split('-');
                if (arr.Length > 1)
                {
                    var lastPart = arr[arr.Length - 1];
                    var num = lastPart.TryParseInt();
                    if (num != 0)
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            arr[arr.Length - 1] = (num + i).ToString();
                            
                            var artNo = string.Join("-", arr);

                            if (OfferService.GetOffer(artNo) == null)
                                return Json(artNo);
                        }
                    }
                }
            }
            
            var count = p.Offers.Count;
            for (int i = 1; i < 10; i++)
            {
                var artNo = p.ArtNo + "-" + (count + i);
                
                if (OfferService.GetOffer(artNo) == null)
                    return Json(artNo);
            }
            
            return Json(Guid.NewGuid().ToString());
        }

        [HttpGet]
        public JsonResult GetProductInfoForOffer(int productId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return JsonError();

            var offer = OfferService.GetProductOffers(productId).OrderByDescending(x => x.Main).FirstOrDefault();

            return Json(new
            {
                BasePrice = offer?.BasePrice ?? 0,
                SupplyPrice = offer?.SupplyPrice ?? 0,
                Weight = offer != null ? offer.Weight : 0,
                Width = offer != null ? offer.Width : 0,
                Height = offer != null ? offer.Height : 0,
                Length = offer != null ? offer.Length : 0,
                BarCode = offer != null ? offer.BarCode : string.Empty,
                OfferPriceRules = 
                    !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes
                        ? PriceRuleService.GetList().Select(x => new OfferPriceRule(){Name = x.Name, PriceRuleId = x.Id}).ToList()
                        : null,
                CountWarehouses = WarehouseService.GetList().Count
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddOffer(AdminOfferModel offer)
        {
            return ProcessJsonResult(new AddUpdateOffer(offer));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateOffer(AdminOfferModel offer)
        {
            return ProcessJsonResult(new AddUpdateOffer(offer));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteOffer(int offerId)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return Json(new { result = false });

            if (offer.Main)
            {
                var newMainOffer = OfferService.GetProductOffers(offer.ProductId).FirstOrDefault(x => !x.Main && x.OfferId != offer.OfferId);
                if (newMainOffer != null)
                {
                    newMainOffer.Main = true;
                    OfferService.UpdateOffer(newMainOffer);
                }
            }

            OfferService.DeleteOffer(offerId, true);

            ProductService.PreCalcProductParams(offer.ProductId);
            CategoryService.CalculateHasProductsForWarehousesInProductCategories(offer.ProductId);

            return JsonOk();
        }

        #region Offer stocks

        [HttpGet]
        public JsonResult GetOfferStocks(int offerId)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return JsonError("Offer not found");

            var offerStocks = WarehouseStocksService.GetOfferStocks(offerId);
            return JsonOk(offerStocks.Select(_ => new OfferStockModel
            {
                OfferId = _.OfferId,
                WarehouseId = _.WarehouseId,
                Quantity = _.Quantity
            }));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveOfferStocks(List<OfferStockModel> stockModels)
        {
            if (stockModels is null || stockModels.Count == 0)
                return JsonError("Stocks is required");

            stockModels.ForEach(stockModel =>
                WarehouseStocksService.AddUpdateStocks(
                    new WarehouseStock
                    {
                        OfferId = stockModel.OfferId,
                        WarehouseId = stockModel.WarehouseId,
                        Quantity = stockModel.Quantity
                    },
                    trackChanges: true,
                    calcHasProductsForWarehouse: false));
            
            var firstStockModel = stockModels.FirstOrDefault();
            if (firstStockModel != null)
            {
                var o = OfferService.GetOffer(firstStockModel.OfferId);
                if (o != null)
                {
                    ProductService.PreCalcProductParams(o.ProductId);
                    CategoryService.CalculateHasProductsForWarehousesInProductCategories(o.ProductId);
                    CategoryService.RecalculateProductsCountInCategories(o.ProductId);
                }
            }

            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetDataForOfferStocks(int offerId)
        {
            return JsonOk(new
            {
                Warehouses = WarehouseService.GetList().Select(_ => new {_.Id, _.Name})
            });
        }

        #endregion

        [HttpGet]
        public JsonResult GetColors()
        {
            var colors = ColorService.GetAllColors().Select(x => new SelectItemModel(x.ColorName, x.ColorId));
            return Json(colors);
        }

        [HttpGet]
        public JsonResult GetSizes(int? categoryId = null, int? productId = null)
        {
            if (!categoryId.HasValue && productId.HasValue)
                categoryId = ProductService.GetFirstCategoryIdByProductId(productId.Value);
            
            if (categoryId.HasValue)
                return Json(SizeService.GetAllSizesForCategory(categoryId.Value).Select(x => new SelectItemModel(x.GetFullName(), x.SizeId)));
            
            return Json(SizeService.GetAllSizes().Select(x => new SelectItemModel(x.ToString(), x.SizeId)));
        }

        #endregion

        #region Photos

        [HttpGet]
        public JsonResult GetPhotos(int productId)
        {
            var photos = PhotoService.GetPhotos<ProductPhoto>(productId, PhotoType.Product).Select(x => new
            {
                x.PhotoId,
                x.Description,
                x.Main,
                x.PhotoSortOrder,
                x.PhotoName,
                ImageSrc = x.ImageSrcSmall(),
                ColorId = x.ColorID ?? 0,
                PhotoCategoryId = x.PhotoCategoryId,
            });
            return Json(photos);
        }

        [HttpGet]
        public JsonResult GetPhoto(int photoId)
        {
            var photo = PhotoService.GetPhoto<ProductPhoto>(photoId, PhotoType.Product);

            return Json(new
            {
                photo.PhotoId,
                photo.Description,
                photo.Main,
                photo.PhotoSortOrder,
                photo.PhotoName,
                ImageSrc = photo.ImageSrcSmall(),
                ColorId = photo.ColorID ?? 0
            });
        }

        [HttpGet]
        public JsonResult GetPhotoColors(int productId)
        {
            var colors = SQLDataAccess.Query<Color>(
                "Select Color.ColorID, ColorName From Catalog.Photo inner join Catalog.Color on Color.ColorID=Photo.ColorID where objId=@productId and type='Product' " +
                "union " +
                "Select Color.ColorID, ColorName From Catalog.Color inner join catalog.Offer on offer.ColorID=Color.Colorid where productid=@productId " +
                "Order by ColorName", new { productId }).ToList();

            colors.Insert(0, new Color() { ColorName = T("Admin.Catalog.NoColor") });

            return Json(colors.Select(x => new { value = x.ColorId, label = x.ColorName }));
        }

        [HttpGet]
        public JsonResult GetPhotoCategories()
        {
            var photoCategories = PhotoCategoryService.GetAllPhotoCategories();

            return Json(photoCategories.Select(x => new { value = x.Id, label = x.Name }));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePhoto(int photoId)
        {
            var photo = PhotoService.GetPhoto(photoId);
            if (photo != null && photo.Type == PhotoType.Product)
            {
                PhotoService.DeleteProductPhoto(photoId);
                ProductService.PreCalcProductParams(photo.ObjId);
                return Json(true);
            }
            return Json(false);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePhotos(List<int> photoIds)
        {
            if (photoIds == null || photoIds.Count == 0) 
                return Json(false);
            
            var productId = 0;
            foreach (var photoId in photoIds)
            {
                var photo = PhotoService.GetPhoto(photoId);
                if (photo != null && photo.Type == PhotoType.Product)
                {
                    PhotoService.DeleteProductPhoto(photoId);
                    productId = photo.ObjId;
                }
            }
            ProductService.PreCalcProductParams(productId);
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPhotos(List<HttpPostedFileBase> files, int productId)
        {
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddPhoto_File);
            return Json(new UploadProductPictures(files, productId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPictureByLink(int productId, string fileLink)
        {
            if (string.IsNullOrWhiteSpace(fileLink) || productId == 0)
                return Json(false);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddPhoto_ByUrl);
            return Json(new UploadProductPicturesByLink(productId, fileLink).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPicturesByLink(int objId, List<string> fileLinks)
        {
            if (fileLinks == null || fileLinks.Count == 0 || objId == 0)
                return JsonError();

            var errors = new List<string>();

            foreach (var fileLink in fileLinks)
            {
                var result = new UploadProductPicturesByLink(objId, fileLink).Execute();
                if (!string.IsNullOrEmpty(result.Error))
                    errors.Add(result.Error);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddPhoto_Bing);

            return errors.Count == 0 ? JsonOk() : JsonError(String.Join(";<br>", errors));
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditPhoto(int photoId, string alt, int? colorId)
        {
            var photo = PhotoService.GetPhoto(photoId);
            if (photo != null)
            {
                photo.Description = alt;
                photo.ColorID = colorId != 0 ? colorId : default(int?);

                PhotoService.UpdatePhoto(photo);
            }

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePhotoColor(int photoId, int colorId)
        {
            var photo = PhotoService.GetPhoto(photoId);
            if (photo != null && photo.Type == PhotoType.Product)
            {
                photo.ColorID = colorId != 0 ? colorId : default(int?);

                PhotoService.UpdatePhoto(photo);
                ProductService.PreCalcProductParams(photo.ObjId);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePhotoCategory(int photoId, int? photoCategoryId)
        {
            var photo = PhotoService.GetPhoto(photoId);
            if (photo != null && photo.Type == PhotoType.Product)
            {
                photo.PhotoCategoryId = photoCategoryId;

                PhotoService.UpdatePhoto(photo);
                ProductService.PreCalcProductParams(photo.ObjId);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeMainPhoto(int photoId)
        {
            var photo = PhotoService.GetPhoto(photoId);
            if (photo != null && photo.Type == PhotoType.Product)
            {
                PhotoService.SetProductMainPhoto(photoId);
                ProductService.PreCalcProductParams(photo.ObjId);
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePhotoSortOrder(int productId, int photoId, int? prevPhotoId, int? nextPhotoId)
        {
            return Json(new ChangeProductPhotoSortOrder(productId, photoId, prevPhotoId, nextPhotoId).Execute());
        }

        #endregion

        #region Photos 360

        [HttpGet]
        public JsonResult GetPhotos360(int productId)
        {
            var photos = PhotoService.GetPhotos<ProductPhoto>(productId, PhotoType.Product360).Select(x => new
            {
                x.PhotoId,
                x.Description,
                x.Main,
                x.PhotoSortOrder,
                x.PhotoName,
                ImageSrc = x.ImageSrcRotate(),
                ColorId = x.ColorID
            }).FirstOrDefault();

            return Json(photos);
        }

        [HttpGet]
        public JsonResult GetActivityPhotos360(int productId)
        {
            var p = ProductService.GetProduct(productId);
            return Json(p.ActiveView360);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePhoto360(int productId)
        {
            foreach (var photo in PhotoService.GetPhotos<ProductPhoto>(productId, PhotoType.Product360))
            {
                PhotoService.DeleteProductPhoto(photo.PhotoId);
            }

            var p = ProductService.GetProduct(productId);
            p.ActiveView360 = false;
            ProductService.UpdateProduct(p, false);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPhotos360(List<HttpPostedFileBase> files, int productId)
        {
            var p = ProductService.GetProduct(productId);
            if (p != null && !p.ActiveView360)
            {
                p.ActiveView360 = true;
                ProductService.UpdateProduct(p, false);
            }

            return Json(new UploadProductPictures360(files, productId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetActivityPhotos360(int productId, bool isActive)
        {
            var p = ProductService.GetProduct(productId);
            p.ActiveView360 = isActive;
            ProductService.UpdateProduct(p, false);

            return JsonOk();
        }

        #endregion

        #region Categories

        [HttpGet]
        public JsonResult GetCategories(int productId)
        {
            var result = new GetProductCategories(productId).Execute().Select(x => new { label = x.Value, value = x.Key });
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetMainCategory(int productId, int categoryId)
        {
            ProductService.SetMainLink(productId, categoryId, true);
            ProductService.PreCalcProductParams(productId);
            ProductService.SetProductHierarchicallyEnabled(productId);

            var p = ProductService.GetProduct(productId);
            if (p != null)
                ProductWriter.AddUpdate(p);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategory(int productId, List<int> categoryIds)
        {
            foreach (var categoryId in categoryIds)
            {
                ProductService.DeleteProductLink(productId, categoryId, trackChanges:true);
            }
            
            ProductService.SetProductHierarchicallyEnabled(productId);
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();

            var p = ProductService.GetProduct(productId);
            if (p != null)
                ProductWriter.AddUpdate(p);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategory(int productId, int categoryId)
        {
            ProductService.AddProductLink(productId, categoryId, 0, true, trackChanges:true);
            
            ProductService.SetProductHierarchicallyEnabled(productId);
            CategoryService.CalculateHasProductsForWarehousesInProductCategories(productId);

            var p = ProductService.GetProduct(productId);
            if (p != null)
                ProductWriter.AddUpdate(p);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategories(int productId, List<int> categories)
        {
            foreach (var categoryId in categories)
                ProductService.AddProductLink(productId, categoryId, 0, true, trackChanges: true);
            
            ProductService.SetProductHierarchicallyEnabled(productId);
            CategoryService.CalculateHasProductsForWarehousesInProductCategories(productId);
            CategoryService.CalculateHasProductsForWarehousesInProductCategories(productId);

            var p = ProductService.GetProduct(productId);
            if (p != null)
                ProductWriter.AddUpdate(p);

            return Json(true);
        }

        #endregion

        #region Videos

        [HttpGet]
        public JsonResult GetVideos(int productId)
        {
            return Json(new { DataItems = ProductVideoService.GetProductVideos(productId) });
        }

        [HttpGet]
        public JsonResult GetVideo(int productVideoId)
        {
            return Json(ProductVideoService.GetProductVideo(productVideoId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateVideo(ProductVideo productVideo)
        {
            var video = ProductVideoService.GetProductVideo(productVideo.ProductVideoId);
            if (video == null)
                return Json(false);

            video.Name = productVideo.Name.DefaultOrEmpty();
            video.PlayerCode = productVideo.PlayerCode.DefaultOrEmpty();
            video.Description = productVideo.Description.DefaultOrEmpty();
            video.VideoSortOrder = productVideo.VideoSortOrder;

            ProductVideoService.UpdateProductVideo(video);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddVideo(ProductVideo productVideo, string link)
        {
            if (productVideo.ProductId == 0)
                return JsonError();

            var playerCode = productVideo.PlayerCode.DefaultOrEmpty();
            
            if (string.IsNullOrEmpty(playerCode))
            {
                playerCode = ProductVideoService.GetPlayerCodeFromLink(link, out var error);
                
                if (!string.IsNullOrEmpty(error))
                    return Json(new { result = false, error = error });
            }

            ProductVideoService.AddProductVideo(new ProductVideo
            {
                ProductId = productVideo.ProductId,
                Name = productVideo.Name.DefaultOrEmpty(),
                PlayerCode = playerCode,
                Description = productVideo.Description.DefaultOrEmpty().Trim(),
                VideoSortOrder = productVideo.VideoSortOrder
            });

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteVideo(int productVideoId)
        {
            ProductVideoService.DeleteProductVideo(productVideoId);
            return JsonOk();
        }

        #endregion

        #region Brand

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeBrand(int productId, int brandId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return JsonError();

            var brand = BrandService.GetBrandById(brandId);
            if (brand == null)
                return JsonError();

            product.BrandId = brandId;
            ProductService.UpdateProduct(product, true, true);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteBrand(int productId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return JsonError();

            product.BrandId = 0;
            ProductService.UpdateProduct(product, true, true);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);

            return JsonOk();
        }

        #endregion

        #region Tags

        public JsonResult GetTags(int productId)
        {
            return Json(new
            {
                selectedTags = TagService.Gets(productId, ETagType.Product, onlyEnabled: false).Select(x => new { value = x.Name })
            });
        }

        public JsonResult GetTagsByPaging(int itemsPerPage, string q)
        {
            var tags = TagService.GetAutocompleteTags(itemsPerPage, true, q);
            return Json(tags.Select(x => new {value = x.Name}));
        }

        #endregion

        #region Copy product

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyProduct(int productId, string copyName, int count = 1)
        {
            var newProductId = 0;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    newProductId = new CopyProduct(productId, copyName.Replace("#N#", i > 0 ? (i + 1).ToString() : "")).Execute();
                    if (newProductId == 0)
                        return Json(new { result = false, error = T("Admin.Catalog.ErrorAddingProduct") });
                }
                catch (BlException ex)
                {
                    return Json(new { result = false, error = ex.Message });
                }
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ProductCopyCreated);

            return count == 1
                ? Json(new { result = true, productId = newProductId })
                : Json(new { result = true, categoryId = ProductService.GetFirstCategoryIdByProductId(productId) });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyProducts(CatalogFilterModel filter, string copyName, int count = 1)
        {
            if (filter.SelectMode != Infrastructure.Admin.SelectModeCommand.None)
            {
                filter.Ids = new GetCatalog(filter).GetItemsIds<int>("[Product].[ProductID]");
            }

            foreach (var id in filter.Ids)
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        var newProductId = new CopyProduct(id, copyName.Replace("#N#", (i + 1).ToString())).Execute();
                        if (newProductId == 0)
                            return Json(new { result = false, error = T("Admin.Catalog.ErrorAddingProduct") });
                    }
                    catch (BlException ex)
                    {
                        return Json(new { result = false, error = ex.Message });
                    }
                }
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ProductCopyCreated);

            return Json(new { result = true });
        }

        #endregion

        #region Export options

        [HttpGet]
        public JsonResult GetExportOptions(int productId)
        {
            return ProcessJsonResult(new GetExportOptionsHandler(productId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveExportOptions(ProductExportOptionsModel model)
        {
            return ProcessJsonResult(new UpdateExportOptionsHandler(model));
        }

        #endregion

        #region Properties

        [HttpGet]
        public JsonResult GetProperties(int productId)
        {
            var model = new GetProductProperties(productId).Execute();
            return Json(model);
        }

        [HttpGet]
        public JsonResult GetAllProperties(string q, int? productId, int page = 1, int count = 100, bool? useInDetails = null)
        {
            return Json(new GetAllProperties(new PropertiesFilterModel()
                {
                    Search = q, 
                    Page = page, 
                    ItemsPerPage = count, 
                    ProductId = productId, 
                    UseInDetails = useInDetails
                })
                .Execute());
        }

        [HttpGet]
        public JsonResult GetAllPropertyValues(int propertyId, string q, int page = 1, int count = 100)
        {
            return Json(new GetAllPropertyValues(new PropertyValuesFilterModel() { PropertyId = propertyId, Search = q, Page = page, ItemsPerPage = count }).Execute());
        }

        [HttpGet]
        public JsonResult GetPropertyValues(int propertyId, string search = null)
        {
            var items =
                PropertyService.GetValuesByPropertyId(propertyId)
                    .Where(x => search.IsNullOrEmpty() || x.Value.ToLower().Contains(search.ToLower()));
            //.Select(x => new SelectItemModel(x.Value, x.PropertyValueId.ToString()));

            return Json(items);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddPropertyValue(int productId, int propertyId, int? propertyValueId, string value, bool isNew)
        {
            var propValueId = new AddPropertyValue(productId, propertyId, propertyValueId, value, isNew).Execute();

            return Json(new { result = propValueId != 0, propertyValueId = propValueId });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePropertyValue(int productId, int propertyValueId)
        {
            PropertyService.DeleteProductPropertyValue(productId, propertyValueId);
            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddPropertyWithValue(int productId, int? propertyId, int? propertyValueId, string propertyName, string propertyValue)
        {
            var propValueId = new AddPropertyWithValue(productId, propertyId, propertyValueId, propertyName, propertyValue).Execute();

            return Json(new { result = propValueId != 0, propertyValueId = propValueId });
        }

        #endregion

        #region Related products

        [HttpGet]
        public JsonResult GetRelatedProducts(int productId, RelatedType type)
        {
            var products = ProductService.GetAllRelatedProducts(productId, type).Select(x => new
            {
                Id = x.RelatedProductId,
                x.ProductId,
                x.Name,
                x.ArtNo,
                Url = Url.AbsoluteActionUrl("Edit", "Product", new { id = x.ProductId }),
                ImageSrc = x.Photo.ImageSrcSmall()
            });

            return Json(products);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteRelatedProduct(int productId, RelatedType type, int relatedProductId)
        {
            ProductService.DeleteRelatedProduct(productId, relatedProductId, type);
            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddRelatedProduct(int productId, RelatedType type, List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Json(new { result = false });

            foreach (var id in ids)
                ProductService.AddRelatedProduct(productId, id, type);

            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeRelatedProductsSorting(int id, int? prevId, int? nextId)
        {
            ProductService.ChangeRelatedProductsSorting(id, prevId, nextId);

            return JsonOk();
        }

        #endregion

        #region Gifts

        [HttpGet]
        public JsonResult GetGifts(int productId)
        {
            var products =
                ProductGiftService.GetGifts(productId).Select(x => new GiftAdminModel(x)
                {
                    Url = Url.AbsoluteActionUrl("Edit", "Product", new {id = x.GiftProductId}),
                    ImageSrc = x.Photo.ImageSrcSmall(),
                });

            return Json(products);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteGift(int productId, int giftOfferId, int? offerId)
        {
            ProductGiftService.DeleteGift(productId, giftOfferId, offerId);
            ProductService.PreCalcProductParams(productId);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddGifts(int productId, List<int> offerIds, int productCount)
        {
            if (offerIds == null || offerIds.Count == 0)
                return Json(new { result = false });

            var errors = new List<string>();
            
            foreach (var offerId in offerIds)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer == null)
                {
                    errors.Add("Товар не найден");
                    continue;
                }

                var p = ProductService.GetProduct(offer.ProductId);
                if (p == null || !p.Enabled || !p.CategoryEnabled)
                {
                    errors.Add($"Товар \"{p?.Name}\" не активен или не активна категория товара");
                    continue;
                }

                ProductGiftService.AddGift(productId, offerId, null, productCount);
            }

            ProductService.PreCalcProductParams(productId);

            return errors.Count > 0 
                ? JsonError(string.Join(", ", errors)) 
                : JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateGift(int productId, int giftOfferId, int? offerId, int? productCount)
        {
            if (!productCount.HasValue || productCount.Value <= 0)
                return JsonError("Укажите количество основного товара");

            ProductGiftService.UpdateGift(productId, giftOfferId, offerId, productCount.Value);

            return JsonOk();
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateGiftOffer(int productId, int giftOfferId, int? offerId, int? prevOfferId, int productCount)
        {
            return ProcessJsonResult(() =>
            {
                ProductGiftService.DeleteGift(productId, giftOfferId, prevOfferId);
                ProductGiftService.AddGift(productId, giftOfferId, offerId, productCount);
            });
        }

        #endregion

        #region AddProductList

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddProductList(int categoryId, List<string> products)
        {
            foreach (var productName in products)
            {
                if (string.IsNullOrWhiteSpace(productName))
                    continue;

                try
                {
                    new AddProduct(productName, categoryId).Execute();
                }
                catch (BlException e)
                {
                    ModelState.AddModelError(e.Property, e.Message);
                    return JsonError();
                }
            }

            if (!SettingsCongratulationsDashboard.ProductDone)
            {
                SettingsCongratulationsDashboard.ProductDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_ProductDone);
            }
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ProductListCreated);

            return JsonOk();
        }

        #endregion

        #region Landing funnel

        public JsonResult GetLandingFunnelLink(int productId)
        {
            var lp = new LpSiteService().GetByAdditionalSalesProductId(productId);
            var url = lp != null ? UrlService.GetUrl("lp/" + lp.Url + "?inplace=true") : null;
            var id = lp != null ? lp.Id : default(int?);

            return JsonOk(new { id, url });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLandingFunnel(int productId, int landingSiteId)
        {
            new LpSiteService().Delete(landingSiteId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetLandingFunnel(int productId, int landingSiteId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return JsonError();

            var lpService = new LpSiteService();

            var existingLp = lpService.GetByAdditionalSalesProductId(productId);
            if (existingLp != null)
                return JsonError();

            var lp = lpService.Get(landingSiteId);
            if (lp == null)
                return JsonError();

            lpService.AddAdditionalSalesProduct(productId, lp.Id);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UnSetLandingFunnel(int productId, int landingSiteId)
        {
            new LpSiteService().DeleteAdditionalSalesProduct(productId, landingSiteId);
            return JsonOk();
        }

        public JsonResult GetSalesFunnelsList()
        {
            // только воронки с товарами
            return Json(new LpSiteService().GetList().Where(x => x.Template == LpFunnelType.ProductCrossSellDownSell.ToString()).OrderByDescending(x => x.CreatedDate).Select(x => new
            {
                id = x.Id,
                parent = "#",
                text = String.Format("<span class=\"jstree-advantshop-name\">{0}</span>", x.Name),
                name = x.Name,
                children = false,
            }));
        }

        #endregion

        [HttpGet]
        public JsonResult GetProductNameByOfferId(int offerId)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null || offer.Product == null)
                return Json(null);

            var minAmount = offer.Product.GetMinAmount();
            var photo = offer.Product.ProductPhotos.FirstOrDefault(x => x.Main);

            return Json(new
            {
                ProductId = offer.Product.ProductId,
                ArtNo = offer.ArtNo,
                Name = offer.Product.Name,
                Enabled = offer.Product.Enabled && offer.Product.CategoryEnabled,
                ImgSrc = photo != null ? photo.ImageSrcSmall() : null,
                Price = offer.BasePrice,
                MinAmount = minAmount,
                MaxAmount = offer.Product.MaxAmount.HasValue ? offer.Product.MaxAmount.Value : Int16.MaxValue,
                Multiplicity = offer.Product.Multiplicity,
                Color = offer.Color,
                Size = offer.Size
            });
        }

        [HttpGet]
        public JsonResult GetProductInfoByProductId(int id)
        {
            var product = ProductService.GetProduct(id);
            if (product == null)
                return Json(null);

            var photo = product.ProductPhotos.FirstOrDefault(x => x.Main);

            return Json(new
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                ImgSrc = photo != null ? photo.ImageSrcSmall() : null,
                Enabled = product.Enabled && product.CategoryEnabled,
            });
        }

        #region sales channels

        [HttpPost]
        public JsonResult GetProductSalesChannels(int id)
        {
            return JsonOk(new GetProductSalesChannelsHandler(id).Execute());
        }

        [HttpPost]
        public JsonResult SetProductSalesChannels(ProductEnableInChannelModel model)
        {
            new SetProductSalesChannelsHandler(model).Execute();
            return JsonOk();
        }

        public JsonResult GetSalesChannelsActive(CatalogFilterModel filterModel)
        {
            return JsonOk(new GetSalesChannelsEnableHandler(filterModel).Execute());
        }

        public JsonResult SetSalesChannelsActive(ProductListEnableInChannelModel model)
        {
            new SetSalesChannelsEnableHandler(model).Execute();
            return JsonOk();
        }

        #endregion

        #region Ratio
        [HttpGet]
        public JsonResult GetRatioByProductId(int productId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return Json(null);

            return Json(new
            {
                Ratio = product.Ratio,
                ManualRatio = product.ManualRatio,
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetManualRatioByProductId(int productId, double? manualRatio)
        {
            if (manualRatio.HasValue && (manualRatio < 0 || manualRatio > 5))
                return JsonError();

            ProductService.UpdateProductManualRatioByProductId(productId, manualRatio);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetAllProductsManualRatio(double? manualRatio)
        {
            if (manualRatio.HasValue && (manualRatio < 0 || manualRatio > 5))
                return JsonError();
            ProductService.SetAllProductsManualRatio(manualRatio);
            return JsonOk();
        }
        #endregion

        public JsonResult GetCreatedBySelectOptions()
        {
            var createdByList = ProductService.GetCreatedByList().Select(x => new SelectItemModel(x, x)).ToList();
            createdByList.Insert(0, new SelectItemModel(T("Admin.Catalog.CreatedByOptions.OtherSource"), string.Empty));
            return Json(createdByList);
        }
    }
}
