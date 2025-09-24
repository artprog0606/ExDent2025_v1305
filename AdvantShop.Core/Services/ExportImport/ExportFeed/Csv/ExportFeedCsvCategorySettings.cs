using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvCategorySettings
    {
        public static string CsvEnconing
        {
            get => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvEnconing"];
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvEnconing"] = value;
        }

        public static string CsvSeparator
        {
            get => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvSeparator"];
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvSeparator"] = value;
        }

        public static string CsvSeparatorCustom
        {
            get => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvSeparatorCustom"];
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvSeparatorCustom"] = value;
        }

        public static List<CategoryFields> FieldMapping
        {
            get
            {
                var data = SettingProvider.Items["ExportFeedCsvCategorySettings_FieldMapping"] ?? string.Empty;
                return JsonConvert.DeserializeObject<List<CategoryFields>>(data) ?? new List<CategoryFields>();
            }
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_FieldMapping"] = JsonConvert.SerializeObject(value);
        }

        public static string PropertySeparator
        {
            get
            {
                var value = SettingProvider.Items["ExportFeedCsvCategorySettings_PropertySeparator"];
                if (value.IsNullOrEmpty())
                    return ";";
                return value;
            }
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_PropertySeparator"] = value;
        }

        public static string NameSameProductProperty
        {
            get
            {
                var value = SettingProvider.Items["ExportFeedCsvCategorySettings_NameSameProductProperty"];
                if (value.IsNullOrEmpty())
                    return "SameProperty";
                return value;
            }
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_NameSameProductProperty"] = value;
        }

        public static string NameNotSameProductProperty
        {
            get
            {
                var value = SettingProvider.Items["ExportFeedCsvCategorySettings_NameNotSameProductProperty"];
                if (value.IsNullOrEmpty())
                    return "NotSameProperty";
                return value;
            }
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_NameNotSameProductProperty"] = value;
        }
    }

    public class ImportCsvCategorySettings
    {
        public static string CsvEnconing
        {
            get => SettingProvider.Items["ImportCsvCategorySettings_CsvEnconing"];
            set => SettingProvider.Items["ImportCsvCategorySettings_CsvEnconing"] = value;
        }

        public static string CsvSeparator
        {
            get => SettingProvider.Items["ImportCsvCategorySettings_CsvSeparator"];
            set => SettingProvider.Items["ExportFeedCsvCategorySettings_CsvSeparator"] = value;
        }
    }
}