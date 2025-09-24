using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetImportLeadsModel
    {
        public ImportLeadsModel Execute()
        {
            var model = new ImportLeadsModel
            {
                HaveHeader = true,
                Encoding = EncodingsEnum.Windows1251.StrName(),
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                PropertySeparator = ";",
                PropertyValueSeparator = ":",
                UpdateCustomer = true,
                DoNotDuplicate = true,
            };

            return model;
        }
    }
}
