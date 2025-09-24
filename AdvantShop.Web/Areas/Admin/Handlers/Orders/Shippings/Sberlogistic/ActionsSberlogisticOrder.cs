using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.Sberlogistic;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sberlogistic
{
    public  class ActionsSberlogisticOrder
    {
        private readonly int _orderId;
        public ActionsSberlogisticOrder(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var uuid = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderUuidInOrderAdditionalData);
            var isCanceled = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderIsCanceledInOrderAdditionalData);

            return new OrderActionsModel()
            {
                ShowSendOrder = string.IsNullOrEmpty(uuid),
                ShowCancelOrder = !string.IsNullOrEmpty(uuid)
                                      && string.IsNullOrEmpty(isCanceled),
            };
        }
    }
}
