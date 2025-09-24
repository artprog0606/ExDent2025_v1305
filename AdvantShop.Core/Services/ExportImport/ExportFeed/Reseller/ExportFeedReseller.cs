//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Reseller")]
    public class ExportFeedReseller : BaseExportFeed<ExportFeedResellerOptions>
    {
        private readonly ExportCsvProductsService<ExportFeedCsvProduct, ExportFeedResellerOptions> _exportService;
        private readonly ExportFeedResellerOptions _options;

        public ExportFeedReseller(int exportFeedId) : this(exportFeedId, false) { }

        public ExportFeedReseller(int exportFeedId, bool useCommonStatistic) : base(exportFeedId, useCommonStatistic)
        {
            _exportService = new ExportCsvProductsService<ExportFeedCsvProduct, ExportFeedResellerOptions>(exportFeedId, Settings);
            _options = Settings?.AdvancedSettings;
        }

        protected override void Handle()
        {
            var url = GetDownloadableExportFeedFileLink();
            
            CsSetFileName(url);
            CsSetFileUrl(url);

            var productsCount = _exportService.GetProductsCount();
            var products = _exportService.GetProducts(productsCount, (reader) => ExportFeedResellerService.GetCsvProductFromReader(reader, Settings));

            var csvOptions = new ExportFeedSettings<ExportFeedCsvOptions>(Settings)
            {
                AdvancedSettings = new ExportFeedCsvOptions
                {
                    CsvEnconing = EncodingsEnum.Utf8.StrName(),
                    CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                    CsvColumSeparator = ";",
                    CsvPropertySeparator = ":",
                    CsvExportNoInCategory = _options.CsvExportNoInCategory,
                    CsvCategorySort = true,
                    FieldMapping = _options.FieldMapping,
                    ModuleFieldMapping = _options.ModuleFieldMapping
                }
            };
            CsvExport.Factory(products, csvOptions, productsCount, useCommonStatistic: UseCommonStatistic).Process();
        }

        public override void SetDefaultSettings()
        {
            var resellerCode = Guid.NewGuid();

            ExportFeedSettingsProvider.SetSettings<ExportFeedResellerOptions>(_exportFeedId,
                  new ExportFeedSettings<ExportFeedResellerOptions>()
                  {
                      Interval = 1,
                      IntervalType = Core.Scheduler.TimeIntervalType.Hours,
                      Active = false,
                      
                      FileName = "export/resellers/" + resellerCode,
                      FileExtention = "csv",

                      AdvancedSettings = new ExportFeedResellerOptions()
                      {
                          ResellerCode = resellerCode.ToString(),

                          CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                          CsvColumSeparator = ";",
                          CsvPropertySeparator = ":",
                          CsvEnconing = EncodingsEnum.Utf8.StrName(),
                          CsvCategorySort = true,

                          FieldMapping = new List<ProductFields>(Enum.GetValues(typeof(ProductFields)).OfType<ProductFields>().Where(item => item != ProductFields.None && item != ProductFields.YandexDeliveryDays).ToList()),
                      },
                      AdditionalUrlTags = string.Empty,
                      ExportAdult = true
                  });

            ExportFeedService.InsertCategory(_exportFeedId, 0, false);
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override object GetAdvancedSettings()
        {
            return ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedResellerOptions>(_exportFeedId);
        }

        public override string GetDownloadableExportFeedFileLink()
        {
            return SettingsMain.SiteUrl + "/api/resellers/catalog?id=" + _options.ResellerCode;
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return new List<string> { "csv" };
        }
    }
}