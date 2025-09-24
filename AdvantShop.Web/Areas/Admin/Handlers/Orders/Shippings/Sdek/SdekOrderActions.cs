using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.Sdek;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek
{
    public class SdekOrderActions
    {
        private readonly int _orderId;

        public SdekOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var dispatchNumber = OrderService.GetOrderAdditionalData(_orderId, Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData);
            var sdekOrderUuid = OrderService.GetOrderAdditionalData(_orderId, Shipping.Sdek.Sdek.KeyNameSdekOrderUuidInOrderAdditionalData);

            return new OrderActionsModel()
            {
                OrderId = _orderId,
                ShowSendOrder = dispatchNumber.IsNullOrEmpty() && sdekOrderUuid.IsNullOrEmpty(),
                ShowDeleteOrder = dispatchNumber.IsNotEmpty() || sdekOrderUuid.IsNotEmpty(),
                ShowDownloadPrintedFormOrder = dispatchNumber.IsNotEmpty(),
                ShowDownloadBarCodeOrder = dispatchNumber.IsNotEmpty(),
                DispatchNumber = dispatchNumber
            };
        }
    }
}
