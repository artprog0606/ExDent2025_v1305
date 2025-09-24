using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Export
{
    public class VkCurrentMarketExportSettings
    {
        public long OwnerId { get; set; }

        public string SiteUrl { get; set; }

        //public bool ExportOffers { get; set; }

        public bool ExportUnavailableProducts { get; set; }

        public bool ExportPreorderProducts { get; set; }

        public bool AddSizeAndColorInDescription { get; set; }

        public bool AddSizeAndColorInName { get; set; }

        public ShowDescriptionMode ShowDescription { get; set; }

        public bool ShowProperties { get; set; }

        public AddLinkToSiteMode AddLinkToSite { get; set; }

        public string TextBeforeLinkToSite { get; set; }

        public Currency Currency { get; set; }

        public bool ConsiderMinimalAmount { get; set; }

        public VkExportMode ExportMode { get; set; }

        public VkExportAmount ExportAmount { get; set; }
    }
}
