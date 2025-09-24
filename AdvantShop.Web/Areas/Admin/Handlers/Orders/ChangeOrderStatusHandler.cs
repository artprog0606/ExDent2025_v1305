using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Orders;
using AdvantShop.Trial;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class ChangeOrderStatusHandlerResponse
    {
        public bool result { get; set; }
        public string color { get; set; }
        public string basis { get; set; }
        public bool isNotifyUserEmail { get; set; }
        public bool isNotifyUserSms { get; set; }
    }
    
    public class ChangeOrderStatusHandler : ICommandHandler<ChangeOrderStatusHandlerResponse>
    {
        private readonly int _orderId;
        private readonly int _statusId;
        private readonly string _basis;

        public ChangeOrderStatusHandler(int orderId, int statusId, string basis)
        {
            _orderId = orderId;
            _statusId = statusId;
            _basis = basis;
        }

        public ChangeOrderStatusHandlerResponse Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            var status = OrderStatusService.GetOrderStatus(_statusId);

            if (order == null || status == null || !OrderService.CheckAccess(order) || order.OrderStatusId == _statusId)
                throw new BlException("Заказ не найден");

            var basis = _basis;

            if (!string.IsNullOrEmpty(basis))
            {
                OrderService.UpdateStatusComment(_orderId, basis, trackChanges: !order.IsDraft);
            }
            else
            {
                basis = order.StatusComment;
            }

            OrderStatusService.ChangeOrderStatus(_orderId, _statusId, basis);

            TrialService.TrackEvent(TrialEvents.ChangeOrderStatus, "");
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderStatusChanged);

            return new ChangeOrderStatusHandlerResponse()
            {
                result = true,
                color = status.Color,
                basis = basis,
                isNotifyUserEmail = !status.Hidden && order.OrderCustomer.Email.IsNotEmpty(),
                isNotifyUserSms = !status.Hidden && order.OrderCustomer.Phone.IsNotEmpty() &&
                                  HasSmsTemplateOnOrderStatus(status.StatusID)
            };
        }
        
        private bool HasSmsTemplateOnOrderStatus(int orderStatusId)
        {
            var module = SmsNotifier.GetActiveSmsModule();
            if (module == null)
                return false;

            var smsTemplate = SmsOnOrderChangingService.GetByOrderStatusId(orderStatusId);
            
            return smsTemplate != null && smsTemplate.Enabled && !string.IsNullOrEmpty(smsTemplate.SmsText);
        }
    }
}