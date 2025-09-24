using AdvantShop.Core.Services.Localization;
using AdvantShop.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SEOSettingsModel
    {
        public SEOSettingsModel()
        {
            OrderStatuses = OrderStatusService.GetOrderStatuses()
                .Select(x => new SelectListItem() { Text = x.StatusName, Value = x.StatusID.ToString() }).ToList();
        }

        #region SEO settings
        public string ProductsDefaultTitle { get; set; }
        public string ProductsDefaultMetaKeywords { get; set; }
        public string ProductsDefaultMetaDescription { get; set; }
        public string ProductsDefaultH1 { get; set; }
        public string ProductsDefaultAdditionalDescription { get; set; }

        public string CategoriesDefaultTitle { get; set; }
        public string CategoriesDefaultMetaKeywords { get; set; }
        public string CategoriesDefaultMetaDescription { get; set; }
        public string CategoriesDefaultH1 { get; set; }

        public string TagsDefaultTitle { get; set; }
        public string TagsDefaultMetaKeywords { get; set; }
        public string TagsDefaultMetaDescription { get; set; }
        public string TagsDefaultH1 { get; set; }

        public string NewsDefaultTitle { get; set; }
        public string NewsDefaultMetaKeywords { get; set; }
        public string NewsDefaultMetaDescription { get; set; }
        public string NewsDefaultH1 { get; set; }


        public string CategoryNewsDefaultTitle { get; set; }
        public string CategoryNewsDefaultMetaKeywords { get; set; }
        public string CategoryNewsDefaultMetaDescription { get; set; }
        public string CategoryNewsDefaultH1 { get; set; }

        public string StaticPageDefaultTitle { get; set; }
        public string StaticPageDefaultMetaKeywords { get; set; }
        public string StaticPageDefaultMetaDescription { get; set; }
        public string StaticPageDefaultH1 { get; set; }

        public string BrandItemDefaultTitle { get; set; }
        public string BrandItemDefaultMetaKeywords { get; set; }
        public string BrandItemDefaultMetaDescription { get; set; }
        public string BrandItemDefaultH1 { get; set; }

        public string BrandsDefaultTitle { get; set; }
        public string BrandsDefaultH1 { get; set; }
        public string BrandsDefaultMetaKeywords { get; set; }
        public string BrandsDefaultMetaDescription { get; set; }

        public string MainPageProductsDefaultTitle { get; set; }
        public string MainPageProductsDefaultMetaKeywords { get; set; }
        public string MainPageProductsDefaultMetaDescription { get; set; }
        public string MainPageProductsDefaultH1 { get; set; }

        public string DefaultTitle { get; set; }
        public string DefaultMetaKeywords { get; set; }
        public string DefaultMetaDescription { get; set; }
        
        public string ShopsDefaultTitle { get; set; }
        public string ShopsDefaultMetaKeywords { get; set; }
        public string ShopsDefaultMetaDescription { get; set; }
        public string ShopsDefaultH1 { get; set; }
        
        public string BonusProgramDefaultTitle { get; set; }
        public string BonusProgramDefaultMetaKeywords { get; set; }
        public string BonusProgramDefaultMetaDescription { get; set; }
        public string BonusProgramDefaultH1 { get; set; }

        #endregion

        #region opengraph
        public bool OpenGraphEnabled { get; set; }
        public string OpenGraphFbAdmins { get; set; }
        public bool EnableCyrillicUrl { get; set; }
        #endregion

        #region Robots
        public string RobotsText { get; set; }
        #endregion

        #region GA
        public string GoogleAnalyticsNumber { get; set; }
        public bool GoogleAnalyticsEnableDemogrReports { get; set; }
        public bool GoogleAnalyticsSendOrderOnStatusChanged { get; set; }
        public int GoogleAnalyticsOrderStatusIdToSend { get; set; }
        public bool GoogleAnalyticsEnabled { get; set; }

        public bool GoogleAnalyticsApiEnabled { get; set; }
        public string GoogleAnalyticsAccountID { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }

        public bool UseGTM { get; set; }
        public string GTMContainerId { get; set; }
        public string GTMOfferIdType { get; set; }
        public Dictionary<string, string> GTMOfferIdTypes
        {
            get
            {
                return new Dictionary<string, string> {
                    { "id",  LocalizationService.GetResource("Admin.Settings.Checkout.ScriptsGTMOfferId") },
                    { "artno",  LocalizationService.GetResource("Admin.Settings.Checkout.ScriptsGTMOfferArtno")}
                };
            }
        }

        public List<SelectListItem> OrderStatuses { get; set; }

        #endregion

        #region 301 redirects

        public bool EnableRedirect301 { get; set; }
        public bool ShowWarningRobotsTxt { get; set; }

        #endregion

    }
}
