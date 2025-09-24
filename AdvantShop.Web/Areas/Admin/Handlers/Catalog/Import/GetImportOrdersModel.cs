using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetImportOrdersModel
    {
        public ImportOrdersModel Execute()
        {
            var model = new ImportOrdersModel
            {
                HaveHeader = true,
                Encoding = EncodingsEnum.Windows1251.StrName(),
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                CustomOptionOptionsSeparator = ":",
                UpdateCustomer = true,
            };

            return model;
        }
    }
}
