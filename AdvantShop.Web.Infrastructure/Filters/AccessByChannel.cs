using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;
using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Saas;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessByChannel : ActionFilterAttribute
    {
        private readonly EProviderSetting _providerSetting;
        private readonly ETypeRedirect _redirect;

        public AccessByChannel(EProviderSetting providerSetting) : this(providerSetting, ETypeRedirect.AdminPanel)
        { }

        public AccessByChannel(EProviderSetting providerSetting, ETypeRedirect redirect)
        {
            _providerSetting = providerSetting;
            _redirect = redirect;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
                return;

            if (filterContext.IsChildAction)
                return;

            var settignValue = SettingProvider.GetSqlSettingValue(_providerSetting);
            var value = false;
            var providerSetting = string.IsNullOrEmpty(settignValue) || (Boolean.TryParse(settignValue, out value) && value);

            if (providerSetting && _providerSetting == EProviderSetting.ActiveLandingPage)
                providerSetting = new LpSiteService().HaveLandingSite(true);

            if (providerSetting)
                return;

            var controller = (string)filterContext.RouteData.Values["controller"];
            var action = (string)filterContext.RouteData.Values["action"];

            var strCompare = StringComparison.OrdinalIgnoreCase;
            if ((controller.Equals("error", strCompare) && action.Equals("liccheck", strCompare)) ||
                (controller.Equals("user", strCompare) && action.Equals("logout", strCompare)))
                return;

            if (_redirect == ETypeRedirect.Empty)
                filterContext.Result = new EmptyResult();

            var needAdminRedirect = false;
            if (_redirect == ETypeRedirect.LandingSite)
            {
                if (SaasDataService.IsSaasEnabled 
                    && SaasDataService.IsEnabledFeature(ESaasProperty.HaveLandingFunnel))
                {
                    var lpSiteService = new LpSiteService();
                    var lpSite = lpSiteService.GetList().FirstOrDefault(site => site.Enabled);
                    if (lpSite != null)
                    {
                        var lpService = new LpService();
                        var lp = lpService.GetList(lpSite.Id).FirstOrDefault(x => x.IsMain && x.Enabled);
                        if (lp != null)
                        {
                            var requestUrl = filterContext.RequestContext.HttpContext.Request.Url;
                            var url = "~/landing/landing?landingId=" + lp.Id + (requestUrl != null && !string.IsNullOrEmpty(requestUrl.Query) ? "&" + requestUrl.Query.TrimStart('?') : "");
                        
                            filterContext.Result = new TransferResult(url);
                            return;
                        }
                    }
                }
                needAdminRedirect = true;
            }
            
            if (_redirect == ETypeRedirect.AdminPanel || needAdminRedirect)
                filterContext.Result = new RedirectResult(UrlService.GetAdminUrl("login", useAdminAreaTemplates: false));

            if (_redirect == ETypeRedirect.Error404)
                filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("error/notfound"));
        }
    }
}
