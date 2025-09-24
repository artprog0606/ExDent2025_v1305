using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Domain.Seo
{
    public abstract class LpSitemapGenerator
    {
        private readonly int _siteId;
        private readonly bool _useHttps;
        private readonly LpService _lpService;
        private readonly LpSiteService _siteService;

        protected string _baseUrl;

        protected LpSitemapGenerator(int siteId, bool useHttps)
        {
            _siteId = siteId;
            _useHttps = useHttps;
            _siteService = new LpSiteService();
            _lpService = new LpService();
        }

        public void Execute()
        {
            var site = _siteService.Get(_siteId);
            if (site == null)
                throw new BlException("Сайт не найден");

            LpService.CurrentSiteId = site.Id;
            LSiteSettings.UseHttpsForSitemap = _useHttps;

            var dir = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, _siteId));
            FileHelpers.CreateDirectory(dir);

            var landingPages = _lpService.GetList(_siteId).Where(x => x.Enabled).ToList();

            _baseUrl =
                (LSiteSettings.UseHttpsForSitemap ? "https://" : "http://") +
                (!string.IsNullOrEmpty(site.DomainUrl) ? site.DomainUrl : UrlService.GetUrl("lp/" + site.Url).Replace("http://", "").Replace("https://", ""));

            GenerateSiteMap(dir, site, landingPages);
        }

        public virtual void GenerateSiteMap(string dir, LpSite site, List<Lp> landingPages)
        {
        }
    }
}
