using System.Text;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public abstract class BizProcessOrderRule : BizProcessRule
    {
        public override EBizProcessObjectType ObjectType => EBizProcessObjectType.Order;

        public override string[] AvailableVariables
        {
            get { return new[] {"#ORDER_NUMBER#", "#NAME#", "#PHONE#", "#EMAIL#"}; }
        }

        public override string ReplaceVariables(string value, IBizObject bizObject)
        {
            var order = (Order)bizObject;
            var sb = new StringBuilder(value);
            sb.Replace("#ORDER_ID#", order.Number); // если кто-то использовал #ORDER_ID#
            sb.Replace("#ORDER_NUMBER#", order.Number);
            sb.Replace("#NAME#", order.OrderCustomer != null ? StringHelper.AggregateStrings(" ", order.OrderCustomer.LastName, order.OrderCustomer.FirstName, order.OrderCustomer.Patronymic) : "");
            sb.Replace("#PHONE#", order.OrderCustomer != null ? order.OrderCustomer.Phone : "");
            sb.Replace("#EMAIL#", order.OrderCustomer != null ? order.OrderCustomer.Email : "");
            
            return sb.ToString();
        }
    }

    public class BizProcessOrderCreatedRule : BizProcessOrderRule
    {
        public override EBizProcessEventType EventType => EBizProcessEventType.OrderCreated;
    }

    public class BizProcessOrderStatusChangedRule : BizProcessOrderRule
    {
        public override EBizProcessEventType EventType => EBizProcessEventType.OrderStatusChanged;

        private string _eventName;
        public override string EventName
        {
            get
            {
                if (_eventName != null)
                    return _eventName;

                var orderStatus = EventObjId.HasValue ? OrderStatusService.GetOrderStatus(EventObjId.Value) : null;
                _eventName = base.EventName + (orderStatus != null ? string.Format(" {0} {1}", LocalizationService.GetResource("Core.BizProcessRule.ChangeState.On"), orderStatus.StatusName) : string.Empty);

                return _eventName;
            }
        }
    }
}
