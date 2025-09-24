using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.PecEasyway;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PecEasyway
{
    public class PecEasywayOrderActions
    {
        private readonly int _orderId;

        public PecEasywayOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.PecEasyway.PecEasyway.KeyNameOrderIdInOrderAdditionalData);
            var orderAdditionalDataCancel = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.PecEasyway.PecEasyway.KeyNameOrderIsCanceledInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = string.IsNullOrEmpty(orderAdditionalData),
                ShowCancelOrder = !string.IsNullOrEmpty(orderAdditionalData) && string.IsNullOrEmpty(orderAdditionalDataCancel),
                IsCanceledOrder = !string.IsNullOrEmpty(orderAdditionalDataCancel),
                ShowGetLabel = !string.IsNullOrEmpty(orderAdditionalData) && string.IsNullOrEmpty(orderAdditionalDataCancel)
            };
        }
    }
}
