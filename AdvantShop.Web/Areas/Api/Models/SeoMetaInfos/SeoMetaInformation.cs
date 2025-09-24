using AdvantShop.Core.Common.Extensions;
using AdvantShop.SEO;

namespace AdvantShop.Areas.Api.Models.MetaInfos
{
    public class SeoMetaInformation
    {
        public bool IsDefault { get; set; }
        public string Title { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string H1 { get; set; }

        public SeoMetaInformation()
        {
        }

        public SeoMetaInformation(MetaInfo meta)
        {
            IsDefault = meta.IsDefaultMeta;
            Title = meta.Title;
            MetaKeywords = meta.MetaKeywords;
            MetaDescription = meta.MetaDescription;
            H1 = meta.H1;
        }
    }

    public static class SeoMetaInformationExtensions
    {
        public static MetaInfo GetMetaInfo(this SeoMetaInformation meta, int id)
        {
            return new MetaInfo(0, id, MetaType.Category,
                meta.Title.DefaultOrEmpty(),
                meta.MetaKeywords.DefaultOrEmpty(),
                meta.MetaDescription.DefaultOrEmpty(),
                meta.H1.DefaultOrEmpty(),
                meta.IsDefault);
        }
    }
}