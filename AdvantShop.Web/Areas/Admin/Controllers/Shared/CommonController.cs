using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Handlers.Shared.Common;
using AdvantShop.Web.Admin.Models.Cms.Menus;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.ViewModels.Shared.Common;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class CommonController : BaseAdminController
    {
        [ChildActionOnly]
        public ActionResult NavMenu()
        {
            var area = Request.RequestContext.RouteData.DataTokens["area"] as string;
            if (area == null || (!area.Equals("adminv2", StringComparison.OrdinalIgnoreCase) && !area.Equals("adminv3", StringComparison.OrdinalIgnoreCase)))
            {
                return new EmptyResult();
            }

            var menuItems = new GetAdminMenuObject().Execute();

            AdminMenuModel selectedRootNode = null;

            foreach (var item in menuItems)
            {
                selectedRootNode = item.Menu.FirstOrDefault(x => x.Selected);
                if (selectedRootNode != null)
                    break;
            }

            //var selectedRootNode = menuItems.FirstOrDefault(x => x.Selected);
            if (selectedRootNode != null && selectedRootNode.MenuItems != null && (selectedRootNode.ShowChildsInNavMenu || area.Equals("adminv2", StringComparison.OrdinalIgnoreCase)))
            {
                if (!(String.Equals(selectedRootNode.Controller, "home", StringComparison.OrdinalIgnoreCase) &&
                      String.Equals(selectedRootNode.Action, "index", StringComparison.OrdinalIgnoreCase)) &&
                    !selectedRootNode.MenuItems.Any(x => x.Selected))
                {
                    foreach (var item in selectedRootNode.MenuItems)
                        if (String.Equals(item.Controller, selectedRootNode.Controller, StringComparison.OrdinalIgnoreCase) &&
                            String.Equals(item.Action, selectedRootNode.Action, StringComparison.OrdinalIgnoreCase) &&
                            item.Route == selectedRootNode.Route)
                        {
                            item.Selected = true;
                            break;
                        }
                }

                return PartialView(selectedRootNode.MenuItems);
            }

            if (area.Equals("adminv3", StringComparison.OrdinalIgnoreCase))
            {
                var salesChannelsMenuItems = new GetSalesChannelsMenu().Execute();
                SalesChannel selectedSalesChannel = salesChannelsMenuItems.FirstOrDefault(x => x.Selected);

                if (selectedSalesChannel != null && selectedSalesChannel.MenuItems != null && selectedSalesChannel.ShowMenuItemsInNavMenu)
                    return PartialView("NavMenuBySale", selectedSalesChannel);
            }

            return PartialView(new List<AdminMenuModel>());
        }

        //[ChildActionOnly]
        public ActionResult LeftMenu()
        {
            var salesChannelItems = new GetSalesChannelsMenu().Execute();
            var isSalesChannelSelected = salesChannelItems.Any(x => x.Selected);


            var model = new LeftMenuViewModel()
            {
                IsDashBoard = true,
                MenuItems = new GetAdminMenuObject().Execute(true, isSalesChannelSelected, SettingsDesign.IsMobileTemplate),
                SalesChannelsMenuItems = salesChannelItems,
                CustomMenuItems = new GetCustomMenuItems().Execute(),
                IsMobile = SettingsDesign.IsMobileTemplate,
                
            };

            return PartialView(model);
        }

        public ActionResult MainMenuMobile(int numberLimitationMenuItem = 0)
        {
            var salesChannelItems = new GetSalesChannelsMenu().Execute();
            var isSalesChannelSelected = salesChannelItems.Any(x => x.Selected);
            var model = new LeftMenuViewModel()
            {
                IsDashBoard = true,
                SalesChannelsMenuItems = salesChannelItems,
                MenuItems = new GetAdminMenuObject().Execute(true, isSalesChannelSelected, SettingsDesign.IsMobileTemplate),
                NumberLimitationMenuItem = numberLimitationMenuItem
            };
            model.ReplaceItems(isMobile: true);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            var currentCustomer = CustomerContext.CurrentCustomer;

            var model = new TopMenuViewModel();
            List<CustomerRoleAction> avalableRoles = !currentCustomer.IsAdmin
                ? RoleActionService.GetCustomerRoleActionsByCustomerId(CustomerContext.CurrentCustomer.Id)
                : null;

            var currentsaasData = SaasDataService.CurrentSaasData;
            var enabledSaas = SaasDataService.IsSaasEnabled;

            model.DisplayCatalog = currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Catalog);
            model.DisplayCustomers = (SettingsMain.StoreActive || SettingsCrm.CrmActive) && (currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Customers));
            model.DisplayOrders = currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Orders);
            model.DisplayCrm = SettingsCrm.CrmActive && (!enabledSaas || currentsaasData.HaveCrm) && (currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Crm));
            model.DisplayTasks = SettingsTasks.TasksActive && (currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Tasks));
            model.DisplayBooking = SettingsMain.BookingActive && (currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Booking));
            model.DisplayCms = SettingsMain.StoreActive && (currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Store));
            model.DisplaySettings = currentCustomer.IsAdmin || avalableRoles.Any(item => item.Role == RoleAction.Settings);
            model.IsDashboard = true;

            return PartialView(model);
        }


        [ChildActionOnly]
        public ActionResult SaasInformation()
        {
            if (SaasDataService.IsSaasEnabled && (CustomerContext.CurrentCustomer.IsAdmin || CustomerContext.CurrentCustomer.IsVirtual))
            {
                return PartialView(SaasDataService.CurrentSaasData);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateSaasInformation()
        {
            SaasDataService.GetSaasData(true);
            return Redirect(Request.GetUrlReferrer() != null ? Request.GetUrlReferrer().ToString() : UrlService.GetAdminUrl());
        }

        #region Avatar

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAvatar(HttpPostedFileBase file, Guid? customerId)
        {
            var customer = customerId.HasValue ? CustomerService.GetCustomer(customerId.Value) : CustomerContext.CurrentCustomer;
            if (customer == null)
                return Json(false);

            var result = new UploadAvatar(customer, file).Execute();

            return result != null
                ? Json(new { result = true, file = result })
                : Json(new { result = false, error = T("Admin.Error.InvalidImageFormat") });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAvatarCropped(string name, string base64String, Guid? customerId)
        {
            var customer = customerId.HasValue ? CustomerService.GetCustomer(customerId.Value) : CustomerContext.CurrentCustomer;
            if (customer == null)
                return Json(false);

            var result = new UploadAvatarCropped(customer, name, base64String).Execute();

            return result != null
                ? Json(new { result = true, file = result })
                : Json(new { result = false, error = T("Admin.Error.InvalidImageFormat") });
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAvatarByUrl(string url, Guid? customerId)
        {
            var customer = customerId.HasValue ? CustomerService.GetCustomer(customerId.Value) : CustomerContext.CurrentCustomer;
            if (customer == null)
                return Json(false);

            // Сохраняем картинку во временную папку из-за того что js не может cors
            var result = new UploadAvatarByUrl(customer, url).Execute();

            return result != null
                ? Json(new { result = true, file = result })
                : Json(new { result = false, error = T("Admin.Error.InvalidImageFormat") });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAvatar(Guid? customerId)
        {
            var customer = customerId.HasValue ? CustomerService.GetCustomer(customerId.Value) : CustomerContext.CurrentCustomer;
            if (customer == null)
                return Json(false);

            var result = new DeleteAvatar(customer).Execute();

            return result != null
                ? Json(new { result = true, file = result })
                : Json(new { result = false, error = T("Admin.Error.InvalidImageFormat") });
        }

        [HttpPost]
        public JsonResult GetAvatar(Guid? customerId)
        {
            var customer = customerId.HasValue ? CustomerService.GetCustomer(customerId.Value) : CustomerContext.CurrentCustomer;
            if (customer == null || customer.Avatar.IsNullOrEmpty())
                return Json(null);

            return Json(FoldersHelper.GetPath(FolderType.Avatar, customer.Avatar, false));
        }

        #endregion

        #region Images

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadImageByUrl(string url)
        {
            var result = new UploadImageByUrl(url).Execute();
            return result != null
                ? Json(new { result = true, file = result })
                : Json(new { result = false, error = T("Admin.Error.InvalidImageFormat") });
        }

        #endregion

        [HttpGet]
        public JsonResult GetLastStatistics()
        {
            return Json(new GetLastStastics().Execute());
        }

        public JsonResult GenerateUrl(string name)
        {
            if (name == null)
                name = "";

            var url = StringHelper.TransformUrl(StringHelper.Translit(name.Trim().Reduce(150)));

            return Json(url);
        }
        
        public ActionResult MetaVariablesDescription(MetaType type, bool showInstruction = true)
        {
            var model = new MetaVariablesDescriptionModel
            {
                MetaVariables = MetaInfoService.GetMetaVariables(type),
                ShowInstruction = showInstruction
            };
            return PartialView(model);
        }
        
        public MvcHtmlString MetaVariablesComplete(MetaType type)
        {
            return new MvcHtmlString(JsonConvert.SerializeObject(MetaInfoService.GetMetaVariables(type).Select(x => x.Value)));
        }


        public ActionResult Oldversion()
        {
            CommonHelper.SetCookie("oldadmin", "true", new TimeSpan(365, 0, 0, 0, 0), false);
            return Redirect("~/admin");
        }

        public ActionResult PictureUploader(PictureUploader model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Counter(bool force = false)
        {
            TrialService.TrackVisitEvent();

            if ((TrialService.IsTrialEnabled && CustomerContext.CurrentCustomer.IsAdmin) || force)
                return Content(TrialService.TrialCounter);
            else
                return new EmptyResult();
        }

        [Obsolete]
        [ChildActionOnly]
        public MvcHtmlString FilesHelpText(EAdvantShopFileTypes type, string maxFileSize)
        {
            var model = FileHelpers.GetFilesHelpText(type, maxFileSize);
            return new MvcHtmlString(model);
        }

        [ChildActionOnly]
        public MvcHtmlString FilesHelpTextByFileType(EFileType type, string maxFileSize)
        {
            var model = FileHelpers.GetFilesHelpText(type, maxFileSize);
            return new MvcHtmlString(model);
        }

        [ChildActionOnly]
        public ActionResult Notification()
        {
            var haveAccess = AdminInformerService.HaveAccess(CustomerContext.CurrentCustomer);
            if (!haveAccess)
                return new EmptyResult();

            return PartialView();
        }

        [HttpGet]
        public JsonResult GetNotifications(int? notify)
        {
            if (notify != null)
            {
                var informer = AdminInformerService.Get(notify.Value);
                if (informer != null)
                    if (informer.EntityId != null)
                        AdminInformerService.SetSeen(informer.Type, informer.EntityId.Value);
                    else
                        AdminInformerService.SetSeen(informer.Id);
            }

            var date = DateTime.Now.AddDays(-3);
            var events =
                AdminInformerService.GetLast(5)
                    .Where(x => !x.Seen || x.CreatedDate >= date)
                    .Select(LeadEventModel.GetLeadEventModel)
                    .Where(x => x != null)
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.InformerId,
                        x.Title,
                        x.Message,
                        x.UserPhoto,
                        x.Seen,
                        Type = x.Type.ToString().ToLower(),
                        Link = x.InformerLink
                    })
                    .ToList();

            var unseenCount = AdminInformerService.GetUnseenCount();

            return Json(new { unseenCount, events });
        }

        [HttpPost]
        public JsonResult NotifyAll()
        {
            var haveAccess = AdminInformerService.HaveAccess(CustomerContext.CurrentCustomer);
            if (!haveAccess)
                return Json(false);

            foreach (var informer in AdminInformerService.GetListUnSeen())
                AdminInformerService.SetSeen(informer.Id);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Common_SetAllNotificationsSeen);

            return Json(true);
        }

        public ActionResult AllNotifications()
        {
            var haveAccess = AdminInformerService.HaveAccess(CustomerContext.CurrentCustomer);
            if (!haveAccess)
                return new EmptyResult();

            var model = new GetInformers().Execute();

            SetMetaInformation(T("Admin.Shared.Notifications"));
            SetNgController(NgControllers.NgControllersTypes.InformersCtrl);

            return View(model);
        }

        public JsonResult GetSocialEnabled(string type)
        {
            var result = false;
            switch (type)
            {
                case "vk":
                    result = VkApiService.IsVkActive();
                    break;
                case "instagram":
                    result = Instagram.Instance.IsActive();
                    break;
                case "fb":
                    result = new FacebookApiService().IsActive();
                    break;
                case "telegram":
                    result = new TelegramApiService().IsActive();
                    break;
                case "ok":
                    result = OkApiService.IsActive();
                    break;
            }
            return Json(new { result });
        }

        public ActionResult GetShowCaseList()
        {
            var dashboard = SalesChannelService.GetByType(ESalesChannelType.Dashboard);
            if (dashboard == null || !dashboard.Enabled)
                return new EmptyResult();
            var result = new Dictionary<string, string>();

            var shopUrl = SettingsMain.IsTechDomainsReady 
                ? SettingsMain.SiteUrl
                : UrlService.GetUrl().ToLower().TrimEnd('/');

            var lpDomainService = new LpDomainService();
            var funnelMainUrls = lpDomainService
                .GetList()
                .Where(x => x.IsMain)
                .Select(x => new KeyValuePair<string, string>(x.DomainUrl, x.DomainUrl))
                .ToList();

            if (funnelMainUrls.All(x => x.Key != SettingsMain.SiteUrlPlain))
            {
                result.Add(shopUrl, !SettingsMain.StoreActive ? SettingsMain.SiteUrlPlain : T("Admin.Common.TopMenu.ShopWindow"));
            }

            foreach (var item in funnelMainUrls.Where(item => !result.ContainsKey(item.Key)))
            {
                result.Add(item.Key, item.Value);
            }

            return PartialView(result);
        }

        public ActionResult Back(BackViewModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SaasWarningMessageClose(BackViewModel model)
        {
            CommonHelper.HideSaasWarning();
            return JsonOk();
        }

        public ActionResult Header(HeaderViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult MobileVersionNotExist()
        {
            if (!MobileHelper.IsMobileAdmin() || CommonHelper.GetCookieString("mobileVersionNotExist") == "true")
            {
                return new EmptyResult();
            }
            
            CommonHelper.SetCookie("mobileVersionNotExist", "true");
            
            return PartialView();
        }
    }
}
