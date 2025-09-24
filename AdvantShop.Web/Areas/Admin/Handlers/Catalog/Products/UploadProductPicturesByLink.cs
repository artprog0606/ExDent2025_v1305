using System;
using System.Drawing;
using System.IO;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Images.WebpDowngrader;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Catalog.Products;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class UploadProductPicturesByLink
    {
        private readonly int _productId;
        private readonly string _fileLink;

        public UploadProductPicturesByLink(int productId, string fileLink)
        {
            _productId = productId;
            _fileLink = fileLink;
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

            FileHelpers.UpdateDirectories();

            if (!FileHelpers.CheckFileExtensionByType(_fileLink, EFileType.Image | EFileType.ModernImage))
                return new UploadPhotoResultModel
                {
                    Result = false,
                    Error = $"Файл {_fileLink} имеет неправильное разрешение"
                };

            AddPhoto(_fileLink, PhotoType.Product, out var error);
            
            ProductService.PreCalcProductParams(_productId);

            return new UploadPhotoResultModel {Result = error.IsNullOrEmpty(), Error = error};
        }


        private bool AddPhoto(string fileLink, PhotoType type, out string error)
        {
            error = string.Empty;
            try
            {
                var fileLinkWithoutQuery = fileLink.Split('?')[0];
                var photo = new Photo(0, _productId, type) { OriginName = fileLinkWithoutQuery };
                
                var tempPhotoName = Path.GetFileName(fileLinkWithoutQuery);
                var tempPhotoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, tempPhotoName);

                if (tempPhotoName.IsNullOrEmpty())
                    return false;
                
                if (WebpDownGraderService.DetectWebpFromLink(fileLink))
                {
                    try
                    {
                        var success = WebpDownGraderService.DowngradeImageByUri(fileLink, tempPhotoFullName);

                        if (success is false)
                        {
                            return false;
                        }
                    }
                    catch (NotSupportedException)
                    {
                        error = LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat");
                        return false;
                    }
                    catch (BlException e)
                    {
                        error = e.Message;
                        return false;
                    }
                    
                    var oldExtension = FileHelpers.GetExtension(fileLinkWithoutQuery);
                    photo.OriginName = photo.OriginName.Replace(oldExtension, ".jpeg");
                }
                else if (!FileHelpers.DownloadRemoteImageFile(fileLink, tempPhotoFullName, out error))
                {
                    return false;
                }
                
                using (var image = Image.FromFile(tempPhotoFullName))
                {
                    PhotoService.AddPhoto(photo);
                    if (photo.PhotoName.IsNotEmpty())
                        FileHelpers.SaveProductImageUseCompress(photo.PhotoName, image);
                }
                
                FileHelpers.DeleteFile(tempPhotoFullName);

                error = string.Empty;
                return true;
            }
            catch (OutOfMemoryException ex)
            {
                Debug.Log.Error($"UploadProductPicturesByLink", ex);
                error = LocalizationService.GetResource("Admin.Catalog.ErrorUploadImageFormat");
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadProductPicturesByLink", ex);
                error = "Ошибка при добавлении файла " + fileLink + " " + ex.Message;
            }

            return false;
        }
    }
}
