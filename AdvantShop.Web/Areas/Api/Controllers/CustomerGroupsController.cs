using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.CustomerGroups;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey]
    public class CustomerGroupsController : BaseApiController
    {
        // GET api/customergroups
        [HttpGet]
        public JsonResult Index() => JsonApi(new GetCustomerGroups());
    }
}