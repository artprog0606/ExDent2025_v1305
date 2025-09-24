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

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    public class UploadImageCropped
    {
        private readonly ReservationResource _reservationResource;
        private readonly string _fileName;
        private readonly HttpPostedFileBase _file;

        public UploadImageCropped(ReservationResource reservationResource, string fileName, HttpPostedFileBase file = null)
        {
            _reservationResource = reservationResource;
            _fileName = fileName;
            _file = file;
        }

        public string Execute()
        {
            if (!FileHelpers.CheckFileExtensionByType(_fileName, EFileType.Image)) 
                return null;
            
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingReservationResource, _reservationResource.Image));

            var ext = Path.GetExtension(_fileName.Split('?')[0]);
            var newFileName = _reservationResource.Id + "_" + DateTime.Now.ToString("yyMMddhhmmss") + ext;
            var newFilePath = FoldersHelper.GetPathAbsolut(FolderType.BookingReservationResource, newFileName);

            try
            {
                using (Image image = Image.FromStream(_file.InputStream))
                    FileHelpers.SaveResizePhotoFile(newFilePath, SettingsPictureSize.BookingServiceImageWidth, SettingsPictureSize.BookingServiceImageHeight, image);

                _reservationResource.Image = newFileName;
                ReservationResourceService.Update(_reservationResource);

                // delete temp fileif exists
                var filePath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, _fileName.Split('/').LastOrDefault());
                FileHelpers.DeleteFile(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadImageCropped", ex);
                return null;
            }

            return FoldersHelper.GetPath(FolderType.BookingReservationResource, _reservationResource.Image, false);
        }
    }
}
