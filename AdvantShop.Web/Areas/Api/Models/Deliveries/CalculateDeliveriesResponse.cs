using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Shipping;

namespace AdvantShop.Areas.Api.Handlers.Deliveries
{
    public class CalculateDeliveriesResponse : List<DeliveryItem>, IApiResponse 
    {
        public CalculateDeliveriesResponse(IEnumerable<DeliveryItem> deliveries)
        {
            this.AddRange(deliveries);
        }
    }

    public class DeliveryItem
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string DeliveryTime { get; }
        public string PreparedPrice { get; }


        public DeliveryItem(BaseShippingOption option)
        {
            Id = option.Id;
            Name = option.NameRate ?? option.Name;
            Description = !string.IsNullOrWhiteSpace(option.Desc) ? option.Desc : null;
            DeliveryTime = !string.IsNullOrWhiteSpace(option.DeliveryTime) ? option.DeliveryTime : null;
            PreparedPrice = option.FinalRate == 0 ? option.ZeroPriceMessage : option.FinalRate.FormatPrice();
        }
    }
}