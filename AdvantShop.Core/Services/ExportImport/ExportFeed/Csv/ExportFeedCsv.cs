//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Csv")]
    public class ExportFeedCsv : BaseExportFeed<ExportFeedCsvOptions>
    {
        private readonly ExportCsvProductsService<ExportFeedCsvProduct, ExportFeedCsvOptions> _exportService;

        public ExportFeedCsv(int exportFeedId) : this(exportFeedId, false) { }

        public ExportFeedCsv(int exportFeedId, bool useCommonStatistic) : base(exportFeedId, useCommonStatistic)
        {
            _exportService = new ExportCsvProductsService<ExportFeedCsvProduct, ExportFeedCsvOptions>(exportFeedId, Settings);
        }

        protected override void Handle()
        {
            var productsCount = _exportService.GetProductsCount();
            var products = _exportService.GetProducts(productsCount , (reader) => ExportFeedCsvService.GetCsvProductsFromReader(reader, Settings));

            CsvExport.Factory(products, Settings, productsCount, useCommonStatistic: UseCommonStatistic).Process();
        }

        public override void SetDefaultSettings()
        {
            var moduleFields = new List<CSVField>();
            
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    moduleFields.AddRange(classInstance.GetCSVFields());
            }
            
            ExportFeedSettingsProvider.SetSettings(_exportFeedId,
                  new ExportFeedSettings<ExportFeedCsvOptions>()
                  {
                      FileName = ExportFeedService.GetNewExportFileName(_exportFeedId, "export/catalog", "csv"),
                      FileExtention = "csv",
                      Interval = 1,
                      IntervalType = Core.Scheduler.TimeIntervalType.Hours,
                      Active = false,
                      
                      AdvancedSettings = new ExportFeedCsvOptions()
                      {
                          CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                          CsvColumSeparator = ";",
                          CsvPropertySeparator = ":",
                          CsvEnconing = EncodingsEnum.Utf8.StrName(),

                          FieldMapping = new List<ProductFields>(Enum.GetValues(typeof(ProductFields)).OfType<ProductFields>().Where(item => item != ProductFields.None 
                            && item != ProductFields.Sorting && item != ProductFields.ExternalCategoryId && item != ProductFields.YandexDeliveryDays).ToList()),
                          ModuleFieldMapping = moduleFields
                      },
                      AdditionalUrlTags = string.Empty,
                      ExportAdult = true
                  });
        }

        public override object GetAdvancedSettings()
        {
            return ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedCsvOptions>(_exportFeedId);
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return new List<string> { "csv", "txt" };
        }
    }
}