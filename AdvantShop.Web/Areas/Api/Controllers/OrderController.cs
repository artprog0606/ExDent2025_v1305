using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Orders;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    public class OrderController : BaseApiController
    {
        [LogRequest, AuthApiKey, HttpPost]
        public JsonResult Add(AddOrderModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new AddOrder(), model);
        }

        [LogRequest, AuthApiKey, HttpGet]
        public JsonResult Get(int id)
        {
            return ProcessJsonResult(new GetOrder(id));
        }

        [LogRequest, AuthApiKey, HttpPost]
        public JsonResult GetList(FilterOrdersModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new GetOrders(model));
        }

        [LogRequest, AuthApiKey, HttpPost]
        public JsonResult ChangeStatus(ChangeStatusModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new ChangeStatus(model));
        }

        [LogRequest, AuthApiKey, HttpPost]
        public JsonResult SetPaid(SetPaidModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new SetPaid(model));
        }
    }
}