namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedCatalogFilterModel : CatalogFilterModel
    {
        public int ExportFeedId { get; set; }

        public bool? ExcludeFromExport { get; set; }
    }
}
