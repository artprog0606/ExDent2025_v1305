using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.Yandex;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Yandex
{
    public  class ActionsYandexOrder
    {
        private readonly int _orderId;
        public ActionsYandexOrder(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var yandexRequestId = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Yandex.YandexDelivery.KeyNameYandexRequestIdInOrderAdditionalData);
            var yandexOrderIsCanceled = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Yandex.YandexDelivery.KeyNameYandexOrderIsCanceledInOrderAdditionalData);

            return new OrderActionsModel()
            {
                ShowSendOrder = string.IsNullOrEmpty(yandexRequestId),
                ShowCancelOrder = !string.IsNullOrEmpty(yandexRequestId)
                                      && string.IsNullOrEmpty(yandexOrderIsCanceled),
            };
        }
    }
}
