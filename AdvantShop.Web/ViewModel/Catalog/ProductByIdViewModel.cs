using AdvantShop.Core.Services.Catalog;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.ViewModel.Catalog
{
    public class ProductByIdViewModel
    {
        public string Title { get; set; }

        public ProductViewModel Products { get; set; }

        public string RelatedType { get; set; }

        public int VisibleItems { get; set; }

        public bool EnabledCarousel { get; set; }

        public string Ids { get; set; }

        public string Type { get; set; }

        public bool? NotSort { get; set; }

        public bool? HideDescription { get; set; }

        public Dictionary<int, ProductByIdMediaQueryOptionsViewModel> CarouselResponsive { get; set; }
    }

    public class ProductByIdMediaQueryOptionsViewModel
    {
        [JsonProperty("slidesToShow")]
        public int SlidesToShow { get; set; }
    }
}