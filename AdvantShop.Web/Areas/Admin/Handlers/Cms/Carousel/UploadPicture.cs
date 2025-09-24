using System.Web;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Cms.Carousel
{
    class UploadPicture
    {
        public UploadPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var img = HttpContext.Current.Request.Files["file"];

            if (img != null && img.ContentLength > 0)
            {
                return AddPhoto(img);
            }

            return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }

        private UploadPictureResult AddPhoto(HttpPostedFile file)
        {
            if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            FileHelpers.DeleteFilesFromImageTemp();
            file.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, file.FileName));

            return new UploadPictureResult
            {
                Result = true,
                Picture = FoldersHelper.GetPath(FolderType.ImageTemp, file.FileName, true),
                FileName = file.FileName
            };
        }
    }
}
