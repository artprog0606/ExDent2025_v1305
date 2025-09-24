namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipTokenRequestModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class ApiShipTokenResponseModel
    {
        public string Token { get; set; }
    }
}
