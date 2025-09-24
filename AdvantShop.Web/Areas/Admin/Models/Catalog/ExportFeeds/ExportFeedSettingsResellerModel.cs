using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsResellerModel : IValidatableObject
    {
        public ExportFeedSettingsResellerModel(ExportFeedResellerOptions options)
        {
            CsvEnconing = options.CsvEnconing;
            CsvSeparator = options.CsvSeparator;
            CsvColumSeparator = options.CsvColumSeparator;
            CsvPropertySeparator = options.CsvPropertySeparator;
            CsvExportNoInCategory = options.CsvExportNoInCategory;
            CsvCategorySort = options.CsvCategorySort;
            FieldMapping = options.FieldMapping;
            ModuleFieldMapping = options.ModuleFieldMapping;
            ResellerCode = options.ResellerCode;

            var recomendedPriceMarginTypeList = new Dictionary<EExportFeedResellerPriceMarginType, string>();
            foreach (EExportFeedResellerPriceMarginType recomendedPriceMarginType in Enum.GetValues(typeof(EExportFeedResellerPriceMarginType)))
            {
                recomendedPriceMarginTypeList.Add(recomendedPriceMarginType, recomendedPriceMarginType.Localize());
            }

            ExportNotAvailable = options.ExportNotAvailable;
            ExportFromMainCategories = options.ExportFromMainCategories;
            UnloadOnlyMainCategory = options.UnloadOnlyMainCategory ?? false;
            StocksFromWarehouses = options.StocksFromWarehouses ?? new List<int>();
        }

        public string CsvEnconing { get; set; }
        public string CsvSeparator { get; set; }
        public string CsvColumSeparator { get; set; }
        public string CsvPropertySeparator { get; set; }
        public bool CsvExportNoInCategory { get; set; }
        public bool CsvCategorySort { get; set; }

        public string ResellerCode { get; set; }


        public bool ExportNotAvailable { get; set; }
        public bool ExportFromMainCategories { get; set; }

        public List<ProductFields> FieldMapping { get; set; }
        public List<CSVField> ModuleFieldMapping { get; set; }

        public bool UnloadOnlyMainCategory { get; set; }
        public List<int> StocksFromWarehouses { get; set; }

        public Dictionary<string, string> CsvSeparatorList
        {
            get
            {
                var csvSeparatorList = new Dictionary<string, string>();
                foreach (SeparatorsEnum csvSeparator in Enum.GetValues(typeof(SeparatorsEnum)))
                {
                    csvSeparatorList.Add(csvSeparator.StrName(), csvSeparator.Localize());
                }
                return csvSeparatorList;
            }
        }

        public Dictionary<string, string> CsvEnconingList
        {
            get
            {
                var csvEnconingList = new Dictionary<string, string>();
                foreach (EncodingsEnum csvEnconing in Enum.GetValues(typeof(EncodingsEnum)))
                {
                    csvEnconingList.Add(csvEnconing.StrName(), csvEnconing.StrName());
                }
                return csvEnconingList;
            }
        }

        public List<SelectListItem> Warehouses =>
            WarehouseService.GetList()
                            .Select(w => new SelectListItem {Value = w.Id.ToString(), Text = w.Name})
                            .ToList();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(CsvColumSeparator))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CsvColumSeparator" });
            }
            if (string.IsNullOrEmpty(CsvPropertySeparator))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CsvPropertySeparator" });
            }
        }
    }
}