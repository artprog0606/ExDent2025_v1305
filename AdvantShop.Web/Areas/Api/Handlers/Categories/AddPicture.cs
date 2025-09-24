using System.Drawing;
using System.Web;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class AddPicture : AbstractCommandHandler<AddPictureResponse>
    {
        private readonly int _id;
        private readonly PhotoType _type;
        private HttpPostedFile _file;
        private Category _category;

        public AddPicture(int id, PhotoType type)
        {
            _id = id;
            _type = type;
        }

        protected override void Load()
        {
            _category = CategoryService.GetCategory(_id);
        }

        protected override void Validate()
        {
            if (_category == null)
                throw new BlException("Категория не найдена");

            switch (_type)
            {
                case PhotoType.CategoryBig:
                    if (_category.Picture != null && !string.IsNullOrEmpty(_category.Picture.PhotoName))
                        throw new BlException("Изображение уже существует");
                    break;
                
                case PhotoType.CategorySmall:
                    if (_category.MiniPicture != null && !string.IsNullOrEmpty(_category.MiniPicture.PhotoName))
                        throw new BlException("Изображение уже существует");
                    break;
                
                case PhotoType.CategoryIcon:
                    if (_category.Icon != null && !string.IsNullOrEmpty(_category.Icon.PhotoName))
                        throw new BlException("Изображение уже существует");
                    break;
            }

            var files = HttpContext.Current.Request.Files;
            if (files == null || files.Count != 1 || files[0] == null)
                throw new BlException("Файл не найден");

            _file = files[0];
            
            if (_file.ContentLength >= 1024 * 1024 * 15) //больше 15МБ
                throw new BlException($"Размер файла {_file.FileName} больше разрешенных 15мб.");

            if (!FileHelpers.CheckFileExtensionByType(_file.FileName, EFileType.Image))
                throw new BlException($"Файл {_file.FileName} имеет неправильное разрешение.");
        }
        
        protected override AddPictureResponse Handle()
        {
            FileHelpers.UpdateDirectories();

            string result = null;
            
            switch (_type)
            {
                case PhotoType.CategoryBig:
                    result = AddPhoto(PhotoType.CategoryBig, CategoryImageType.Big);
                    break;
                
                case PhotoType.CategorySmall:
                    result = AddPhoto(PhotoType.CategorySmall, CategoryImageType.Small);
                    break;
                
                case PhotoType.CategoryIcon:
                    result = AddPhoto(PhotoType.CategoryIcon, CategoryImageType.Icon);
                    break;
            }

            CategoryService.ClearCategoryCache(_id);
            CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(_id));

            return new AddPictureResponse(result);
        }

        private string AddPhoto(PhotoType type, CategoryImageType imgType)
        {
            PhotoService.DeletePhotos(_id, type);

            var photo = new Photo(0, _id, type) {OriginName = _file.FileName};
            var photoName = PhotoService.AddPhoto(photo);

            if (string.IsNullOrWhiteSpace(photoName))
                throw new BlException("Ошибка при сохранении");

            using (var image = Image.FromStream(_file.InputStream))
                FileHelpers.SaveCategoryImage(photoName, image, imgType);

            return FoldersHelper.GetImageCategoryPath(imgType, photoName, false);
        }
    }
}