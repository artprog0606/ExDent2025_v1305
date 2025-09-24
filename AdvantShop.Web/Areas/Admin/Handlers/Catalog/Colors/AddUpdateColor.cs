using System;
using System.Drawing;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Colors;
using Color = AdvantShop.Catalog.Color;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Colors
{
    public class AddUpdateColor
    {
        private readonly ColorModel _model;
        private readonly HttpPostedFileBase _colorIconFile;
        private readonly bool _isEditMode;

        public AddUpdateColor(ColorModel model, HttpPostedFileBase colorIconFile, bool isEditMode)
        {
            _model = model;
            _colorIconFile = colorIconFile;
            _isEditMode = isEditMode;
        }

        public bool Execute()
        {
            try
            {
                var color = new Color()
                {
                    ColorId = _model.ColorId,
                    ColorName = _model.ColorName.DefaultOrEmpty(),
                    ColorCode = _model.ColorCode.DefaultOrEmpty() ?? "",
                    SortOrder = _model.SortOrder,
                };

                var isPhoto = _colorIconFile != null &&
                                   FileHelpers.CheckFileExtensionByType(_colorIconFile.FileName, EFileType.Image);

                if (isPhoto)
                    color.ColorCode = "";

                if (!_isEditMode)
                {
                    color.ColorId = ColorService.AddColor(color);
                    if (!isPhoto && string.Equals(color.ColorCode, "#000000"))
                        ColorService.UpdateUnSetColorsFromAdvantshopCsv(colorName: color.ColorName);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ColorCreated);
                }
                else
                {
                    if (color.IconFileName != null && color.IconFileName.PhotoName != null)
                        color.ColorCode = "";

                    ColorService.UpdateColor(color);
                }

                if (isPhoto)
                    AddColorPhoto(color.ColorId);
                ProductService.PreCalcProductColors(color.ColorId);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return false;
            }

            return true;
        }

        private void AddColorPhoto(int colorId)
        {
            try
            {
                PhotoService.DeletePhotos(colorId, PhotoType.Color);

                var tempName = PhotoService.AddPhoto(new Photo(0, colorId, PhotoType.Color)
                {
                    OriginName = _colorIconFile.FileName
                });

                if (!string.IsNullOrWhiteSpace(tempName))
                {
                    using (Image image = Image.FromStream(_colorIconFile.InputStream))
                    {
                        var isRotated = FileHelpers.RotateImageIfNeed(image);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Catalog, tempName),
                            SettingsPictureSize.ColorIconWidthCatalog, SettingsPictureSize.ColorIconHeightCatalog, image, isRotated: isRotated);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Details, tempName),
                            SettingsPictureSize.ColorIconWidthDetails, SettingsPictureSize.ColorIconHeightDetails, image, isRotated: isRotated);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
    }
}
