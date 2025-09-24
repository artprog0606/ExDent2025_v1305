using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.DownloadableContent;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Home;
using AdvantShop.Web.Admin.Models.Home;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Admin.ViewModels.Customers;
using AdvantShop.Track;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Cms.Menus;

namespace AdvantShop.Web.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        // GET: Admin/Home        
        public ActionResult Index()
        {
            switch (SettingsMain.AdminStartPage)
            {
                case EAdminStartPage.Default:
                    if (!SettingsCongratulationsDashboard.SkipCongratulationsDashboard && !SettingsCongratulationsDashboard.AllDone)
                        return RedirectToAction("CongratulationsDashboard");
                    
                    var salesChannel = SalesChannelService.GetList().FirstOrDefault(x => x.IsShowInMenu());
                    var menuItemReplace = AdminMenuModel.FromSalesChannel(salesChannel);
                    if (menuItemReplace != null && menuItemReplace.Action.IsNotEmpty() && menuItemReplace.Controller.IsNotEmpty())
                        return RedirectToAction(menuItemReplace.Action, menuItemReplace.Controller, menuItemReplace.RouteDictionary);

                    SettingsMain.AdminStartPage = EAdminStartPage.Orders;
                    return RedirectToAction("Index", "Orders");
                case EAdminStartPage.Orders:
                    return RedirectToAction("Index", "Orders");

                case EAdminStartPage.Leads:
                    if (SettingsCrm.CrmActive)
                        return RedirectToAction("Index", "Leads");

                    SettingsMain.AdminStartPage = EAdminStartPage.Orders;
                    return RedirectToAction("Index", "Orders");

                case EAdminStartPage.Dashboard:

                    // if (SalesChannelService.IsFirstTimeCreateStore())
                    // {
                    //     //return RedirectToAction("TrialFirstSession", "Dashboard");
                    //     return RedirectToAction("CreateSite", "Dashboard");
                    // }
                    if (!SettingsCongratulationsDashboard.SkipCongratulationsDashboard && !SettingsCongratulationsDashboard.AllDone)
                    {
                        return RedirectToAction("CongratulationsDashboard");
                    }
                    
                    var dashboard = SalesChannelService.GetByType(ESalesChannelType.Dashboard);
                    if (dashboard != null && dashboard.Enabled)
                        return RedirectToAction("Index", "Dashboard");

                    SettingsMain.AdminStartPage = EAdminStartPage.Orders;
                    return RedirectToAction("Index", "Orders");

                case EAdminStartPage.Tasks:
                    if (SettingsTasks.TasksActive)
                        return RedirectToAction("Index", "Tasks");

                    SettingsMain.AdminStartPage = EAdminStartPage.Orders;
                    return RedirectToAction("Index", "Orders");
            }

            if (SettingsMain.StoreActive)
                return RedirectToAction("Desktop");

            if (SettingProvider.GetSqlSettingValue(EProviderSetting.ActiveLandingPage).TryParseBool())
                return RedirectToAction("Index", "Funnels");

            if (SettingsCrm.CrmActive)
                return RedirectToAction("Index", "Leads");

            if (SettingsTasks.TasksActive)
                return RedirectToAction("Index", "Tasks");

            if (SettingsMain.BonusAppActive)
                return RedirectToAction("Index", "Cards");

            if (SettingsMain.BookingActive)
                return RedirectToAction("Index", "Booking");

            return RedirectToAction("Index", "Settings");
        }

        public ActionResult Desktop()
        {
            var customer = CustomerContext.CurrentCustomer;
            if (customer != null 
                && customer.IsModerator
                && !customer.HasRoleAction(RoleAction.Desktop))
            {
                if (customer.HasRoleAction(RoleAction.Orders))
                    return RedirectToAction("Index", "Orders");

                if (SettingsCrm.CrmActive && customer.HasRoleAction(RoleAction.Crm))
                    return RedirectToAction("Index", "Leads");

                if (customer.HasRoleAction(RoleAction.Customers))
                    return RedirectToAction("Index", "Customers");

                if (customer.HasRoleAction(RoleAction.Catalog))
                    return RedirectToAction("Index", "Catalog");

                if (SettingsTasks.TasksActive && customer.HasRoleAction(RoleAction.Tasks))
                    return RedirectToAction("Index", "Tasks");

                if (SettingsMain.BookingActive && customer.HasRoleAction(RoleAction.Booking))
                    return RedirectToAction("Index", "Booking");

                if (customer.HasRoleAction(RoleAction.Analytics))
                    return RedirectToAction("Index", "Analytics");

                if (customer.HasRoleAction(RoleAction.Modules))
                    return RedirectToAction("Index", "Modules");

                if (customer.HasRoleAction(RoleAction.Settings))
                    return RedirectToAction("Index", "Settings");

                if (SettingsMain.StoreActive && customer.HasRoleAction(RoleAction.Store))
                    return RedirectToAction("Index", "Design");

                if (SettingsMain.StoreActive && customer.HasRoleAction(RoleAction.Store))
                    return RedirectToAction("Index", "Design");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.ActiveLandingPage).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Landing))
                    return RedirectToAction("Index", "Funnels");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.TriggersActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Triggers))
                    return RedirectToAction("Index", "Triggers");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.YandexChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Yandex))
                    return RedirectToAction("IndexCsv", "ExportFeeds");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.GoogleChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Google))
                    return RedirectToAction("IndexGoogle", "ExportFeeds");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.AvitoChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Avito))
                    return RedirectToAction("IndexAvito", "ExportFeeds");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.VkChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Vk))
                    return RedirectToAction("Index", "Vk");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.ActiveBonusSystem).TryParseBool()
                    && customer.HasRoleAction(RoleAction.BonusSystem))
                    return RedirectToAction("Index", "Cards");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.PartnersActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Partners))
                    return RedirectToAction("Index", "Partners");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.ResellerChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Reseller))
                    return RedirectToAction("IndexReseller", "ExportFeeds");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.InstagramChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Instagram))
                    return RedirectToAction("Index", "Instagram");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.OkChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Ok))
                    return RedirectToAction("Index", "Ok");

                if (SettingProvider.GetSqlSettingValue(EProviderSetting.TelegramChannelActive).TryParseBool()
                    && customer.HasRoleAction(RoleAction.Telegram))
                    return RedirectToAction("Index", "Telegram");
                
                return RedirectToAction("Achievements", "Home");
            }

            if (!SettingsCongratulationsDashboard.SkipCongratulationsDashboard &&
                !SettingsCongratulationsDashboard.AllDone)
            {
                return RedirectToAction("CongratulationsDashboard");
            }

            SetMetaInformation(null, string.Empty);
            SetNgController(NgControllers.NgControllersTypes.HomeCtrl);

            var model = new AdminHomeViewModel() { ActionText = ShopActionsService.GetLast() };
            return View("Index", model);
        }

        #region Congratulations Dashboard

        public ActionResult CongratulationsDashboard(bool reset = false)
        {
            if (reset)
            {
                SettingsCongratulationsDashboard.SkipCongratulationsDashboard = false;
                SettingsCongratulationsDashboard.StoreInfoDone = false;
                SettingsCongratulationsDashboard.ProductDone = false;
                SettingsCongratulationsDashboard.DesignDone = false;
                SettingsCongratulationsDashboard.DomainDone = false;
                
                return RedirectToAction("CongratulationsDashboard", new { istest = true });
            }
            
            if (SettingsCongratulationsDashboard.SkipCongratulationsDashboard)
                return RedirectToAction("Index");

            if (SettingsCongratulationsDashboard.AllDone)
            {
                SettingsCongratulationsDashboard.SkipCongratulationsDashboard = true;
                return RedirectToAction("Index");
            }

            SetNgController(NgControllers.NgControllersTypes.CongratulationsDashboardCtrl);
            SetMetaInformation(null, string.Empty);

            return View();
        }
        
        public ActionResult Achievements()
        {
            if (!SettingsAchievements.IsAchievementsEnabled)
            {
                return Error404();
            }
            
            SetNgController(NgControllers.NgControllersTypes.AchievementsCtrl);
            SetMetaInformation(T("Admin.Common.TopPanel.Achievements"));

            return View();
        }

        [HttpGet]
        public JsonResult GetCongratulationsDashboardData()
        {  
            var model = new GetCongratulationsDashboard().Execute();          
            return JsonOk(model);
        }
        
        [HttpGet]
        public JsonResult GetAchievementsData()
        {  
            try
            {
                var model = new GetAchievementsHandler().Execute();          
                return JsonOk(model);
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return JsonError();
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveStoreInfo(StoreInfoModel model)
        {
            return ProcessJsonResult(new SaveStoreInfoHandler(model));
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult TrackCongratulationsDashboardEvents(ETrackEvent trackEvent)
        {
            switch (trackEvent)
            {
                case ETrackEvent.Dashboard_ClickDesignButton:
                    if (!SettingsCongratulationsDashboard.DesignDone) 
                        SettingsCongratulationsDashboard.DesignDone = true;
                    break;
            }
            TrackService.TrackEvent(trackEvent);            
            return JsonOk();
        }
        

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SkipCongratulationsDashboard()
        {
            SettingsCongratulationsDashboard.SkipCongratulationsDashboard = true;
            TrackService.TrackEvent(ETrackEvent.Dashboard_SkipDashboard);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetNotFirstVisit()
        {
            SettingsCongratulationsDashboard.NotFirstVisit = true;
            return JsonOk();
        }

        #endregion

        [ChildActionOnly]
        [Auth(RoleAction.Orders)]
        public ActionResult OrdersDashboard()
        {
            return PartialView(new GetOrdersDasboard().Execute());
        }

        [ChildActionOnly]
        [Auth(RoleAction.Analytics)]
        public ActionResult OrderSourcesDashboard()
        {
            return PartialView(new GetOrderSourcesDasboard().Execute());
        }

        [ChildActionOnly]
        public ActionResult LearningDashboard()
        {
            var model = new GetLearningDasboard().Execute();
            if (model.News.Count == 0)
                return new EmptyResult();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RecommendationsDashboard()
        {
            var model = new GetRecommendationsDasboard().Execute();
            if (model.News.Count == 0)
                return new EmptyResult();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult PartnersDashboard()
        {
            var model = new GetPartnersDasboard().Execute();
            if (model.News.Count == 0)
                return new EmptyResult();

            return PartialView(model);
        }

        [ChildActionOnly]
        [Auth(RoleAction.Analytics)]
        public ActionResult SalesPlanDashboard()
        {
            return PartialView(new GetSalesPlanDasboard().Execute());
        }

        [ChildActionOnly]
        [Auth(RoleAction.Analytics)]
        public ActionResult StatisticsDashboard()
        {
            return PartialView(new GetStatisticsDasboard().Execute());
        }

        [HttpPost]
        [Auth(RoleAction.Orders)]
        public ActionResult SaveStatisticsSettings(StatisticsDashboardModelSetting model)
        {
            new SaveStatisticsDasboard(model).Execute();
            return RedirectToAction("Index");
        }


        [ChildActionOnly]
        [Auth(RoleAction.Analytics)]
        public ActionResult OrderGraphDashboard()
        {
            return PartialView(new GetOrderGraphDashboard().Execute());
        }
        
        [ChildActionOnly]
        [Auth(RoleAction.Orders)]
        public ActionResult SalesChannelsGraphDashboard()
        {
            return PartialView();
        }
        
        [HttpGet, Auth(RoleAction.Orders)]
        public ActionResult GetSalesChannelsGraphDashboard()
        {
            return Json(null);
        }

        [HttpGet, Auth(RoleAction.Orders)]
        public ActionResult GetSalesChannelsGraphDashboardGraph(SalesChannelsGraphDashboardFilter filter, string dateFrom, string dateTo)
        {
            if (filter == null) filter = new SalesChannelsGraphDashboardFilter();
            
            if (!string.IsNullOrEmpty(dateFrom))
                filter.From = dateFrom.TryParseDateTime();

            if (!string.IsNullOrEmpty(dateTo))
            {
                var date = dateTo.TryParseDateTime();
                filter.To = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            }

            return Json(new GetSalesChannelsGraphDashboardGrid(filter).GetGraph());
        }

        [ChildActionOnly]
        [Auth(RoleAction.Orders)]
        public ActionResult LastOrdersDashboard(string status)
        {
            return PartialView();
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetLastOrders(string status)
        {
            var model = new GetLastOrdersDashboard(status).Execute();
            return Json(new { DataItems = model });
        }

        [ChildActionOnly]
        [Auth(RoleAction.Customers)]
        public ActionResult BirthdayDashboard(string status)
        {
            return PartialView(new GetBirthdayDasboard().Execute());
        }


        [ChildActionOnly]
        public ActionResult UserInformation(UserInfoPopup model)
        {
            if (!Trial.TrialService.IsTrialEnabled || CustomerContext.CurrentCustomer.IsVirtual)
                return new EmptyResult();

            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetUserInformation()
        {
            return Json(new GetUserInformationModel().Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveUserInformation(AdditionClientInfo data)
        {
            return Json(new
            {
                result = new GetUserInformationModel().Save(data),
                fio = CustomerContext.CurrentCustomer.FirstName + " " + CustomerContext.CurrentCustomer.LastName
            });
        }

        [HttpGet]
        public JsonResult GetAdminShopName()
        {
            return Json(new { name = SettingsMain.AdminShopName });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveAdminShopName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                SettingsMain.AdminShopName = StringHelper.RemoveHTML(name).Reduce(100);
                TrackService.TrackEvent(ETrackEvent.Core_Common_Head_ShopNameChanged);
            }
            return Json(new { name = SettingsMain.AdminShopName });
        }

        [HttpPost]
        public JsonResult HideActionBlock(int actionId)
        {
            var result = ShopActionsService.HideAction(actionId);
            return Json(new {result});
        }

        public JsonResult GetAdvReferralInfo()
        {
            var model = new GetAdvReferralInfoHandler().Execute();
            
            return model == null ? JsonError() : JsonOk(model);
        }
    }
}