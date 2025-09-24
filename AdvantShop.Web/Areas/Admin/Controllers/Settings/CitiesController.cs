using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.System;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.Location;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class CitiesController : BaseAdminController
    {
        /// <summary>
        /// from database
        /// </summary>
        [ExcludeFilter(typeof(AuthAttribute))]
        public JsonResult GetCitiesAutocomplete(string q)
        {
            var result = CityService.GetCitiesAutocomplete(q);
            return Json(result);
        }

        /// <summary>
        /// from database and modules
        /// </summary>
        [ExcludeFilter(typeof(AuthAttribute))]
        public JsonResult GetCitiesSuggestions(string q, bool? withId)
        {
            var ipZones = IpZoneService.GetIpZonesByCity(q);
            var locations = ipZones.Select(x => (LocationModel)x).ToList();
            if (ipZones.Count < 5)
            {
                var zones = ModulesExecuter.GetIpZonesAutocomplete(q, true);
                locations.AddRange(
                    zones.Where(x => !ipZones.Any(zone => 
                        zone.Region.DefaultOrEmpty().Equals(x.Region.DefaultOrEmpty(), StringComparison.OrdinalIgnoreCase) && 
                        zone.City.DefaultOrEmpty().Equals(x.City.DefaultOrEmpty(), StringComparison.OrdinalIgnoreCase) &&
                        zone.District.DefaultOrEmpty().Equals(x.District.DefaultOrEmpty(), StringComparison.OrdinalIgnoreCase))
                    ).Select(x => (LocationModel)x));
            }

            if (withId is true
                && locations.Any(x => x.CityId.IsDefault()))
            {
                locations = locations
                           .Where(x => x.CityId.IsNotDefault())
                           .ToList();
            }

            return Json(locations);
        }

        /// <summary>
        /// from database
        /// </summary>
        [ExcludeFilter(typeof(AuthAttribute))]
        public JsonResult GetCitiesAutocompleteExt(string q)
        {
            var zones = IpZoneService.GetIpZonesByCity(q);
            zones.ForEach((zone) =>
            {
                if (zones.Count(x => x.City == zone.City) == 1)
                    zone.Region = string.Empty;
            });
            return Json(zones);
        }

        #region Add/Edit/Get/Delete

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCity(CityModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return Json(new { result = false });

            try
            {
                var city = new City()
                {
                    Name = model.Name,
                    District = model.District,
                    RegionId = model.RegionId,
                    CitySort = model.CitySort,
                    DisplayInPopup = model.DisplayInPopup,
                    PhoneNumber = model.PhoneNumber,
                    MobilePhoneNumber = model.MobilePhoneNumber,
                    Zip = model.Zip
                };

                CityService.Add(city);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        public JsonResult EditCity(CityModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return Json(new { result = false });

            try
            {
                var city = new City()
                {
                    CityId = model.CityId,
                    Name = model.Name,
                    District = model.District,
                    RegionId = model.RegionId,
                    CitySort = model.CitySort,
                    DisplayInPopup = model.DisplayInPopup,
                    PhoneNumber = model.PhoneNumber,
                    MobilePhoneNumber = model.MobilePhoneNumber,
                    Zip = model.Zip
                };

                CityService.Update(city);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        public JsonResult GetAdditionalSettings(int cityId)
        {
            var additionalSettings = CityService.GetAdditionalSettings(cityId);
            var model = new CityAdditionalSettingsModel
            {
                CityId = additionalSettings.CityId,
                ShippingZones = additionalSettings.ShippingZones,
                ShippingZonesIframe = additionalSettings.ShippingZonesIframe,
                CityAddressPoints = additionalSettings.CityAddressPoints,
                CityAddressPointsIframe = additionalSettings.CityAddressPointsIframe,
                CityDescription = additionalSettings.CityDescription,
                FiasId = additionalSettings.FiasId,
                KladrId = additionalSettings.KladrId,
            };
            var city = CityService.GetCity(cityId);
            var region = 
                city != null 
                    ? RegionService.GetRegion(city.RegionId) 
                    : null;
            var country =
                region != null
                    ? CountryService.GetCountry(region.CountryId)
                    : null;
            model.CountryIso2 = country?.Iso2;
            return JsonOk(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditAdditionalSettings(CityAdditionalSettingsModel model)
        {
            if (model.CityId == 0)
                return JsonError();

            try
            {
                CityService.UpdateAdditionalSettings(new CityAdditionalSettings
                {
                    CityId = model.CityId,
                    CityAddressPoints = model.CityAddressPoints,
                    CityAddressPointsIframe = model.CityAddressPointsIframe,
                    CityDescription = model.CityDescription,
                    ShippingZones = model.ShippingZones,
                    ShippingZonesIframe = model.ShippingZonesIframe,
                    FiasId = model.FiasId,
                    KladrId = model.KladrId,
                });
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return JsonError();
            }

            return JsonOk();
        }

        public JsonResult GetCity(int сityId, int? regionId)
        {
            var city = CityService.GetCity(сityId);
            var region = RegionService.GetRegion(regionId.HasValue && regionId != 0 ? regionId.Value : city.RegionId);

            return Json(new CityModel
            {
                CityId = city.CityId,
                CitySort = city.CitySort,
                DisplayInPopup = city.DisplayInPopup,
                District = city.District,
                MobilePhoneNumber = city.MobilePhoneNumber,
                Name = city.Name,
                PhoneNumber = city.PhoneNumber,
                RegionId = city.RegionId,
                Zip = city.Zip,
                Regions = region != null 
                            ? RegionService.GetRegions(region.CountryId).Select(x => new RegionModel
                            {
                                CountryId = x.CountryId,
                                Name = x.Name,
                                RegionCode = x.RegionCode,
                                RegionId = x.RegionId,
                                SortOrder = x.SortOrder
                            }).ToList()
                            : null
            });
        }

        public JsonResult GetFormData(int regionId)
        {
            var region = RegionService.GetRegion(regionId);

            return Json(new CityModel
            {
                RegionId = regionId,
                Regions = region != null
                            ? RegionService.GetRegions(region.CountryId).Select(x => new RegionModel
                            {
                                CountryId = x.CountryId,
                                Name = x.Name,
                                RegionCode = x.RegionCode,
                                RegionId = x.RegionId,
                                SortOrder = x.SortOrder
                            }).ToList()
                            : null
            });
        }

        public JsonResult GetCities(AdminCityFilterModel model)
        {
            if (model.id != null)
            {
                model.RegionId = (int)model.id;
            }

            var handler = new GetCity(model);
            var result = handler.Execute();

            return Json(result);
        }

        public JsonResult DeleteCity(AdminCityFilterModel model)
        {
            Command(model, (id, c) =>
            {
                CityService.Delete(id);
                return true;
            });

            return Json(true);
        }

        #endregion

        public JsonResult GetCitiesList(string q, int page = 1, int? cityId = null)
        {
            var cities =
                new GetCitiesList(new CitiesListFilterModel() {ItemsPerPage = q.IsNotEmpty() ? 30 : 10, Page = page, Search = q, Sorting = "DisplayInPopup", SortingType = FilterSortingType.Desc})
                   .Execute().DataItems;
            if (cityId.HasValue)
            {
                var cityModel =
                    new GetCitiesList(new CitiesListFilterModel() {ItemsPerPage = 1, Page = 1})
                       .GetItem(cityId.Value);
                if (cityModel != null)
                    cities.Add(cityModel);
            }
            return Json(cities.Select(x => new { label = x.ToString(), value = x.CityId }));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ActivateCity(AdminCityFilterModel model)
        {
            Command(model, (id, c) =>
            {
                var city = CityService.GetCity(id);
                city.DisplayInPopup = true;
                CityService.Update(city);
                return true;
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableCity(AdminCityFilterModel model)
        {
            Command(model, (id, c) =>
            {
                var city = CityService.GetCity(id);
                city.DisplayInPopup = false;
                CityService.Update(city);
                return true;
            });
            return Json(true);
        }

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceCity(AdminCityFilterModel model)
        {
            var city = CityService.GetCity(model.CityId);

            var sorting = 0;
            Int32.TryParse(model.CitySort, out sorting);
            city.Name = model.Name;
            city.RegionId = model.RegionId;
            city.CitySort = sorting;
            city.DisplayInPopup = (bool)model.DisplayInPopup;
            city.PhoneNumber = model.PhoneNumber;
            city.MobilePhoneNumber = model.MobilePhoneNumber;
            city.Zip = model.Zip;

            CityService.Update(city);

            return Json(new { result = true });
        }

        #endregion

        #region Command

        private void Command(AdminCityFilterModel model, Func<int, AdminCityFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                model.RegionId = model.id != null ? (int)model.id : 0;
                var handler = new GetCity(model);
                var CityiDs = handler.GetItemsIds();

                foreach (int id in CityiDs)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        #endregion
        
        #region Select city modal
        
        [HttpGet]
        public JsonResult GetCitiesInSelectCityModal(CitiesListFilterModel filter)
        {
            filter.Sorting = "SelectCityInModal";
            return Json(new GetCitiesList(filter).Execute());
        }

        [HttpGet]
        public JsonResult GetDefaultGeoData()
        {
            return Json(new
            {
                CountryId = SettingsMain.SellerCountryId,
                Country = CountryService.GetCountry(SettingsMain.SellerCountryId)?.Name,
                RegionId = SettingsMain.SellerRegionId != 0 ? SettingsMain.SellerRegionId : default(int?),
                Region = RegionService.GetRegion(SettingsMain.SellerRegionId)?.Name,
                City = SettingsMain.City
            });
        }
        
        [HttpGet]
        public JsonResult GetCitiesInSelectCitiesModal(CitiesListFilterModel filter)
        {
            filter.Sorting = "SelectCityInModal";
            filter.ItemsPerPage = 500;
            filter.Page = 1;
            
            return Json(new GetCitiesList(filter).Execute());
        }
        
        #endregion
    }
}
