using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.DeliveryByZones;
using Newtonsoft.Json;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("DeliveryByZones")]
    public class DeliveryByZonesShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public List<DeliveryZone> Zones
        {
            get
            {
                var zonesStr = Params.ElementOrDefault(DeliveryByZonesTemplate.Zones);
                if (!string.IsNullOrEmpty(zonesStr))
                {
                    var zones = JsonConvert.DeserializeObject<List<DeliveryZone>>(zonesStr);
                    
                    // чистка несуществующих складов
                    var warehousesId = WarehouseService.GetListIds();
                    zones
                       .Where(x => x.CheckWarehouses?.Count > 0)
                       .ForEach(x =>
                            x.CheckWarehouses =
                                x.CheckWarehouses
                                 .Where(wId => warehousesId.Contains(wId))
                                 .ToList());

                    return zones;
                }
                else
                    return new List<DeliveryZone>();
            }
            set => Params.TryAddValue(DeliveryByZonesTemplate.Zones, JsonConvert.SerializeObject(value ?? new List<DeliveryZone>()));
        }

        public string ZonesJson
        {
            get => JsonConvert.SerializeObject(Zones);
            set => Zones = JsonConvert.DeserializeObject<List<DeliveryZone>>(value);
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(DeliveryByZonesTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(DeliveryByZonesTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public string WithoutZoneMessage
        {
            get { return Params.ElementOrDefault(DeliveryByZonesTemplate.WithoutZoneMessage); }
            set { Params.TryAddValue(DeliveryByZonesTemplate.WithoutZoneMessage, value.DefaultOrEmpty()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(YaMapsApiKey))
                yield return new ValidationResult("Укажите API-ключ яндекс.карт", new[] { nameof(YaMapsApiKey) });
            if (!(Zones?.Count > 0))
                yield return new ValidationResult("Укажите зоны доставки", new[] { nameof(Zones) });
        }
    }
}