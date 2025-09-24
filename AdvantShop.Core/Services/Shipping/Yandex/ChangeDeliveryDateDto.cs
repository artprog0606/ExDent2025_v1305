using AdvantShop.Shipping.Yandex.Api;

namespace AdvantShop.Shipping.Yandex
{
    public class ChangeDeliveryDateDto
    {
        public string TimeOfDelivery { get; set; }
        public IntervalOffer SelectedInterval { get; set; }
    }
}