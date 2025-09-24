using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Web.Infrastructure.Controllers
{
    public class NgControllers
    {
        public enum NgControllersTypes
        {
            // client
            [Description("app")] AppCtrl,
            [Description("catalog")] CatalogCtrl,
            [Description("checkout")] CheckOutCtrl,
            [Description("checkoutSuccess")] CheckOutSuccessCtrl,
            [Description("product")] ProductCtrl,
            [Description("feedback")] FeedbackCtrl,
            [Description("home")] HomeCtrl,
            [Description("myaccount")] MyAccountCtrl,
            [Description("preorder")] PreorderCtrl,
            [Description("giftcertificate")] GiftCertificateCtrl,
            [Description("brand")] BrandCtrl,
            [Description("staticPage")] StaticPageCtrl,
            [Description("brandsList")] BrandsListCtrl,
            [Description("wishlistPage")] WishlistPageCtrl,
            [Description("checkout")] BillingCtrl,
            [Description("forgotPassword")] ForgotPasswordCtrl,
            [Description("registrationPage")] RegistrationPageCtrl,
            [Description("cartPage")] CartPageCtrl,
            [Description("comparePage")] ComparePageCtrl,
            [Description("error")] ErrorCtrl,
            [Description("login")] LoginCtrl,
            [Description("productList")] ProductListCtrl,
            [Description("bonusPage")] BonusPageCtrl,

            // admin panel
            [Description("settingsCrm")] SettingsCrmCtrl,
            [Description("bonuses")] BonusesCtrl,
            [Description("calls")] CallsCtrl,
            [Description("category")] CategoryCtrl,
            [Description("lead")] LeadCtrl,
            [Description("leads")] LeadsCtrl,
            [Description("modules")] ModulesCtrl,
            [Description("module")] ModuleCtrl,
            [Description("mainpageproducts")] MainPageProductsCtrl,
            [Description("mailSettings")] MailSettingsCtrl,
            [Description("orders")] OrdersCtrl,
            [Description("order")] OrderCtrl,
            [Description("orderstatuses")] OrderStatusesCtrl,
            [Description("ordersources")] OrderSourcesCtrl,
            [Description("orderReviews")] OrderReviewsCtrl,
            [Description("customers")] CustomersCtrl,
            [Description("customer")] CustomerCtrl,
            [Description("customerView")] CustomerViewCtrl,
            [Description("customergroups")] CustomerGroupsCtrl,
            [Description("customerSegments")] CustomerSegmentsCtrl,
            [Description("customerSegment")] CustomerSegmentCtrl,
            [Description("customerTags")] CustomerTagsCtrl,
            [Description("tasks")] TasksCtrl,
            [Description("taskgroups")] TaskGroupsCtrl,
            [Description("taskstatuses")] TaskStatusesCtrl,
            [Description("properties")] PropertiesCtrl,
            [Description("propertyvalues")] PropertyValuesCtrl,
            [Description("productlists")] ProductListsCtrl,
            [Description("design")] DesignCtrl,
            [Description("csseditor")] CssEditorCtrl,
            [Description("sizes")] SizesCtrl,
            [Description("colors")] ColorsCtrl,
            [Description("tags")] TagsCtrl,
            [Description("reviews")] ReviewsCtrl,
            [Description("discountsPriceRange")] DiscountsPriceRangeCtrl,
            [Description("discountsByTime")] DiscountsByTimeCtrl,
            [Description("coupons")] CouponsCtrl,
            [Description("tariffs")] TariffsCtrl,
            [Description("menus")] MenusCtrl,
            [Description("news")] NewsCtrl,
            [Description("newsItem")] NewsItemCtrl,
            [Description("newsCategory")] NewsCategoryCtrl,
            [Description("carouselPage")] CarouselPageCtrl,
            [Description("files")] FilesCtrl,
            [Description("staticPages")] StaticPagesCtrl,
            [Description("staticBlock")] StaticBlockCtrl,
            [Description("certificates")] CertificatesCtrl,
            [Description("subscription")] SubscriptionCtrl,
            [Description("shippingMethod")] ShippingMethodCtrl,
            [Description("paymentMethod")] PaymentMethodCtrl,
            [Description("grades")] GradesCtrl,
            [Description("cards")] CardsCtrl,
            [Description("notificationtemplates")] NotificationTemplatesCtrl,
            [Description("rules")] RulesCtrl,
            [Description("landingPages")] LandingPagesCtrl,
            [Description("bookingJournal")] BookingJournalCtrl,
            [Description("bookingReservationResources")] BookingReservationResourcesCtrl,
            [Description("bookingCategories")] BookingCategoriesCtrl,
            [Description("bookingServices")] BookingServicesCtrl,
            [Description("bookingAffiliateSettings")] BookingAffiliateSettingsCtrl,
            [Description("bookingAnalytics")] BookingAnalyticsCtrl,
            [Description("informers")] InformersCtrl,
            [Description("congratulationsDashboard")] CongratulationsDashboardCtrl,
            [Description("achievements")] AchievementsCtrl,
            [Description("searchQueries")] SearchQueriesCtrl,
            [Description("triggers")] TriggersCtrl,
            [Description("triggerCategories")] TriggerCategoriesCtrl,
            [Description("triggerAnalytics")] TriggerAnalyticsCtrl,
            [Description("manualEmailing")] ManualEmailingCtrl,
            [Description("manualWithoutEmailing")] ManualWithoutEmailingCtrl,
            [Description("manualEmailings")] ManualEmailingsCtrl,
            [Description("emailingLog")] EmailingLogCtrl,
            [Description("partners")] PartnersCtrl,
            [Description("partnersReport")] PartnersReportCtrl,
            [Description("partnersPayoutReports")] PartnersPayoutReportsCtrl,
            [Description("partnerView")] PartnerViewCtrl,
            [Description("vkMarketExport")] VkMarketExportCtrl,
            [Description("okMarketExport")] OkMarketExportCtrl,
            [Description("domainsManage")] DomainsManageCtrl,
            [Description("shippingReplaceGeo")] ShippingReplaceGeoCtrl,
            [Description("priceRules")] PriceRulesCtrl,
            [Description("photoCategory")] PhotoCategoryCtrl,
            [Description("warehouse")] WarehouseCtrl,
            [Description("sizeChart")] SizeChartCtrl,

            // landing
            [Description("landings")] LandingsAdminCtrl,
            [Description("landingSite")] LandingSiteAdminCtrl,

            [Description("exportFeeds")] ExportFeedsCtrl,
            [Description("exportCategories")] ExportCategoriesCtrl,
            [Description("analytics")] AnalyticsCtrl,
            [Description("analyticsReport")] AnalyticsReportCtrl,
            [Description("analyticsFilter")] AnalyticsFilterCtrl,
            [Description("import")] ImportCtrl,
            [Description("funnelDetails")] FunnelDetailsCtrl,
            [Description("createFunnel")] CreateFunnelCtrl,
            [Description("dashboardSites")] DashboardSitesCtrl,
            [Description("createSite")] CreateSiteCtrl,
            [Description("storePage")] StorePageCtrl,

            //settings                                   
            [Description("settings")] SettingsCtrl,
            [Description("settingsCheckout")] SettingsCheckoutCtrl,
            [Description("settingsSeo")] SettingsSeoCtrl,
            [Description("settingsSystem")] SettingsSystemCtrl,
            [Description("settingsSocial")] SettingsSocialCtrl,
            [Description("settingsCatalog")] SettingsCatalogCtrl,
            [Description("settingsCoupons")] SettingsCouponsCtrl,
            [Description("settingsCustomers")] SettingsCustomersCtrl,
            [Description("settingsUsers")] SettingsUsersCtrl,
            [Description("settingsSearch")] SettingsSearchCtrl,
            [Description("settingsTasks")] SettingsTasksCtrl,
            [Description("settingsTelephony")] SettingsTelephonyCtrl,
            [Description("settingsBonus")] SettingsBonusCtrl,
            [Description("settingsTemplatesDocx")] SettingsTemplatesDocxCtrl,
            [Description("settingsPartners")] SettingsPartnersCtrl,
            [Description("settingsBooking")] SettingsBookingCtrl,
            [Description("settingsMobile")] SettingsMobileCtrl,
            [Description("settingsTemplate")] SettingsTemplateCtrl,
            [Description("vkMain")] VkMainCtrl,
            [Description("okMain")] OkMainCtrl,
            [Description("settingsWarehouses")] SettingsWarehousesCtrl,

            //partners
            [Description("customers"), StringName("CustomersCtrl")] PartnerCustomersCtrl,
            [Description("forgotPassword"), StringName("ForgotPasswordCtrl")] PartnerForgotPasswordCtrl,
            [Description("home"), StringName("HomeCtrl")] PartnerHomeCtrl,
            [Description("rewards"), StringName("RewardsCtrl")] PartnerRewardsCtrl,
            [Description("settings"), StringName("SettingsCtrl")] PartnerSettingsCtrl,
        }

        private static ConcurrentDictionary<string, string> _controllerlDescriptions = null;
        private static ConcurrentDictionary<string, string> _controllerlNames = null;

        public static string GetNgControllerInitString(NgControllersTypes controllerType)
        {
            if (_controllerlDescriptions == null)
                _controllerlDescriptions = new ConcurrentDictionary<string, string>();
            if (_controllerlNames == null)
                _controllerlNames = new ConcurrentDictionary<string, string>();

            string controller = controllerType.ToString();

            string description = null;
            if (!_controllerlDescriptions.TryGetValue(controller, out description))
            {
                var type = typeof(NgControllersTypes);
                var memInfo = type.GetMember(controllerType.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                description = ((DescriptionAttribute) attributes[0]).Description;

                _controllerlDescriptions.TryAdd(controller, description);            
            }

            string controllerName = null;
            if (!_controllerlNames.TryGetValue(controller, out controllerName))
            {
                var type = typeof(NgControllersTypes);
                var memInfo = type.GetMember(controllerType.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(StringNameAttribute), false);
                controllerName = attributes != null && attributes.Any() 
                    ? ((StringNameAttribute)attributes[0]).Value 
                    : controller;

                _controllerlNames.TryAdd(controller, controllerName);
            }

            return string.Format("data-ng-controller=\"{0} as {1}\"", controllerName, description);
        }
    }
}
