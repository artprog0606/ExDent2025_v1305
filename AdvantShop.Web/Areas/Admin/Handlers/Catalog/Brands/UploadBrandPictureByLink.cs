using System.Drawing;
using System.IO;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Brands;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Brands
{
    public class UploadBrandPictureByLink
    {
        private readonly int _brandId;
        private readonly bool _isEditMode;
        private readonly string _fileLink;

        public UploadBrandPictureByLink(int? brandId, string fileLink)
        {
            _brandId = brandId != null ? brandId.Value : -1;
            _isEditMode = _brandId != -1;
            _fileLink = fileLink;
        }

        public UploadBrandPictureResult Execute()
        {
            if (string.IsNullOrEmpty(_fileLink))
                return new UploadBrandPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            if (!FileHelpers.CheckFileExtensionByType(_fileLink, EFileType.Image))
                return new UploadBrandPictureResult() { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            FileHelpers.UpdateDirectories();

            return AddPhoto(_fileLink, PhotoType.Brand, SettingsPictureSize.BrandLogoWidth, SettingsPictureSize.BrandLogoHeight);
        }


        private UploadBrandPictureResult AddPhoto(string fileLink, PhotoType type, int width, int height)
        {
            string error = null;

            var photo = new Photo(0, _brandId, type) { OriginName = fileLink };
            var tempPhotoName = Path.GetFileName(fileLink.Split('?')[0]);
            var tempPhotoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, tempPhotoName);

            if (!string.IsNullOrWhiteSpace(tempPhotoName) && FileHelpers.DownloadRemoteImageFile(fileLink, tempPhotoFullName, out error))
            {
                using (var image = Image.FromFile(tempPhotoFullName))
                {
                    if (_isEditMode)
                        PhotoService.DeletePhotos(_brandId, type);

                    PhotoService.AddPhoto(photo);

                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, photo.PhotoName),
                        width, height, image);
                }

                CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
                CacheManager.RemoveByPattern("BrandId_");
                CacheManager.RemoveByPattern("Brand_");

                return new UploadBrandPictureResult()
                {
                    Result = true,
                    PictureId = photo.PhotoId,
                    Picture = FoldersHelper.GetPath(FolderType.BrandLogo, photo.PhotoName, true)
                };
            }

            return new UploadBrandPictureResult() { Error = error ?? LocalizationService.GetResource("Admin.Catalog.ErrorLoadingImage") };
        }
    }
}
