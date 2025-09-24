using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Orders;
using AdvantShop.Areas.Api.Models.Orders;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class OrdersController : BaseApiController
    {
        // GET orders/me
        [HttpGet]
        public JsonResult Me(OrdersMeFilterModel filter) => JsonApi(new GetOrdersMe(filter));
        
        // GET orders/me/{id}
        [HttpGet]
        public JsonResult MeGetById(int id) => JsonApi(new GetOrder(id, CustomerContext.CustomerId));
        
        // POST orders/me/{id}/review
        [HttpPost]
        public JsonResult MeAddOrderReview(int id, OrderReview review) => JsonApi(new AddOrderReview(id, review));
        
        // POST orders/me/{id}/cancel
        [HttpPost]
        public JsonResult MeCancelOrder(int id) => JsonApi(new CancelOrder(id));
    }
}