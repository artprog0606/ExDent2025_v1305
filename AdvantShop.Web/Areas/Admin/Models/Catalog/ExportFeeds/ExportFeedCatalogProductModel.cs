namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedCatalogProductModel : CatalogProductModelBySelector
    {
        public int ExportFeedId { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool ExcludeFromExport { get; set; }
    }
}