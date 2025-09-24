using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class CrmSettingsModel
    {
        public bool CrmActive { get; set; }
        public int OrderStatusIdFromLead { get; set; }
        public int DefaultSalesFunnelId { get; set; }

        public ImportLeadsModel ImportLeadsModel { get; set; }
    }
}
