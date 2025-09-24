using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.PickPoint;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PickPoint
{
    public class PickPointOrderActions
    {
        private readonly int _orderId;

        public PickPointOrderActions(int orderId)
        {
            _orderId = orderId;
        }


        public OrderActionsModel Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.PickPoint.PickPoint.KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = string.IsNullOrEmpty(orderAdditionalData),
                ShowDeleteOrder = !string.IsNullOrEmpty(orderAdditionalData),
                ShowMakeLabel = !string.IsNullOrEmpty(orderAdditionalData),
                ShowMakeZebraLabel = !string.IsNullOrEmpty(orderAdditionalData),
            };
        }
    }
}
