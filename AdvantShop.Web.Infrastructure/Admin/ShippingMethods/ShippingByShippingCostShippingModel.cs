using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.ShippingByShippingCost;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("ShippingByShippingCost")]
    public class ShippingByShippingCostShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public bool ByMaxShippingCost
        {
            get { return Params.ElementOrDefault(ShippingByShippingCostTemplate.ByMaxShippingCost).TryParseBool(); }
            set { Params.TryAddValue(ShippingByShippingCostTemplate.ByMaxShippingCost, value.ToString()); }
        }

        public bool UseAmount
        {
            get { return Params.ElementOrDefault(ShippingByShippingCostTemplate.UseAmount).TryParseBool(); }
            set { Params.TryAddValue(ShippingByShippingCostTemplate.UseAmount, value.ToString()); }
        }

        public string DeliveryTime
        {
            get { return Params.ElementOrDefault(ShippingByShippingCostTemplate.DeliveryTime); }
            set { Params.TryAddValue(ShippingByShippingCostTemplate.DeliveryTime, value.DefaultOrEmpty()); }
        }

        public string DefaultShippingCost
        {
            get { return Params.ElementOrDefault(ShippingByShippingCostTemplate.DefaultShippingCost, "0"); }
            set { Params.TryAddValue(ShippingByShippingCostTemplate.DefaultShippingCost, value.TryParseFloat().ToString()); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
