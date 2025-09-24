using System;
using System.IO;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using AdvantShop.FilePath;
using AdvantShop.Catalog;
using System.Linq;
using System.Web.SessionState;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Helpers;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class AvitoController : BaseClientController
    {
        public FileResult AvitoPhoto(int photoId)
        {
            var photo = PhotoService.GetPhoto<ProductPhoto>(photoId, PhotoType.Product);
            
            if (photo == null || string.IsNullOrEmpty(photo.PhotoName))
                return null;
            
            var photoName = photo.PhotoName;
            var photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photo.PhotoName);

            if (photo.PhotoName.Contains("://"))
            {
                if (photo.PhotoName.Contains("cs71.advantshop.net"))  // https://cs71.advantshop.net/15705.jpg
                {
                    photoName = photoPath.Split('/').LastOrDefault();
                    photoPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                    var name = photo.PhotoName.Split('/').LastOrDefault();
                    var url = photo.PhotoName.Replace(name, "") + "pictures/product/big/" + name.Replace(".", "_big.");

                    if (!FileHelpers.DownloadRemoteImageFile(url, photoPath))
                        return null;

                    photoName = photoName.Replace(".", "_tmp.");
                }
                else
                {
                    photoName = Guid.NewGuid() + "_" + photo.PhotoName.Split('/').LastOrDefault();
                    photoPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                    if (!FileHelpers.DownloadRemoteImageFile(photo.PhotoName, photoPath))
                        return null;

                    photoName = photoName.Replace(".", "_tmp.");
                }
            }

            var tempPhoto = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);
            FileHelpers.DeleteFile(tempPhoto);

            if (!System.IO.File.Exists(photoPath))
                return null;

            if (!new FileExtensionContentTypeHelper().TryGetContentType(photoPath, out var contentType))
                contentType = "image/jpeg";

            using (var image = Image.FromFile(photoPath))
            {
                if (image.Width < 210 || image.Height < 210 || (double)image.Width / image.Height > 1.33 || (double)image.Height / image.Width > 1.33)
                {
                    if (!Resize(image, tempPhoto))
                        return null;
                    
                    return File(tempPhoto, contentType);
                }
            }
            return File(photoPath, contentType);
        }

        private bool Resize(Image image, string resultPath)
        {
            var width = image.Width;
            var height = image.Height;
            if (width > height && (double)width / height > 1.33)
            {
                height = Convert.ToInt32(width / 1.33);
            }
            if (width > height && (double)height / width > 1.33)
            {
                width = Convert.ToInt32(height * 1.33);
            }

            try
            {
                using (var img = new Bitmap(image))
                using (var result = new Bitmap(width, height))
                {
                    result.MakeTransparent();
                    using (var graphics = Graphics.FromImage(result))
                    {
                        graphics.Clear(System.Drawing.Color.White);
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(img, Math.Abs(image.Width - width) / 2, Math.Abs(image.Height - height) / 2, img.Width, img.Height);

                        graphics.Flush();
                        using (var stream = new FileStream(resultPath, FileMode.CreateNew))
                        {
                            result.Save(stream, ImageFormat.Jpeg);
                            stream.Close();
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                //_logger.Error(ex);
                return false;
            }
            return true;
        }
    }
}