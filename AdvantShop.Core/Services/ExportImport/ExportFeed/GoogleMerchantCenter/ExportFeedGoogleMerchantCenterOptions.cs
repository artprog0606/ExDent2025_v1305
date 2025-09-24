using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    [Serializable()]
    public class ExportFeedGoogleMerchantCenterOptions : IExportFeedFilterOptions
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

        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "RemoveHtml")]
        public bool RemoveHtml { get; set; }

        [JsonProperty(PropertyName = "DatafeedTitle")]
        public string DatafeedTitle { get; set; }

        [JsonProperty(PropertyName = "DatafeedDescription")]
        public string DatafeedDescription { get; set; }

        [JsonProperty(PropertyName = "GoogleProductCategory")]
        public string GoogleProductCategory { get; set; }

        [JsonProperty(PropertyName = "ProductDescriptionType")]
        public string ProductDescriptionType { get; set; }

        [JsonProperty(PropertyName = "OfferIdType")]
        public string OfferIdType { get; set; }

        [JsonProperty(PropertyName = "ColorSizeToName")]
        public bool ColorSizeToName { get; set; }
        
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}
