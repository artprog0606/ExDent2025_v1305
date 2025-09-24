using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Common
{
    public class UploadFaviconPictureByLinkHandler
    {
        private readonly string _fileLink;
        
        public UploadFaviconPictureByLinkHandler(string fileLink)
        {
            _fileLink = fileLink;
        }

        public UploadPictureResult Execute() => 
            string.IsNullOrEmpty(_fileLink) 
                ? new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") } 
                : AddPhoto();

        private UploadPictureResult AddPhoto()
        {
            var fileName = _fileLink.Substring(_fileLink.LastIndexOf("/") + 1);
            var newFileName = fileName.FileNamePlusDate("favicon");
            
            if (!FileHelpers.CheckFileExtensionByType(newFileName, EFileType.Svg | EFileType.Favicon))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.InvalidFaviconFormat") };
            
            var photoFullName = FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFileName);

            if (!FileHelpers.DownloadRemoteImageFile(_fileLink, photoFullName, out var error))
                return new UploadPictureResult
                    { Error = error ?? LocalizationService.GetResource("Admin.Error.FileNotFound") };
            
            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.FaviconImageName));

            SettingsMain.FaviconImageName = newFileName;
            SettingsMain.IsFaviconImageSvg = FileHelpers.CheckFileExtensionByType(newFileName, EFileType.Svg);
                
            return new UploadPictureResult
            {
                Result = true,
                Picture = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.FaviconImageName, true)
            };

        }
    }
}
