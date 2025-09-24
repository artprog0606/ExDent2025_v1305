using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Export
{
    public class OkCurrentMarketExportSettings
    {
        public Currency Currency { get; set; }

        public string GroupId { get; set; }

        public string SiteUrl { get; set; }

        public bool IsBusinessGroup { get; set; }

        public bool ExportUnavailableProducts { get; set; }

        public bool SizeAndColorInName { get; set; }

        public bool UpdateProductPhotos { get; set; }

        public bool SizeAndColorInDescription { get; set; }

        public bool ExportProperties { get; set; }

        public OkMarketAddLinkToSiteMode ExportLinkToSite { get; set; }

        public OkMarketShowDescriptionMode ExportDescription { get; set; }
    }
}