using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Webhook;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Api
{
    public class ApiWebhookExecuter : WebhookExecuter
    {
        public static void OrderAdded(Order order)
        {
            var data = OrderModel.FromOrder(order);
            if (data == null)
                return;

            MakeRequestWithOutServiceAsync<ApiWebhookUrlList, ApiWebhookUrl>(x => x.EventType == ApiWebhookEventType.OrderCreated, data);
        }

        public static void OrderStatusChanged(Order order)
        {
            var data = OrderStatusChangedModel.FromOrder(order);
            if (data == null)
                return;

            MakeRequestWithOutServiceAsync<ApiWebhookUrlList, ApiWebhookUrl>(x => x.EventType == ApiWebhookEventType.OrderStatusChanged, data);
        }

        public static void OrderPaymentStatusChanged(Order order)
        {
            var data = OrderPaymentStatusChangedModel.FromOrder(order);
            if (data == null)
                return;

            MakeRequestWithOutServiceAsync<ApiWebhookUrlList, ApiWebhookUrl>(x => x.EventType == ApiWebhookEventType.OrderPaymentStatusChanged, data);
        }

    }


    public class ApiWebhookUrl : WebhookUrl
    {
        public ApiWebhookEventType EventType { get; set; }
    }

    public class ApiWebhookUrlList : WebhookUrlList<ApiWebhookUrl>
    {
        public override EWebhookType WebhookType { get { return EWebhookType.Api; } }
    }

    public enum ApiWebhookEventType
    {
        None = 0,
        [Localize("Core.Services.ApiWebhookEventType.OrderCreated")]
        OrderCreated = 1,
        [Localize("Core.Services.ApiWebhookEventType.OrderStatusChanged")]
        OrderStatusChanged = 2,
        [Localize("Core.Services.ApiWebhookEventType.OrderPaymentStatusChanged")]
        OrderPaymentStatusChanged = 3,
    }
}
