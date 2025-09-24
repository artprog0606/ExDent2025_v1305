using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Helpers;
using AdvantShop.Models.Warehouse;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Warehouse
{
    public class GetWarehousesInfo : ICommandHandler<List<WarehouseInfoModel>>
    {
        public List<WarehouseInfoModel> Execute()
        {
            var model = new List<WarehouseInfoModel>();
            
            foreach (var warehouse in WarehouseService.GetList(enabled: true))
            {
                model.Add(new WarehouseInfoModel()
                {
                    Name = warehouse.Name,
                    Type = warehouse.TypeId.HasValue
                        ? TypeWarehouseService.Get(warehouse.TypeId.Value)?.Name
                        : null,
                    Address = StringHelper.AggregateStrings(", ", 
                        warehouse.CityId.HasValue 
                            ? CityService.GetCity(warehouse.CityId.Value)?.Name
                            : null, 
                        warehouse.Address),
                    AddressComment = warehouse.AddressComment,
                    Latitude = warehouse.Latitude,
                    Longitude = warehouse.Longitude,
                    TimeOfWorkList =  
                        TimeOfWorkService.GetWarehouseTimeOfWork(warehouse.Id)
                                         .Select(TimeOfWorkService.FormatTimeOfWork)
                                         .ToArray(),
                });
            }

            return model;
        }
    }
}