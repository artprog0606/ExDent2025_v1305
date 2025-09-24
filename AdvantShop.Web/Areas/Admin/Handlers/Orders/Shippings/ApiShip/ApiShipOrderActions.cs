using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.ApiShip;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.ApiShip
{
    public class ApiShipOrderActions
    {
        private readonly int _orderId;

        public ApiShipOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var result = OrderService.GetOrderAdditionalData(_orderId, Shipping.ApiShip.ApiShip.KeyTextSendOrder);
            return new OrderActionsModel()
            {
                OrderId = _orderId,
                SendOrder = !string.IsNullOrEmpty(result) ? bool.Parse(result) : false,
            };
        }
    }
}
