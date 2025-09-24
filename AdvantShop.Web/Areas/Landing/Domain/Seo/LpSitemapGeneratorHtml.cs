using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Domain.Seo
{
    public class LpSitemapGeneratorHtml : LpSitemapGenerator
    {
        public LpSitemapGeneratorHtml(int siteId, bool useHttps) : base(siteId, useHttps)
        {
        }

        public override void GenerateSiteMap(string dir, LpSite site, List<Lp> landingPages)
        {
            var filePath = dir + "sitemap.html";
            
            FileHelpers.DeleteFile(filePath);

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>" + _baseUrl + " - " +
                             LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.SiteMapGenerateHeader") +
                             "</title></head><body><div>");

                sw.WriteLine("<b><a href='{0}'>{0}</a></b><br/><br/>", _baseUrl);
                sw.WriteLine("<ul>");

                foreach (var page in landingPages)
                {
                    sw.WriteLine("<li><a href='{0}'>{1}</a></li>", page.IsMain ? _baseUrl : _baseUrl + "/" + page.Url, page.Name);
                }

                sw.WriteLine("</ul>");
                sw.WriteLine("</div></body></html>");
                sw.Close();
            }
        }


    }
}
