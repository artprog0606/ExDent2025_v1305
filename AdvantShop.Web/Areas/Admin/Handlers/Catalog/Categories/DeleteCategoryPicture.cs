using AdvantShop.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.Categories;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class DeleteCategoryPictureHandler
    {
        private readonly int _puctureId;

        public DeleteCategoryPictureHandler(int puctureId)
        {
            _puctureId = puctureId;
        }

        public UploadPictureResult Execute()
        {
            var photo = PhotoService.GetPhoto(_puctureId);

            if (photo == null)
            {
                return new UploadPictureResult { Error = "Невозможно удалить изображение по умолчанию" };
            }

            PhotoService.DeletePhotoWithPath(photo.Type, photo.PhotoName);

            if (photo.ObjId != 0)
            {                
                CategoryService.ClearCategoryCache(photo.ObjId);
            }

            var nophoto = string.Empty;

            switch (photo.Type)
            {
                case PhotoType.CategoryIcon:
                    nophoto = "../images/nophoto_xsmall.png";
                    break;

                case PhotoType.CategorySmall:
                    nophoto = "../images/nophoto_small.png";
                    break;
                case PhotoType.CategoryBig:
                    nophoto = "../images/nophoto_big.png";
                    break;
            }

            return new UploadPictureResult() { Result = true, Picture = nophoto };
        }
    }
}
