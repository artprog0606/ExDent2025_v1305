using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;

namespace AdvantShop.Areas.Mobile.Models.Product
{
    public class ProductPhotoMobileViewModel : ProductPhotoMobileSetting
    {
        public ProductPhotoMobileViewModel(ProductPhotoMobileSetting setting)
        {
            BlockProductPhotoHeight = setting.BlockProductPhotoHeight;
            ProductImageType = setting.ProductImageType;
            PhotoWidth = setting.PhotoWidth;
            PhotoHeight = setting.PhotoHeight;
            IsProductPhotoLazy = setting.IsProductPhotoLazy;
            ProductId = setting.ProductId;
            LimitPhotoCount = setting.LimitPhotoCount;
            ColorId = setting.ColorId;
            RenderedPhotoId = setting.RenderedPhotoId;

        }

        public ProductPhotoMobileViewModel(ProductViewModel productViewModel, ProductItem productItem, int limitPhotoCount, bool isLazy)
        {
            BlockProductPhotoHeight = productViewModel.ProductViewMode == ProductViewMode.Single
                ? productViewModel.BlockProductPhotoMiddleHeight
                : productViewModel.BlockProductPhotoHeight;
            ProductImageType = productViewModel.ProductImageType;
            PhotoWidth = productViewModel.PhotoWidth;
            PhotoHeight = productViewModel.PhotoHeight;
            IsProductPhotoLazy = isLazy;
            Photos = new List<ProductPhoto>() { productItem.Photo };
            ProductId = productItem.ProductId;
            StartPhotoJson = productItem.StartPhotoJson;
            LimitPhotoCount = limitPhotoCount;
            ColorId = productItem.ColorId;
            RenderedPhotoId = productItem.Photo.PhotoId;
        }

        public List<ProductPhoto> Photos { get; set; }
        public string StartPhotoJson { get; }
    }

    public class ProductPhotoMobileSetting
    {
        public int BlockProductPhotoHeight { get; set; }
        public ProductImageType ProductImageType { get; set; }
        public ProductViewMode ProductViewMode { get; set; }
        public int PhotoWidth { get; set; }
        public int PhotoHeight { get; set; }
        public bool IsProductPhotoLazy { get; set; }
        public int ProductId { get; set; }
        public int LimitPhotoCount { get; set; }
        public int ColorId { get; set; }
        public int RenderedPhotoId { get; set; }
    }
}