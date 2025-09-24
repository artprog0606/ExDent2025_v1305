using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Warehouses
{
    public class GetWarehouseModel
    {
        private readonly Warehouse _warehouse;

        public GetWarehouseModel(Warehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public WarehouseModel Execute()
        {
            var model = new WarehouseModel
            {
                Id = _warehouse.Id,
                Name = _warehouse.Name,
                UrlPath = _warehouse.UrlPath,
                Description = _warehouse.Description,
                TypeId = _warehouse.TypeId,
                SortOrder = _warehouse.SortOrder,
                Enabled = _warehouse.Enabled,
                CityId = _warehouse.CityId,
                Address = _warehouse.Address,
                Longitude = _warehouse.Longitude.ToString(),
                Latitude = _warehouse.Latitude.ToString(),
                AddressComment = _warehouse.AddressComment,
                Phone = _warehouse.Phone,
                Phone2 = _warehouse.Phone2,
                Email = _warehouse.Email,
                DateAdded = _warehouse.DateAdded,
                DateModified = _warehouse.DateModified,
                ModifiedBy = _warehouse.ModifiedBy,
            };

            if (_warehouse.TypeId.HasValue)
                model.TypeName = TypeWarehouseService.Get(_warehouse.TypeId.Value)?.Name;

            if (_warehouse.CityId.HasValue)
            {
                var city = CityService.GetCity(_warehouse.CityId.Value);
                if (city != null)
                {
                    model.City = city.Name;

                    var region = RegionService.GetRegion(city.RegionId);
                    var country = CountryService.GetCountry(region.CountryId);
                    model.CityDescription = $"{country.Name}, {region.Name}";
                }
            }

            model.TimesOfWork =
                TimeOfWorkService.GetWarehouseTimeOfWork(_warehouse.Id)
                                 .Select(item => new TimeOfWorkModel()
                                  {
                                      Id = item.Id,
                                      DayOfWeeks =
                                          item.DayOfWeeks
                                              .GetUniqueFlags()
                                              .Cast<FlagDayOfWeek>()
                                              .Select(day => day.GetDayOfWeek())
                                              .ToList(),
                                      OpeningTime = item.OpeningTime?.ToString("hh\\:mm"),
                                      ClosingTime = item.ClosingTime?.ToString("hh\\:mm"),
                                      BreakStartTime = item.BreakStartTime?.ToString("hh\\:mm"),
                                      BreakEndTime = item.BreakEndTime?.ToString("hh\\:mm"),
                                  })
                                 .ToList();

            var meta = MetaInfoService.GetMetaInfo(_warehouse.Id, _warehouse.MetaType);
            if (meta == null)
            {
                model.DefaultMeta = true;
            }
            else
            {
                model.DefaultMeta = false;
                model.SeoTitle = meta.Title;
                model.SeoH1 = meta.H1;
                model.SeoKeywords = meta.MetaKeywords;
                model.SeoDescription = meta.MetaDescription;
            }
            
            model.CanDeleting = 
                WarehouseService.GetList().Count > 1 
                && WarehouseStocksService.SumStocksOfWarehouse(_warehouse.Id) <= 0f
                && !DistributionOfOrderItemService.HasWarehouseInOrderItems(_warehouse.Id)
                && _warehouse.Id != SettingsCatalog.DefaultWarehouse;

            return model;
        }
    }
}