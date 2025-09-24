using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderPaymentStatusChangedModel
    {
        public static OrderPaymentStatusChangedModel FromOrder(Order order)
        {
            if (order == null || order.IsDraft)
                return null;

            return new OrderPaymentStatusChangedModel
            {
                Id = order.OrderID,
                Number = order.Number,
                IsPaied = order.Payed,
            };
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public bool IsPaied { get; set; }
    }
}
