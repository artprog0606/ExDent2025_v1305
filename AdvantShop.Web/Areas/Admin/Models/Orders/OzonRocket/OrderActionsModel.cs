namespace AdvantShop.Web.Admin.Models.Orders.OzonRocket
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        public bool ShowSendOrder { get; set; }
        public bool ShowUpdateOrder { get; set; }
        public bool ShowCancelOrder { get; set; }
    }
}