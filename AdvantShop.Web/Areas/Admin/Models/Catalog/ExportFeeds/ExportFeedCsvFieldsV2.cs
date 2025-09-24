using System.Collections.Generic;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedCsvFieldsV2
    {
        public int Id { get; set; }

        public List<EProductField> FieldMapping { get; set; }
        public List<CSVField> ModuleFieldMapping { get; set; }

        public Dictionary<string, string> AllFields { get; set; }

        public string DefaultExportFields { get; set; }
        
    }
}
