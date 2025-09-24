using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Cart;
using AdvantShop.Areas.Api.Models.Cart;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class CartController : BaseApiController
    {
        // POST api/cart
        [HttpPost]
        public JsonResult Get(CartApiModel cart) => JsonApi(new GetCartApi(cart));
        
        // GET api/cart
        [HttpGet]
        public JsonResult GetCurrentCart() => JsonApi(new GetCartApi());
    }
}