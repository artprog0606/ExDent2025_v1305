using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.WishList;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class WishListController : BaseApiController
    {
        // GET api/wishlist
        [HttpGet]
        public JsonResult GetList() => JsonApi(new GetWishListApi());
        
        // POST api/wishlist/add
        [HttpPost]
        public JsonResult Add(int offerId) => JsonApi(new AddToWishList(offerId));
        
        // POST api/wishlist/remove
        [HttpPost]
        public JsonResult Remove(int offerId) => JsonApi(new RemoveFromWishList(offerId));
    }
}