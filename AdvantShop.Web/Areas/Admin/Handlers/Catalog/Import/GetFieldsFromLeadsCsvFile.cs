using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.ExportImport.ImportServices;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromLeadsCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;

        public GetFieldsFromLeadsCsvFile(ImportLeadsModel model, string outputFilePath) : base(model, outputFilePath)
        {
            _propertySeparator = model.PropertySeparator;
            _propertyValueSeparator = model.PropertyValueSeparator;
        }

        protected override void LoadData()
        {
            CsvRows = new CsvImportLeads(OutputFilePath, false, ColumnSeparator, _propertySeparator, _propertyValueSeparator, Encoding, null, useCommonStatistic: false).ReadFirstRecord();
        }

        protected override object HandleData()
        {
            foreach (ELeadFields item in Enum.GetValues(typeof(ELeadFields)))
            {
                if (item == ELeadFields.None)
                    continue;
                AllFields.Add(item.StrName().ToLower(), item.Localize());
            }

            foreach (var leadField in LeadFieldService.GetLeadFields())
            {
                if (!AllFields.ContainsKey(leadField.Name.ToLower()))
                    AllFields.Add(leadField.Name.ToLower(), leadField.Name);
            }

            foreach (var additionalField in CustomerFieldService.GetCustomerFields(true))
            {
                if (!AllFields.ContainsKey(additionalField.Name.ToLower()))
                    AllFields.Add(additionalField.Name.ToLower(), additionalField.Name);
            }

            return new { FirstItem, AllFields, Headers };
        }
    }
}
