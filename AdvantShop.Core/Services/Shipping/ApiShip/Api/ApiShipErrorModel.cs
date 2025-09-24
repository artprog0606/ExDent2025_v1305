using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipErrorModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string Moreinfo { get; set; }
        public List<ErrorText> Errors { get; set; }
    }

    public class ErrorText
    {
        public string Field { get; set; } 
        public string Message { get; set; }
    }
}
