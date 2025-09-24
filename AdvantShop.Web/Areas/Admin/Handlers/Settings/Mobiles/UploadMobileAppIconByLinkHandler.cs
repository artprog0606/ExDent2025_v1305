using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;
using System.Drawing;
using System.IO;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mobiles
{
    public class UploadMobileAppIconByLinkHandler
    {
        private readonly string _fileLink;

        public UploadMobileAppIconByLinkHandler(string fileLink)
        {
            _fileLink = fileLink;
        }

        public UploadPictureResult Execute()
        {
            if (string.IsNullOrEmpty(_fileLink))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var fileName = _fileLink.Substring(_fileLink.LastIndexOf("/") + 1);
            var newAppIconName = fileName.FileNamePlusDate("icon");

            if (!FileHelpers.CheckFileExtensionByType(newAppIconName, EFileType.Image))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            if (!string.IsNullOrEmpty(SettingsMobile.MobileAppIconImageName))
            {
                FileHelpers.DeleteMobileAppIcons(SettingsMobile.MobileAppIconImageName);
                SettingsMobile.MobileAppIconImageName = null;
            }

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            var tempPathWithName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, newAppIconName);

            if (!FileHelpers.DownloadRemoteImageFile(_fileLink, tempPathWithName, out var error)
                || tempPathWithName == null || !File.Exists(tempPathWithName))
            {
                return new UploadPictureResult { Error = error ?? LocalizationService.GetResource("Admin.Error.FileNotFound") };
            }

            var correctProportions = false;
            using (var lStream = new FileStream(tempPathWithName, FileMode.Open, FileAccess.Read))
            {
                using (var image = Image.FromStream(lStream))
                {
                    if (image.Width == image.Height)
                    {
                        correctProportions = true;

                        FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon));

                        FileHelpers.ResizeMobileAppIcon(newAppIconName, image);
                    }
                }
            }

            if (!correctProportions)
            {
                File.Delete(tempPathWithName);
                return new UploadPictureResult { Error = "Изображение должно быть квадратным" };
            }

            File.Move(tempPathWithName, FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon) + newAppIconName);

            SettingsMobile.MobileAppIconImageName = newAppIconName;

            return new UploadPictureResult
            {
                Result = true,
                Picture = FoldersHelper.GetPath(FolderType.MobileAppIcon, SettingsMobile.MobileAppIconImageName, true)
            };
        }
    }
}
