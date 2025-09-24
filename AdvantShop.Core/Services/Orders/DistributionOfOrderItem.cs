namespace AdvantShop.Core.Services.Orders
{
    public class DistributionOfOrderItem
    {
        public int OrderItemId { get; set; }
        public int WarehouseId { get; set; }
        public float Amount { get; set; }
        public float DecrementedAmount { get; set; }
    }
}