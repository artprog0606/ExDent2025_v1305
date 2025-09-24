using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetImportCustomersModel
    {
        public ImportCustomersModel Execute()
        {
            var model = new ImportCustomersModel
            {
                HaveHeader = true,
                Encoding = EncodingsEnum.Windows1251.StrName(),
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
            };

            return model;
        }
    }
}
