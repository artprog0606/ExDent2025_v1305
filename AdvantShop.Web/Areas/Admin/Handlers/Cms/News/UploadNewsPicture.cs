using System.Drawing;
using System.IO;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.News;
using AdvantShop.Web.Admin.Models.Cms.News;

namespace AdvantShop.Web.Admin.Handlers.Cms.News
{
    public class UploadNewsPicture
    {
        private readonly int _newsId;
        private readonly bool _isEditMode;

        public UploadNewsPicture(int? newsId)
        {
            _newsId = newsId ?? -1;
            _isEditMode = _newsId != -1;
        }

        public UploadNewsPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadNewsPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            FileHelpers.UpdateDirectories();

            var img = HttpContext.Current.Request.Files["file"];
            if (img != null && img.ContentLength > 0)
            {
                return AddPhoto(img);
            }

            return new UploadNewsPictureResult { Error = LocalizationService.GetResource("Admin.Cms.ErrorWhileLoadingImage") };
        }

        private UploadNewsPictureResult AddPhoto(HttpPostedFile file)
        {
            if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                return new UploadNewsPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            if (_isEditMode)
                PhotoService.DeletePhotos(_newsId, PhotoType.News, false);

            var photo = new Photo(0, _newsId, PhotoType.News) {OriginName = file.FileName};
            PhotoService.AddPhoto(photo);

            if (string.IsNullOrWhiteSpace(photo.PhotoName))
                return new UploadNewsPictureResult { Error = LocalizationService.GetResource("Admin.Cms.ErrorWhileLoadingImage") };

            var fileViewPath = FoldersHelper.GetPath(FolderType.News, photo.PhotoName, true);
            using (var img = Image.FromStream(file.InputStream))
            {
                FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.News, photo.PhotoName), SettingsPictureSize.NewsImageWidth, SettingsPictureSize.NewsImageHeight, img);
            }

            NewsService.ClearCache();

            return new UploadNewsPictureResult
            {
                Result = true,
                Picture = fileViewPath,
                FileName = file.FileName,
                PictureId = photo.PhotoId
            };
        }
    }
}
