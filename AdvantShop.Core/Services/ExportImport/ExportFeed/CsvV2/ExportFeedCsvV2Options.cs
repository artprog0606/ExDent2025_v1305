using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    [Serializable()]
    public class ExportFeedCsvV2Options : IExportFeedCsvFilterOptions
    {
        [JsonIgnore]
        public bool ExportNotAvailable => true;
        public bool CsvExportNoInCategory { get; set; }
        [JsonIgnore]
        public bool ExportFromMainCategories => false;

        public string CsvEnconing { get; set; }
        public string CsvSeparator { get; set; }
        public string CsvSeparatorCustom { get; set; }
        public string CsvColumSeparator { get; set; }
        public string CsvPropertySeparator { get; set; }
        public bool CsvCategorySort { get; set; }
        public List<EProductField> FieldMapping { get; set; }
        public List<CSVField> ModuleFieldMapping { get; set; }
    }
}
