namespace AdvantShop.Web.Admin.Models.Orders.Sdek
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        public bool ShowSendOrder { get; set; }
        public bool ShowDeleteOrder { get; set; }
        public bool ShowDownloadPrintedFormOrder { get; set; }
        public bool ShowDownloadBarCodeOrder { get; set; }
        public string DispatchNumber { get; set; }
    }
}
