using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportCustomersHandler : BaseStartImportHandler
    {
        private readonly int _defaultCustomerGroupId;

        public StartImportCustomersHandler(ImportCustomersModel model, string inputFilePath) : base(model, inputFilePath)
        {
            _defaultCustomerGroupId = model.DefaultCustomerGroupId;
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(ECustomerFields.Email.StrName()) &&
                !FieldMapping.ContainsKey(ECustomerFields.CustomerId.StrName()))
                throw new BlException(LocalizationService.GetResource("Admin.ImportCustomers.Errors.FieldsRequired"));
        }

        protected override void Handle()
        {
            var importCustomers = new CsvImportCustomers(InputFilePath, HaveHeader, ColumnSeparator, Encoding,
                                                         FieldMapping, _defaultCustomerGroupId);
            
            importCustomers.ProcessThroughACommonStatistic(
                "settingscustomers?tab=importCustomers", LocalizationService.GetResource("Admin.ImportCustomers.ProcessName"));

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_ImportCustomers);
        }
    }
}
