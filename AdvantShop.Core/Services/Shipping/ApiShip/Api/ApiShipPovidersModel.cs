using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipPovidersModel
    {
        public List<ApiShipProvider> Rows { get; set; }
        public MetaTariff Meta { get; set; }
    }

    public class ApiShipProvider
    {
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
