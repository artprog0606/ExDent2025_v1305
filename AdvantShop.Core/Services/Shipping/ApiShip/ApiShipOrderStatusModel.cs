namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipOrderStatusModel
    {
        public ApiShipOrderInfo orderInfo { get; set; }
        public ApiShipStatus status { get; set; }
    }

    public class ApiShipOrderInfo
    {
        public int orderId { get; set; }
        public string providerKey { get; set; }
        public string providerNumber { get; set; }
        public string returnProviderNumber { get; set; }
        public string additionalProviderNumber { get; set; }
        public string barcode { get; set; }
        public string clientNumber { get; set; }
        public string trackingUrl { get; set; }
    }
    public class ApiShipStatus
    {
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created { get; set; }
        public string providerCode { get; set; }
        public string providerName { get; set; }
        public string providerDescription { get; set; }
        public string createdProvider { get; set; }
        public string errorCode { get; set; }
    }
}
