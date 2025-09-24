using System.IO;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Core.Services.SEO.MetaData;
using AdvantShop.Customers;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.SEO;
using AdvantShop.ViewModel.ProductDetails;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Repository.Currencies;
using AdvantShop.App.Landing.Models;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.ViewModel.ProductDetailsLanding;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    public partial class ProductController : BaseClientProductController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(string url, int? color, int? size, string v)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Error404();

            var product = ProductService.GetProductByUrl(url);
            if (product == null || !product.Enabled || !product.CategoryEnabled)
                return Error404();

            var model = new GetProductHandler(product, color, size, v).Get();

            model.BreadCrumbs =
                CategoryService.GetParentCategories(product.CategoryId)
                    .Reverse()
                    .Select(x => new BreadCrumbs(x.Name, x.CategoryId == 0 ? Url.AbsoluteRouteUrl("CatalogRoot") : Url.AbsoluteRouteUrl("Category", new { url = x.UrlPath })))
                    .ToList();

            model.BreadCrumbs.Insert(0, new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home")));
            model.BreadCrumbs.Add(new BreadCrumbs(product.Name, Url.AbsoluteRouteUrl("Product", new { url = product.UrlPath })));

            RecentlyViewService.SetRecentlyView(CustomerContext.CustomerId, product.ProductId);

            SetNgController(NgControllers.NgControllersTypes.ProductCtrl);

            var category = CategoryService.GetCategory(product.CategoryId);
            var offerArtNos = product.Offers.Select(x => x.ArtNo).ToList();

            SetMetaInformation(
                product.Meta, product.Name, category != null ? category.Name : string.Empty,
                product.Brand != null ? product.Brand.Name : string.Empty,
                tags: product.Tags.Select(x => x.Name).ToList(),
                price: PriceFormatService.FormatPricePlain(model.FinalPrice, CurrencyService.CurrentCurrency),
                offerArtNo: offerArtNos.Count > 0 ? string.Join(", ", offerArtNos) : string.Empty,
                productArtNo: product.ArtNo);

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

            if (model.Offer != null)
            {
                model.AmountPriceString = PriceService
                    .SimpleRoundPrice(model.FinalPrice * model.Offer.Product.GetMinAmount(),
                        CurrencyService.CurrentCurrency).FormatPrice();
            }

            MetaDataContext.CurrentObject = new OpenGraphModel
            {
                Type = OpenGraphType.Product,
            };
            if (product.ProductPhotos.Count > 0)
                MetaDataContext.CurrentObject.Image = product.ProductPhotos
                                                            .OrderByDescending(x => color.HasValue ? x.ColorID == color : x.Main)
                                                            .ThenBy(x => x.PhotoSortOrder)
                                                            .FirstOrDefault().ImageSrcMiddle();
            
            WriteLog(model.Product.Name, Url.AbsoluteRouteUrl("Product", new { url = product.UrlPath }), ePageType.product);
            return CustomView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductLanding(int productId, bool showButton = true, bool showPrice = true,
                                           bool hideShipping = false, int? landingId = null, SliderModel slider = null, bool previewInAdmin = false,
                                           int? blockId = null, bool? showVideo = true, LpButton button = null, bool productDetails = false)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null || !product.Enabled)
                return new EmptyResult();

            var discount = button != null && button.Discount != null ? (Discount) button.Discount : null; 

            var productModel = new GetProductHandler(product, null, null, null, discount).Get();

            productModel.ShowAddButton = showButton;
            productModel.LpShowPrice = showPrice;
            productModel.HideShipping = hideShipping;
            productModel.LandingId = landingId;
            productModel.BlockId = blockId;
            productModel.ShowVideo = showVideo;
            productModel.LpButton = button;
            productModel.ProductDetails = productDetails;

            var model = new ProductDetailsViewModelLanding(productModel)
            {
                Slider = slider,
                PreviewInAdmin = previewInAdmin
            };

            SettingsDesign.IsMobileTemplate = false;

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductInfoShortLanding(int productId, bool showButton = true,
                                           int? landingId = null, bool previewInAdmin = false,
                                           int? blockId = null, LpButton button = null, 
                                           bool showButton2 = true, LpButton button2 = null, 
                                           string showShippingsMethods = "Never")
        {
            var product = ProductService.GetProduct(productId);
            if (product == null || !product.Enabled)
                return new EmptyResult();

            var discount = button != null && button.Discount != null ? (Discount)button.Discount : null;

            var productModel = new GetProductHandler(product, null, null, null, discount, true).Get();
            SettingsDesign.eShowShippingsInDetails showShippingsMethodsParsed;
            var isOkShowShippingsMethodsParsed =
                SettingsDesign.eShowShippingsInDetails.TryParse(showShippingsMethods, out showShippingsMethodsParsed);
            
            productModel.ShowAddButton = showButton;
            productModel.LandingId = landingId;
            productModel.BlockId = blockId;
            productModel.LpButton = button;
            productModel.ShowButton2 = showButton2;
            productModel.LpButton2 = button2;
            productModel.ShowShippingsMethods = isOkShowShippingsMethodsParsed ? showShippingsMethodsParsed : SettingsDesign.eShowShippingsInDetails.Never;
            
            var model = new ProductDetailsViewModelLanding(productModel) {PreviewInAdmin = previewInAdmin};

            SettingsDesign.IsMobileTemplate = false;

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductLandingUpsell(int productId)
        {
            var product = ProductService.GetProduct(productId);
            var model = new GetProductHandler(product, null, null, null).Get();

            SettingsDesign.IsMobileTemplate = false;

            SetMetaInformation(
                product.Meta, product.Name, CategoryService.GetCategory(product.CategoryId).Name,
                product.Brand != null ? product.Brand.Name : string.Empty, tags: product.Tags.Select(x => x.Name).ToList());

            return PartialView(model);
        }

        public ActionResult ProductQuickView(int productId, int? color, int? size, string from, int? landingId,
                                             bool? hideShipping, bool? showLeadButton, int? blockId, bool? showVideo = true, string descriptionMode = "", 
                                             SettingsDesign.eCartAddTypeButton cartAddType = SettingsDesign.eCartAddTypeButton.Classic)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null || !product.Enabled || !product.CategoryEnabled)
                return new EmptyResult();

            var model = new GetProductHandler(product, color, size, null).Get();
            model.AllowReviews = false;
            model.CartAddTypeButton = cartAddType;
            model.ShowBriefDescription = true;
            //model.ShowDescription = true;
            model.LandingId = landingId;
            model.HideShipping = hideShipping != null && hideShipping.Value;

            RecentlyViewService.SetRecentlyView(CustomerContext.CustomerId, product.ProductId);

            SettingsDesign.IsMobileTemplate = false;
            
            var category = CategoryService.GetCategory(product.CategoryId);
            var offerArtNos = product.Offers.Select(x => x.ArtNo).ToList();

            SetMetaInformation(
                product.Meta, product.Name, category != null ? category.Name : string.Empty,
                product.Brand != null ? product.Brand.Name : string.Empty,
                tags: product.Tags.Select(x => x.Name).ToList(),
                price: PriceFormatService.FormatPricePlain(model.FinalPrice, CurrencyService.CurrentCurrency),
                offerArtNo: offerArtNos.Count > 0 ? string.Join(", ", offerArtNos) : string.Empty,
                productArtNo: product.ArtNo);

            if (from == "landing")
            {
                model.ShowLeadButton = showLeadButton;
                model.BlockId = blockId;
                model.ShowVideo = showVideo;
                model.DescriptionMode = descriptionMode;

                if (blockId != null)
                {
                    var block = new LpBlockService().Get(blockId.Value);
                    if (block != null)
                    {
                        model.ShowAddButton = block.TryGetSetting("show_button_quickview") == null ||
                                              block.TryGetSetting("show_button_quickview") == true;

                        model.LpButton = block.TryGetSetting<LpButton>("button");
                        
                        model.HidePrice = SettingsCatalog.HidePrice;
                        model.LpShowPrice = block.TryGetSetting("show_price") == true;
                    }
                }

                var modelProduct = new ProductDetailsViewModelLanding(model);
                return PartialView("ProductQuickViewLanding", modelProduct);
            }

            return PartialView(model);
        }


        [ChildActionOnly]
        public ActionResult ProductPhotos(ProductDetailsViewModel productModel, bool enabledModalPreview = true, bool landing = false, bool previewInAdmin = false, bool quickView = false)
        {
            var product = productModel.Product;

            var model = new ProductPhotosViewModel()
            {
                Product = product,
                Discount = productModel.FinalDiscount, // todo: Check it
                ProductModel = productModel,
                Photos = productModel.ColorId.HasValue ? product.ProductPhotos.OrderByDescending(x => x.ColorID == productModel.ColorId.Value).ToList()  : product.ProductPhotos,
                EnabledModalPreview = enabledModalPreview,
                EnabledZoom = SettingsDesign.EnableZoom,

                ActiveThreeSixtyView = productModel.Product.ActiveView360 && product.ProductPhotos360.Any(),
                Photos360 = product.ProductPhotos360,
                Photos360Ext = product.ProductPhotos360.Any() ? Path.GetExtension(product.ProductPhotos360.First().PhotoName) : string.Empty,
                CustomViewPath = productModel.CustomViewPath
            };

            foreach (var photo in model.Photos)
            {
                photo.Title =
                    photo.Alt =
                        !string.IsNullOrWhiteSpace(photo.Description)
                            ? photo.Description
                            : $"{product.Name} {product.ArtNo} - {T("Product.ProductPhotos.AltText")} {photo.PhotoId}";
            }

            if (productModel.Offer != null && productModel.Offer.Photo != null) //&& productModel.Offer.Photo.PhotoName.IsNotEmpty())
            {
                model.MainPhoto = productModel.Offer.Photo;
            }
            else
            {
                model.MainPhoto =
                    model.Photos.OrderByDescending(item => item.Main)
                                .ThenBy(item => item.PhotoSortOrder)
                                .FirstOrDefault(item => item.Main);
            }
            
            if (model.MainPhoto == null)
            {
                model.MainPhoto = new ProductPhoto(product.ProductId, PhotoType.Product, "");
            }

            model.MainPhoto.Title =
                model.MainPhoto.Alt =
                    !string.IsNullOrWhiteSpace(model.MainPhoto.Description)
                        ? model.MainPhoto.Description
                        : $"{product.Name} {product.ArtNo} - {T("Product.ProductPhotos.AltText")} {model.MainPhoto.PhotoId}";

            model.Video = product.ProductVideos.FirstOrDefault();

            var customLabels = new List<ProductLabel>();

            var modules = AttachedModules.GetModuleInstances<ILabel>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                {
                    var label = module.GetLabel();
                    if (label != null)
                        customLabels.Add(label);

                    var labels = module.GetLabels();
                    if (labels != null)
                        customLabels.AddRange(labels);
                }
            }

            model.Labels = customLabels.Where(l => l.ProductIds.Contains(product.ProductId)).Select(l => l.LabelCode).ToList();

            model.CarouselPhotoHeight = SettingsPictureSize.XSmallProductImageHeight;
            model.CarouselPhotoWidth = SettingsPictureSize.XSmallProductImageWidth;
            model.PreviewPhotoHeight = SettingsPictureSize.MiddleProductImageHeight;
            model.PreviewPhotoWidth = SettingsPictureSize.MiddleProductImageWidth;

            if (landing == true) {

                var modelProduct = new ProductPhotosViewLandingModel(model);
                modelProduct.PreviewInAdmin = previewInAdmin;
                modelProduct.QuickView = quickView;
                return View("ProductPhotosLanding", modelProduct);
            }
                

            return CustomPartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductInfo(ProductDetailsViewModel productInfoModel)
        {
            return CustomPartialView(productInfoModel);
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

            var modules = AttachedModules.GetModuleInstances<IProductTabs>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                    model.Tabs.AddRange(module.GetProductDetailsTabsCollection(productModel.Product.ProductId));
            }

            if (SettingsSEO.ProductAdditionalDescription.IsNotEmpty())
            {
                var category = CategoryService.GetCategory(productModel.Product.CategoryId);

                model.AdditionalDescription =
                    GlobalStringVariableService.TranslateExpression(
                        SettingsSEO.ProductAdditionalDescription, MetaType.Product, productModel.Product.Name,
                        categoryName: category != null ? category.Name : null,
                        brandName: productModel.Product.Brand != null ? productModel.Product.Brand.Name : string.Empty,
                        price: PriceFormatService.FormatPricePlain(productModel.FinalPrice, CurrencyService.CurrentCurrency),
                        tags: productModel.Product.Tags.Select(x => x.Name).ToList().AggregateString(" "), 
                        productArtNo: productModel.Product.ArtNo);
            }
            model.VideosCount = ProductVideoService.GetProductVideosCount(productModel.Product.ProductId);

            return PartialView(model);
        }
        
        public ActionResult ProductProperties(ProductDetailsViewModel productModel, int productId = 0, bool renderInplaceBlock = true, int countVisisble = 0)
        {
            var showInPlaceEditor = InplaceEditorService.CanUseInplace(RoleAction.Catalog);

            var propertyValues = new List<PropertyValue>();
            var prodPropertyValues = productId == 0
                                        ? productModel.ProductProperties.Where(v => v.Property.UseInDetails).ToList()
                                        : PropertyService.GetPropertyValuesByProductId(productId).Where(v => v.Property.UseInDetails).ToList();

            if (!showInPlaceEditor)
            {
                foreach (var value in prodPropertyValues.Where(propValue => propertyValues.All(x => x.PropertyId != propValue.PropertyId)))
                {
                    propertyValues.Add(new PropertyValue()
                    {
                        Property = value.Property,
                        PropertyId = value.PropertyId,
                        PropertyValueId = value.PropertyValueId,
                        SortOrder = value.SortOrder,
                        Value = String.Join(", ", prodPropertyValues.Where(x => x.PropertyId == value.PropertyId).Select(x => x.Value))
                    });
                }
            }
            else
            {
                propertyValues = prodPropertyValues;
            }

            if (propertyValues.Count == 0 && !showInPlaceEditor)
                return new EmptyResult();

            var model = new PropductPropertiesViewModel()
            {
                ProductId = productId == 0 ? productModel.Product.ProductId : productId,
                PropertyValues = propertyValues,
                ShowInPlaceEditor = showInPlaceEditor,
                RenderInplaceAddBlock = showInPlaceEditor && renderInplaceBlock,
                CustomViewPath = productModel.CustomViewPath,
                CountVisible = countVisisble
            };

            return CustomPartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductVideos(int productId)
        {
            if (!ProductVideoService.HasVideo(productId))
                return new EmptyResult();

            return PartialView(productId);
        }
        
        [ChildActionOnly]
        public ActionResult ProductReviews(ProductDetailsViewModel productModel, int productId, bool reviewsReadonly = false, string headerText = "")
        {
            var reviews =
                ReviewService.GetReviews(productId, EntityType.Product)
                    .Where(review => review.Checked || !SettingsCatalog.ModerateReviews).ToList();

            var customer = CustomerContext.CurrentCustomer;
          
            var model = new ProductReviewsViewModel()
            {
                EntityId = productId,
                EntityType = (int)EntityType.Product,
                ModerateReviews = SettingsCatalog.ModerateReviews,
                ReviewsVoiteOnlyRegisteredUsers = SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers,
                IsAdmin = customer.IsAdmin,
                RegistredUser = customer.RegistredUser,
                Reviews = reviews,
                UserName = customer.RegistredUser ? customer.FirstName + " " + customer.LastName : string.Empty,
                Email = customer.EMail,
                CustomViewPath = productModel.CustomViewPath,
                ReviewsReadonly = reviewsReadonly || !(productModel.AllowAddReviews || customer.IsAdmin),
                HeaderText = headerText,
                DisplayImage = SettingsCatalog.DisplayReviewsImage,
                WhoAllowReviews = SettingsDesign.WhoAllowReviews,
                ShowVerificationCheckmarkAtAdminInReviews = SettingsDesign.ShowVerificationCheckmarkAtAdminInReviews,
                Rating = RatingService.GetUserProductRating(productId, customer.Id),
                AllowAddReviews = customer.IsAdmin || !ReviewService.IsCustomerHaveReview(productId, EntityType.Product, customer.Id)
            };

            return CustomPartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductReviewsOnMain(int count = 0)
        {
            try
            {
                var isCarousel = TemplateSettingsProvider.Items["MainPageProductReviewsVisibility"].TryParseBool();
                var model = new ProductReviewsOnMainCarouselHandler(isCarousel, count).Execute();
                
                return isCarousel 
                    ? PartialView("~/Views/Product/ProductReviewsOnMainCarousel.cshtml", model) 
                    : PartialView(model);
            }
            catch (BlException)
            {
                return new EmptyResult();
            }
        }
        
        [ChildActionOnly]
        public ActionResult SizeColorPicker(Product product, int? color, int? size, SettingsDesign.eSizeColorControlType? colorsControlType, SettingsDesign.eSizeColorControlType? sizesControlType)
        {
            return PartialView(new GetSizeColorPicker(product, color, size, colorsControlType ?? SettingsDesign.ColorsControlType, sizesControlType ?? SettingsDesign.SizesControlType).Execute());
        }

        [ChildActionOnly]
        public ActionResult CustomOptions(int productId)
        {
            if (productId == 0)
                return new EmptyResult();

            return PartialView(productId);
        }

        [ChildActionOnly]
        public ActionResult RelatedProducts(Product product, RelatedType type, bool enabledCarousel = true)
        {
            if (product == null)
                return new EmptyResult();

            var model = new GetRelatedProductsHandler(product, type, SettingsDesign.IsMobileTemplate).Get();

            if (model == null || !model.IsNotEmpty)
                return new EmptyResult();

            model.EnabledCarousel = enabledCarousel;
            model.Products.LazyLoadType = model.EnabledCarousel ? eLazyLoadType.Carousel : eLazyLoadType.Default;
            model.Products.DisplayReviewCount = false;

            return PartialView(type + "Products", model);
        }

        [ChildActionOnly]
        public ActionResult ProductGifts(ProductDetailsViewModel productModel, bool landing = false)
        {
            if (productModel.Gifts == null || productModel.Gifts.Count == 0)
                return new EmptyResult();

            var model = new ProductGiftsViewModel()
            {
                Gifts = productModel.Gifts,
                CustomViewPath = productModel.CustomViewPath,
                Product = productModel.Product,
                ShowProductLink = !landing
            };

            return CustomPartialView(model);
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetAllProductsManualRatio(double? manualRatio)
        {
            if (manualRatio.HasValue && (manualRatio < 0 || manualRatio > 5))
                return JsonError();
            ProductService.SetAllProductsManualRatio(manualRatio);
            return JsonOk();
        }
    }
}