using System.IO;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetLpSitemapInfo
    {
        private readonly int _siteId;

        public GetLpSitemapInfo(int siteId)
        {
            _siteId = siteId;
        }

        public LpSitemapInfoModel Execute()
        {
            var site = new LpSiteService().Get(_siteId);
            if (site == null)
                throw new BlException("Сайт не найден");

            var model = new LpSitemapInfoModel();

            if (!string.IsNullOrEmpty(site.DomainUrl))
            {
                LpService.CurrentSiteId = _siteId;

                var url = (LSiteSettings.UseHttpsForSitemap ? "https://" : "http://") + site.DomainUrl + "/";

                model.SitemapXmlUrl = url + "sitemap.xml";
                model.SitemapHtmlUrl = url + "sitemap.html";
            }

            var dir = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, _siteId));

            var xmlFilePath = dir + "sitemap.xml";

            model.LastDateXml = !File.Exists(xmlFilePath)
                ? LocalizationService.GetResource("Admin.Handlers.Landings.GetLpSitemapInfo.FileMissing")
                : Culture.ConvertDate(new FileInfo(xmlFilePath).LastWriteTime);

            var htmlFilePath = dir + "sitemap.html";

            model.LastDateHtml = !File.Exists(htmlFilePath)
                ? LocalizationService.GetResource("Admin.Handlers.Landings.GetLpSitemapInfo.FileMissing")
                : Culture.ConvertDate(new FileInfo(htmlFilePath).LastWriteTime);

            return model;
        }
    }

    public class LpSitemapInfoModel
    {
        public string LastDateXml { get; set; }
        public string LastDateHtml { get; set; }
        public string SitemapXmlUrl { get; set; }
        public string SitemapHtmlUrl { get; set; }
    }
}
