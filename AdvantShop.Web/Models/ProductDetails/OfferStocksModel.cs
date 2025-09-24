using System.Collections.Generic;

namespace AdvantShop.Models.ProductDetails
{
    public class OfferStocksModel
    {
        public List<WarehouseStockModel> Stocks { get; set; }
    }

    public class WarehouseStockModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string[] TimeOfWorkList { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public string AddressComment { get; set; }
        public string Stock { get; set; }
        public string StockColor { get; set; }
        public string Unit { get; set; }
    }
}