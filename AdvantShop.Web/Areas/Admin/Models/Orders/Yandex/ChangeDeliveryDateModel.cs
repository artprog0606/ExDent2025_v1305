using AdvantShop.Shipping.Yandex.Api;
using AdvantShop.Shipping.Yandex;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Orders.Yandex
{
    public class ChangeDeliveryDateModel
    {
        public static ChangeDeliveryDateModel CreateByDto(ChangeDeliveryDateDto dto) =>
            new ChangeDeliveryDateModel
            {
                TimeOfDelivery = dto.TimeOfDelivery,
                SelectedInterval = dto.SelectedInterval,
            };
        
        public string TimeOfDelivery { get; set; }
        public IntervalOffer SelectedInterval { get; set; }

        public ChangeDeliveryDateDto ToDto() =>
            new ChangeDeliveryDateDto
            {
                TimeOfDelivery = TimeOfDelivery,
                SelectedInterval = SelectedInterval,
            };

    }
}
