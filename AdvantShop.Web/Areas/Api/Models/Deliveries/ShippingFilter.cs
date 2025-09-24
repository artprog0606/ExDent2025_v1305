using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Deliveries
{
    public class ShippingFilter
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        
        public string Type { get; set; }
        public List<string> Types { get; set; }
        
        public bool? LoadWarehouses { get; set; }
    }
}