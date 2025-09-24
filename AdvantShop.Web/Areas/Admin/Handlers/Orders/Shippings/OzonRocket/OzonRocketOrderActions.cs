using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OzonRocket;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.OzonRocket
{
    public class OzonRocketOrderActions
    {
        private readonly int _orderId;

        public OzonRocketOrderActions(int orderId)
        {
            _orderId = orderId;
        }
 
        public OrderActionsModel Execute()
        {
            var ozonRocketOrderId = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIdInOrderAdditionalData);
            var ozonRocketOrderIsCanceled = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIsCanceledInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = string.IsNullOrEmpty(ozonRocketOrderId),
                ShowUpdateOrder = !string.IsNullOrEmpty(ozonRocketOrderId) 
                                  && string.IsNullOrEmpty(ozonRocketOrderIsCanceled),
                ShowCancelOrder = !string.IsNullOrEmpty(ozonRocketOrderId) 
                                  && string.IsNullOrEmpty(ozonRocketOrderIsCanceled),
            };
        }
    }
}