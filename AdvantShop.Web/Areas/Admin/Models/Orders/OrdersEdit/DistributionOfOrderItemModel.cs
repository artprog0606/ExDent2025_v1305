namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class DistributionOfOrderItemModel
    {
        public int WarehouseId { get; set; }
        public float Amount { get; set; }
        public float DecrementedAmount { get; set; }
    }
}