using System.Collections.Generic;
using System.Linq;
using AdvantShop.ExportImport;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using AdvantShop.Web.Admin.ViewModels.Catalog.ExportFeeds;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetExportFeedsHandler
    {
        private readonly List<EExportFeedType> _exportFeedTypes;

        public GetExportFeedsHandler(EExportFeedType exportFeedType, params EExportFeedType[] additionalTypes)
        {
            _exportFeedTypes = new List<EExportFeedType> { exportFeedType };
            if (additionalTypes != null)
                _exportFeedTypes.AddRange(additionalTypes);
        }

        public ExportFeedsModel Execute()
        {
            var exportFeeds = new List<ExportFeed>();
            foreach (var type in _exportFeedTypes)
            {
                if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveExportFeeds && (type == EExportFeedType.YandexMarket || type == EExportFeedType.GoogleMerchentCenter || type == EExportFeedType.YandexDirect || type == EExportFeedType.YandexWebmaster))
                    continue;
                exportFeeds.AddRange(ExportFeedService.GetExportFeeds(type));
            }

            var model = new ExportFeedsModel
            {
                CurrentExportFeedsType = _exportFeedTypes[0],
                ExportFeeds = exportFeeds
                    .OrderBy(x => x.Id) // в порядке добавления
                    .Select(exportFeed => new ExportFeedModel
                    {
                        Id = exportFeed.Id,
                        Name = exportFeed.Name,
                        Description = exportFeed.Description,
                        LastExport = exportFeed.LastExport,
                        LastExportFileFullName = exportFeed.LastExportFileFullName,
                        Type = exportFeed.FeedType
                    }).ToList()
            };

            return model;
        }
    }
}
