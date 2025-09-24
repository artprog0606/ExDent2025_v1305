using System.Collections.Generic;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.ExportFeeds
{
    public class ExportFeedsModel
    {
        public List<ExportFeedModel> ExportFeeds { get; set; }

        public ExportFeedModel CurrentExportFeed { get; set; }

        public EExportFeedType? CurrentExportFeedsType { get; set; }
    }
}
