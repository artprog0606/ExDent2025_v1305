namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipAddOrderResponseModel : ApiShipErrorModel
    {
        public int OrderId { get; set; }
        public string Created {  get; set; }
    }
}
