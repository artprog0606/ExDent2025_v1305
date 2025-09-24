using AdvantShop.ViewModel.ProductDetails;

namespace AdvantShop.ViewModel.ProductDetailsLanding
{
    public class ProductPhotosViewLandingModel
    {
        public ProductPhotosViewLandingModel(ProductPhotosViewModel productPhotosModel)
        {
            ProductPhotosModel = productPhotosModel;
        }

        public ProductPhotosViewModel ProductPhotosModel { get; private set; }
        public bool QuickView { get; set; }
        public bool PreviewInAdmin { get; set; }
    }
}