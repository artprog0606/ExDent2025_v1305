using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    [Serializable()]
    public class ExportFeedResellerOptions : IExportFeedCsvFilterOptions
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ExportNotAvailable { get; set; }
        public bool CsvExportNoInCategory { get; set; }
        public bool ExportFromMainCategories { get; set; }


        public string ResellerCode { get; set; }
        public string CsvEnconing { get; set; }
        public string CsvSeparator { get; set; }
        public string CsvColumSeparator { get; set; }
        public string CsvPropertySeparator { get; set; }
        public bool CsvCategorySort { get; set; }
        public List<ProductFields> FieldMapping { get; set; }
        public List<CSVField> ModuleFieldMapping { get; set; }
        public bool? UnloadOnlyMainCategory { get; set; }
        public List<int> StocksFromWarehouses { get; set; }
    }

    public enum EExportFeedResellerPriceMarginType
    {
        [Localize("Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent")]
        Percent,

        [Localize("Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.AbsoluteValue")]
        AbsoluteValue
    }
}