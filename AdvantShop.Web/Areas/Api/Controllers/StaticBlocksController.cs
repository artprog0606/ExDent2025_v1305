using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.StaticBlocks;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class StaticBlocksController : BaseApiController
    {
        // GET staticblocks?keys=key1,key2
        [HttpGet]
        public JsonResult GetList(string keys) => JsonApi(new GetStaticBlocks(keys));
    }
}