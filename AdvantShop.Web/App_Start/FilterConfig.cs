using System.Web.Mvc;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new CompressFilter());
            filters.Add(new AntiforgeryHandleErrorAttribute());            

            //add security headers
            //filters.Add(new XContentTypeOptionsAttribute());
            //filters.Add(new XDownloadOptionsAttribute());
            //filters.Add(new XFrameOptionsAttribute { Policy = XFrameOptionsPolicy.Disabled });
            //filters.Add(new XXssProtectionAttribute(XXssProtectionPolicy.FilterEnabled, true));
            //filters.Add(new HttpStrictTransportSecurityAttribute { MaxAge = new TimeSpan(365, 0, 0, 0), IncludeSubdomains = true, Preload = true });
        }
    }
}