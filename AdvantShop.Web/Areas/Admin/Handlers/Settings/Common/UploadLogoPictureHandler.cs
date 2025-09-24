using System.IO;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Settings;
using Image = System.Drawing.Image;
namespace AdvantShop.Web.Admin.Handlers.Settings.Common
{
    public class UploadLogoPictureHandler
    {
        private readonly bool _isLogoMobile;
        private bool _isSvg;
        private readonly bool _isLogoBlog;
        private readonly bool _isDarkThemeLogo;

        public UploadLogoPictureHandler(bool isLogoMobile = false, bool isLogoBlog = false, bool isDarkThemeLogo = false)
        {
            _isLogoMobile = isLogoMobile;
            _isLogoBlog = isLogoBlog;
            _isDarkThemeLogo = isDarkThemeLogo;
        }

        public UploadPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var file = HttpContext.Current.Request.Files["file"];
            if (file == null || file.ContentLength == 0)
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            
            _isSvg = FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Svg);

            return !_isSvg ? AddLogo(file) : AddLogoSvg(file);
        }

        private UploadPictureResult AddLogo(HttpPostedFile file)
        {
            if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.InvalidLogoFormat") };
            var imageName = _isLogoMobile ? SettingsMobile.LogoImageName : _isLogoBlog ? SettingsMain.LogoBlogImageName : _isDarkThemeLogo ? SettingsMain.DarkThemeLogoName : SettingsMain.LogoImageName;

            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, imageName));

            var newFile = file.FileName.FileNamePlusDate("logo" + (_isLogoMobile ? "_mobile" : _isLogoBlog ? "_blog" : _isLogoBlog ? "_blog" : _isDarkThemeLogo ? "_dark" : ""));
            var img = Image.FromStream(file.InputStream, true, true);

            if (_isLogoMobile)
            {
                SettingsMobile.LogoImageName = newFile;
                SettingsMobile.LogoImageWidth = img.Width;
                SettingsMobile.LogoImageHeight = img.Height;
            }
            else if (_isLogoBlog)
            {
                SettingsMain.LogoBlogImageName = newFile;
            }
            else if (_isDarkThemeLogo)
            {
                SettingsMain.DarkThemeLogoName = newFile;
                SettingsMain.DarkThemeLogoImageWidth = img.Width;
                SettingsMain.DarkThemeLogoImageHeight = img.Height;
            }
            else
            {
                SettingsMain.LogoImageName = newFile;
                SettingsMain.IsLogoImageSvg = false;
                SettingsMain.LogoImageWidth = img.Width;
                SettingsMain.LogoImageHeight = img.Height;
            }

            file.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile));

            TrialService.TrackEvent(TrialEvents.ChangeLogo, "");

            // if (!SettingsCongratulationsDashboard.LogoDone)
            // {
            //     SettingsCongratulationsDashboard.LogoDone = true;
            //     Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_LogoDone);
            // }

            return new UploadPictureResult
            {
                Result = true,
                Picture = FoldersHelper.GetPath(FolderType.Pictures, newFile, true)
            };
        }


        private UploadPictureResult AddLogoSvg(HttpPostedFile file)
        {
            if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Svg))
                return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.InvalidLogoFormat") };
                        
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));

            var newFile = file.FileName.FileNamePlusDate("logo");            

            SettingsMain.LogoImageName = newFile;
            SettingsMain.IsLogoImageSvg = true;
            SettingsMain.LogoImageWidth = 0;
            SettingsMain.LogoImageHeight = 0;

            file.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile));

            TrialService.TrackEvent(TrialEvents.ChangeLogo, "");

            // if (!SettingsCongratulationsDashboard.LogoDone)
            // {
            //     SettingsCongratulationsDashboard.LogoDone = true;
            //     Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_LogoDone);
            // }

            return new UploadPictureResult
            {
                Result = true,
                Picture = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, true)
            };
        }

    }
}
