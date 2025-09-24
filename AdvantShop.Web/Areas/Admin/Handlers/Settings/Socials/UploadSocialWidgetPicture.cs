using System;
using System.Drawing;
using System.IO;
using System.Web;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Categories;

namespace AdvantShop.Web.Admin.Handlers.Settings.Socials
{
    public class UploadSocialWidgetPicture
    {
        private readonly HttpPostedFileBase _file;
        private readonly int _type;

        public UploadSocialWidgetPicture(HttpPostedFileBase file, int type)
        {
            _type = type;
            _file = file;
        }

        public UploadPictureResult Execute()
        {
            if (_file == null || string.IsNullOrEmpty(_file.FileName))
                return new UploadPictureResult { Error = "Файл не найден" };

            if (!FileHelpers.CheckFileExtensionByType(_file.FileName, EFileType.Image))
                return new UploadPictureResult { Error = "Файл имеет неправильное разрешение" };

            try
            {
                FileHelpers.UpdateDirectories();

                var ext = Path.GetExtension(_file.FileName).ToLower();
                var fileName = Guid.NewGuid().ToString("N") + ext;


                using (Image image = Image.FromStream(_file.InputStream))
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.SocialWidget, fileName), 55, 55, image);

                if (_type == 1)
                {
                    DeletePrevious(SettingsSocialWidget.CustomLinkIcon1);
                    SettingsSocialWidget.CustomLinkIcon1 = fileName;
                }
                else if (_type == 2)
                {
                    DeletePrevious(SettingsSocialWidget.CustomLinkIcon2);
                    SettingsSocialWidget.CustomLinkIcon2 = fileName;
                }
                else if (_type == 3)
                {
                    DeletePrevious(SettingsSocialWidget.CustomLinkIcon3);
                    SettingsSocialWidget.CustomLinkIcon3 = fileName;
                }

                return new UploadPictureResult
                {
                    Result = true,
                    PictureId = _type,
                    Picture = FoldersHelper.GetPath(FolderType.SocialWidget, fileName, false)
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new UploadPictureResult { Error = "Файл не найден" };
        }

        private void DeletePrevious(string picture)
        {
            if (string.IsNullOrEmpty(picture))
                return;
            
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.SocialWidget, picture));
        }

    }
}
