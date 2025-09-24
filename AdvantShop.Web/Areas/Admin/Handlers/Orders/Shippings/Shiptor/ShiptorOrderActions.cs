using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.Shiptor;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Shiptor
{
    public class ShiptorOrderActions
    {
        private readonly int _orderId;

        public ShiptorOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Shiptor.Shiptor.KeyNameOrderShiptorIdInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = string.IsNullOrEmpty(orderAdditionalData),
            };
        }
    }
}
