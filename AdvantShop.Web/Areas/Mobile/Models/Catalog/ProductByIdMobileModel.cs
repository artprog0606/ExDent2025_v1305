using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Areas.Mobile.Models.Catalog
{
    public class ProductByIdMobileModel
    {
        public string Title { get; set; }

        public ProductViewModel Products { get; set; }

        public string RelatedType { get; set; }

        public bool EnabledCarousel { get; set; }

        public string Ids { get; set; }

        public string Type { get; set; }

        public bool? NotSort { get; set; }

        public bool? HideDescription { get; set; }
    }

}