using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    public class ExportFeedFacebookOptions : IExportFeedFilterOptions
    {
        public bool ExportNotAvailable { get; set; }
        public bool AllowPreOrderProducts { get; set; }
        public bool OnlyMainOfferToExport { get; set; }
        [JsonIgnore]
        public int? PriceRuleId => null;
        [JsonIgnore]
        public int? PriceRuleIdForOldPrice => null;
        [JsonIgnore]
        public decimal? NotExportAmountCount => null;
        [JsonIgnore]
        public bool DontExportProductsWithoutDimensionsAndWeight => false;
        [JsonIgnore] 
        public bool ConsiderMultiplicityInPrice => false;

        public List<int> WarehouseIds => null;

        public string Currency { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        
        public bool RemoveHtml { get; set; }
        public string DatafeedTitle { get; set; }
        public string DatafeedDescription { get; set; }
        public string GoogleProductCategory { get; set; }
        public string ProductDescriptionType { get; set; }
        public string OfferIdType { get; set; }
        public bool ColorSizeToName { get; set; }
    }
}