namespace AdvantShop.Web.Admin.Models.Orders.RussianPost
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        public bool ShowSendOrder { get; set; }
        public bool ShowDeleteOrder { get; set; }
        public bool ShowGetDocumentsBeforShipment { get; set; }
        public bool ShowGetDocuments { get; set; }
    }
}
