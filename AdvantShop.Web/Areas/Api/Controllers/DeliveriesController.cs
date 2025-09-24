using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Deliveries;
using AdvantShop.Areas.Api.Models.Deliveries;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class DeliveriesController : BaseApiController
    {
        // GET deliveries/types
        [HttpGet]
        public JsonResult GetShippingTypes(ShippingFilter filter) => JsonApi(new GetShippingTypes(filter));

        // POST deliveries/check-delivery-zone
        [HttpPost]
        public JsonResult CheckShippingByDeliveryZones(ShippingAddress address) => JsonApi(new CheckShippingByDeliveryZones(address));
        
        // POST deliveries/calculate
        [HttpPost]
        public JsonResult CalculateDeliveries(CalculateDeliveriesModel model) => JsonApi(new CalculateDeliveries(model));

        
        // GET deliveries/point-deliveries
        [HttpGet]
        public JsonResult GetPointDeliveries(PointDeliveryFilter filter) => JsonApi(new GetPointDeliveries(filter));
    }
}