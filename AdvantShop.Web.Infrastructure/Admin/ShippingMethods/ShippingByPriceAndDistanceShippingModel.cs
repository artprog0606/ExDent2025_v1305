using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ShippingByRangePriceAndDistance;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("ShippingByPriceAndDistance")]
    public class ShippingByPriceAndDistanceShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        /// <summary>
        /// json string, example: [{"OrderPrice":10.0,"Price":100.0},{"OrderPrice":20.2,"Price":123.45}]
        /// </summary>
        public string PriceLimit
        {
            get { return Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.PriceLimit); }
            set { Params.TryAddValue(ShippingByRangePriceAndDistanceTemplate.PriceLimit, value.DefaultOrEmpty()); }
        }
        
        public string MaxDistance
        {
            get { return Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.MaxDistance, "50"); }
            set { Params.TryAddValue(ShippingByRangePriceAndDistanceTemplate.MaxDistance, value.TryParseFloat().ToString()); }
        }

        public string DeliveryTime
        {
            get { return Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.DeliveryTime); }
            set { Params.TryAddValue(ShippingByRangePriceAndDistanceTemplate.DeliveryTime, value.DefaultOrEmpty()); }
        }

        public string DefaultDistance
        {
            get { return Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.DefaultDistance, "1"); }
            set { Params.TryAddValue(ShippingByRangePriceAndDistanceTemplate.DefaultDistance, value.TryParseInt().ToString()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (string.IsNullOrEmpty(DistanceLimit))
            //    yield return new ValidationResult("Укажите диапазон");

            return new List<ValidationResult>();
        }
    }
}
