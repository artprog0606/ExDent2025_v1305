using System;
using System.Web.Mvc;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Infrastructure.Filters.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ContentSecurityPolicyAttribute : HttpHeaderAttributeBase
    {
        public override void SetHttpHeadersOnActionExecuted(ActionExecutedContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.AddHeaderRemovePrevious(HeaderConstants.ContentSecurityPolicyHeader,
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' *.advantshop.net *.advstatic.ru *.advant.shop cdn.jsdelivr.net ymetrica.com mc.yandex.ru *.yandex.net yastatic.net *.yandex.ru *.jivosite.com *.chat2desk.com static.woopra.com www.woopra.com dadata.ru www.googletagmanager.com tagmanager.google.com ajax.googleapis.com *.youtube.com youtube.com *.boxberry.de *.grastin.ru *.hermesrussia.ru *.ozon.ru *.cdek.ru *.shiptor.ru data:; worker-src blob:");
        }
    }
}
