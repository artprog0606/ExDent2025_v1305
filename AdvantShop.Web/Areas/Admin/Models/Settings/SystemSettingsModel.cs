using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SystemSettingsModel
    {
        public SystemSettingsModel()
        {
            Features = new Dictionary<string, bool>();
        }

        #region common

        //public string AdditionalHeadMetaTag { get; set; }

        public string LogoImageAlt { get; set; }
        public string AdminDateFormat { get; set; }
        public string ShortDateFormat { get; set; }
        public bool EnablePhoneMask { get; set; }

        public long FilesSize { get; set; }
        public string FilesSizeFormatted
        {
            get { return FileHelpers.FileSize(FilesSize); }
        }

        //public string SiteLanguage { get; set; }
        //public List<SelectListItem> Languages { get; set; }
        public EAdminStartPage AdminStartPage { get; set; }
        public List<SelectListItem> AdminStartPages { get; set; }

        public long ImageQuality { get; set; }

        public string SiteLanguage { get; set; }
        public List<SelectListItem> Languages { get; set; }
        #endregion

        #region captcha

        public bool EnableCaptcha { get; set; }
        public bool EnableCaptchaInCheckout { get; set; }
        public bool EnableCaptchaInRegistration { get; set; }
        public bool EnableCaptchaInPreOrder { get; set; }
        public bool EnableCaptchaInGiftCerticate { get; set; }
        public bool EnableCaptchaInFeedback { get; set; }
        public bool EnableCaptchaInProductReview { get; set; }
        public bool EnableCaptchaInBuyInOneClick { get; set; }
        public bool EnableCaptchaInSmsConfirmation { get; set; }
        public int CaptchaMode { get; set; }
        public List<SelectListItem> CaptchaModes { get; set; }
        public int CaptchaLength { get; set; }

        #endregion

        #region oauth
        public bool GoogleActive { get; set; }
        public bool MailActive { get; set; }
        public bool YandexActive { get; set; }
        public bool VkontakteActive { get; set; }
        public bool FacebookActive { get; set; }
        public bool OdnoklassnikiActive { get; set; }
        public bool AuthByCodeActive { get; set; }
        public int AuthByCodeMethod { get; set; }
        public List<SelectListItem> AuthByCodeMethods { get; set; }

        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }

        public string VkontakeClientId { get; set; }
        public string VkontakeSecret { get; set; }

        public string FacebookClientId { get; set; }
        public string FacebookApplicationSecret { get; set; }

        public string OdnoklassnikiClientId { get; set; }
        public string OdnoklassnikiPublicApiKey { get; set; }
        public string OdnoklassnikiSecret { get; set; }

        public string MailAppId { get; set; }
        public string MailClientSecret { get; set; }

        public string YandexClientId { get; set; }
        public string YandexClientSecret { get; set; }

        public string CallbackUrl { get; set; }
        public string VkIdCallbackUrl { get; set; }
        
        public int AuthMethod { get; set; }
        public List<SelectListItem> AuthMethods { get; set; }
        #endregion

        #region License
        public string LicKey { get; set; }
        public bool ActiveLic { get; set; }
        #endregion

        #region sitemap        
        /*public string SiteMapFileXmlLink { get; set; }
        public string SiteMapFileXmlLinkText { get; set; }
        public string SiteMapFileXmlDate { get; set; }

        public string SiteMapFileHtmlLink { get; set; }
        public string SiteMapFileHtmlLinkText { get; set; }
        public string SiteMapFileHtmlDate { get; set; }*/
        
        public List<SiteMapDomainModel> SiteMapDomainModels { get; set; }
        
        #endregion

        #region customers notifications

        //public bool ShowUserAgreementText { get; set; }
        //public string UserAgreementText { get; set; }
        //public bool ShowCookiesPolicyMessage { get; set; }
        //public string CookiesPolicyMessage { get; set; }

        #endregion

        #region Applications

        //public bool StoreActive { get; set; }
        //public bool LandingActive { get; set; }
        //public bool CrmActive { get; set; }
        //public bool TasksActive { get; set; }
        //public bool BonusActive { get; set; }
        //public bool BookingActive { get; set; }
        //public bool TriggersActive { get; set; }
        //public bool PartnersActive { get; set; }

        #endregion

        #region CssEditor

        public string CssEditorText { get; set; }

        #endregion

        public bool IsSaas { get; set; }
        public bool IsTrial { get; set; }


        public bool ShowImageSearchEnabled { get; set; }

        public string AdminAreaColorScheme { get; set; }
        public List<SelectListItem> AdminAreaColorSchemes { get; set; }

        public string AdminAreaTemplate { get; set; }
        public List<SelectListItem> AdminAreaTemplates { get; set; }

        public bool TrackProductChanges { get; set; }


        #region Sms



        #endregion

        #region Experimental Features

        public bool EnableExperimentalFeatures { get; set; }
        public Dictionary<string, bool> Features { get; set; }

        //public bool EnableProductCategoriesAutoMapping { get; set; }

        #endregion
        
        public bool ShowCustomCopyright { get; set; }
        public string CopyrightMode { get; set; }
        public string CopyrightText { get; set; }
        public bool DisableCopyrightEdit => CopyrightService.IsCopyrightLocked;

        public bool AllowSearchBotsFromOtherCountries { get; set; }
        public bool ProhibitRegistration { get; set; }
        
        public bool VkIdActive { get; set; }
        public string VkIdClientId { get; set; }
    }
}