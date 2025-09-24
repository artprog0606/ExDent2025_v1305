using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Products;

namespace AdvantShop.Areas.Api.Models.Deliveries
{
    public class CalculateDeliveriesModel
    {
        public CalculateDeliveriesAddress Address { get; set; }
        public List<GetPriceModel> Products { get; set; }
        
        public bool InProductDetails { get; set; }
    }

    public class CalculateDeliveriesAddress
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}