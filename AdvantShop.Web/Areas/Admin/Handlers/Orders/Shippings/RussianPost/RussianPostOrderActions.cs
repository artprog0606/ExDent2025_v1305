using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.RussianPost;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.RussianPost
{
    public class RussianPostOrderActions
    {
        private readonly int _orderId;

        public RussianPostOrderActions(int orderId)
        {
            _orderId = orderId;
        }


        public OrderActionsModel Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.RussianPost.RussianPost.KeyNameOrderRussianPostIdInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = string.IsNullOrEmpty(orderAdditionalData),
                ShowGetDocumentsBeforShipment = !string.IsNullOrEmpty(orderAdditionalData),
                ShowGetDocuments = !string.IsNullOrEmpty(orderAdditionalData),
                ShowDeleteOrder = !string.IsNullOrEmpty(orderAdditionalData)
            };
        }
    }
}
