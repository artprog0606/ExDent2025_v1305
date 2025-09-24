namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipTokenRequestModel
    {
        public string login { get; set; }
        public string password { get; set; }
    }

    public class ApiShipTokenResponseModel
    {
        public string token { get; set; }
    }
}
