using System.Drawing;
using System.IO;
using System.Web;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Landings.MobileApp
{
    public class UploadMobileAppIconByLinkHandler
    {
        private readonly int _siteId;
        private readonly string _fileLink;
        public UploadMobileAppIconByLinkHandler(int siteId, string fileLink)
        {
            LpService.CurrentSiteId = _siteId = siteId;
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

            var landingPath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath, _siteId));

            if (!string.IsNullOrEmpty(LSiteSettings.MobileAppIconImageName))
            {
                FileHelpers.DeleteMobileAppIcons(LSiteSettings.MobileAppIconImageName, landingPath);
                LSiteSettings.MobileAppIconImageName = null;
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

                        FileHelpers.ResizeMobileAppIcon(newAppIconName, image, landingPath);
                    }
                }
            }

            if (!correctProportions)
            {
                File.Delete(tempPathWithName);
                return new UploadPictureResult { Error = "Изображение должно быть квадратным" };
            }

            File.Move(tempPathWithName, landingPath + newAppIconName);

            LSiteSettings.MobileAppIconImageName = newAppIconName;

            return new UploadPictureResult
            {
                Result = true,
                Picture = LSiteSettings.GetMobileAppIconImagePath()
            };
        }
    }
}
