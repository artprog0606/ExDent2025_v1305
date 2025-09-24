using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportCategories
{
    public class ExportCategoriesSettingsModel
    {
        public string CsvSeparator { get; set; }
        public string CsvEncoding { get; set; }
        public List<string> ExportCategoriesFields { get; set; }
        public string CsvSeparatorCustom { get; set; }
        public string PropertySeparator { get; set; }
        public string NameSameProductProperty { get; set; }
        public string NameNotSameProductProperty { get; set; }
    }
}
