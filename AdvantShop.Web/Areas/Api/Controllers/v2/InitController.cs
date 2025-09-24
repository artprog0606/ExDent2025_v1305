using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Inits;
using AdvantShop.Areas.Api.Models.Inits;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers.v2
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class InitController : BaseApiController
    {
        // GET init
        [HttpGet]
        public JsonResult Index(InitApiModel model) => JsonApi(new InitApiV2(model)); 
    }
}