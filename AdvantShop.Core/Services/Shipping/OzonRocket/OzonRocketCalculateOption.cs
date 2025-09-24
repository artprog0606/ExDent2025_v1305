using AdvantShop.Shipping.OzonRocket.Api;

namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketCalculateOption
    {
        public long? FromPlaceId { get; set; }
        public long DeliveryVariantId { get; set; }
        public bool IsCourier { get; set; }
    }
}