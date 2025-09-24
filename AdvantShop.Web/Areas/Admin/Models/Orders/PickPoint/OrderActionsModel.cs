namespace AdvantShop.Web.Admin.Models.Orders.PickPoint
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        public bool ShowSendOrder { get; set; }
        public bool ShowDeleteOrder { get; set; }
        public bool ShowMakeLabel { get; set; }
        public bool ShowMakeZebraLabel { get; set; }
    }
}
