using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class BrandShortApi
    {
        public string Name { get; set; }
        public string BriefDescription { get; set; }
        public string Logo { get; }
        public string BrandSiteUrl { get; set; }
        public string BrandCountry { get; }
        public string UrlPath { get; }

        public BrandShortApi(Brand brand)
        {
            Name = brand.Name;
            BriefDescription = brand.BriefDescription;
            Logo = brand.BrandLogo?.ImageSrc();
            BrandSiteUrl = brand.BrandSiteUrl;
            BrandCountry = brand.BrandCountry?.Name;
            UrlPath = brand.UrlPath;
        }
    }

    public class BrandApi : BrandShortApi
    {
        public string Description { get; set; }

        public BrandApi(Brand brand) : base(brand)
        {
            Description = brand.Description;
        }
    }
}