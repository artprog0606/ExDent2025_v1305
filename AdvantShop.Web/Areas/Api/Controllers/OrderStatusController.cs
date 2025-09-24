using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.OrderStatus;
using AdvantShop.Areas.Api.Model.OrderStatus;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    public class OrderStatusController : BaseApiController
    {
        [LogRequest, AuthApiKey, HttpPost]
        public JsonResult GetList(FilterOrderStatusesModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new GetOrderStatuses(model));
        }
    }
}