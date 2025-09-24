//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.SEO
{
    public class SiteMapData
    {
        public string Loc { get; set; }
        public string Title { get; set; }
        public DateTime? Lastmod { get; set; }
        public string Changefreq { get; set; }
        public float Priority { get; set; }
        public bool IsBaseUrl { get; private set; }

        public SiteMapData()
        {
        }

        public SiteMapData(bool isBaseUrl)
        {
            IsBaseUrl = isBaseUrl;
        }
    }
}