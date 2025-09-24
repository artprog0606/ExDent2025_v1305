using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Saas;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessBySettings : ActionFilterAttribute
    {
        private readonly List<EProviderSetting> _providerSettings;
        private readonly ESaasProperty? _saasProperty;
        private readonly ETypeRedirect? _redirect;

        #region Constructors

        public AccessBySettings()
            : this(null, null, null)
        { }

        public AccessBySettings(EProviderSetting providerSetting)
            : this(null, null, new[] { providerSetting })
        { }

        public AccessBySettings(ESaasProperty saasProperty)
            : this((ESaasProperty?)saasProperty, null, null)
        { }

        public AccessBySettings(EProviderSetting providerSetting, ETypeRedirect redirect)
            : this(null, (ETypeRedirect?)redirect, new[] { providerSetting })
        { }

        public AccessBySettings(ETypeRedirect redirect, params EProviderSetting[] providerSettings)
            : this(null, (ETypeRedirect?)redirect, providerSettings)
        { }

        public AccessBySettings(ESaasProperty saasProperty, ETypeRedirect redirect)
            : this((ESaasProperty?)saasProperty, (ETypeRedirect?)redirect, null)
        { }

        public AccessBySettings(EProviderSetting providerSetting, ESaasProperty saasProperty, ETypeRedirect redirect)
            : this((ESaasProperty?)saasProperty, (ETypeRedirect?)redirect, new[] { providerSetting })
        { }

        private AccessBySettings(ESaasProperty? saasProperty, ETypeRedirect? redirect, EProviderSetting[] providerSettings)
        {
            _providerSettings = providerSettings != null ? providerSettings.ToList() : new List<EProviderSetting>();
            _saasProperty = saasProperty;
            _redirect = redirect;
        }

        #endregion

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                return;
            }

            if (filterContext.IsChildAction || !_redirect.HasValue)
            {
                return;
            }

            var providerSetting = _providerSettings
                    .Select(setting => SettingProvider.GetSqlSettingValue(setting))
                    .Any(strVal => strVal.IsNullOrEmpty() || strVal.TryParseBool());
            var saasSetting = !_saasProperty.HasValue;

            if (_saasProperty.HasValue)
                saasSetting = SaasDataService.IsEnabledFeature(_saasProperty.Value);

            if (providerSetting && saasSetting)
                return;

            var controller = (string)filterContext.RouteData.Values["controller"];
            var action = (string)filterContext.RouteData.Values["action"];

            var strCompare = StringComparison.OrdinalIgnoreCase;
            if ((controller.Equals("error", strCompare) && action.Equals("liccheck", strCompare)) ||
                (controller.Equals("user", strCompare) && action.Equals("logout", strCompare)))
                return;

            if (_redirect.Value == ETypeRedirect.Empty)
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
                            var url = "~/landing/landing?landingId=" + lp.Id +
                                      (requestUrl != null && !string.IsNullOrEmpty(requestUrl.Query)
                                          ? "&" + requestUrl.Query.TrimStart('?')
                                          : "");

                            filterContext.Result = new TransferResult(url);
                            return;
                        }
                    }
                }

                needAdminRedirect = true;
            }
            
            if (_redirect == ETypeRedirect.AdminPanel || needAdminRedirect)
                filterContext.Result = new RedirectResult(UrlService.GetAdminUrl("login", useAdminAreaTemplates: false));

            if (_redirect.Value == ETypeRedirect.Error404)
                filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("error/notfound"));
        }
    }

    public enum ETypeRedirect
    {
        Empty,
        Error404,
        AdminPanel,
        LandingSite
    }
}
