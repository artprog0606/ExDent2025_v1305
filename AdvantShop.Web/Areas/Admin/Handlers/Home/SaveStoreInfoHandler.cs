using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Track;
using AdvantShop.Web.Admin.Models.Home;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class SaveStoreInfoHandler : AbstractCommandHandler
    {
        private StoreInfoModel _model;

        public SaveStoreInfoHandler(StoreInfoModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model.StoreName.IsNullOrEmpty() ||
                _model.City.IsNullOrEmpty() ||
                _model.Phone.IsNullOrEmpty())
                throw new BlException("Заполните обязательные поля");
        }

        protected override void Handle()
        {
            SettingsMain.ShopName = _model.StoreName;
            SettingsMain.AdminShopName = _model.StoreName;
            SettingsMain.LogoImageAlt = _model.StoreName;

            var countries = CountryService.GetAllCountries();
            SettingsMain.SellerCountryId = countries.Any(x => x.CountryId == _model.CountryId)
                ? _model.CountryId
                : (countries.Count > 0 ? countries.FirstOrDefault().CountryId : 0);

            var regions = RegionService.GetRegions(_model.CountryId);
            SettingsMain.SellerRegionId = regions.Any(x => x.RegionId == _model.RegionId) ? _model.RegionId : 0;

            SettingsMain.City = _model.City;
            SettingsMain.Phone = _model.Phone;

            var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone, true, true);
            SettingsMain.MobilePhone = standardPhone.HasValue && standardPhone.Value != 0 
                ? (_model.Phone.StartsWith("+") ? "+" : "") + standardPhone.Value
                : string.Empty;

            if (!SettingsCongratulationsDashboard.StoreInfoDone)
            {
                SettingsCongratulationsDashboard.StoreInfoDone = true;
                TrackService.TrackEvent(ETrackEvent.Core_CongratulationsDashboard_StoreInfoDone);
            }
        }
    }
}
