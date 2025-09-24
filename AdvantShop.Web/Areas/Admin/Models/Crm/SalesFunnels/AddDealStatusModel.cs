using AdvantShop.Core.Services.Crm.DealStatuses;

namespace AdvantShop.Web.Admin.Models.Crm.SalesFunnels
{
    public class AddDealStatusModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public SalesFunnelStatusType Status { get; set; }
    }
}
