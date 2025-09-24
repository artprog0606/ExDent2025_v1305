using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportProductsHandler : BaseStartImportHandler
    {
        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;
        private readonly EImportProductActionType _productActionType;
        private readonly bool _importRemains;
        private readonly bool _onlyUpdateProducts;
        private readonly bool _updatePhotos;
        private readonly bool _enabled301Redirects;
        private readonly bool _addProductInParentCategory;
        private readonly int? _stocksToWarehouseId;

        public StartImportProductsHandler(ImportProductsModel model, string inputFilePath) : base(model, inputFilePath)
        {
            _propertySeparator = model.PropertySeparator;
            _propertyValueSeparator = model.PropertyValueSeparator;
            _productActionType = model.ProductActionType;
            _importRemains = model.ImportRemainsType == EImportRemainsType.Remains.StrName();
            _onlyUpdateProducts = model.OnlyUpdateProducts;
            _updatePhotos = model.UpdatePhotos;
            _enabled301Redirects = model.Enabled301Redirects;
            _addProductInParentCategory = model.AddProductInParentCategory;
            _stocksToWarehouseId = model.StocksToWarehouseId;
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(ProductFields.Sku.StrName()) && !FieldMapping.ContainsKey(ProductFields.Name.StrName()))
                throw new BlException(LocalizationService.GetResource("Admin.ImportCsv.Errors.FieldsRequired"));
        }

        protected override void Handle()
        {
            CsvImport.Factory(InputFilePath, HaveHeader, _productActionType, ColumnSeparator, Encoding, FieldMapping,
                    _propertySeparator, _propertyValueSeparator, false, _importRemains,
                    onlyUpdateProducts: _onlyUpdateProducts,
                    updatePhotos: _updatePhotos,
                    trackChanges: true, 
                    enabled301Redirects: _enabled301Redirects, 
                    addProductInParentCategory: _addProductInParentCategory,
                    stocksToWarehouseId: _stocksToWarehouseId)
                .ProcessThroughACommonStatistic("import?importTab=importProducts",
                    LocalizationService.GetResource("Admin.ImportCsv.ProcessName"));

            TrialService.TrackEvent(TrialEvents.MakeCSVImport, "");
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ImportProducts_Csv);
        }
    }
}
