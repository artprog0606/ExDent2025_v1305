using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.Import
{
    public class ImportBrandsModel : BaseImportModel
    {
        public ImportBrandsModel()
        {
            HaveHeader = true;
            Encoding = EncodingsEnum.Utf8.StrName();
            ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName();
        }
    }
}
