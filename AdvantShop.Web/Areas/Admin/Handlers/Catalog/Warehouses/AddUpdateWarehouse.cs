using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Warehouses
{
    public class AddUpdateWarehouse
    {
        private readonly WarehouseModel _model;

        public AddUpdateWarehouse(WarehouseModel model)
        {
            _model = model;
        }

        public int Execute()
        {
            if (_model.Id.IsDefault()
                && SaasDataService.IsSaasEnabled
                && WarehouseService.GetCount() >= SaasDataService.CurrentSaasData.WarehouseAmount)
                throw new BlException(LocalizationService.GetResource("Admin.Warehouse.AddWarehouse.Error.ExceedingSaasValue"));
            
            var warehouse = _model.Id.IsNotDefault()
                ? WarehouseService.Get(_model.Id)
                : new Warehouse();

            warehouse.Name = _model.Name.Trim();
            warehouse.UrlPath = _model.UrlPath.Trim();
            warehouse.Description =
                _model.Description == null || _model.Description == "<br />" || _model.Description == "&nbsp;" || _model.Description == "\r\n"
                    ? string.Empty
                    : _model.Description;
            warehouse.TypeId = _model.TypeId;
            warehouse.SortOrder = _model.SortOrder;
            warehouse.Enabled = _model.Enabled;
            warehouse.CityId = _model.CityId;
            warehouse.Address = _model.Address;
            warehouse.Longitude = _model.Longitude.TryParseFloat(true);
            warehouse.Latitude = _model.Latitude.TryParseFloat(true);
            warehouse.AddressComment = _model.AddressComment;
            warehouse.Phone = _model.Phone;
            warehouse.Phone2 = _model.Phone2;
            warehouse.Email = _model.Email;
            warehouse.Meta =
                new MetaInfo(0, _model.Id, warehouse.MetaType, _model.SeoTitle.DefaultOrEmpty(),
                    _model.SeoKeywords.DefaultOrEmpty(), _model.SeoDescription.DefaultOrEmpty(),
                    _model.SeoH1.DefaultOrEmpty());

            if (warehouse.Longitude is null
                && warehouse.Latitude is null
                && warehouse.Address.IsNotEmpty())
            {
                if (warehouse.CityId.HasValue)
                {
                    var city = CityService.GetCity(warehouse.CityId.Value);
                    if (city != null)
                    {
                        var region = RegionService.GetRegion(city.RegionId);
                        var country = CountryService.GetCountry(region.CountryId);
                        
                        var fullAddress = StringHelper.AggregateStrings(", ",
                            country.Name,
                            region.Name,
                            city.District,
                            city.Name,
                            warehouse.Address);

                        /*
                         * !!! Warning !!!
                         * Тут нельзя запрашивать координаты через яндекс,
                         * т.к. записывая их в БД мы нарушаем условия бесплатной лицензии
                         */
                        var geocoderMetaData = ModulesExecuter.Geocode(fullAddress);
                        if (geocoderMetaData?.Point != null)
                        {
                            warehouse.Latitude = (float)geocoderMetaData.Point.Latitude;
                            warehouse.Longitude = (float)geocoderMetaData.Point.Longitude;
                        }
                    }
                }
            }
            
            if (warehouse.Id.IsDefault())
                warehouse.Id = WarehouseService.Add(warehouse);
            else 
                WarehouseService.Update(warehouse);

            if (warehouse.Id.IsDefault())
                return warehouse.Id;

            if (_model.TimesOfWork != null
                && _model.TimesOfWork.Count > 0)
            {
                var currentTimeOfWork = TimeOfWorkService.GetWarehouseTimeOfWork(warehouse.Id);
                
                foreach (var workModel in _model.TimesOfWork)
                {
                    TimeSpan temp;
                    var timeOfWork = new TimeOfWork
                    {
                        Id = workModel.Id,
                        WarehouseId = warehouse.Id,
                        OpeningTime =
                            TimeSpan.TryParseExact(workModel.OpeningTime, "hh\\:mm", CultureInfo.InvariantCulture, out temp)
                                ? temp
                                : (TimeSpan?) null,
                        ClosingTime =
                            TimeSpan.TryParseExact(workModel.ClosingTime, "hh\\:mm", CultureInfo.InvariantCulture, out temp)
                                ? temp
                                : (TimeSpan?) null,
                        BreakStartTime =
                            TimeSpan.TryParseExact(workModel.BreakStartTime, "hh\\:mm", CultureInfo.InvariantCulture, out temp)
                                ? temp
                                : (TimeSpan?) null,
                        BreakEndTime =
                            TimeSpan.TryParseExact(workModel.BreakEndTime, "hh\\:mm", CultureInfo.InvariantCulture, out temp)
                                ? temp
                                : (TimeSpan?) null,
                        DayOfWeeks = workModel.DayOfWeeks.Aggregate((FlagDayOfWeek) 0,
                            (flag, day) => flag | day.GetFlagDayOfWeek())
                    };
                    if (timeOfWork.Id > 0)
                        TimeOfWorkService.Update(timeOfWork);
                    else
                        TimeOfWorkService.Add(timeOfWork);
                }
                    
                // удалям не пришедшие
                currentTimeOfWork
                   .Where(curr => _model.TimesOfWork.All(newItem => newItem.Id != curr.Id))
                   .ForEach(curr => TimeOfWorkService.Delete(curr.Id));
            }
            else
            {
                TimeOfWorkService.DeleteWarehouseTimeOfWork(warehouse.Id);
            }

            if (_model.WarehouseCitiesJson.IsNotEmpty())
            {
                try
                {
                    var cities = JsonConvert.DeserializeObject<List<CityOfWarehouse>>(_model.WarehouseCitiesJson);
                    if (cities != null && cities.Count > 0)
                    {
                        var sortOrder = WarehouseCityService.GetMaxSortOrder(warehouse.Id) + 10;

                        foreach (var city in cities)
                        {
                            WarehouseCityService.Add(
                                new WarehouseCity()
                                {
                                    WarehouseId = warehouse.Id, 
                                    CityId = city.CityId, 
                                    SortOrder = sortOrder
                                });
                            sortOrder += 10;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();

            return warehouse.Id;
        }
    }
}