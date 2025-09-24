using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.Products;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Handlers.Booking;
using AdvantShop.App.Landing.Handlers.Landings;
using AdvantShop.App.Landing.Models;
using AdvantShop.App.Landing.Models.Landing;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Helpers;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.App.Landing.Handlers.Booking.Cart;
using AdvantShop.App.Landing.Handlers.Reviews;
using AdvantShop.App.Landing.Models.Booking.Cart;
using AdvantShop.App.Landing.Models.Catalogs;
using AdvantShop.Core.Services.Landing.Reviews;
using AdvantShop.Catalog;
using AdvantShop.Saas;
using AdvantShop.Core.Services.Landing.Pictures;
using Newtonsoft.Json.Linq;

namespace AdvantShop.App.Landing.Controllers
{    
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class LandingController : LandingBaseController
    {
        #region Ctor

        private readonly LpService _lpService;
        private readonly LpBlockService _blockService;
        private readonly LpBlockConfigService _blockConfigService;
        private readonly LpFormService _formService;
        private readonly LpSiteService _siteService;

        public LandingController()
        {
            _lpService = new LpService();
            _siteService = new LpSiteService();
            _blockService = new LpBlockService();
            _formService = new LpFormService();
            _blockConfigService = new LpBlockConfigService();
        }

        #endregion

        // {url}/{lpurl}?inplace=true           - /mylanding/contact
        // ?landingId={landingId}&inplace=true  - rewrite from domainUrl
        public ActionResult Index(LandingIndexModel model)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveLandingFunnel)
            {
                return RedirectToAction("GetFeature", "ServiceController", new { id = ESaasProperty.HaveLandingFunnel.ToString() });
            }

            Lp lp = null;
            LpSite lpSite = null;

            if (model.LandingId != null)
            {
                lp = _lpService.Get(model.LandingId.Value);
                lpSite = _siteService.Get(lp.LandingSiteId);
            }
            else if (!string.IsNullOrEmpty(model.Url))
            {
                lpSite = _siteService.GetByUrl(model.Url);
                if (lpSite == null)
                    return Error404();

                lp = !string.IsNullOrEmpty(model.LpUrl) ? _lpService.Get(lpSite.Id, model.LpUrl) : _lpService.GetByMain(lpSite.Id);
            }

            var canEdit = _lpService.CanEdit(CustomerContext.CurrentCustomer);

            if (lp == null || (!lp.Enabled && !canEdit) || (!lpSite.Enabled && !canEdit))
                return Error404();

            LpService.CurrentLanding = lp;
            LpService.HasAccess = canEdit && !LpService.PreviewInAdmin;
            LpService.ShowShoppingCart = LPageSettings.ShowShoppingCart(lp.Id);
            LpService.IgnoredLpId = model.Without;
            LpService.Mode = model.Mode;

            var cookieInplace = CommonHelper.GetCookie("inplace");
            var cookieInplaceEnabled = cookieInplace == null || cookieInplace.Value == "true";

            if (model.Inplace != null)
            {
                CommonHelper.SetCookie("inplace", model.Inplace.Value.ToLowerString());
                cookieInplaceEnabled = model.Inplace.Value;
            }

            LpService.Inplace = canEdit && cookieInplaceEnabled && !LpService.PreviewInAdmin;

            var isPreviewFromAdmin = LpService.PreviewInAdmin && Request.GetUrlReferrer() != null &&
                                     (Request.GetUrlReferrer().Host == Request.Url.Host ||
                                      Request.GetUrlReferrer().Host == lpSite.DomainUrl);

