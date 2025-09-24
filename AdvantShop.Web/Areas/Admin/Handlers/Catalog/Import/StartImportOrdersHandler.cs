using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport.ImportServices;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportOrdersHandler : BaseStartImportHandler
    {
        private readonly string _customOptionOptionsSeparator;

        private readonly bool _updateCustomer;

        public new readonly CsvImportOrdersFieldsMapping FieldMapping;

        public StartImportOrdersHandler(ImportOrdersModel model, string inputFilePath) : base(model, inputFilePath)
        {
            _customOptionOptionsSeparator = model.CustomOptionOptionsSeparator;

            _updateCustomer = model.UpdateCustomer;

            FieldMapping = new CsvImportOrdersFieldsMapping();
            for (int i = 0; i < model.SelectedFields.Count; i++)
            {
                FieldMapping.AddField(model.SelectedFields[i], i);
            }
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(EOrderFields.Number.StrName()))
                throw new BlException(LocalizationService.GetResource("Admin.ImportOrders.Errors.FieldsRequired"));
        }

        protected override void Handle()
        {
            var importOrders = new CsvImportOrders(InputFilePath, HaveHeader, ColumnSeparator,
                _customOptionOptionsSeparator, Encoding, FieldMapping, _updateCustomer);
            
            importOrders.ProcessThroughACommonStatistic("settingscheckout?checkoutTab=importOrders",
                LocalizationService.GetResource("Admin.ImportOrders.ProcessName"));
        }
    }
}
