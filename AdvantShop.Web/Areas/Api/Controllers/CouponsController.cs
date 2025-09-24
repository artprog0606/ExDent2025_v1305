using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Coupons;
using AdvantShop.Areas.Api.Models.Coupons;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class CouponsController : BaseApiController
    {
        // POST api/coupons/me/add
        [HttpPost]
        public JsonResult MeAdd(CouponModel coupon) => JsonApi(new AddCouponApi(coupon));
        
        // POST api/coupons/me/remove
        [HttpPost]
        public JsonResult MeRemove() => JsonApi(new RemoveCouponApi());
    }
}