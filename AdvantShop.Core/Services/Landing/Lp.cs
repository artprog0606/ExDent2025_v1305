using System;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Core.Services.Landing
{
    /// <summary>
    /// Страница Landing Page
    /// </summary>
    public class Lp : ICloneable
    {
        public int Id { get; set; }
        public int LandingSiteId { get; set; }
        public string Url { get; set; }
        public string SiteUrl { get; set; }

        public string Href
        {
            get
            {
                return UrlService.GetUrl("lp/" + SiteUrl + "/" + (!IsMain ? Url : ""));
            }
        }
        public string Name { get; set; }
        public string Template { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsMain { get; set; }

        public LpTemplatePageType PageType { get; set; }

        public int? ProductId { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
