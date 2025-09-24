using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipErrorModel
    {
        public string code { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public string moreinfo { get; set; }
        public List<ErrorText> errors { get; set; }
    }

    public class ErrorText
    {
        public string field { get; set; } 
        public string message { get; set; }
    }
}
