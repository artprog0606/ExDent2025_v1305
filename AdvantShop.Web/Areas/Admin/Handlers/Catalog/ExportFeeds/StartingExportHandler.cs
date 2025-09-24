using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class StartingExportHandler
    {
        private readonly int _exportFeedId;

        public StartingExportHandler(int exportFeedId)
        {
            _exportFeedId = exportFeedId;
        }

        public string Execute()
        {
            var exportFeed = ExportFeedService.GetExportFeed(_exportFeedId);

            return MakeExportFile(exportFeed);
        }

        private static string MakeExportFile(ExportFeed exportFeed)
        {
            var currentExportFeed = ExportFeedService.GetExportFeedInstance(exportFeed.Type, exportFeed.Id, true);
            var fileName = currentExportFeed.Export();

            if (exportFeed.FeedType != EExportFeedType.None)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_ExportManual, exportFeed.Type.ToString());

            return fileName;
        }
    }
}
