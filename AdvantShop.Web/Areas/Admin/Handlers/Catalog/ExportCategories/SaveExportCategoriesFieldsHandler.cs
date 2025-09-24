using System;
using System.Collections.Generic;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportCategories;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportCategories
{
    public class SaveExportCategoriesFieldsHandler
    {

        private readonly ExportCategoriesSettingsModel _model;

        public SaveExportCategoriesFieldsHandler(ExportCategoriesSettingsModel model)
        {
            _model = model;
        }

        public bool Execute()
        {
            if (_model.ExportCategoriesFields == null)
                return false;

            var fieldMapping = new List<CategoryFields>();
            foreach (var field in _model.ExportCategoriesFields)
            {
                if (Enum.TryParse(field, out CategoryFields currentField))
                {
                    if (!fieldMapping.Contains(currentField) && currentField != CategoryFields.None)
                        fieldMapping.Add(currentField);
                }
            }

            if (fieldMapping.Count == 0 || string.IsNullOrEmpty(_model.CsvEncoding) || string.IsNullOrEmpty(_model.CsvSeparator))
            {
                return false;
            }

            ExportFeedCsvCategorySettings.CsvEnconing = _model.CsvEncoding;
            ExportFeedCsvCategorySettings.CsvSeparator = _model.CsvSeparator;
            ExportFeedCsvCategorySettings.CsvSeparatorCustom = _model.CsvSeparatorCustom;
            ExportFeedCsvCategorySettings.FieldMapping = fieldMapping;
            ExportFeedCsvCategorySettings.NameSameProductProperty = _model.NameSameProductProperty;
            ExportFeedCsvCategorySettings.NameNotSameProductProperty = _model.NameNotSameProductProperty;
            ExportFeedCsvCategorySettings.PropertySeparator = _model.PropertySeparator;

            return true;
        }
    }
}
