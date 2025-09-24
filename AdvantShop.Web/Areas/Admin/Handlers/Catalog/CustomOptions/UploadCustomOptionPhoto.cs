using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using System;
using System.Drawing;
using System.IO;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using System.Web;

namespace AdvantShop.Web.Admin.Handlers.Catalog.CustomOptions
{
    public class UploadCustomOptionPhoto
    {
        private readonly int _optionId;
        private readonly HttpPostedFileBase _file;

        public UploadCustomOptionPhoto(int optionId, HttpPostedFileBase file)
        {
            _optionId = optionId;
            _file = file;
        }

        public void Execute()
        {
            PhotoService.DeletePhotos(_optionId, PhotoType.CustomOptions);

            if (_file == null || _file.FileName.IsNullOrEmpty()) 
                return;
            
            if (!FileHelpers.CheckFileExtensionByType(_file.FileName, EFileType.Image))
                throw new BlException("Данный формат не поддерживается");

            var photoNameToSave = AddPhoto(_optionId, _file.FileName);
            var newFilePath = GetPicturePath(photoNameToSave);

            try
            {
                var directoryPath = FoldersHelper.GetPathAbsolut(FolderType.CustomOptions);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                using (Image image = Image.FromStream(_file.InputStream))
                    FileHelpers.SaveResizePhotoFile(newFilePath, SettingsPictureSize.CustomOptionsImageWidth, SettingsPictureSize.CustomOptionsImageHeight, image);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException("Не удалось добавить изображение");
            }
        }
        
        private string GetPicturePath(string pictureName) => FoldersHelper.GetPathAbsolut(FolderType.CustomOptions, pictureName);

        private string AddPhoto(int optionId, string pictureName)
        {
            var photo = new Photo(0, optionId, PhotoType.CustomOptions)
            {
                OriginName = pictureName,
                PhotoName = $"{optionId}_{pictureName}"
            };
            return PhotoService.AddPhoto(photo);
        }
    }
}
