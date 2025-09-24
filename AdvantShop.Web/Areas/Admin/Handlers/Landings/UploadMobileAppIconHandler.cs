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

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class UploadMobileAppIconHandler
    {
        private readonly int siteId;
        public UploadMobileAppIconHandler(int id)
        {
            siteId = id;
        }

        public UploadPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var img = HttpContext.Current.Request.Files["file"];

            if (img != null && img.ContentLength > 0)
            {
                return AddPhoto(img);
            }

            return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }

        public UploadPictureResult AddPhoto(HttpPostedFile file)
        {
            if (!FileHelpers.CheckFileExtension(file.FileName, EAdvantShopFileTypes.Photo))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Errors.UnsupportedImageFormat") };

            var newFile = file.FileName.FileNamePlusDate("icon");
            
            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            var tempPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, newFile);
            file.SaveAs(tempPath);

            if (tempPath != null && File.Exists(tempPath))
            {
                var correctProportions = false;
                using (var lStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
                {
                    using (var image = Image.FromStream(lStream))
                    {
                        if (image.Width == image.Height)
                        {
                            correctProportions = true;
                            var landingPath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath, siteId));

                            //FileHelpers.ResizeMobileAppIcon(newFile, image);
                        }
                    }
                }

                if (!correctProportions)
                {
                    File.Delete(tempPath);
                    return new UploadPictureResult() { Error = "Изображение должно быть квадратным" };
                }
                else
                {
                    var originalPath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath, siteId));
                    File.Move(tempPath, originalPath);

                    LSiteSettings.MobileAppIconImageName = newFile;
                }
            }

            return new UploadPictureResult()
            {
                Result = true,
                Picture = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath + LSiteSettings.MobileAppIconImageName, siteId))
            };
        }
    }
}
