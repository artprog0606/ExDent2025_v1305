using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Preorder;
using AdvantShop.Areas.Api.Models.Preorder;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class PreorderController : BaseApiController
    {
        // POST api/preorder
        [HttpPost]
        public JsonResult Preorder(PreorderModel model) => JsonApi(new PreorderApi(model));
        
        // GET api/preorder/settings
        [HttpGet]
        public JsonResult GetSettings() => JsonApi(new GetPreorderSettings());
    }
}