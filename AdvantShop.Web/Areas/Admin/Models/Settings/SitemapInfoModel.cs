namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SitemapInfoModel
    {
        public string SiteMapFileHtmlLink { get; set; }
        public string SiteMapFileXmlLink { get; set; }

        public SitemapInfoModel(string siteMapFileHtmlLink, string siteMapFileXmlLink)
        {
            SiteMapFileHtmlLink = siteMapFileHtmlLink;
            SiteMapFileXmlLink = siteMapFileXmlLink;
        }
    }
}