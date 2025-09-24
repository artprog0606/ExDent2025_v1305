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
    [ExportFeedKey("CsvV2")]
    public class ExportFeedCsvV2 : BaseExportFeed<ExportFeedCsvV2Options>
    {
        private readonly ExportCsvProductsService<ExportFeedCsvV2Product, ExportFeedCsvV2Options> _exportService;

        public ExportFeedCsvV2(int exportFeedId) : this(exportFeedId, false) { }

        public ExportFeedCsvV2(int exportFeedId, bool useCommonStatistic) : base(exportFeedId, useCommonStatistic)
        {
            _exportService = new ExportCsvProductsService<ExportFeedCsvV2Product, ExportFeedCsvV2Options>(exportFeedId, Settings);
        }

        protected override void Handle()
        {
            var productsCount = _exportService.GetProductsCount();
            var products = _exportService.GetProducts(productsCount, (reader) => ExportFeedCsvV2Service.GetCsvProductsFromReader(reader, Settings));

            CsvExportV2.Factory(products, Settings, productsCount, useCommonStatistic: UseCommonStatistic).Process();
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
                new ExportFeedSettings<ExportFeedCsvV2Options>
                {
                    FileName = ExportFeedService.GetNewExportFileName(_exportFeedId, "export/catalog", "csv"),
                    FileExtention = "csv",
                    Interval = 1,
                    IntervalType = Core.Scheduler.TimeIntervalType.Hours,
                    Active = false,
                    AdvancedSettings = new ExportFeedCsvV2Options
                    {
                        CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                        CsvColumSeparator = ";",
                        CsvPropertySeparator = ":",
                        CsvEnconing = EncodingsEnum.Utf8.StrName(),

                        FieldMapping = new List<EProductField>(Enum.GetValues(typeof(EProductField)).Cast<EProductField>()
                            .Where(item => item != EProductField.None && item != EProductField.Sorting && item != EProductField.ExternalCategoryId 
                                && item != EProductField.YandexDeliveryDays).ToList()),
                        ModuleFieldMapping = moduleFields
                    },
                    AdditionalUrlTags = string.Empty,
                    ExportAdult = true
                });
        }

        public override object GetAdvancedSettings()
        {
            return ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedCsvV2Options>(_exportFeedId);
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override List<string> GetAvailableFileExtentions() => new List<string> { "csv", "txt" };
    }
}