namespace AdvantShop.Areas.Api.Models.Deliveries.v2
{
    public sealed class DeliveryFilter : ShippingFilter
    {
        public bool? InHouse { get; set; }
        
        public UserCoordinates UserCoordinates { get; set; }
    }
    
    public sealed class UserCoordinates
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}