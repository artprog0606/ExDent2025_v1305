using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Deliveries
{
    public class CheckShippingByDeliveryZonesResponse : IApiResponse
    {
        public bool HasDelivery { get; set; }
        public int[][] WarehousesByZone { get; set; }
    }
}