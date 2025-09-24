using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Brands;
using Size = System.Drawing.Size;

namespace AdvantShop.Web.Admin.Handlers.Cms.Menus
{
    public class UploadMenuIcon
    {
        private readonly int? _itemId;
        private readonly Size _iconSize;

        public UploadMenuIcon(int? itemId, Size size)
        {
            _itemId = itemId;
            _iconSize = size;
        }

        public UploadBrandPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadBrandPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            FileHelpers.UpdateDirectories();

            var img = HttpContext.Current.Request.Files["file"];
            if (img != null && img.ContentLength > 0)
            {
                if (!FileHelpers.CheckFileExtensionByType(img.FileName, EFileType.Image))
                    return new UploadBrandPictureResult { Error = LocalizationService.GetResource("Admin.Error.InvalidImageFormat") };

                try
                {
                    Bitmap bmp = new Bitmap(Image.FromStream(img.InputStream));
                    if (_itemId.HasValue)
                    {
                        PhotoService.DeletePhotos(_itemId.Value, PhotoType.MenuIcon);

                        var photo = new Photo(0, _itemId.Value, PhotoType.MenuIcon) { OriginName = img.FileName };
                        PhotoService.AddPhoto(photo);
                        if (photo.PhotoName.IsNotEmpty())
                        {
                            string path = FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, photo.PhotoName);
                            FileHelpers.SaveResizePhotoFile(path, _iconSize.Width, _iconSize.Height, bmp);
                        }

                        MenuService.UpdateMenuItemIcon(_itemId.Value, photo.PhotoName);
                        
                        return new UploadBrandPictureResult
                        {
                            Result = true,
                            PictureId = photo.PhotoId,
                            PictureName = photo.PhotoName,
                            Picture = FoldersHelper.GetPath(FolderType.MenuIcons, photo.PhotoName, false)
                        };
                    }
                    else
                    {
                        var filePathAbs = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, img.FileName);
                        FileHelpers.DeleteFile(filePathAbs);
                        FileHelpers.SaveResizePhotoFile(filePathAbs, _iconSize.Width, _iconSize.Height, bmp);

                        return new UploadBrandPictureResult
                        {
                            Result = true,
                            PictureName = img.FileName,
                            Picture = FoldersHelper.GetPath(FolderType.ImageTemp, img.FileName, false)
                        };
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message, ex);
                }
            }

            return new UploadBrandPictureResult { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }
    }
}
