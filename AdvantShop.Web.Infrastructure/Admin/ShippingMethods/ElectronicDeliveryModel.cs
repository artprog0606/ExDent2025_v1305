using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.ElectronicDelivery;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("ElectronicDelivery")]
    public class ElectronicDeliveryModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ShippingPrice
        {
            get { return Params.ElementOrDefault(ElectronicDeliveryTemplate.ShippingPrice, "0"); }
            set { Params.TryAddValue(ElectronicDeliveryTemplate.ShippingPrice, value.TryParseFloat().ToString()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ShippingPrice))
                yield return new ValidationResult("Укажите стоимость доставки");
            
            if (!ShippingPrice.IsDecimal())
                yield return new ValidationResult("Стоимость доставки дожна быть числом");
        }
    }
}
