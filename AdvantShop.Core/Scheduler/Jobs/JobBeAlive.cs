//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Net;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    //[DisallowConcurrentExecution]
    public class JobBeAlive : IJob
    {
        public const string UserAgent = "Advantshop KeepAliveBot crawler bot";

        public void Execute(IJobExecutionContext context)
        {
            var urls = new List<string>
            {
                UrlService.GetUrl(),
                UrlService.GetUrl("cart"),
                UrlService.GetUrl("checkout")
            };

            var category = CategoryService.GetFirstCategory();
            if (category != null)
            {
                urls.Add(UrlService.GetUrl(UrlService.GetLink(ParamType.Category, category.UrlPath, category.ID)));
            }

            var product = ProductService.GetFirstProduct();
            if (product != null)
            {
                urls.Add(UrlService.GetUrl(UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId)));

                var lpSite = new LpSiteService().GetList().FirstOrDefault(x => x.Enabled);
                if (lpSite != null)
                {
                    var lp = new LpService().GetByMain(lpSite.Id);
                    if (lp != null)
                        urls.Add(UrlService.GetUrl(
                            $"/product/productquickview?productId={product.ProductId}&from=landing&landingId={lp.Id}"));
                }
            }

            var page = StaticPageService.GetFirstPage();
            if (page != null)
            {
                urls.Add(UrlService.GetUrl(UrlService.GetLink(ParamType.StaticPage, page.UrlPath, page.ID)));
            }

            ProcessUrls(urls);
        }

        private static void ProcessUrls(List<string> urls)
        {
            foreach (var url in urls)
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        wc.Headers.Add("User-agent", UserAgent);
                        wc.Headers.Add(UrlRewriteExtensions.TechnicalHeaderName, SettingsLic.AdvId);

                        _ = wc.DownloadString(url);
                    }
                }
                catch
                {
                    // empty catch: we no need to log this error
                    break;
                }
            }
        }
    }
}