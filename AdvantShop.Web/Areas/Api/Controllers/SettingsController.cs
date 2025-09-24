using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Settings;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest]
    public class SettingsController : BaseApiController
    {
        // GET api/settings?keys=settingName1,settingName2
        [HttpGet, AuthApiKey]
        public JsonResult Get(string keys) => JsonApi(new GetSettings(keys));
        
        // GET api/settings/dadata
        [HttpGet, AuthApiKeyByUser]
        public JsonResult Dadata() => JsonApi(new GetDadataSettings());
    }
}