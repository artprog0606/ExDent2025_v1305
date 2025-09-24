using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Handlers.Products;
using AdvantShop.App.Landing.Models.Catalogs;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;


namespace AdvantShop.App.Landing.Domain.Products
{
    public class LpProductService
    {
        public static CatalogProductPagingModel GetProductsByCategory(int categoryId, int countPerPage, int page = 1, bool? indepth = null)
        {
            return GetProductsByCategory(new ProductsByCategoryModel()
            {
                CategoryId = categoryId,
                CountPerPage = countPerPage,
                Page = page,
                Indepth = indepth
            });
        }

        public static CatalogProductPagingModel GetProductsByCategory(ProductsByCategoryModel model)
        {
            var indepth = model.Indepth != null || CategoryService.GetAllChildCategoriesIdsByCategoryId(model.CategoryId).Any(x => x != model.CategoryId);

            var result = new CatalogProductPaging(model, indepth).Execute();
            return result;
        }

        public static ProductViewModel GetProductsByIds(List<int> productIds, bool moveNotAvaliableToEnd = false, bool showOnlyAvailable = false)
        {
            if (productIds == null || productIds.Count == 0)
                return null;

            var products = ProductService.GetProductsByIds(productIds, moveNotAvaliableToEnd, showOnlyAvailable);
            return new ProductViewModel(products);
        }

        public static List<LpProductPhoto> GetPhotos(int productId)
        {
            var photos = PhotoService.GetPhotos<ProductPhoto>(productId, PhotoType.Product).Select(photo => new LpProductPhoto()
            {
                PathXSmall = photo.ImageSrcXSmall(),
                PathSmall = photo.ImageSrcSmall(),
                PathMiddle = FoldersHelper.GetImageProductPath(ProductImageType.Middle, photo.PhotoName, false),
                PathBig = FoldersHelper.GetImageProductPath(ProductImageType.Big, photo.PhotoName, false),
                ColorId = photo.ColorID,
                PhotoId = photo.PhotoId,
                Description = photo.Description,
                XSmallProductImageHeight = SettingsPictureSize.XSmallProductImageHeight,
                XSmallProductImageWidth = SettingsPictureSize.XSmallProductImageWidth,
                SmallProductImageHeight = SettingsPictureSize.SmallProductImageHeight,
                SmallProductImageWidth = SettingsPictureSize.SmallProductImageWidth,
                MiddleProductImageWidth = SettingsPictureSize.MiddleProductImageWidth,
                MiddleProductImageHeight = SettingsPictureSize.MiddleProductImageHeight
            }).ToList();

            return photos;
        }
    }
}
