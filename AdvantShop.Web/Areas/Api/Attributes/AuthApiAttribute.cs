using System.Web.Mvc;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Saas;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Areas.Api.Attributes
{
    public class AuthApiKeyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var apiKey = SettingsApi.ApiKey;

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.API)
            {
                filterContext.Result = ErrorResult("Api is not available on this tariff");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                filterContext.Result = ErrorResult("Check apikey");
            }
            else
            {
                var apikey = filterContext.HttpContext.Request["apikey"];
                var authApiKey = filterContext.HttpContext.Request.Headers["X-API-KEY"];
                
                if (string.IsNullOrWhiteSpace(apikey) && string.IsNullOrWhiteSpace(authApiKey))
                {
                    filterContext.Result = ErrorResult("Invalid apikey");
                }
                else if (!string.IsNullOrWhiteSpace(apikey) && apikey != apiKey)
                {
                    filterContext.Result = ErrorResult("Invalid apikey");
                }
                else if (!string.IsNullOrWhiteSpace(authApiKey) && authApiKey != apiKey)
                {
                    filterContext.Result = ErrorResult("Invalid X-API-KEY");
                }
            }
        }

        private ActionResult ErrorResult(string error)
        {
            return new JsonNetResult {Data = new ApiError(error)};
        }
    }
}