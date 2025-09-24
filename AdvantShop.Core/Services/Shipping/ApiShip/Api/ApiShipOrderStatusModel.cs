namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipOrderStatusModel
    {
        public ApiShipOrderInfo OrderInfo { get; set; }
        public ApiShipStatus Status { get; set; }
    }

    public class ApiShipOrderInfo
    {
        public int OrderId { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderNumber { get; set; }
        public string ReturnProviderNumber { get; set; }
        public string AdditionalProviderNumber { get; set; }
        public string Barcode { get; set; }
        public string ClientNumber { get; set; }
        public string TrackingUrl { get; set; }
    }
    public class ApiShipStatus
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Created { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderDescription { get; set; }
        public string CreatedProvider { get; set; }
        public string ErrorCode { get; set; }
    }
}
