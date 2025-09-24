using System.Linq;
using System.Text.RegularExpressions;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Common
{
    public class SaveCommonSettings
    {
        private CommonSettingsModel _model;

        public SaveCommonSettings(CommonSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {

            TrackEvents();

            string newUrl = StringHelper.CompileUrl(_model.StoreUrl);

            bool needUpdate = newUrl != SettingsMain.SiteUrl;
            SettingsMain.SiteUrl = newUrl;

            if (needUpdate)
            {
                new System.UpdateSiteMapsHandler().Execute();
            }
            
            SettingsMain.ShopName = _model.StoreName.DefaultOrEmpty();

            SettingsMain.LogoImageAlt = _model.LogoImgAlt.DefaultOrEmpty();

            var countries = CountryService.GetAllCountries();
            SettingsMain.SellerCountryId = countries.Any(x => x.CountryId == _model.CountryId)
                                                ? _model.CountryId
                                                : (countries.Count > 0 ? countries.FirstOrDefault().CountryId : 0);

            var regions = RegionService.GetRegions(_model.CountryId);
            SettingsMain.SellerRegionId = regions.Any(x => x.RegionId == _model.RegionId) ? _model.RegionId : 0;

            SettingsMain.City = _model.City.DefaultOrEmpty();

            SettingsMain.Phone = _model.Phone.DefaultOrEmpty();
            SettingsMain.MobilePhone = StringHelper.ConvertToMobileStandardPhone(_model.MobilePhone.DefaultOrEmpty());


            // Sales plan settings
            _model.SalesPlan = _model.SalesPlan >= 0 ? _model.SalesPlan : 0;
            _model.ProfitPlan = _model.ProfitPlan >= 0 ? _model.ProfitPlan : 0;
            OrderStatisticsService.SetProfitPlan(_model.SalesPlan, _model.ProfitPlan);

            SettingsFeedback.FeedbackAction = _model.FeedbackAction;

            SettingsCheckout.IsShowUserAgreementTextValue = _model.ShowUserAgreementText;
            SettingsCheckout.AgreementDefaultChecked = _model.AgreementDefaultChecked;
            SettingsCheckout.UserAgreementText = _model.UserAgreementText;
            SettingsDesign.ShowUserAgreementForPromotionalNewsletter = _model.ShowUserAgreementForPromotionalNewsletter;
            SettingsDesign.UserAgreementForPromotionalNewsletter = _model.UserAgreementForPromotionalNewsletter;
            SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked = _model.SetUserAgreementForPromotionalNewsletterChecked;

            SettingsNotifications.ShowCookiesPolicyMessage = _model.ShowCookiesPolicyMessage;
            SettingsNotifications.CookiesPolicyMessage = _model.CookiesPolicyMessage;
        }

        private void TrackEvents()
        {
            bool haveChanges = false;
            if (SettingsMain.ShopName != _model.StoreName)
            {
                TrialService.TrackEvent(TrialEvents.ChangeShopName, "");
                haveChanges = true;
            }

            if (SettingsMain.Phone != _model.Phone)
            {
                TrialService.TrackEvent(TrialEvents.ChangePhoneNumber, "");
                haveChanges = true;
            }

            if (SettingsMain.SiteUrl != _model.StoreUrl)
            {
                TrialService.TrackEvent(TrialEvents.ChangeDomain, "");
                haveChanges = true;
            }

            haveChanges |= SettingsMain.SellerCountryId != _model.CountryId;
            haveChanges |= SettingsMain.SellerRegionId != _model.RegionId;
            haveChanges |= SettingsMain.City != _model.City;
            if (haveChanges)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_ChangeAboutData);
        }
    }
}
