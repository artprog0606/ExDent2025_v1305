using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport.ImportServices;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportLeadsHandler : BaseStartImportHandler
    {
        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;
        private readonly int _baseSalesFunnelId;

        private readonly bool _updateCustomer;
        private readonly bool _doNotDuplicate;

        public StartImportLeadsHandler(ImportLeadsModel model, string inputFilePath) : base(model, inputFilePath)
        {
            _propertySeparator = model.PropertySeparator;
            _propertyValueSeparator = model.PropertyValueSeparator;
            _baseSalesFunnelId = model.BasicSalesFunnelId.TryParseInt();

            _updateCustomer = model.UpdateCustomer;
            _doNotDuplicate = model.DoNotDuplicate;
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(ELeadFields.Email.StrName()) &&
                !FieldMapping.ContainsKey(ELeadFields.CustomerId.StrName()) &&
                !FieldMapping.ContainsKey(ELeadFields.Phone.StrName()))
                throw new BlException(LocalizationService.GetResource("Admin.ImportLeads.Errors.FieldsRequired"));
        }

        protected override void Handle()
        {
            var importLeads = new CsvImportLeads(InputFilePath, HaveHeader, ColumnSeparator, _propertySeparator,
                _propertyValueSeparator, Encoding, FieldMapping, _baseSalesFunnelId, _updateCustomer, _doNotDuplicate);
            
            importLeads.ProcessThroughACommonStatistic("settingscrm?crmTab=importLeads",
                LocalizationService.GetResource("Admin.ImportLeads.ProcessName"));
        }
    }
}
