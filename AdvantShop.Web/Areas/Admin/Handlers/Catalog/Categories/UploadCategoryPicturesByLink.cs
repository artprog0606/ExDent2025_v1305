using System;
using System.Drawing;
using System.IO;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Categories;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class UploadCategoryPicturesByLink
    {
        private readonly int _categoryId;
        private readonly bool _isEditMode;
        private readonly PhotoType _type;
        private readonly string _fileLink;

        public UploadCategoryPicturesByLink(PhotoType type, int? categoryId, string fileLink)
        {
            _categoryId = categoryId ?? -1;
            _isEditMode = _categoryId != -1;
            _type = type;
            _fileLink = fileLink;
        }

        public UploadPictureResult Execute()
        {
            if (string.IsNullOrEmpty(_fileLink))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            if (!FileHelpers.CheckFileExtensionByType(_fileLink, EFileType.Image))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            UploadPictureResult result;

            FileHelpers.UpdateDirectories();

            if (_isEditMode)
            {
                var cacheKey = CacheNames.GetCategoryCacheObjectName(_categoryId);
                CacheManager.Remove(cacheKey);
            }


            switch (_type)
            {
                case PhotoType.CategoryBig:
                    result = AddPhoto(_fileLink, PhotoType.CategoryBig, CategoryImageType.Big);
                    break;
                case PhotoType.CategorySmall:
                    result = AddPhoto(_fileLink, PhotoType.CategorySmall, CategoryImageType.Small);
                    break;
                case PhotoType.CategoryIcon:
                    result = AddPhoto(_fileLink, PhotoType.CategoryIcon, CategoryImageType.Icon);
                    break;
                default:
                    result = new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
                    break;
            }

            CategoryService.ClearCategoryCache(_categoryId);

            return result;
        }


        private UploadPictureResult AddPhoto(string fileLink, PhotoType type, CategoryImageType imgType)
        {
            string error = null;

            try
            {
                var photo = new Photo(0, _categoryId, type) {OriginName = FileHelpers.GetExtension(fileLink)};
                var tempPhotoName = Path.GetFileName(fileLink.Split('?')[0]);
                var tempPhotoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, tempPhotoName);

                if (!string.IsNullOrWhiteSpace(tempPhotoName) &&
                    FileHelpers.DownloadRemoteImageFile(fileLink, tempPhotoFullName, out error))
                {
                    using (var image = Image.FromFile(tempPhotoFullName))
                    {
                        if (_isEditMode)
                            PhotoService.DeletePhotos(_categoryId, type);

                        PhotoService.AddPhoto(photo);

                        FileHelpers.SaveCategoryImage(photo.PhotoName, image, imgType);
                    }

                    return new UploadPictureResult
                    {
                        Result = true,
                        PictureId = photo.PhotoId,
                        Picture = FoldersHelper.GetImageCategoryPath(imgType, photo.PhotoName, false)
                    };
                }
            }
            catch (OutOfMemoryException ex)
            {
                Debug.Log.Error($"UploadCategoryPicturesByLink {fileLink}", ex);
                error = LocalizationService.GetResource("Admin.Catalog.ErrorUploadImageFormat");
            }
            catch (Exception ex)
            {
                Debug.Log.Error($"UploadCategoryPicturesByLink {fileLink}", ex);
            }
            
            return new UploadPictureResult { Error = error ?? LocalizationService.GetResource("Admin.Catalog.ErrorLoadingImage") };
        }

    }
}
