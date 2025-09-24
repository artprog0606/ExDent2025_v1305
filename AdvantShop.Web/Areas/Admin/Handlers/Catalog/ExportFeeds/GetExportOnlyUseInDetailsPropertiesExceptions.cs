using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetExportOnlyUseInDetailsPropertiesExceptions : ICommandHandler<ExportOnlyUseInDetailsPropertiesExceptionsModel>
    {
        private readonly int _exportFeedId;

        public GetExportOnlyUseInDetailsPropertiesExceptions(int exportFeedId)
        {
            _exportFeedId = exportFeedId;
        }

        public ExportOnlyUseInDetailsPropertiesExceptionsModel Execute()
        {
            var exceptionProperties = new List<SelectItemModel<int>>();

            var advancedSettings = ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedYandexOptions>(_exportFeedId);
            if (advancedSettings != null)
            {
                var ids = advancedSettings.ExportOnlyUseInDetailsPropertiesExceptionIds ?? new List<int>();
                if (ids.Count > 0)
                {
                    var feedProperties = SQLDataAccess
                        .Query<PropertyDto>("Select PropertyId, Name From Catalog.Property Where PropertyId in @ids",
                            new { ids })
                        .ToList(); 
                    
                    exceptionProperties = feedProperties.Select(x => new SelectItemModel<int>(x.Name, x.PropertyId)).ToList();
                }
            }

            return new ExportOnlyUseInDetailsPropertiesExceptionsModel() { ExceptionProperties = exceptionProperties };
        }
        
        private class PropertyDto
        {
            public int PropertyId { get; set; }
            public string Name { get; set; }
        }
    }
}