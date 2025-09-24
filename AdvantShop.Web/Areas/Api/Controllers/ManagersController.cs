using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Managers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey]
    public class ManagersController : BaseApiController
    {
        // GET api/managers
        [HttpGet]
        public JsonResult Index() => JsonApi(new GetManagers());
    }
}