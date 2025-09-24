using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Booking.Services
{
    public class UploadImageCropped
    {
        private readonly Service _service;
        private readonly string _fileName;
        private readonly HttpPostedFileBase _file;

        public UploadImageCropped(Service service, string fileName, HttpPostedFileBase file = null)
        {
            _service = service;
            _fileName = fileName;
            _file = file;
        }

        public string Execute()
        {
            if (!FileHelpers.CheckFileExtensionByType(_fileName, EFileType.Image)) 
                return null;
            
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingService, _service.Image));

            var ext = Path.GetExtension(_fileName.Split('?')[0]);
            var newFileName = _service.Id + "_" + DateTime.Now.ToString("yyMMddhhmmss") + ext;
            var newFilePath = FoldersHelper.GetPathAbsolut(FolderType.BookingService, newFileName);

            try
            {
                using (Image image = Image.FromStream(_file.InputStream))
                    FileHelpers.SaveResizePhotoFile(newFilePath, SettingsPictureSize.BookingServiceImageWidth, SettingsPictureSize.BookingServiceImageHeight, image);

                _service.Image = newFileName;
                ServiceService.Update(_service);

                // delete temp fileif exists
                var filePath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, _fileName.Split('/').LastOrDefault());
                FileHelpers.DeleteFile(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadImageCropped", ex);
                return null;
            }

            return FoldersHelper.GetPath(FolderType.BookingService, _service.Image, false);
        }
    }
}
