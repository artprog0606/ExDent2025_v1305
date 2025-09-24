using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.Pec;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Pec
{
    public class PecOrderActions
    {
        private readonly int _orderId;

        public PecOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Pec.Pec.KeyNameCargoCodeInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = string.IsNullOrEmpty(orderAdditionalData),
                ShowCancelOrder = !string.IsNullOrEmpty(orderAdditionalData),
            };
        }
    }
}
