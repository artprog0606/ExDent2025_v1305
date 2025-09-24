using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Web.Admin.Models.Settings.Api
{
    public class WebhookModel : ApiWebhookUrl
    {
        public static WebhookModel FromApiWebhook(ApiWebhookUrl apiWebhookUrl)
        {
            return new WebhookModel
            {
                EventType = apiWebhookUrl.EventType,
                Url = apiWebhookUrl.Url
            };
        }

        public string EventTypeName { get { return EventType.Localize(); } }
    }
}
