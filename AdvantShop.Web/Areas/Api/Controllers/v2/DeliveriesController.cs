using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Deliveries.v2;
using AdvantShop.Areas.Api.Models.Deliveries.v2;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers.v2
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class DeliveriesController : BaseApiController
    {
        // POST api/v2/deliveries
        [HttpPost]
        public JsonResult GetDeliveries(DeliveryFilter filter) => JsonApi(new GetDeliveries(filter));
    }
}