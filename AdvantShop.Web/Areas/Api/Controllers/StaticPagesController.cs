using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.StaticPages;
using AdvantShop.Areas.Api.Models.StaticPages;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class StaticPagesController : BaseApiController
    {
        // GET staticpages
        [HttpGet]
        public JsonResult GetList(StaticPagesFilter filter) => JsonApi(new GetStaticPagesApi(filter));
        
        // GET staticpages/{id}
        [HttpGet]
        public JsonResult GetStaticPage(int id) => JsonApi(new GetStaticPageApi(id));
    }
}