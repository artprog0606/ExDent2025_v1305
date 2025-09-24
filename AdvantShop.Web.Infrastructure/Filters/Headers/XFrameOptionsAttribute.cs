using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Web.Infrastructure.Extensions;
using Enumerable=System.Linq.Enumerable;

namespace AdvantShop.Web.Infrastructure.Filters.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class XFrameOptionsAttribute : HttpHeaderAttributeBase
    {
        private readonly List<string> _yandexHosts = new List<string>
        {
            "webvisor.com",
            "yandex.ru",
            "yandex.net",
            "yandex.by",
            "yandex.com",
            "yandex.com.tr"
        };

        private readonly List<string> _advantshopAdminHosts = new List<string>
        {
            "my.advantshop.net",
            "my2.advantshop.net",
        };

        public XFrameOptionsPolicy Policy { get; set; }
        public string AllowUrl { get; set; }

        public XFrameOptionsAttribute(XFrameOptionsPolicy disabled)
        {
            this.Policy = disabled;
        }

        public override void SetHttpHeadersOnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                return;
            }

            var request = filterContext.HttpContext.Request;
            var url = request.Url.ToString();
            
            var socialType = UrlService.IsSocialUrl(url);
            if (socialType != UrlService.ESocialType.none)
                return;
            
            var referrer = request.GetUrlReferrer();

            if (SettingsGeneral.DisableXFrameOptionsHeader ||
                Demo.IsDemoEnabled ||
                referrer != null && (referrer.Host == request.Url?.Host 
                                     || Enumerable.Any(_yandexHosts, host => referrer.Host.Contains(host))
                                     || Enumerable.Any(_advantshopAdminHosts, host => referrer.Host.Contains(host))))
            {
                return;
            }

            var response = filterContext.HttpContext.Response;

            switch (Policy)
            {
                case XFrameOptionsPolicy.Disabled:
                    return;
                case XFrameOptionsPolicy.Deny:
                    response.AddHeaderRemovePrevious(HeaderConstants.XFrameOptionsHeader, "Deny");
                    return;
                case XFrameOptionsPolicy.SameOrigin:
                    response.AddHeaderRemovePrevious(HeaderConstants.XFrameOptionsHeader, "SameOrigin");
                    return;
                case XFrameOptionsPolicy.AllowFrom:
                    response.AddHeaderRemovePrevious(HeaderConstants.XFrameOptionsHeader, "ALLOW-FROM " + AllowUrl);
                    return;
                default:
                    throw new NotImplementedException("Wrong XFrameOptionsPolicy " + Policy);
            }
        }
    }

    public enum XFrameOptionsPolicy
    {
        //Specifies that the X-Frame-Options header should not be set in the HTTP response.
        Disabled,

        //Specifies that the X-Frame-Options header should be set in the HTTP response, instructing the browser to not
        //display the page when it is loaded in an iframe.
        Deny,

        //Specifies that the X-Frame-Options header should be set in the HTTP response, instructing the browser to
        //display the page when it is loaded in an iframe - but only if the iframe is from the same origin as the page.
        SameOrigin,

        //The page can only be displayed in a frame on the specified origin. not work in chrome and safari
        AllowFrom
    }
}
