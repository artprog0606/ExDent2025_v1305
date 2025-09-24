using System.Collections.Generic;
using AdvantShop.Shipping;

namespace AdvantShop.Models.Checkout
{
    public class GetCheckoutShippingsResponse
    {
        public BaseShippingOption selectShipping { get; set; }
        public List<BaseShippingOption> option { get; set; }
        public string typeCalculationVariants { get; set; }
    }
}