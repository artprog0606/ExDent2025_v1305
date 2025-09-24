using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.ViewModel.ProductDetails;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetSizeColorPicker
    {
        private readonly Product _product;
        private readonly int? _color;
        private readonly int? _size;
        private readonly SettingsDesign.eSizeColorControlType _colorsControlType;
        private readonly SettingsDesign.eSizeColorControlType _sizeControlType;
        
        public GetSizeColorPicker(Product product, int? color, int? size, 
            SettingsDesign.eSizeColorControlType colorsColorControlType = SettingsDesign.eSizeColorControlType.Enumeration,
            SettingsDesign.eSizeColorControlType sizeColorControlType = SettingsDesign.eSizeColorControlType.Enumeration)
        {
            _product = product;
            _color = color;
            _size = size;
            _colorsControlType = colorsColorControlType;
            _sizeControlType = sizeColorControlType;
        }

        public SizeColorPickerViewModel Execute()
        {
            var offers = _product.Offers;

            var offerColors =
                offers.Where(o =>
                        o.Color != null &&
                        (o.Amount > 0 || _product.AllowPreOrder || !SettingsCatalog.ShowOnlyAvalible))
                    .OrderBy(o => o.Color.SortOrder)
                    .ThenBy(o => o.Color.ColorName);

            if (SettingsCatalog.MoveNotAvaliableToEnd)
            {
                offerColors = offerColors.OrderByDescending(x => x.BasePrice > 0 && x.Amount > 0);
            }

            var colors = offerColors.Select(x => new ColorPickerModel(x.Color)).Distinct().ToList();

            var sizeOffers =
                offers.Where(o =>
                        o.SizeForCategory != null &&
                        (o.Amount > 0 || _product.AllowPreOrder || !SettingsCatalog.ShowOnlyAvalible))
                    .OrderBy(o => o.SizeForCategory.SortOrder);

            if (SettingsCatalog.MoveNotAvaliableToEnd)
            {
                sizeOffers = sizeOffers.OrderByDescending(x => x.BasePrice > 0 && x.Amount > 0);
            }

            var sizes = sizeOffers.Select(x => new SizePickerModel(x.SizeForCategory)).Distinct().ToList();

            var model = new SizeColorPickerViewModel
            {
                ColorsList = colors,
                SizesList = sizes,
                ColorIconWidthDetails = SettingsPictureSize.ColorIconWidthDetails,
                ColorIconHeightDetails = SettingsPictureSize.ColorIconHeightDetails,
                SizesHeader = SettingsCatalog.SizesHeader,
                ColorsHeader = SettingsCatalog.ColorsHeader,
                SelectedColorId = _color,
                SelectedSizeId = _size,
                ColorsControlType = _colorsControlType,
                SizesControlType = _sizeControlType
            };

            return model;
        }
    }
}