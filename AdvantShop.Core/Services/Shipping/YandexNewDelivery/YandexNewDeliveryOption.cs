using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewDeliveryOption : BaseShippingOption
    {
        public YandexNewDeliveryAdditionalData PickPointAdditionalData { get; set; }

        public YandexNewDeliveryOption() { }
        public YandexNewDeliveryOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = string.Empty,
                PickPointAddress = string.Empty,
                AdditionalData = JsonConvert.SerializeObject(PickPointAdditionalData)
            };
        }
    }
}
