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
    public class GetExportFeedFields
    {
        private int _exportFeedId;
        private EExportFeedType _exportFeedType;

        public GetExportFeedFields(int exportFeedId, EExportFeedType exportFeedType)
        {
            _exportFeedId = exportFeedId;
            _exportFeedType = exportFeedType;
        }

        public ExportFeedFields Execute()
        {
            var allFields = GetAllFields();

            var defaultExportFields = Enum.GetNames(typeof(ProductFields)).Where(item => item != ProductFields.None.ToString()
                                    && item != ProductFields.Sorting.ToString() && item != ProductFields.YandexDeliveryDays.ToString()
                                    && item != ProductFields.ExternalCategoryId.ToString()).ToList();
            defaultExportFields.AddRange(GetModuleFields().Select(item => item.StrName));

            try
            {
                List<ProductFields> fieldMapping = null;
                List<CSVField> moduleFieldMapping = null;
                switch (_exportFeedType)
                {
                    case EExportFeedType.Csv:
                        var advancedSettingsCsv =
                            ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedCsvOptions>(_exportFeedId);
                        fieldMapping = advancedSettingsCsv.FieldMapping;
                        moduleFieldMapping = advancedSettingsCsv.ModuleFieldMapping;
                        break;
                    case EExportFeedType.Reseller:
                        var advancedSettingsReseller =
                            ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedCsvOptions>(_exportFeedId);
                        fieldMapping = advancedSettingsReseller.FieldMapping.Where(item => item != ProductFields.Sorting && item != ProductFields.ExternalCategoryId).ToList();
                        moduleFieldMapping = advancedSettingsReseller.ModuleFieldMapping;
                        break;
                }
                return new ExportFeedFields
                {
                    AllFields = allFields,
                    FieldMapping = fieldMapping,
                    ModuleFieldMapping = moduleFieldMapping,
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
            var result = new Dictionary<string, string>();

            foreach (ProductFields item in Enum.GetValues(typeof(ProductFields)))
            {
                if (item == ProductFields.Sorting || item == ProductFields.ExternalCategoryId)
                    continue;
                //result.Add(item.StrName(), item.ResourceKey());
                result.Add(item.ToString(), item.Localize());
            }

            foreach (var moduleField in GetModuleFields())
            {
                //if (result.ContainsKey(moduleField.StrName))
                //{
                //    continue;
                //}
                result.Add(moduleField.StrName, moduleField.DisplayName);
            }

            return result;
        }

        private List<string> GetExportFields(List<ProductFields> FieldMapping, List<CSVField> ModuleFieldMapping)
        {
            var result = new List<string>();

            foreach (ProductFields item in FieldMapping)
            {
                //result.Add(item.StrName());
                result.Add(item.ToString());
            }

            foreach (var moduleField in GetModuleFields())
            {
                result.Add(moduleField.StrName);
            }

            return result;
        }

        private List<CSVField> GetModuleFields()
        {
            var result = new List<CSVField>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                {
                    result.AddRange(classInstance.GetCSVFields());
                }
            }
            return result;
        }
    }
}
