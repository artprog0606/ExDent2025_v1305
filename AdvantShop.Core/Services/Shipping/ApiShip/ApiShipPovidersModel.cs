using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipPovidersModel
    {
        public List<ApiShipProvider> rows { get; set; }
        public MetaTariff meta { get; set; }
    }

    public class ApiShipProvider
    {
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
