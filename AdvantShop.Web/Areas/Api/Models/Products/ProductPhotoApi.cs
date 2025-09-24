using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductPhotoApi
    {
        public string SmallSrc { get; }
        public string MiddleSrc { get; }
        public string BigSrc { get; }
        public int? ColorId { get; }
        public bool Main { get; }

        public ProductPhotoApi(ProductPhoto photo)
        {
            SmallSrc = photo.ImageSrcSmall();
            MiddleSrc = photo.ImageSrcMiddle();
            BigSrc = photo.ImageSrcBig();
            ColorId = photo.ColorID;
            Main = photo.Main;
        }
    }
}