using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetExportFeedModelHandler
    {
        private readonly int _id;

        public GetExportFeedModelHandler(int id)
        {
            _id = id;
        }

        public ExportFeedModel Execute()
        {
            var exportFeed = ExportFeedService.GetExportFeed(_id);
            if (exportFeed == null)
                return null;

            var model = new ExportFeedModel
            {
                Id = exportFeed.Id,
                Name = exportFeed.Name,
                Description = exportFeed.Description,
                LastExport = exportFeed.LastExport,
                LastExportFileFullName = exportFeed.LastExportFileFullName,
                Type = exportFeed.FeedType
            };

            var handler = new GetExportFeedSettings(ExportFeedSettingsProvider.GetSettings(model.Id), exportFeed.FeedType);
            model.ExportFeedSettings = handler.Execute();
            model.ExportAllProducts = ExportFeedService.IsExportAllCategories(model.Id);
            model.ExportCatalogType = ExportFeedService.GetExportFeedCatalogType(model.Id);

            return model;
        }
    }
}
