using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.ExportImport.ImportServices;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromOrdersCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        private readonly string _customOptionOptionsSeparator;

        public GetFieldsFromOrdersCsvFile(ImportOrdersModel model, string outputFilePath) : base(model, outputFilePath)
        {
            _customOptionOptionsSeparator = model.CustomOptionOptionsSeparator;
        }

        protected override void LoadData()
        {
            CsvRows = new CsvImportOrders(OutputFilePath, false, ColumnSeparator, _customOptionOptionsSeparator, Encoding, null, useCommonStatistic: false).ReadFirstRecord();
        }

        protected override object HandleData()
        {var fieldNames = new Dictionary<string, string>();
            foreach (EOrderFields item in Enum.GetValues(typeof(EOrderFields)))
            {
                if (item == EOrderFields.None)
                    continue;
                AllFields.Add(item.StrName().ToLower(), item.Localize());
                fieldNames.TryAddValue(item.StrName().ToLower(), item.StrName().ToLower());
            }

            SelectedFields = new List<string>();
            foreach (var header in Headers)
            {
                var identPart = header.Split(':').FirstOrDefault();
                SelectedFields.Add(fieldNames.TryGetValue(header, out var fieldName)
                    ? fieldName
                    : fieldNames.ElementOrDefault(identPart, EOrderFields.None.StrName()));
            }
            return new { FirstItem, AllFields, Headers, SelectedFields };
        }
    }
}
