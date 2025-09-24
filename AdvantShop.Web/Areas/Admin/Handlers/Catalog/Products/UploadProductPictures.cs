using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Catalog.Products;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class UploadProductPictures
    {
        private readonly int _productId;
        private readonly List<HttpPostedFileBase> _files;

        public UploadProductPictures(List<HttpPostedFileBase> files, int productId)
        {
            _files = files;
            _productId = productId;
        }

        public UploadPhotoResultModel Execute()
        {
            if (SaasDataService.IsSaasEnabled)
            {
                var maxPhotoCount = SaasDataService.CurrentSaasData.PhotosCount;

                if (PhotoService.GetCountPhotos(_productId, PhotoType.Product) >= maxPhotoCount)
                {
                    return new UploadPhotoResultModel
                    {
                        Error = LocalizationService.GetResource("Admin.UploadPhoto.MaxReached") + maxPhotoCount
                    };
                }
            }

            if (HttpContext.Current == null || _files.Count == 0)
                return new UploadPhotoResultModel { Error = "Файл не найден" };

            FileHelpers.UpdateDirectories();

            var errors = "";

            foreach (var file in _files)
            {
                if (file == null || string.IsNullOrEmpty(file.FileName))
                    continue;

                if (file.ContentLength >= 1024 * 1024 * 15) //больше 15МБ
                {
                    errors += $"Размер файла {file.FileName} больше разрешенных 15мб. ";
                    continue;
                }

                if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                {
                    errors += $"Файл {file.FileName} имеет неправильное разрешение. ";
                    continue;
                }

                if (!AddPhoto(file, PhotoType.Product, out var error))
                    errors += error;
            }

            ProductService.PreCalcProductParams(_productId);

            return new UploadPhotoResultModel {Result = errors == "", Error = errors};
        }


        private bool AddPhoto(HttpPostedFileBase file, PhotoType type, out string error)
        {
            error = null;
            try
            {
                var photo = new Photo(0, _productId, type) { OriginName = file.FileName };
                var photoName = PhotoService.AddPhoto(photo);

                if (!string.IsNullOrWhiteSpace(photoName))
                {
                    using (Image image = Image.FromStream(file.InputStream))
                        FileHelpers.SaveProductImageUseCompress(photoName, image);

                    return true;
                }
            }
            catch (OutOfMemoryException ex)
            {
                Debug.Log.Error($"UploadProductPictures", ex);
                error = LocalizationService.GetResource("Admin.Catalog.ErrorUploadImageFormat");
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadProductPictures", ex);
                error = "Ошибка при добавлении файла " + file.FileName;
            }
            
            return false;
        }

    }
}
