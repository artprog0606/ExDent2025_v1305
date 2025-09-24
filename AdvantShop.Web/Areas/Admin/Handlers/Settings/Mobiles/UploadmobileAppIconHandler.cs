using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;
using System.Drawing;
using System.IO;
using System.Web;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mobiles
{
    public class UploadMobileAppIconHandler
    {
        public UploadPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var img = HttpContext.Current.Request.Files["file"];

            if (img == null || img.ContentLength == 0)
            {
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
            }

            if (!FileHelpers.CheckFileExtensionByType(img.FileName, EFileType.Image))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            if (!string.IsNullOrEmpty(SettingsMobile.MobileAppIconImageName))
            {
                FileHelpers.DeleteMobileAppIcons(SettingsMobile.MobileAppIconImageName);
                SettingsMobile.MobileAppIconImageName = null;
            }

            var newAppIconName = img.FileName.FileNamePlusDate("icon");

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            var tempPathWithName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, newAppIconName);

            img.SaveAs(tempPathWithName);

            if (!File.Exists(tempPathWithName))
            {
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
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
                return new UploadPictureResult() { Error = "Изображение должно быть квадратным" };
            }

            File.Move(tempPathWithName, FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon) + newAppIconName);

            SettingsMobile.MobileAppIconImageName = newAppIconName;

            return new UploadPictureResult()
            {
                Result = true,
                Picture = FoldersHelper.GetPath(FolderType.MobileAppIcon, SettingsMobile.MobileAppIconImageName, true)
            };
        }
    }
}
