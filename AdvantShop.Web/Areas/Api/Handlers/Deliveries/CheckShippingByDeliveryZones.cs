using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Deliveries;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Shipping.DeliveryByZones;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Deliveries
{
    public class CheckShippingByDeliveryZones : AbstractCommandHandler<CheckShippingByDeliveryZonesResponse>
    {
        private readonly ShippingAddress _address;

        public CheckShippingByDeliveryZones(ShippingAddress address)
        {
            _address = address;
        }
        
        protected override CheckShippingByDeliveryZonesResponse Handle()
        {
            var shippingCalculationParameters = GetShippingCalculationParameters();

            var options =
                GetShippings(shippingCalculationParameters)
                   .Select(x => CreateDeliveryByZones(x, shippingCalculationParameters))
                   .SelectMany(x => x.CalculateOptions(CalculationVariants.All))
                   .Where(x => x != null)
                   .Select(x =>
                   {
                       // моб приложение использует CheckShippingByDeliveryZones для проверки адреса при добавлении контакта,
                       // в этот момент проверять на интервал доставки не нужно, отключаем перед валидацией
                       x.UseDeliveryInterval = false; 
                       return x;
                   })
                   .Where(x => x.Validate().IsValid)
                   .ToList();

            var warehousesOptions =
                options
                   .Select(x => x.GetWarehouseIds())
                   .Where(x => x != null)
                   .Select(x => x.ToArray())
                   .ToArray();

            return new CheckShippingByDeliveryZonesResponse()
            {
                HasDelivery = options.Count > 0,
                WarehousesByZone = warehousesOptions.Length != 0
                    ? warehousesOptions
                    : null
            };
        }

        private static DeliveryByZones CreateDeliveryByZones(ShippingMethod method, ShippingCalculationParameters shippingCalculationParameters)
        {
            var deliveryByZones = new DeliveryByZones(method, shippingCalculationParameters);
            // убираем настройку минимальной стоимости заказа, т.к. нам нужно проверить только вхождение адреса в зону доставки
            deliveryByZones.Zones?.ForEach(x => x.MinimalOrderPrice = null);
            return deliveryByZones;
        }

        private List<ShippingMethod> GetShippings(ShippingCalculationParameters shippingCalculationParameters)
        {
            var items = new List<ShippingMethod>();
            var deliveryByZonesShippingKey = AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>(typeof(DeliveryByZones));
            var shippings = 
                ShippingMethodService.GetAllShippingMethods(true)
                                     .Where(x => x.ShippingType == deliveryByZonesShippingKey);

            foreach (var shipping in shippings)
            {
                var validGeo = false;
                
                if (ShippingPaymentGeoMaping.IsExistGeoShipping(shipping.ShippingMethodId))
                {
                    if (ShippingPaymentGeoMaping.CheckShippingEnabledGeo(
                            shipping.ShippingMethodId, 
                            shippingCalculationParameters.Country, 
                            shippingCalculationParameters.Region, 
                            shippingCalculationParameters.City, 
                            shippingCalculationParameters.District))
                        validGeo = true;
                }
                else
                    validGeo = true;

                if (validGeo)
                    items.Add(shipping);
            }

            return items;
        }
        private ShippingCalculationParameters GetShippingCalculationParameters()
        {
            return
                ShippingCalculationConfigurator
                   .Configure()
                   .WithCountry(_address.Country)
                   .WithRegion(_address.Region)
                   .WithDistrict(_address.District)
                   .WithCity(_address.City)
                   .WithZip(_address.Zip)
                   .WithStreet(_address.Street)
                   .WithHouse(_address.House)
                   .WithStructure(_address.Structure)
                   .WithPreOrderItems(new List<PreOrderItem>())
                   .WithCurrency(CurrencyService.CurrentCurrency)
                   .Build();
        }
    }
}