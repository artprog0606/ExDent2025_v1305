using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;

        private readonly List<CSVField> _moduleFields;

        public GetFieldsFromCsvFile(ImportProductsModel model, string outputFilePath) : base(model, outputFilePath)
        {
            _propertySeparator = model.PropertySeparator;
            _propertyValueSeparator = model.PropertyValueSeparator;
            
            _moduleFields = new List<CSVField>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    _moduleFields.AddRange(classInstance.GetCSVFields());
            }
        }

        protected override void LoadData()
        {
            CsvRows =
                CsvImport.Factory(OutputFilePath, false, EImportProductActionType.Nothing, ColumnSeparator, Encoding, null, _propertySeparator,
                                    _propertyValueSeparator, useCommonStatistic: false)
                         .ReadFirst2();
        }

        protected override object HandleData()
        {
            foreach (ProductFields item in Enum.GetValues(typeof(ProductFields)))
                AllFields.Add(item.StrName().ToLower(), item.Localize());

            foreach (var moduleField in _moduleFields)
            {
                if (!AllFields.ContainsKey(moduleField.StrName.ToLower()))
                    AllFields.Add(moduleField.StrName.ToLower(), moduleField.DisplayName);
            }
            
            string warningMsg = null;
            // var containsAmount = Headers != null && Headers.Contains(ProductFields.Amount.StrName());
            // var containsMultiOffer = Headers != null && Headers.Contains(ProductFields.MultiOffer.StrName());

            // if (containsAmount || containsMultiOffer)
            // {
            //     var warehouses = WarehouseService.GetList();
            //     if (warehouses.Count > 1)
            //         warningMsg = T("Admin.ImportProducts.AmountErrorByWarehousesCsv1",
            //                         containsAmount ? ProductFields.Amount.Localize() : ProductFields.MultiOffer.Localize());
            // }

            return new { FirstItem, AllFields, Headers, WarningMsg = warningMsg };
        }
    }
}
