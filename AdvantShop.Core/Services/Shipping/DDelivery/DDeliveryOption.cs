using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.DDelivery
{
    public class DDeliveryOption : BaseShippingOption
    {
        public DDeliveryOption() { }

        public DDeliveryOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = false;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public int DeliveryCompanyId { get; set; }
        public int DeliveryTypeId { get; set; }
    }
}