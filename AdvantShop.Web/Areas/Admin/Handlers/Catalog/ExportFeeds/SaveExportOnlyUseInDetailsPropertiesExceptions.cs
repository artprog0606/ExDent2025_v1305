using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class SaveExportOnlyUseInDetailsPropertiesExceptions : ICommandHandler<SaveExportOnlyUseInDetailsPropertiesExceptionsModel>
    {
        private readonly int _exportFeedId;
        private readonly List<SelectItemModel<int>> _exceptionProperties;

        public SaveExportOnlyUseInDetailsPropertiesExceptions(int exportFeedId, List<SelectItemModel<int>> exceptionProperties)
        {
            _exportFeedId = exportFeedId;
            _exceptionProperties = exceptionProperties;
        }

        public SaveExportOnlyUseInDetailsPropertiesExceptionsModel Execute()
        {
            var advancedSettings = ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedYandexOptions>(_exportFeedId);
            if (advancedSettings == null)
                throw new BlException("Экспорт не найден");
            
            advancedSettings.ExportOnlyUseInDetailsPropertiesExceptionIds =
                _exceptionProperties == null || _exceptionProperties.Count == 0
                    ? null
                    : _exceptionProperties.Select(x => x.value)
                        .Where(x => PropertyService.GetPropertyById(x) != null)
                        .ToList();
            
            ExportFeedSettingsProvider.SetAdvancedSettings(_exportFeedId, advancedSettings);

            var names = advancedSettings.ExportOnlyUseInDetailsPropertiesExceptionIds != null
                ? advancedSettings.ExportOnlyUseInDetailsPropertiesExceptionIds
                    .Select(PropertyService.GetPropertyById)
                    .Where(x => x != null)
                    .Select(x => x.Name)
                    .ToList()
                : null;

            return new SaveExportOnlyUseInDetailsPropertiesExceptionsModel()
            {
                Names = names != null ? string.Join(", ", names) : "",
                Ids = advancedSettings.ExportOnlyUseInDetailsPropertiesExceptionIds
            };
        }
    }

    public class SaveExportOnlyUseInDetailsPropertiesExceptionsModel
    {
        public string Names { get; set; }
        public List<int> Ids { get; set; }
    }
}