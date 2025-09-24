namespace AdvantShop.Web.Admin.Models.Orders.PecEasyway
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        public bool ShowSendOrder { get; set; }
        public bool ShowCancelOrder { get; set; }
        public bool IsCanceledOrder { get; set; }
        public bool ShowGetLabel { get; set; }
    }
}
