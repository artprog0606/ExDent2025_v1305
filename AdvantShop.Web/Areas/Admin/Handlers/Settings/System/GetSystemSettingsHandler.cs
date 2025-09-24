using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Design;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.SiteMaps;
using AdvantShop.Web.Admin.Models.Services;
using AdvantShop.Web.Admin.Models.Settings;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class GetSystemSettingsHandler
    {
        public SystemSettingsModel Execute()
        {
            var model = new SystemSettingsModel
            {
                LogoImageAlt = SettingsMain.LogoImageAlt,
                AdminDateFormat = SettingsMain.AdminDateFormat,
                ShortDateFormat = SettingsMain.ShortDateFormat,
                EnablePhoneMask = SettingsMain.EnablePhoneMask,

                EnableCaptcha = SettingsMain.EnableCaptcha,
                EnableCaptchaInCheckout = SettingsMain.EnableCaptchaInCheckout,
                EnableCaptchaInRegistration = SettingsMain.EnableCaptchaInRegistration,
                EnableCaptchaInFeedback = SettingsMain.EnableCaptchaInFeedback,
                EnableCaptchaInProductReview = SettingsMain.EnableCaptchaInProductReview,
                EnableCaptchaInPreOrder = SettingsMain.EnableCaptchaInPreOrder,
                EnableCaptchaInGiftCerticate = SettingsMain.EnableCaptchaInGiftCerticate,
                EnableCaptchaInBuyInOneClick = SettingsMain.EnableCaptchaInBuyInOneClick,
                EnableCaptchaInSmsConfirmation = SettingsMain.EnableCaptchaInSmsConfirmation,
                CaptchaMode = (int)SettingsMain.CaptchaMode,
                CaptchaLength = SettingsMain.CaptchaLength,

                GoogleActive = SettingsOAuth.GoogleActive,
                MailActive = SettingsOAuth.MailActive,
                YandexActive = SettingsOAuth.YandexActive,
                VkontakteActive = SettingsOAuth.VkontakteActive,
                VkIdActive = SettingsOAuth.VkIdActive,
                FacebookActive = SettingsOAuth.FacebookActive,
                OdnoklassnikiActive = SettingsOAuth.OdnoklassnikiActive,
                AuthByCodeActive = SettingsOAuth.AuthByCodeActive,
                AuthByCodeMethod = (int)SettingsOAuth.AuthByCodeMethod,
                GoogleClientId = SettingsOAuth.GoogleClientId,
                GoogleClientSecret = SettingsOAuth.GoogleClientSecret,
                VkontakeClientId = SettingsOAuth.VkontakeClientId,
                VkontakeSecret = SettingsOAuth.VkontakeSecret,
                VkIdClientId = SettingsOAuth.VkIdClientId,
                OdnoklassnikiClientId = SettingsOAuth.OdnoklassnikiClientId,
                OdnoklassnikiSecret = SettingsOAuth.OdnoklassnikiSecret,
                OdnoklassnikiPublicApiKey = SettingsOAuth.OdnoklassnikiPublicApiKey,
                FacebookClientId = SettingsOAuth.FacebookClientId,
                FacebookApplicationSecret = SettingsOAuth.FacebookApplicationSecret,
                MailAppId = SettingsOAuth.MailAppId,
                MailClientSecret = SettingsOAuth.MailClientSecret,
                YandexClientId = SettingsOAuth.YandexClientId,
                YandexClientSecret = SettingsOAuth.YandexClientSecret,

                CallbackUrl = StringHelper.ToPuny(SettingsMain.SiteUrl).TrimEnd('/') + "/login",
                VkIdCallbackUrl = StringHelper.ToPuny(SettingsMain.SiteUrl).TrimEnd('/') + "/auth/vkid",
                
                AuthMethod = (int)SettingsOAuth.AuthMethod,

                IsSaas = SaasDataService.IsSaasEnabled,
                IsTrial = Trial.TrialService.IsTrialEnabled,
                LicKey = SettingsLic.LicKey,
                ActiveLic = SettingsLic.ActiveLic,

                FilesSize = SettingsMain.CurrentFilesStorageSize,

                ShowImageSearchEnabled = SettingsCatalog.ShowImageSearchEnabled,

                TrackProductChanges = SettingsMain.TrackProductChanges,

                AdminStartPage = SettingsMain.AdminStartPage,

                ImageQuality = SettingsMain.ImageQuality,
                SiteLanguage = SettingsMain.Language,
                
                ShowCustomCopyright = SettingsDesign.ShowCustomCopyright,
                CopyrightText = SettingsDesign.CopyrightText,
                
                ProhibitRegistration = SettingsMain.RegistrationIsProhibited
            };

            model.Languages = LanguageService.GetList().Select(x => new SelectListItem { Text = x.Name, Value = x.LanguageCode }).ToList();
            if (model.Languages.Count == 0)
                model.Languages.Add(new SelectListItem { Text = "Нет языков" });

            model.CaptchaModes = new List<SelectListItem>();
            foreach (CaptchaMode value in Enum.GetValues(typeof(CaptchaMode)))
            {
                model.CaptchaModes.Add(new SelectListItem()
                {
                    Text = value.Localize(),
                    Value = ((int)value).ToString()
                });
            }

            model.EnableExperimentalFeatures = SettingsFeatures.EnableExperimentalFeatures;
            model.Features = Enum.GetValues(typeof(EFeature)).Cast<EFeature>()
                .ToDictionary(feature => feature.ToString(), feature => SettingsFeatures.IsFeatureEnabled(feature));

            model.AllowSearchBotsFromOtherCountries = SettingsGeneral.AllowSearchBotsFromOtherCountries;

            //model.Languages = new List<SelectListItem>();

            //var languagesList = LanguageService.GetList();
            //if (languagesList != null)
            //{
            //    model.Languages.AddRange(languagesList.Select(item => new SelectListItem { Text = item.Name, Value = item.LanguageCode }));
            //}
            //else
            //{
            //    model.Languages.Add(new SelectListItem { Text = "Нет языков" });
            //}


            var sitemapFilePathXml = SettingsGeneral.AbsolutePath + "sitemap.xml";
            var sitemapFilePathHtml = SettingsGeneral.AbsolutePath + "sitemap.html";
            
            if (model.SiteMapDomainModels == null)
            {
                model.SiteMapDomainModels = new List<SiteMapDomainModel>();
            }

            
            var siteMapFileXmlLinkText = SettingsMain.SiteUrl + "/sitemap.xml";
            var siteMapFileHtmlLinkText = SettingsMain.SiteUrl + "/sitemap.html";
            
            model.SiteMapDomainModels.Add(new SiteMapDomainModel()
            {
                SiteMapFileXmlLinkText = siteMapFileXmlLinkText,
                SiteMapFileXmlLink = siteMapFileXmlLinkText + "?rnd=" + (new Random().Next(10000)),
                SiteMapFileHtmlLinkText = siteMapFileHtmlLinkText,
                SiteMapFileHtmlLink = siteMapFileHtmlLinkText + "?rnd=" + (new Random().Next(10000))
            });
            
            
            /*if (File.Exists(sitemapFilePathXml))
            {
                model.SiteMapFileXmlDate = Localization.Culture.ConvertDate(new FileInfo(sitemapFilePathXml).LastWriteTime);
                model.SiteMapFileXmlLinkText = SettingsMain.SiteUrl + "/sitemap.xml";
                model.SiteMapFileXmlLink = model.SiteMapFileXmlLinkText + "?rnd=" + (new Random().Next(10000));
            }

            if (File.Exists(sitemapFilePathHtml))
            {
                model.SiteMapFileHtmlDate = Localization.Culture.ConvertDate(new FileInfo(sitemapFilePathHtml).LastWriteTime);
                model.SiteMapFileHtmlLinkText = SettingsMain.SiteUrl + "/sitemap.html";
                model.SiteMapFileHtmlLink = model.SiteMapFileHtmlLinkText + "?rnd=" + (new Random().Next(10000));
            }*/
            
            var domains = DomainSiteMapsService.GetSitemapDomainModels();

            foreach (var domain in domains)
            {
                var sitemapDomainFilePathXml = SettingsGeneral.AbsolutePath + $"{domain.Url.Split('.')[0]}_sitemap.xml";
                var sitemapDomainFilePathHtml = SettingsGeneral.AbsolutePath + $"{domain.Url.Split('.')[0]}_sitemap.html";
                siteMapFileXmlLinkText = $"http://{Uri.EscapeDataString(domain.Url.Split('.')[0])}_sitemap.xml";
                siteMapFileHtmlLinkText = $"http://{Uri.EscapeDataString(domain.Url.Split('.')[0])}_sitemap.html";
                
                model.SiteMapDomainModels.Add(new SiteMapDomainModel()
                {
                    SiteMapFileXmlLinkText = siteMapFileXmlLinkText,
                    SiteMapFileXmlLink = siteMapFileXmlLinkText + "?rnd=" + (new Random().Next(10000)),
                    SiteMapFileHtmlLinkText = siteMapFileHtmlLinkText,
                    SiteMapFileHtmlLink = siteMapFileHtmlLinkText + "?rnd=" + (new Random().Next(10000))
                });
            }

            model.AdminAreaColorScheme = SettingsDesign.AdminAreaColorScheme;
            var colorSchemes = new List<AdminAreaColorScheme>();

            try
            {
                foreach (var dir in Directory.GetDirectories(HostingEnvironment.MapPath("~/areas/admin/templates/adminv3/content/src/color-schemes/")))
                {
                    var configPath = dir + "\\config.json";
                    var cssPath = dir + "\\styles.css";

                    if (!File.Exists(configPath) || !File.Exists(cssPath))
                        continue;

                    var key = dir.Split('\\').LastOrDefault();
                    var colorScheme = JsonConvert.DeserializeObject<AdminAreaColorScheme>(File.ReadAllText(configPath));
                    colorScheme.Key = key;

                    colorSchemes.Add(colorScheme);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            model.AdminAreaColorSchemes = colorSchemes.OrderBy(x => x.SortOrder).Select(x => new SelectListItem() {Text = x.Name, Value = x.Key }).ToList();

            model.AdminAreaTemplate = AdminAreaTemplate.Template;
            model.AdminAreaTemplates = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Новая версия", Value = "adminv3"},
                new SelectListItem() {Text = "Старая версия", Value = "adminv2"},
            };

            model.AdminStartPages = new List<SelectListItem>()
            {
                new SelectListItem() {Text = EAdminStartPage.Default.Localize(), Value = ((int)EAdminStartPage.Default).ToString()},
                new SelectListItem() {Text = EAdminStartPage.Desktop.Localize(), Value = ((int)EAdminStartPage.Desktop).ToString()},
                new SelectListItem() {Text = EAdminStartPage.Orders.Localize(), Value = ((int)EAdminStartPage.Orders).ToString()},
            };

            model.AuthByCodeMethods = new List<SelectListItem>();
            foreach (EAuthByCodeMode item in Enum.GetValues(typeof(EAuthByCodeMode)))
            {
                model.AuthByCodeMethods.Add(new SelectListItem() { Text = item.Localize(), Value = ((int)item).ToString() });
            }

            if (SettingsCrm.CrmActive)
                model.AdminStartPages.Add(new SelectListItem() { Text = EAdminStartPage.Leads.Localize(), Value = ((int)EAdminStartPage.Leads).ToString() });

            var dashboard = SalesChannelService.GetByType(ESalesChannelType.Dashboard);
            if (dashboard.Enabled)
                model.AdminStartPages.Add(new SelectListItem() { Text = EAdminStartPage.Dashboard.Localize(), Value = ((int)EAdminStartPage.Dashboard).ToString() });
            else if (model.AdminStartPage == EAdminStartPage.Dashboard)
                model.AdminStartPage = SettingsMain.AdminStartPage = EAdminStartPage.Orders;

            if (SettingsTasks.TasksActive)
                model.AdminStartPages.Add(new SelectListItem() {Text = EAdminStartPage.Tasks.Localize(), Value = ((int)EAdminStartPage.Tasks).ToString() });
            else if (model.AdminStartPage == EAdminStartPage.Tasks)
                model.AdminStartPage = SettingsMain.AdminStartPage = EAdminStartPage.Orders;

            model.AuthMethods = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Value = ((int)EAuthMethod.Email).ToString(), 
                    Text = EAuthMethod.Email.Localize(),
                },
            };

            if (model.AuthByCodeActive)
            {
                model.AuthMethods.Add(new SelectListItem()
                {
                    Value = ((int)EAuthMethod.Code).ToString(),
                    Text = EAuthMethod.Code.Localize()
                });
            }
            
            return model;
        }

        private string DomainUrlEncode(string siteMapDomainUrl)
        {
            var result = siteMapDomainUrl.Split('.')[0];

            return HttpUtility.UrlEncode(result);
        }
    }
}
