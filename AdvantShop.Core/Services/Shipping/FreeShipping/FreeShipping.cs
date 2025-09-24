//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.FreeShipping
{
    public struct FreeShippingTemplate
    {
        public const string DeliveryTime = "DeliveryTime";
    }

    [ShippingKey("FreeShipping")]
    public class FreeShipping : BaseShipping, IShippingNoUseExtracharge, IShippingNoUseCurrency, IShippingNoUseExtraDeliveryTime, IShippingNoUseTax, IShippingNoUsePaymentMethodAndSubjectTypes, IShippingUseDeliveryInterval, IShippingRequiresSpecifyingTypeOfDelivery
    {
        public FreeShipping(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var option = new BaseShippingOption(_method, _totalPrice)
            {
                Rate = 0F,
                DeliveryTime = _method.Params.ElementOrDefault(FreeShippingTemplate.DeliveryTime)
            };
            return new List<BaseShippingOption> { option };
        }
    }
}