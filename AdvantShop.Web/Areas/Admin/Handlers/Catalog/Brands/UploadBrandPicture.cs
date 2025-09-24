using System.Drawing;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Brands;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Brands
{
    public class UploadBrandPicture
    {
        private readonly int _brandId;
        private readonly bool _isEditMode;

        public UploadBrandPicture(int? brandId)
        {
            _brandId = brandId ?? -1;
            _isEditMode = _brandId != -1;
        }

        public UploadBrandPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadBrandPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            FileHelpers.UpdateDirectories();

            var img = HttpContext.Current.Request.Files["file"];
            if (img != null && img.ContentLength > 0)
            {
                return AddPhoto(img, PhotoType.Brand, SettingsPictureSize.BrandLogoWidth, SettingsPictureSize.BrandLogoHeight);
            }

            return new UploadBrandPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }


        private UploadBrandPictureResult AddPhoto(HttpPostedFile file, PhotoType type, int width, int height)
        {
            if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                return new UploadBrandPictureResult() { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            if (_isEditMode)
                PhotoService.DeletePhotos(_brandId, type);

            var photo = new Photo(0, _brandId, type) { OriginName = file.FileName };
            var photoName = PhotoService.AddPhoto(photo);

            if (!string.IsNullOrWhiteSpace(photoName))
            {
                using (Image image = Image.FromStream(file.InputStream))
                {
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, photoName), width, height, image);
                }

                CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
                CacheManager.RemoveByPattern("BrandId_");
                CacheManager.RemoveByPattern("Brand_");

                return new UploadBrandPictureResult()
                {
                    Result = true,
                    PictureId = photo.PhotoId,
                    Picture = FoldersHelper.GetPath(FolderType.BrandLogo, photoName, true)                    
                };
            }

            return new UploadBrandPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }

    }
}
