using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.App.Landing.Domain.Seo
{
    public class LpSitemapGeneratorXml : LpSitemapGenerator
    {
        public LpSitemapGeneratorXml(int siteId, bool useHttps) : base(siteId, useHttps)
        {
        }

        public override void GenerateSiteMap(string dir, LpSite site, List<Lp> landingPages)
        {
            var filePath = dir + "sitemap.xml";
            var lastmod = DateTime.Now;

            FileHelpers.DeleteFile(filePath);

            using (var outputFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = XmlWriter.Create(outputFile))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var page in landingPages)
                {
                    var item = new SiteMapData()
                    {
                        Title = page.Name ?? "",
                        Loc = page.IsMain ? _baseUrl : _baseUrl + "/" + page.Url,
                        Priority = page.IsMain ? 1f : 0.8f,
                        Changefreq = "daily",
                        Lastmod = lastmod
                    };

                    WriteLine(writer, item);
                }
            }
        }

        private void WriteLine(XmlWriter writer, SiteMapData item)
        {
            writer.WriteStartElement("url");

            writer.WriteStartElement("loc");
            writer.WriteString(item.Loc);
            writer.WriteEndElement();

            writer.WriteStartElement("lastmod");
            writer.WriteString(item.Lastmod.HasValue ? item.Lastmod.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            writer.WriteStartElement("changefreq");
            writer.WriteString(item.Changefreq);
            writer.WriteEndElement();

            writer.WriteStartElement("priority");
            writer.WriteString(item.Priority.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            
            writer.WriteEndElement();
        }
    }
}
