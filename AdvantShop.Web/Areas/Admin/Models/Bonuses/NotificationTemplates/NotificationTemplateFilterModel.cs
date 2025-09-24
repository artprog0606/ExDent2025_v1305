using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Bonuses.NotificationTemplates
{
    public class NotificationTemplateFilterModel : BaseFilterModel
    {
        public string Name { get; set; }

        public decimal BonusPercent { get; set; }

        public int SortOrder { get; set; }

        public decimal PurchaseBarrier { get; set; }
    }
}
