using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Notifications;
using AdvantShop.Areas.Api.Models.Notifications;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey]
    public class NotificationsController : BaseApiController
    {
        // POST notifications/sendPush
        [HttpPost]
        public JsonResult SendPush(NotificationModel notification) => JsonApi(new SendPush(notification));
    }
}