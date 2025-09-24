using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportCategories;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportCategories
{
    public class GetExportCategoriesHandler
    {
        public ExportCategoriesModel Execute()
        {
            var csvEnconingList = new Dictionary<string, string>();

            foreach (EncodingsEnum enumItem in Enum.GetValues(typeof(EncodingsEnum)))
            {
                csvEnconingList.Add(enumItem.StrName(), enumItem.StrName());
            }

            var csvSeparatorList = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                csvSeparatorList.Add(enumItem.Localize(), enumItem.StrName());
            }

            var model = new ExportCategoriesModel
            {
                CsvEncoding = ExportFeedCsvCategorySettings.CsvEnconing ?? EncodingsEnum.Utf8.StrName(),
                CsvSeparator = string.IsNullOrEmpty(ExportFeedCsvCategorySettings.CsvSeparator)
                    ? SeparatorsEnum.SemicolonSeparated.StrName()
                    : ExportFeedCsvCategorySettings.CsvSeparator,
                CsvSeparatorCustom = ExportFeedCsvCategorySettings.CsvSeparatorCustom,
                FieldMapping = ExportFeedCsvCategorySettings.FieldMapping.Count == 0
                    ? Enum.GetNames(typeof(CategoryFields)).Select(item => item.TryParseEnum<CategoryFields>())
                        .Where(item => item != CategoryFields.None).ToList()
                    : ExportFeedCsvCategorySettings.FieldMapping,

                AllFields = GetAllFields(),
                DefaultExportFields = JsonConvert.SerializeObject(Enum.GetNames(typeof(CategoryFields))
                    .Where(item => item != CategoryFields.None.ToString()).ToList()),
                CsvSeparatorList = csvSeparatorList,
                CsvEnconingList = csvEnconingList,
                NameSameProductProperty = ExportFeedCsvCategorySettings.NameSameProductProperty,
                NameNotSameProductProperty = ExportFeedCsvCategorySettings.NameNotSameProductProperty,
                PropertySeparator = ExportFeedCsvCategorySettings.PropertySeparator
            };

            return model;
        }

        private Dictionary<string, string> GetAllFields()
        {
            var result = new Dictionary<string, string>();

            foreach (CategoryFields item in Enum.GetValues(typeof(CategoryFields)))
            {
                result.Add(item.ToString(), item.Localize());
            }
            return result;
        }
    }
}
