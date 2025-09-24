using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderStatusChangedModel
    {
        public static OrderStatusChangedModel FromOrder(Order order)
        {
            if (order == null || order.IsDraft)
                return null;

            return new OrderStatusChangedModel
            {
                Id = order.OrderID,
                Number = order.Number,
                Status = OrderStatusModel.FromOrderStatus(order.OrderStatus),
            };
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public OrderStatusModel Status { get; set; }
    }
}
