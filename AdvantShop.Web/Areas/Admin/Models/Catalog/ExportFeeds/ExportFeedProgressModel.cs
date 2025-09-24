using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedProgressModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public EExportFeedType Type { get; set; }
        

        public int TotalRows { get; set; }
    }
}
