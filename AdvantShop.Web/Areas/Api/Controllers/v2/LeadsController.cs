using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Leads;
using AdvantShop.Areas.Api.Models.Leads;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers.v2
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class LeadsController : BaseApiController
    {        
        public JsonResult Add(AddLeadV2Model model) => JsonApi(new AddLeadV2(model));
    }
}