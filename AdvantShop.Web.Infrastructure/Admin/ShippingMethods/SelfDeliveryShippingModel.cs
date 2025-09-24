using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.SelfDelivery;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("SelfDelivery")]
    public class SelfDeliveryShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ShippingPrice
        {
            get => Params.ElementOrDefault(SelfDeliveryTemplate.ShippingPrice, "0");
            set => Params.TryAddValue(SelfDeliveryTemplate.ShippingPrice, value.DefaultOrEmpty());
        }

        public string DeliveryTime
        {
            get => Params.ElementOrDefault(SelfDeliveryTemplate.DeliveryTime);
            set => Params.TryAddValue(SelfDeliveryTemplate.DeliveryTime, value.DefaultOrEmpty());
        }

        public string Address
        {
            get => Params.ElementOrDefault(SelfDeliveryTemplate.Address);
            set => Params.TryAddValue(SelfDeliveryTemplate.Address, value.DefaultOrEmpty());
        }

        public string Coordinates
        {
            get
            {
                var latitude = Params.ElementOrDefault(SelfDeliveryTemplate.Latitude, "").TryParseDecimal(true);
                var longitude = Params.ElementOrDefault(SelfDeliveryTemplate.Longitude, "").TryParseDecimal(true);
                
                if (latitude.HasValue
                    && longitude.HasValue)
                    return $"{latitude.Value.ToInvariantString()}, {longitude.Value.ToInvariantString()}";
                
                return string.Empty;
            }
            set
            {
                decimal? latitude = null;
                decimal? longitude = null;

                var coordinats = value?.Replace("[", "").Replace("]", "").Split(',');
                if (coordinats?.Length == 2)
                {
                    latitude = coordinats[0].Trim().TryParseDecimal(true);
                    longitude = coordinats[1].Trim().TryParseDecimal(true);
                }

                Params.TryAddValue(SelfDeliveryTemplate.Latitude, (latitude?.ToInvariantString()).DefaultOrEmpty());
                Params.TryAddValue(SelfDeliveryTemplate.Longitude, (longitude?.ToInvariantString()).DefaultOrEmpty());
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShippingPrice.IsNullOrEmpty())
                yield return new ValidationResult("Укажите стоимость доставки", new[] { "ShippingPrice" });
            else if (!ShippingPrice.IsDecimal())
                yield return new ValidationResult("Стоимость доставки дожна быть числом", new[] { "ShippingPrice" });
        }
    }
}
