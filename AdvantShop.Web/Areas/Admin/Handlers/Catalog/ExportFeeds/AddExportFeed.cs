using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class AddExportFeed : AbstractCommandHandler<object>
    {
        private readonly string _name;
        private readonly string _description;
        private readonly EExportFeedType _type;

        public AddExportFeed(string name, string description, EExportFeedType type)
        {
            _name = name;
            _description = description;
            _type = type;
        }

        protected override object Handle()
        {
            var exportFeedId = ExportFeedService.AddExportFeed(new ExportFeed(_type)
            {
                Name = _name,
                Description = _description
            });

            ExportFeedService.InsertCategory(exportFeedId, 0, false);

            var currentExportFeed = ExportFeedService.GetExportFeedInstance(_type.ToString(), exportFeedId);
            currentExportFeed.SetDefaultSettings();

            if (_type != EExportFeedType.None)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_ExportFeedCreated, _type.ToString());

            return new
            {
                id = exportFeedId,
                typeUrlPostfix = ExportFeedService.AllowShowExportFeedInCatalog(_type)
                    ? string.Empty
                    : _type.StrName()
            };
        }
    }
}