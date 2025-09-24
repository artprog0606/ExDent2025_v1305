using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Deliveries.v2;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Repository;

namespace AdvantShop.Areas.Api.Models.Shared
{
    public class WarehouseApi
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string City { get; private set; }

        public string Address { get; private set; }

        public string AddressComment { get; private set; }

        public string[] Phones { get; private set; }
        
        public string[] TimeOfWorkList { get; private set; }
        
        public IGeoCoordinates Coordinates { get; private set;  }

        
        public WarehouseApi(Warehouse warehouse)
        {
            Id = warehouse.Id;
            Name = warehouse.Name;
            Description = warehouse.Description;
            
            if (warehouse.CityId != null)
            {
                var city = CityService.GetCity(warehouse.CityId.Value);
                if (city != null)
                    City = city.Name;
            }
            
            Address = warehouse.Address;
            AddressComment = warehouse.AddressComment;
            
            var phones = new List<string>(){warehouse.Phone, warehouse.Phone2}.Where(x => x.IsNotEmpty()).ToArray();
            if (phones.Length > 0)
                Phones = phones;
            
            TimeOfWorkList =
                TimeOfWorkService.GetWarehouseTimeOfWork(warehouse.Id)
                    .Select(TimeOfWorkService.FormatTimeOfWork)
                    .ToArray();
            
            Coordinates =
                warehouse.Latitude.HasValue && warehouse.Longitude.HasValue
                    ? new DeliveryPointCoordinates(warehouse.Latitude.Value, warehouse.Longitude.Value)
                    : null;
        }
    }
}