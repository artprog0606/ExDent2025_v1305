using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Checkout;
using AdvantShop.Areas.Api.Models.Checkout;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class CheckoutController : BaseApiController
    {
        // POST api/checkout
        [HttpPost]
        public JsonResult Get(CheckoutApiModel model) => JsonApi(new GetCheckoutApi(model));
    }
}