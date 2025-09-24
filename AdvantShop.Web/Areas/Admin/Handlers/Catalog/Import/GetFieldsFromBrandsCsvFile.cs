using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport.ImportServices;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromBrandsCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        public GetFieldsFromBrandsCsvFile(ImportBrandsModel model, string outputFilePath) : base(model, outputFilePath)
        {
        }

        protected override void LoadData()
        {
            CsvRows = new CsvImportBrands(
                OutputFilePath,
                new ImportBrandsSettings
                {
                    HasHeaders = false,
                    ColumnSeparator = ColumnSeparator,
                    Encodings = Encoding,
                    FieldMapping = null,
                    UseCommonStatistic = false
                }).ReadFirstRecord();
        }

        protected override object HandleData()
        {
            foreach (EBrandFields item in Enum.GetValues(typeof(EBrandFields)))
            {
                AllFields.Add(item.StrName().ToLower(), item.Localize());
            }

            return new {FirstItem, AllFields, Headers};
        }
    }
}