            if ((LPageSettings.RequireAuth || LSiteSettings.RequireAuth) && !LpService.Inplace && !isPreviewFromAdmin)
            {
                if (LSiteSettings.RequireAuth && LPageSettings.AllowAccess) // если стоит проверка у сайта и у страницы открыт доступ
                {
                    // do nothing
                }
                else
                {
                    var customer = CustomerContext.CurrentCustomer;
                    if (!customer.RegistredUser)
                        return RedirectToAction("Auth", "LandingUser", new { id = lp.Id });

                    // настройки доступа к странице приоритетнее настроек воронки
                    switch (LPageSettings.RequireAuth ? LPageSettings.AuthFilterRule : LSiteSettings.AuthFilterRule)
                    {
                        case ELpAuthFilterRule.WithOrderAndProduct:
                            var authOrderProductIds = LPageSettings.RequireAuth ? LPageSettings.AuthOrderProductIds : LSiteSettings.AuthOrderProductIds;
                            if (authOrderProductIds == null || !_lpService.CheckAuthByOrderAndProduct(customer.Id, authOrderProductIds))
                                return RedirectToAction("AuthNotAccess", "LandingUser", new { id = lp.Id });
                            break;
                        case ELpAuthFilterRule.WithLead:
                            var authLeadSalesFunnelId = LPageSettings.RequireAuth ? LPageSettings.AuthLeadSalesFunnelId : LSiteSettings.AuthLeadSalesFunnelId;
                            var authLeadDealStatusId = LPageSettings.RequireAuth ? LPageSettings.AuthLeadDealStatusId : LSiteSettings.AuthLeadDealStatusId;
                            if (!_lpService.CheckAuthByLead(customer.Id, authLeadSalesFunnelId, authLeadDealStatusId))
                                return RedirectToAction("AuthNotAccess", "LandingUser", new { id = lp.Id });
                            break;
                    }
                }
            }

            var result = new IndexViewModel()
            {
                LandingPage = lp,
                ShoppingCartType = LPageSettings.GetShoppingCartType(lp.Id),
                ShoppingCartHideShipping = LPageSettings.ShoppingCartHideShipping(lp.Id),
                Blocks = _blockService.GetList(lp.Id),
                BlocksOnAllPagesTop = 
                    LPageSettings.DisableBlocksOnAllPages
                        ? new List<LpBlock>()
                        : _blockService.GetBlocksShowOnAllPages(lp.LandingSiteId, lp.Id, true),
                BlocksOnAllPagesBottom = 
                    LPageSettings.DisableBlocksOnAllPages
                        ? new List<LpBlock>()
                        : _blockService.GetBlocksShowOnAllPages(lp.LandingSiteId, lp.Id, false),
            };

            var ignoreActionParams = LPageSettings.IgnoreActionParams(lp.Id);
            if (!ignoreActionParams)
            {
                if (!string.IsNullOrEmpty(model.Code))
                {
                    TempData["orderid"] = LpService.EntityId = OrderService.GetOrderIdByCode(model.Code);
                    LpService.EntityType = "order";
                    LpService.OrderCode = model.Code;
                }

                if (!string.IsNullOrEmpty(model.Lid))
                {
                    var lead = LeadService.GetLead(model.Lid.TryParseInt());
                    LpService.EntityId = lead != null ? lead.Id : 0;
                    LpService.EntityType = "lead";
                }

                if (string.IsNullOrEmpty(model.Lid) && string.IsNullOrEmpty(model.Code) && !string.IsNullOrEmpty(model.Bid))
                {
                    LpService.EntityId = model.Bid.TryParseInt();
                    LpService.EntityType = "booking";
                }
            }

            if (model.Errors != null)
                foreach (var error in model.Errors)
                    ShowMessage(Core.Controls.NotifyType.Error, error);

            SetMetaInformation(new MetaInfo()
            {
                Type = MetaType.Landing,
                Title = LPageSettings.PageTitle,
                MetaKeywords = LPageSettings.PageKeywords,
                MetaDescription = LPageSettings.PageDescription
            });

