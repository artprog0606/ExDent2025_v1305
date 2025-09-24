using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;
using System.Drawing;
using System.IO;
using System.Web;

namespace AdvantShop.Web.Admin.Handlers.Landings.MobileApp
{
    public class UploadMobileAppIconHandler
    {
        private readonly int _siteId;
        public UploadMobileAppIconHandler(int siteId)
        {
            LpService.CurrentSiteId = _siteId = siteId;
        }

        public UploadPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var img = HttpContext.Current.Request.Files["file"];

            if (img == null || img.ContentLength == 0)
            {
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
            }

            if (!FileHelpers.CheckFileExtensionByType(img.FileName, EFileType.Image))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            var landingPath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath, _siteId));

            if (!string.IsNullOrEmpty(LSiteSettings.MobileAppIconImageName))
            {
                FileHelpers.DeleteMobileAppIcons(LSiteSettings.MobileAppIconImageName, landingPath);
                LSiteSettings.MobileAppIconImageName = null;
            }

            var newAppIconName = img.FileName.FileNamePlusDate("icon");

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            var tempPathWithName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, newAppIconName);

            img.SaveAs(tempPathWithName);

            if (!File.Exists(tempPathWithName))
            {
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
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
