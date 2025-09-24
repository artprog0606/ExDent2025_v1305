using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.ProductDetails;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.SEO;
using AdvantShop.ViewModel.ProductDetails;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.UrlRewriter;
using System.Collections.Generic;
using AdvantShop.Areas.Mobile.Models.Product;
using AdvantShop.FilePath;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Mobile.Controllers
{
    public class ProductController : BaseMobileController
    {
        public ActionResult Index(string url, int? color, int? size)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Error404();

            var product = ProductService.GetProductByUrl(url);
            if (product == null || !product.Enabled || !product.CategoryEnabled)
                return Error404();

            var model = new GetProductHandler(product, color, size, null).Get();

            model.BreadCrumbs =
                CategoryService.GetParentCategories(product.CategoryId)
                    .Reverse()
                    .Select(x => new BreadCrumbs(x.Name, x.CategoryId == 0 ? Url.AbsoluteRouteUrl("CatalogRoot") : Url.AbsoluteRouteUrl("Category", new { url = x.UrlPath })))
                    .ToList();

            model.BreadCrumbs.Insert(0, new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home")));
            model.BreadCrumbs.Add(new BreadCrumbs(product.Name, Url.AbsoluteRouteUrl("Product", new { url = product.UrlPath })));

            RecentlyViewService.SetRecentlyView(CustomerContext.CustomerId, product.ProductId);

            SetMobileTitle(CategoryService.GetCategory(0).Name);
            SetNgController(NgControllers.NgControllersTypes.ProductCtrl);

            var category = CategoryService.GetCategory(product.CategoryId);

            var offerArtNo = product.Offers.Select(x => x.ArtNo).ToList();
            var productArtNo = product.ArtNo;

            SetMetaInformation(
                product.Meta, product.Name, category != null ? category.Name : string.Empty,
                product.Brand != null ? product.Brand.Name : string.Empty,
                tags: product.Tags.Select(x => x.Name).ToList(),
                price: PriceFormatService.FormatPricePlain(model.FinalPrice, CurrencyService.CurrentCurrency),
                offerArtNo: offerArtNo.Count > 0 ? string.Join(", ", offerArtNo) : string.Empty,
                productArtNo: productArtNo);
            
            var tagManager = GoogleTagManagerContext.Current;
            if (tagManager.Enabled)
            {
                tagManager.PageType = ePageType.product;
                tagManager.ProdId = model.Offer != null ? model.Offer.OfferId.ToString() : model.Product.ProductId.ToString();
                tagManager.ProdArtno = model.Offer != null ? model.Offer.ArtNo : model.Product.ArtNo;
                tagManager.ProdName = model.Product.Name;
                tagManager.ProdValue = model.Offer?.RoundedPrice ?? 0;
                tagManager.CatCurrentId = model.Product.MainCategory?.ID ?? 0;
                tagManager.CatCurrentName = model.Product.MainCategory?.Name ?? "";
            }

            var referrer = Request.GetUrlReferrer();

            if (referrer != null 
                && referrer.AbsolutePath != "/" 
                && referrer.AbsoluteUri.StartsWith(UrlService.GetUrl()) 
                && referrer.AbsolutePath != Request.Url.AbsolutePath 
                && referrer.AbsolutePath.ToLower().Contains("/categories/"))
            {
                model.ReturnUrl = referrer.AbsoluteUri;
                model.UseHistoryApiForBack = true;
            }
            else if (product.MainCategory != null)
            {
                model.ReturnUrl = Url.AbsoluteRouteUrl("Category", new {url = product.MainCategory.UrlPath});
                model.UseHistoryApiForBack = false;
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ProductTabs(ProductDetailsViewModel productModel)
        {
            var model = new ProductTabsViewModel()
            {
                ProductModel = productModel,
                UseStandartReviews = productModel.UseStandartReviews,
                ReviewsCount = productModel.ReviewsCountInt
            };

            foreach (var tabsModule in AttachedModules.GetModules<IProductTabs>())
            {
                var classInstance = (IProductTabs)Activator.CreateInstance(tabsModule, null);
                model.Tabs.AddRange(classInstance.GetProductDetailsTabsCollection(productModel.Product.ProductId));
            }

            if (SettingsSEO.ProductAdditionalDescription.IsNotEmpty())
            {
                model.AdditionalDescription =
                    GlobalStringVariableService.TranslateExpression(
                        SettingsSEO.ProductAdditionalDescription, MetaType.Product, productModel.Product.Name,
                        CategoryService.GetCategory(productModel.Product.CategoryId).Name,
                        productModel.Product.Brand != null ? productModel.Product.Brand.Name : string.Empty, 
                        price: PriceFormatService.FormatPricePlain(productModel.FinalPrice, CurrencyService.CurrentCurrency),
                        tags: productModel.Product.Tags.Select(x => x.Name).ToList().AggregateString(" "),
                        productArtNo: productModel.Product.ArtNo);
            }

            model.VideosCount = ProductVideoService.GetProductVideosCount(productModel.Product.ProductId);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductPhotos(ProductDetailsViewModel productModel)
        {
            var product = productModel.Product;

            var model = new ProductPhotosMobileViewModel()
            {
                Product = product,
                Discount = productModel.FinalDiscount, // todo: Check it
                ProductModel = productModel,
                Photos =
                    product.ProductPhotos.OrderByDescending(item => productModel.ColorId.HasValue ? item.ColorID == productModel.ColorId.Value : item.Main)
                        .ThenBy(item => item.PhotoSortOrder)
                        .ToList(),
                ActiveThreeSixtyView = productModel.Product.ActiveView360 && product.ProductPhotos360.Any(),
                Photos360 = product.ProductPhotos360,
                Photos360Ext = product.ProductPhotos360.Any() ? Path.GetExtension(product.ProductPhotos360.First().PhotoName) : string.Empty,
                ColorId = productModel.ColorId.HasValue ? productModel.ColorId : (productModel.Offer != null ? productModel.Offer.ColorID : null)
            };
            
            model.CarouselPhotoHeight = SettingsPictureSize.SmallProductImageHeight;
            model.CarouselPhotoWidth = SettingsPictureSize.SmallProductImageWidth;
            model.PreviewPhotoHeight = SettingsPictureSize.MiddleProductImageHeight;
            model.PreviewPhotoWidth = SettingsPictureSize.MiddleProductImageWidth;

            foreach (var photo in model.Photos)
            {
                photo.Title =
                    photo.Alt =
                        !string.IsNullOrWhiteSpace(photo.Description)
                            ? photo.Description
                            : $"{product.Name} {product.ArtNo} - {T("Product.ProductPhotos.AltText")} {photo.PhotoId}";
            }

            model.Video = product.ProductVideos.FirstOrDefault();

            var customLabels = new List<ProductLabel>();

            foreach (var labelModule in AttachedModules.GetModules<ILabel>())
            {
                var classInstance = (ILabel)Activator.CreateInstance(labelModule);
                var label = classInstance.GetLabel();
                if (label != null)
                {
                    customLabels.Add(label);
                }

                var labels = classInstance.GetLabels();
                if (labels != null)
                {
                    customLabels.AddRange(labels);
                }
            }

            model.Labels = customLabels.Where(l => l.ProductIds.Contains(product.ProductId)).Select(l => l.LabelCode).ToList();

            return PartialView(model);
        }
        
        public ActionResult ProductViewPhoto(ProductPhotoMobileViewModel model)
        {
            return PartialView(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ProductViewPhoto(ProductPhotoMobileSetting setting)
        {
            var productPhotos = PhotoService.GetPhotos<ProductPhoto>(setting.ProductId, PhotoType.Product);
            var photos = productPhotos
                .Where(photo => setting.RenderedPhotoId != photo.PhotoId && (photo.ColorID == setting.ColorId || photo.ColorID.HasValue == false))
                .OrderByDescending(photo => photo.ColorID.HasValue)
                .Take(setting.LimitPhotoCount).ToList();
            var model = new ProductPhotoMobileViewModel(setting)
            {
                Photos = photos,
                ProductImageType = setting.ProductViewMode == ProductViewMode.Single ? ProductImageType.Middle : ProductImageType.Small,
                
            };
            return PartialView(model);
        }
    }
}