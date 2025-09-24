namespace AdvantShop.Areas.Api.Models.Deliveries
{
    public class PointDeliveryFilter
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        
        public int? TypeId { get; set; }
        
        public bool? InHouse { get; set; }
    }
}