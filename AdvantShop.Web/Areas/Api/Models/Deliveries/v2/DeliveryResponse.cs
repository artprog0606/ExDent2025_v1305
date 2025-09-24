using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Deliveries.v2
{
    internal sealed class DeliveryResponse : IApiResponse
    {
        public List<Delivery> Deliveries { get; }
        public NearestDelivery NearestDelivery { get; }
        
        public DeliveryResponse(List<Delivery> deliveries, NearestDelivery nearestDelivery)
        {
            Deliveries = deliveries;
            NearestDelivery = nearestDelivery;
        }
    }

    internal sealed class NearestDelivery
    {
        public int Id { get; set; }
        public string PointId { get; set; }
    }
}