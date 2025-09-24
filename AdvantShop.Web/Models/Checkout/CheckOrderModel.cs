using AdvantShop.Models.MyAccount;

namespace AdvantShop.Models.Checkout
{
    public class CheckOrderModel
    {
        public string Error { get; set; }
        public string OrderNumber { get; set; }
        public string StatusName { get; set; }
        public string StatusComment { get; set; }
        public ShippingHistoryOfMovementInfo ShippingHistory { get; set; }
    }
}