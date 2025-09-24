using System.Collections.Generic;
using AdvantShop.Configuration;
using Newtonsoft.Json;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class SizeColorPickerViewModel
    {
        [JsonIgnore]
        public List<SizePickerModel> SizesList { get; set; }
        
        public string Sizes => SizesList.Count > 0 ? JsonConvert.SerializeObject(SizesList) : "";

        public int? SelectedSizeId { get; set; }
        public SettingsDesign.eSizeColorControlType ColorsControlType { get; set; }
        public SettingsDesign.eSizeColorControlType SizesControlType { get; set; }
        [JsonIgnore]
        public List<ColorPickerModel> ColorsList { get; set; }
        
        public string Colors => ColorsList.Count > 0 ? JsonConvert.SerializeObject(ColorsList) : "";

        public int? SelectedColorId { get; set; }

        public int ColorIconHeightDetails { get; set; }
        public int ColorIconWidthDetails { get; set; }

        public string SizesHeader { get; set; }

        public string ColorsHeader { get; set; }
    }
}