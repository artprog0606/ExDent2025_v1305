using AdvantShop.Catalog;
using AdvantShop.FilePath;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class ColorPickerModel
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string PhotoName { get; set; }
        public string ImageSrcForProduct { get; }

        public ColorPickerModel(Color color)
        {
            ColorId = color.ColorId;
            ColorName = color.ColorName;
            ColorCode = color.ColorCode;
            PhotoName = color.IconFileName.PhotoName;
            ImageSrcForProduct = color.IconFileName.ImageSrc(ColorImageType.Details);
        }

        public override bool Equals(object obj)
        {
            var color = obj as ColorPickerModel;
            if (color == null)
                return false;

            return this.ColorId == color.ColorId &&
                   this.ColorName == color.ColorName &&
                   this.ColorCode == color.ColorCode &&
                   this.PhotoName == color.PhotoName;
        }

        public override int GetHashCode()
        {
            return ColorId.GetHashCode() ^
                   (ColorName ?? "").GetHashCode() ^
                   (ColorCode ?? "").GetHashCode() ^
                   (PhotoName ?? "").GetHashCode();
        }
    }
}