using System.Drawing;
using System.IO;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class AddPictureByLink : AbstractCommandHandler<AddPictureResponse>
    {
        private readonly int _id;
        private readonly PhotoType _type;
        private readonly string _link;
        private Category _category;

        public AddPictureByLink(int id, PhotoType type, string link)
        {
            _id = id;
            _type = type;
            _link = link;
        }

        protected override void Load()
        {
            _category = CategoryService.GetCategory(_id);
        }

        protected override void Validate()
        {
            if (_category == null)
                throw new BlException("Категория не найдена");

            if (string.IsNullOrWhiteSpace(_link))
                throw new BlException("Файл не найден");
            
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
            var tempPhotoName = Path.GetFileName(_link.Split('?')[0]);

            if (!FileHelpers.CheckFileExtensionByType(tempPhotoName, EFileType.Image))
            {
                throw new BlException($"Файл {tempPhotoName} имеет неправильное разрешение.");
            }

            var tempPhotoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, tempPhotoName);
            
            if (!FileHelpers.DownloadRemoteImageFile(_link, tempPhotoFullName, out _))
            {
                throw new BlException(LocalizationService.GetResource("Admin.Catalog.ErrorLoadingImage"));
            }
            
            PhotoService.DeletePhotos(_id, type);
            
            var photo = new Photo(0, _id, type) { OriginName = _link };
            var photoName = PhotoService.AddPhoto(photo);

            using (var image = Image.FromFile(tempPhotoFullName))
                FileHelpers.SaveCategoryImage(photo.PhotoName, image, imgType);

            return FoldersHelper.GetImageCategoryPath(imgType, photoName, false);
        }
    }
}