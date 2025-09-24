using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using AdvantShop.Diagnostics;
using AdvantShop.Configuration;
using AdvantShop.Web.Admin.Models.Home;
using AdvantShop.Repository;
using System.Web.Mvc;
using AdvantShop.Core.SQL;
using AdvantShop.Trial;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetCongratulationsDashboard
    {
        public CongratulationsDashboardModel Execute()
        {
            var url = HttpContext.Current.Request.Url;
            var isTest = url.Query.ToLower().Contains("istest=true");
            
            if (!SettingsCongratulationsDashboard.DomainDone && 
                !SettingsMain.IsTechDomain(url) &&
                !isTest)
            {
                SettingsCongratulationsDashboard.DomainDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DomenDone);
                Debug.Log.Info("DomainDone " + HttpContext.Current.Request.Url);
            }

            var storeInfoDone = SettingsCongratulationsDashboard.StoreInfoDone;
            var currentZone = IpZoneContext.CurrentZone;

            if (!SettingsCongratulationsDashboard.ProductDone)
            {
                var hasProducts = SQLDataAccess.ExecuteScalar<bool>(
                    "if exists(Select 1 From Catalog.Product Where IsDemo is null or IsDemo <> 1) Select 1 Else Select 0", CommandType.Text);
                if (hasProducts)
                    SettingsCongratulationsDashboard.ProductDone = true;
            }

            var model = new CongratulationsDashboardModel
            {
                AllDone = SettingsCongratulationsDashboard.AllDone,
                FirstVisit = !SettingsCongratulationsDashboard.NotFirstVisit,
                Steps = new List<CongratulationsDashboardStep>()
                {
                    new CongratulationsDashboardStep((int)ECongratulationsDashboardStep.StoreInfo, SettingsCongratulationsDashboard.StoreInfoDone),
                    new CongratulationsDashboardStep((int)ECongratulationsDashboardStep.Product, SettingsCongratulationsDashboard.ProductDone),
                    new CongratulationsDashboardStep((int)ECongratulationsDashboardStep.Design, SettingsCongratulationsDashboard.DesignDone),
                    new CongratulationsDashboardStep((int)ECongratulationsDashboardStep.Domain, SettingsCongratulationsDashboard.DomainDone),
                    //new CongratulationsDashboardStep((int)ECongratulationsDashboardStep.Course, SettingsCongratulationsDashboard.CourseDone)
                },
                IsTest = isTest
            };

            if (!SettingsCongratulationsDashboard.StoreInfoDone)
            {
                model.StoreName = storeInfoDone ? SettingsMain.ShopName : null;
                model.Phone = storeInfoDone ? SettingsMain.Phone : null;
                model.CountryId = storeInfoDone ? SettingsMain.SellerCountryId : currentZone.CountryId;
                model.Countries = CountryService.GetAllCountries().Select(x => new SelectListItem()
                    {Text = x.Name, Value = x.CountryId.ToString()}).ToList();
                model.RegionId = storeInfoDone ? SettingsMain.SellerRegionId : currentZone.RegionId;
                model.Regions = RegionService.GetRegions(model.CountryId)
                    .Select(x => new SelectListItem() {Text = x.Name, Value = x.RegionId.ToString()}).ToList();
                model.City = storeInfoDone ? SettingsMain.City : currentZone.City;
            }

            return model;
        }
    }
}
