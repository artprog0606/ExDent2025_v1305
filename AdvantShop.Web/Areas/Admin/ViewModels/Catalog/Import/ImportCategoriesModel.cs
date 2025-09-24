using AdvantShop.Saas;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.Import
{
    public class ImportCategoriesModel : BaseImportModel
    {
        public SaasData CurrentSaasData { get; set; }
        public string PropertySeparator { get; set; }
        public string NameSameProductProperty { get; set; }
        public string NameNotSameProductProperty { get; set; }
    }
}
