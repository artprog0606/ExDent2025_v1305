using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Common
{
    public class GetCommonSettings : AbstractCommandHandler<CommonSettingsModel>
    {
        protected override CommonSettingsModel Handle()
        {
            // common settings
            var model = new CommonSettingsModel()
            {
                StoreUrl = SettingsMain.SiteUrl,
                StoreName = SettingsMain.ShopName,


                LogoImgSrc =
                    !string.IsNullOrEmpty(SettingsMain.LogoImageName)
                        ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, true)
                        : "../images/nophoto_small.png",

                LogoImgAlt = SettingsMain.LogoImageAlt,

                FaviconSrc =
                    !string.IsNullOrEmpty(SettingsMain.FaviconImageName)
                        ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.FaviconImageName, true)
                        : "../images/nophoto_small.png",

                LogoBlogImgSrc =
                    !string.IsNullOrEmpty(SettingsMain.LogoBlogImageName)
                        ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoBlogImageName, true)
                        : "../images/nophoto_small.png",
            };

            var countryId = SettingsMain.SellerCountryId;
            model.CountryId = countryId;
            model.Countries = CountryService.GetAllCountries().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.CountryId.ToString(),
                Selected = x.CountryId == countryId
            }).ToList();


            var regionId = SettingsMain.SellerRegionId;
            model.RegionId = regionId;
            model.Regions = RegionService.GetRegions(countryId).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.RegionId.ToString(),
                Selected = x.RegionId == regionId
            }).ToList();
            model.HasRegions = model.Regions.Count > 0;

            model.City = SettingsMain.City;
            model.Phone = SettingsMain.Phone;
            model.MobilePhone = SettingsMain.MobilePhone;
            

            // sales plan settings
            model.SalesPlan = OrderStatisticsService.SalesPlan;
            model.ProfitPlan = OrderStatisticsService.ProfitPlan;


            model.FeedbackAction = SettingsFeedback.FeedbackAction;

            List<SelectListItem> feedbackActions = new List<SelectListItem>();
            feedbackActions.Add(new SelectListItem() { Text = T("Admin.Settings.Feedback.FeedbackAction.SendEmail"), Value = EnFeedbackAction.SendEmail.ToString() });
            feedbackActions.Add(new SelectListItem() { Text = T("Admin.Settings.Feedback.FeedbackAction.CreateLead"), Value = EnFeedbackAction.CreateLead.ToString(), Disabled = SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveCrm });
            model.FeedbackActions = feedbackActions;

            model.ShowUserAgreementText = SettingsCheckout.IsShowUserAgreementTextValue;
            model.AgreementDefaultChecked = SettingsCheckout.AgreementDefaultChecked;
            model.UserAgreementText = SettingsCheckout.UserAgreementText;
            model.ShowUserAgreementForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter;
            model.UserAgreementForPromotionalNewsletter = SettingsDesign.UserAgreementForPromotionalNewsletter;
            model.SetUserAgreementForPromotionalNewsletterChecked = SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked;

            model.ShowCookiesPolicyMessage = SettingsNotifications.ShowCookiesPolicyMessage;
            model.CookiesPolicyMessage = SettingsNotifications.CookiesPolicyMessage;

            return model;
        }
    }
}
