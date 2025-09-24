using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetExportFeedCsvFieldsV2
    {
        private int _exportFeedId;
        private ExportFeedCsvV2Options _advancedSettings;
        private List<CSVField> _moduleFields;

        public GetExportFeedCsvFieldsV2(int exportFeedId, ExportFeedCsvV2Options advancedSettings)
        {
            _exportFeedId = exportFeedId;
            _advancedSettings = advancedSettings;

            _moduleFields = new List<CSVField>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    _moduleFields.AddRange(classInstance.GetCSVFields());
            }
        }

        public ExportFeedCsvFieldsV2 Execute()
        {
            var allFields = GetAllFields();

            var defaultExportFields = Enum.GetValues(typeof(EProductField)).Cast<EProductField>()
                .Where(item => item != EProductField.None && item != EProductField.Sorting && item != EProductField.YandexDeliveryDays && item != EProductField.ExternalCategoryId)
                .Select(item => item.ToString()).ToList();
            defaultExportFields.AddRange(_moduleFields.Select(item => item.StrName));

            try
            {
                var settings = new ExportFeedSettingsCsvV2Model(_advancedSettings);
                return new ExportFeedCsvFieldsV2
                {
                    AllFields = allFields,
                    FieldMapping = settings.FieldMapping,
                    ModuleFieldMapping = settings.ModuleFieldMapping,
                    Id = _exportFeedId,
                    DefaultExportFields = JsonConvert.SerializeObject(defaultExportFields)
                };
            }
            catch
            {
                return null;
            }
        }

        private Dictionary<string, string> GetAllFields()
        {
            var result = Enum.GetValues(typeof(EProductField)).Cast<EProductField>()
                .Where(item => item != EProductField.Sorting && item != EProductField.ExternalCategoryId)
                .ToDictionary(item => item.ToString(), item => item.Localize());
            result.AddRange(_moduleFields.Select(moduleField => new KeyValuePair<string, string>(moduleField.StrName, moduleField.DisplayName)));

            return result;
        }
    }
}
