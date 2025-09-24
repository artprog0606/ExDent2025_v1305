using System.Drawing;
using System.IO;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.News;
using AdvantShop.Web.Admin.Models.Cms.News;

namespace AdvantShop.Web.Admin.Handlers.Cms.News
{
    public class UploadNewsPictureByLink
    {
        private readonly int _newsId;
        private readonly bool _isEditMode;
        private readonly string _fileLink;

        public UploadNewsPictureByLink(int? newsId, string fileLink)
        {
            _newsId = newsId ?? -1;
            _isEditMode = _newsId != -1;
            _fileLink = fileLink;
        }

        public UploadNewsPictureResult Execute()
        {
            if (string.IsNullOrEmpty(_fileLink))
                return new UploadNewsPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            if (!FileHelpers.CheckFileExtensionByType(_fileLink, EFileType.Image))
                return new UploadNewsPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            FileHelpers.UpdateDirectories();

            return AddPhoto(_fileLink, PhotoType.News, SettingsPictureSize.NewsImageWidth, SettingsPictureSize.NewsImageHeight);
        }

        private UploadNewsPictureResult AddPhoto(string fileLink, PhotoType type, int width, int height)
        {
            string error = null;
            fileLink = fileLink.Split('?')[0];
            var photo = new Photo(0, _newsId, type) { OriginName = fileLink };
            var tempPhotoName = Path.GetFileName(fileLink);
            var tempPhotoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, tempPhotoName);

            if (!string.IsNullOrWhiteSpace(tempPhotoName) && FileHelpers.DownloadRemoteImageFile(fileLink, tempPhotoFullName, out error))
            {
                using (var image = Image.FromFile(tempPhotoFullName))
                {
                    if (_isEditMode)
                        PhotoService.DeletePhotos(_newsId, type);

                    PhotoService.AddPhoto(photo);

                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.News, photo.PhotoName),
                        width, height, image);
                }

                NewsService.ClearCache();

                return new UploadNewsPictureResult
                {
                    Result = true,
                    PictureId = photo.PhotoId,
                    Picture = FoldersHelper.GetPath(FolderType.News, photo.PhotoName, true)
                };
            }

            return new UploadNewsPictureResult { Error = error ?? LocalizationService.GetResource("Admin.Cms.ErrorWhileLoadingImage") };
        }
    }
}
