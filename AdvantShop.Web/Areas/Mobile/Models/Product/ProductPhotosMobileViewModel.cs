using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.ViewModel.ProductDetails;

namespace AdvantShop.Areas.Mobile.Models.ProductDetails
{
    public class ProductPhotosMobileViewModel
    {
        public List<ProductPhoto> Photos { get; set; }

        public Discount Discount { get; set; }

        public AdvantShop.Catalog.Product Product { get; set; }
        public List<string> Labels { get; set; }
        public int CarouselPhotoHeight { get; set; }
        public int CarouselPhotoWidth { get; set; }

        public int PreviewPhotoWidth { get; set; }
        public int PreviewPhotoHeight { get; set; }
        public bool ActiveThreeSixtyView { get; set; }
        public List<ProductPhoto> Photos360 { get; set; }
        public string Photos360Ext { get; set; }
        public BaseProductViewModel ProductModel { get; set; }

        public int? ColorId { get; set; }
        public ProductVideo Video { get; set; }

    }
}