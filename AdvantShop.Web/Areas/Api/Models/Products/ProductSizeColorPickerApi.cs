using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;
using AdvantShop.ViewModel.ProductDetails;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductSizeColorPickerApi
    {
        public List<SizeApi> Sizes { get;}
        public int? SelectedSizeId { get; set; }

        public List<ColorApi> Colors { get; }
        public int? SelectedColorId { get; set; }

        public int ColorIconHeight { get; }
        public int ColorIconWidth { get; }

        public string SizesHeader { get; }
        public string ColorsHeader { get; }

        public ProductSizeColorPickerApi(SizeColorPickerViewModel picker)
        {
            Sizes = picker.SizesList.Select(x => new SizeApi(x)).ToList();
            Colors = picker.ColorsList.Select(x => new ColorApi(x)).ToList();
            
            ColorIconHeight = picker.ColorIconHeightDetails;
            ColorIconWidth = picker.ColorIconWidthDetails;
            
            SizesHeader = picker.SizesHeader;
            ColorsHeader = picker.ColorsHeader;
        }
    }
    
    public class SizeApi
    {
        public int Id { get; }
        public string Name { get; }
        
        public SizeApi(SizePickerModel size)
        {
            Id = size.SizeId;
            Name = size.SizeName;
        }
        
        public SizeApi(Size size)
        {
            Id = size.SizeId;
            Name = size.SizeName;
        }
    }
    
    public class ColorApi
    {
        public int Id { get; }
        public string Name { get; }
        public string Code { get; }
        public string PhotoSrc { get; }
        
        public ColorApi(ColorPickerModel color)
        {
            Id = color.ColorId;
            Name = color.ColorName;
            Code = !string.IsNullOrWhiteSpace(color.ColorCode) ? color.ColorCode : null;
            PhotoSrc = !string.IsNullOrWhiteSpace(color.ImageSrcForProduct) ? color.ImageSrcForProduct : null;
        }

        public ColorApi(ProductColorModel color)
        {
            Id = color.ColorId;
            Name = color.ColorName;
            Code = !string.IsNullOrWhiteSpace(color.ColorCode) ? color.ColorCode : null;
            PhotoSrc = !string.IsNullOrWhiteSpace(color.PhotoName)
                ? FoldersHelper.GetImageColorPath(ColorImageType.Catalog, color.PhotoName, false)
                : null;
        }

        public ColorApi(Color color)
        {
            Id = color.ColorId;
            Name = color.ColorName;
            Code = !string.IsNullOrWhiteSpace(color.ColorCode) ? color.ColorCode : null;
            PhotoSrc = color.IconFileName?.ImageSrc(ColorImageType.Catalog).Default(null);
        }
    }
}