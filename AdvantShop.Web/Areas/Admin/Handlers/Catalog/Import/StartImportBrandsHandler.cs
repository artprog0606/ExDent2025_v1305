using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport.ImportServices;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportBrandsHandler : BaseStartImportHandler
    {
        public StartImportBrandsHandler(ImportBrandsModel model, string inputFilePath) : base(model, inputFilePath)
        {
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(EBrandFields.Name.StrName()))
                throw new BlException(LocalizationService.GetResource("Admin.ImportBrands.Errors.FieldsRequired"));
        }

        protected override void Handle()
        {
            var importBrands = new CsvImportBrands(
                InputFilePath, new ImportBrandsSettings
                {
                    HasHeaders = HaveHeader,
                    ColumnSeparator = ColumnSeparator,
                    Encodings = Encoding,
                    FieldMapping = FieldMapping,
                    ImportMode = ImportMode.Full
                });
            importBrands.ProcessThroughACommonStatistic("settingscatalog?catalogTab=brand&BrandsListTab=import",
                LocalizationService.GetResource("Admin.ImportBrands.ProcessName"));
        }
    }
}
