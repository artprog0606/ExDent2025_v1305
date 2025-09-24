using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Bonuses;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey, BonusSystem]
    public class BonusGradesController : BaseApiController
    {
        // Грейды
        // GET api/bonus-grades
        [HttpGet]
        public JsonResult Grades() => JsonApi(new GetGrades());
    }
}