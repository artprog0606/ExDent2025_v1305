using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Trial;

namespace AdvantShop.Web.Admin.Handlers.Cms.Carousel
{
    class UploadPictureByLink
    {
        private readonly string _fileLink;

        public UploadPictureByLink(string fileLink)
        {
            _fileLink = fileLink;
        }

        public UploadPictureResult Execute()
        {
            if(string.IsNullOrEmpty(_fileLink))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            if (!FileHelpers.CheckFileExtensionByType(_fileLink, EFileType.Image))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            return AddPhoto(_fileLink);
        }

        private UploadPictureResult AddPhoto(string fileLink)
        {
            string error = null;

            var fileName = fileLink.Substring(fileLink.LastIndexOf("/") + 1);

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            FileHelpers.DeleteFilesFromImageTemp();
            var photoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, fileName);

            if (!string.IsNullOrEmpty(fileName) && FileHelpers.DownloadRemoteImageFile(fileLink, photoFullName, out error))
            {                   
                TrialService.TrackEvent(TrialEvents.ChangeLogo, "");
                return new UploadPictureResult
                {
                    Result = true,
                    Picture = FoldersHelper.GetPath(FolderType.ImageTemp, fileName, true),
                    FileName = fileName
                };
            }

            return new UploadPictureResult { Error = error ?? LocalizationService.GetResource("Admin.Catalog.ErrorLoadingImage") };
        }
    }
}
