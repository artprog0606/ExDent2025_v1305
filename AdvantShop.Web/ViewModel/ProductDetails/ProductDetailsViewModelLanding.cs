using AdvantShop.App.Landing.Models;
using AdvantShop.ViewModel.ProductDetails;

namespace AdvantShop.ViewModel.ProductDetailsLanding
{
    public class ProductDetailsViewModelLanding
    {
        public ProductDetailsViewModelLanding(ProductDetailsViewModel productModel) {
            ProductModel = productModel;
        }

        public ProductDetailsViewModel ProductModel { get; private set; }
        public SliderModel Slider { get; set; }
        public bool PreviewInAdmin { get; set; }
    }

}