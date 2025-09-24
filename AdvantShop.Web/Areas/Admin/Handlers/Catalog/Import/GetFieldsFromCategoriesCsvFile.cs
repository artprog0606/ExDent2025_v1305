using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromCategoriesCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        public GetFieldsFromCategoriesCsvFile(ImportCategoriesModel model, string outputFilePath) : base(model, outputFilePath)
        {
        }

        protected override void LoadData()
        {
            CsvRows = new CsvImportCategories(OutputFilePath, false, ColumnSeparator, Encoding, null, null, null, null, useCommonStatistic: false).ReadFirstRecord();
        }

        protected override object HandleData()
        {
            var fieldNames = new Dictionary<string, string>();
            foreach (CategoryFields item in Enum.GetValues(typeof(CategoryFields)))
            {
                AllFields.Add(item.StrName().ToLower(), item.Localize());
                fieldNames.TryAddValue(item.StrName().ToLower(), item.StrName().ToLower());
            }
            SelectedFields = new List<string>();
            foreach (var header in Headers)
            {
                var identPart = header.Split(':').FirstOrDefault().ToLower();
                SelectedFields.Add(fieldNames.ContainsKey(header.ToLower())
                    ? fieldNames[header]
                    : fieldNames.ElementOrDefault(identPart, EProductField.None.StrName()));
            }
            return new { FirstItem, AllFields, Headers, SelectedFields };
        }
    }
}