            return View(result);
        }

        public ActionResult Block(int id, bool? convertToHtml, bool? inplace)
        {
            var block = _blockService.Get(id);
            if (block == null)
                return new EmptyResult();

            if (LpService.CurrentLanding == null)
                LpService.CurrentLanding = _lpService.Get(block.LandingId);

            if (convertToHtml != null && convertToHtml.Value)
                LpService.ConvertingToHtml = true;

            var blockConfig = _blockConfigService.Get(block.Name, LpService.CurrentLanding.Template);
            if (blockConfig == null)
                return new EmptyResult();
            
            var model = new BlockModel()
            {
                Block = block,
                Config = blockConfig,
                Inplace = inplace ?? LpService.Inplace
            };

            if (SettingsMain.IsStoreClosed)
            {
                var dependsOnStore = blockConfig.DependsOn != null &&
                                     blockConfig.DependsOn.Contains(LpBlockConfigDependency.StoreClosed);

                if (dependsOnStore && !model.Inplace)
                    return new EmptyResult();

                model.ShowStoreClosedWarning = dependsOnStore;
            }

            return PartialView("_WrapBlock", model);
        }


        [ChildActionOnly]
        public ActionResult SubBlock(int blockId, string name, bool? inplace, bool? hidePlaceholder)
        {
            var subBlock = _blockService.GetSubBlock(blockId, name);
            if (subBlock == null)
                return new EmptyResult();

            var model = new SubBlockModel(subBlock);
            if (inplace != null)
                model.InPlace = inplace.Value;

            if (hidePlaceholder != null)
                model.HidePlaceholder = hidePlaceholder.Value;

            var viewPath = $"~/Areas/Landing/{LpService.CurrentLanding.Template}/SubBlocks/{subBlock.Type}.cshtml";
            if (ViewExist(viewPath))
                model.ViewPath = viewPath;
            else
            {
                viewPath = $"~/Areas/Landing/Views/SubBlocks/{subBlock.Type}.cshtml";
                if (ViewExist(viewPath))
                    model.ViewPath = viewPath;
            }

            return PartialView("~/Areas/Landing/Views/Shared/_WrapSubBlock.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult SubBlockPicture(int blockId, string name, PictureLoaderTriggerModel pictureLoaderModel, bool? inplace)
        {
            var subBlock = _blockService.GetSubBlock(blockId, name);
            if (subBlock == null)
                return new EmptyResult();

            var model = new SubBlockPictureModel(subBlock) {PictureModel = pictureLoaderModel};

            if (inplace != null)
                model.InPlace = inplace.Value;

            var nameBlock = subBlock.Type;

            var viewPath = $"~/Areas/Landing/{LpService.CurrentLanding.Template}/SubBlocks/{nameBlock}.cshtml";
            if (ViewExist(viewPath))
            {
                model.ViewPath = viewPath;
            }
            else
            {
                viewPath = $"~/Areas/Landing/Views/SubBlocks/{nameBlock}.cshtml";
                if (ViewExist(viewPath))
                    model.ViewPath = viewPath;
            }

            return PartialView("~/Areas/Landing/Views/Shared/_WrapSubBlock.cshtml", model);
        }


        [ChildActionOnly]
        public HtmlString Favicon()
        {
            string path = null;

            if (!string.IsNullOrEmpty(LSiteSettings.Favicon))
                path = LSiteSettings.GetFaviconPath(LpService.CurrentLanding.LandingSiteId);

            if (string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(SettingsMain.FaviconImageName))
                path = FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.FaviconImageName, false);

            if (string.IsNullOrEmpty(path))
                return null;

            return new HtmlString(
                string.Format("<link rel=\"{0}\" href=\"{1}\" />",
                    Request.Browser.Browser == "IE" ? "SHORTCUT ICON" : "shortcut icon",
                    UrlService.GetUrl(path)));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SubmitForm(SubmitFormModel model)
        {
            return ProcessJsonResult(new SubmitForm(model, System.Web.HttpContext.Current.Request.Files));
        }

        #region Products by category

        public JsonResult GetProductsByCategory(ProductsByCategoryModel model)
        {
            return Json(LpProductService.GetProductsByCategory(model));
        }
        
        [HttpGet]
        public string GetProductLabels(bool recommended, bool sales, bool best, bool news, float discountPercent, float discountAmount, DiscountType discountType, bool discountHasValue, int labelCount = 5, List<string> customLabels = null, bool warranty = false)
        {
            var discount = new Discount(discountPercent, discountAmount, discountType);
            var helper = new System.Web.Mvc.HtmlHelper(new ViewContext(), new ViewPage());
            var result = AdvantShop.Web.Infrastructure.Extensions.LayoutExtensions.RenderLabels(helper, recommended, sales, best, news, discount, labelCount, customLabels, warranty);

            return result.ToString();
        }

        #endregion

        #region Product

        public ActionResult ProductShippingVariant(ProductShippingVariantsModel model)
        {
            return PartialView("~/Areas/Landing/Views/Common/ProductShippingVariants.cshtml",model);
        }
        #endregion
        
        
        #region Booking

        [HttpGet]
        public JsonResult GetBookingFormData(BookingFormDataDto model)
        {
            return ProcessJsonResult(new GetBookingFormDataHandler(model));
        }

        [HttpGet]
        public JsonResult GetBookingTimes(int resourceId, int affiliateId, DateTime selectedDate, List<int> selectedServices = null)
        {
            return ProcessJsonResult(new GetBookingTimesHandler(resourceId, affiliateId, selectedDate, selectedServices));
        }

        [HttpGet]
        public JsonResult GetFreeDayByTime(GetBookingByTimeFreeDayDto model)
        {
            return ProcessJsonResult(new GetBookingByTimeFreeDayHandler(model));
        }

        [HttpGet]
        public JsonResult GetBookingByTimeMonthFreeDays(GetBookingByTimeMonthDaysDto model)
        {
            return ProcessJsonResult(new GetBookingMonthFreeDaysHandler(model));
        }

        [HttpGet]
        public JsonResult GetStartDayByDays(GetBookingByDaysStartDayDto model)
        {
            return ProcessJsonResult(new GetBookingByDaysStartDayHandler(model));
        }

        [HttpGet]
        public JsonResult GetBookingByDaysMonthStartDays(GetBookingByDaysMonthStartDaysDto model)
        {
            return ProcessJsonResult(new GetBookingByDaysMonthStartDaysHandler(model));
        }

        [HttpGet]
        public JsonResult GetBookingByDaysMonthEndDays(GetBookingByDaysMonthEndDaysDto model)
        {
            return ProcessJsonResult(new GetBookingByDaysMonthEndDaysHandler(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddBooking(AddBookingDto model)
        {
            return ProcessJsonResult(new AddBookingHandler(model, System.Web.HttpContext.Current.Request.Files));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateBookingCustomer(SubmitFormModel model)
        {
            return ProcessJsonResult(new UpdateBookingCustomerHandler(model));
        }

        #region BookingCart

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetBookingCart()
        {
            return ProcessJsonResult(new GetBookingCart());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddToBookingCart(CartItemAddingModel item)
        {
            return ProcessJsonResult(new AddToBookingCart(item));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveFromBookingCart(int itemId)
        {
            return ProcessJsonResult(new RemoveFromBookingCart(itemId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ClearBookingCart()
        {
            return ProcessJsonResult(new ClearBookingCart());
        }

        #endregion

        #region BookingServices

        [HttpGet]
        public ActionResult GetBookingServices(Models.Booking.Services.GetBookingServicesModel model)
        {
            var modelResult = new Models.Booking.Services.BookingServicesModel
            {
                Block = _blockService.Get(model.BlockId)
            };

            if (modelResult.Block == null)
                return new EmptyResult();

            if (LpService.CurrentLanding == null)
                LpService.CurrentLanding = _lpService.Get(modelResult.Block.LandingId);

            modelResult.Config = _blockConfigService.Get(modelResult.Block.Name, LpService.CurrentLanding.Template);
            if (modelResult.Config == null)
                return new EmptyResult();

            modelResult.Services =
                Core.Services.Booking.ServiceService.GetListReservationResourceServices(model.AffiliateId, model.ResourceId)
                    .Select(service => new ServicesContentItemsModel
                    {
                        Name = service.Name,
                        Description = service.Description,
                        Price = Core.Services.Catalog.PriceFormatService.FormatPrice(service.BasePrice, service.Currency),
                        ShowPrice = model.ShowPrice
                    }).ToList();

            return PartialView("~/Areas/Landing/Views/Common/BookingServices.cshtml", modelResult);
        }

        #endregion

        #endregion

        #region Reviews

        public JsonResult AddReview(int blockId, LpReview review)
        {
            return ProcessJsonResult(new AddLpReview(blockId, review));
        }

        #endregion

        #region Error404

        public ActionResult Error404Page()
        {
            SetResponse(HttpStatusCode.NotFound);
            var ext = VirtualPathUtility.GetExtension(Request.RawUrl);
            if (ext != null)
            {
                var list = new List<string> { ".css", ".js", ".jpg", ".jpeg", ".png", ".map", ".ico", ".gif" };
                if (list.Contains(ext.ToLower()))
                    return new EmptyResult();
            }

            SetMetaInformation(T("Error.NotFound.Title"));

            LpService.CurrentLanding = new Lp();

            return View();
        }

        private void SetResponse(HttpStatusCode httpStatusCode)
        {
            try
            {
                Response.Clear();
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)httpStatusCode;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription((int)httpStatusCode);
            }
            catch
            {

            }
        }

        #endregion

        public ActionResult HeadCss(int id)
        {
            var site = _siteService.Get(id);
            if (site == null)
                return new EmptyResult();

            LpService.CurrentSiteId = site.Id;
            return View("~/Areas/Landing/Views/Common/HeadCss.cshtml");
        }

        public ActionResult FormFields(int id, string ngModel, string objId, bool isVertical = false)
        {
            var form = _formService.Get(id);
            if (form == null)
                return new EmptyResult();

            return PartialView("~/Areas/Landing/Views/Common/FormFields.cshtml", new LpFormModel()
            {
                Form = form,
                NgModel = ngModel,
                IsVertical = isVertical,
                ObjId = objId
            });
        }

        public ActionResult Rating(int id, string ngModel, string objId, int ratio = 1, string ratioAsNgModel = null, bool readOnly = false)
        {
            return PartialView("~/Areas/Landing/Views/Common/Rating.cshtml", new RatingModel()
            {
                NgModel = ngModel,
                Ratio = ratio,
                RatioAsNgModel = ratioAsNgModel,
                ObjId = objId,
                ReadOnly = readOnly
            });
        }

        [ChildActionOnly]
        public ActionResult CookiesPolicy(string urlSite)
        {
            if (string.IsNullOrWhiteSpace(urlSite))
                urlSite = $"ls{LpService.CurrentSiteId}";
            
            var cookieName = $"{urlSite}_CookiesPolicyAccepted";

            if (!SettingsNotifications.ShowCookiesPolicyMessage ||
                CommonHelper.GetCookieString(HttpUtility.UrlEncode(cookieName)) == "true" ||
                BrowsersHelper.IsBot() ||
                Request.IsLighthouse())
            {
                return new EmptyResult();
            }

            return PartialView((object)cookieName);
        }

        [HttpGet]
        public JsonResult GetPagingFromSettings(int blockId, int page, int size, string name = "items")
        {
            var block = _blockService.Get(blockId);
            if (block == null)
                return JsonError($"Not found block with ID {blockId}");

            return JsonOk(block.TryGetSettingAsList<SimpleConvertibleObject>(name).Skip(page * size).Take(size));
        }

        [HttpGet]
        public JsonResult GetPagingFromSettingsWithRows(int blockId, int take, int skip, string name = "items", string modelName = "")
        {
            var block = _blockService.Get(blockId);
            if (block == null)
                return JsonError(string.Format("Not found block with ID {0}", blockId));

            if (!string.IsNullOrEmpty(modelName))
            {
                var type = LpBlockService.GetTypes(typeof(ILpPagingModel).Name).Find(x => x.Name.ToLower() == modelName.ToLower());
                if (type != null)
                {
                    var value = block.TryGetSetting(name);
                    if (value == null)
                        return JsonOk(null);

                    var arr = value as JArray;
                    if (arr != null)
                    {
                        var result = arr.Select(x => x.ToObject(type)).Skip(skip).Take(take).ToList();
                        return JsonOk(result);
                    }
                    return JsonOk(null);
                }
            }

            return JsonOk(block.TryGetSettingAsList<NewsModel, OldNewsModel>(name).Skip(skip).Take(take));
        }

        [ChildActionOnly]
        public ActionResult AdminMenu(int landingId)
        {
            if (!LpService.Inplace && !LpService.HasAccess)
                return new EmptyResult();

            var model = new AdminMenuModel()
            {
                SiteId = LpService.CurrentSiteId,
                LandingId = LpService.CurrentLanding.Id,
                Inplace = LpService.Inplace,
                Items = _lpService.GetList(landingId).Take(15).ToList()
            };

            return PartialView("~/Areas/Landing/Views/Common/AdminMenu.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult Button(int blockId, string offerId, string colorId, string amount)
        {
            var block = _blockService.Get(blockId);
            if (block == null)
                return new EmptyResult();

            var button = block.TryGetSetting<LpButton>("button");
            if (button == null)
                return new EmptyResult();

            button.BlockId = blockId;
            button.ActionOfferId = offerId;
            button.AdditionalData = 
                "{\"offerId\": " + offerId + 
                (!string.IsNullOrEmpty(colorId) ? ", \"colorId\": " + colorId : null) +
                (!string.IsNullOrEmpty(amount) ? ", \"amount\": " + amount : null) +
                "}";

            return PartialView("~/Areas/Landing/Views/Shared/_Button.cshtml", button);
        }

        [ChildActionOnly]
        public ActionResult LpButton(int blockId, LpButton button)
        {
            button.BlockId = blockId;
            return PartialView("~/Areas/Landing/Views/Shared/_Button.cshtml", button);
        }

        [ChildActionOnly]
        public HtmlString WebManifest()
        {
            if (!LSiteSettings.MobileAppActive || string.IsNullOrEmpty(LSiteSettings.MobileAppManifestName))
                return null;
            
            return new HtmlString($"<link rel=\"manifest\" href=\"{UrlService.GetUrl(string.Format(LpFiles.LpSitePathRelative, LpService.CurrentSiteId) + LSiteSettings.MobileAppManifestName)}\" />");
        }
    }
}
