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
    public class ExportFeedSettingsCsvModel : IValidatableObject
    {
        public ExportFeedSettingsCsvModel(ExportFeedCsvOptions exportFeedCsvOptions)
        {
            CsvEnconing = exportFeedCsvOptions.CsvEnconing;
            CsvSeparator = exportFeedCsvOptions.CsvSeparator;
            CsvSeparatorCustom = exportFeedCsvOptions.CsvSeparatorCustom;
            CsvColumSeparator = exportFeedCsvOptions.CsvColumSeparator;
            CsvPropertySeparator = exportFeedCsvOptions.CsvPropertySeparator;
            CsvExportNoInCategory = exportFeedCsvOptions.CsvExportNoInCategory;
            CsvCategorySort = exportFeedCsvOptions.CsvCategorySort;
            AllOffersToMultiOfferColumn = exportFeedCsvOptions.AllOffersToMultiOfferColumn;
            FieldMapping = exportFeedCsvOptions.FieldMapping;
            ModuleFieldMapping = exportFeedCsvOptions.ModuleFieldMapping;
            StocksFromWarehouses = exportFeedCsvOptions.StocksFromWarehouses ?? new List<int>();

            CsvSeparatorList = Enum.GetValues(typeof(SeparatorsEnum)).Cast<SeparatorsEnum>()
                .Select(x => new SelectListItem { Value = x.ToString(), Text = x.Localize() }).ToList();
            CsvEnconingList = Enum.GetValues(typeof(EncodingsEnum)).Cast<EncodingsEnum>()
                .Select(x => new SelectListItem { Value = x.StrName(), Text = x.StrName() }).ToList();
            Warehouses =
                WarehouseService.GetList()
                                .Select(w => new SelectListItem {Value = w.Id.ToString(), Text = w.Name})
                                .ToList();
        }

        public string CsvEnconing { get; set; }
        public string CsvSeparator { get; set; }
        public string CsvSeparatorCustom { get; set; }
        public string CsvColumSeparator { get; set; }
        public string CsvPropertySeparator { get; set; }
        public bool CsvExportNoInCategory { get; set; }
        public bool CsvCategorySort { get; set; }
        public bool AllOffersToMultiOfferColumn { get; set; }
        public List<ProductFields> FieldMapping { get; set; }
        public List<CSVField> ModuleFieldMapping { get; set; }
        public List<int> StocksFromWarehouses { get; set; }
        
        public List<SelectListItem> CsvSeparatorList { get; set; }

        public List<SelectListItem> CsvEnconingList { get; set; }

        public List<SelectListItem> Warehouses { get; set; }


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
