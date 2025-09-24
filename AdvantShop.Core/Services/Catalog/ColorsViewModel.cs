using AdvantShop.Configuration;
using AdvantShop.FilePath;

namespace AdvantShop.ViewModel.Shared
{
    public class ColorsViewModel
    {
        public ColorsViewModel()
        {
            NgColorSelected = "productViewItem.colorSelected";
            NgInitColors = "productViewItem.initColors(colorsViewer)";
            NgInitCarousel = "productViewItem.initColorsCarousel(carousel)";
            NgChangeColor = "productViewItem.changeColor(color)";
            EnabledSlider = true;
            InitilazeTo = ".js-color-viewer-slider";
            ImageType = ColorImageType.Catalog;
            ControlType = SettingsDesign.ColorsControlType;
        }

        public string NgColors { get; set; }

        public int ColorWidth { get; set; }

        public int ColorHeight { get; set; }

        public string NgColorSelected { get; set; }

        public string HeaderText { get; set; }

        public string NgInitColors { get; set; }

        public string NgInitCarousel { get; set; }

        public string NgChangeColor { get; set; }

        public bool EnabledSlider { get; set; }

        public bool Multiselect { get; set; }

        public string InitilazeTo { get; set; }

        public string SelectedColors { get; set; }

        public int? SelectedColorId { get; set; }

        public ColorImageType ImageType { get; set; }

        public ColorsViewMode ColorsViewMode { get; set; }

        public bool IsHiddenColorName { get; set; }

        public SettingsDesign.eSizeColorControlType ControlType { get; set; }
    }
}