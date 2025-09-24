using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.ViewModels.Crm.Leads
{
    public class LeadsListViewModel
    {
        public List<DealStatusWithCount> DealStatuses
        {
            get
            {
                return DealStatusService.GetListWithCount(SalesFunnelId);
            }
        }

        public bool UseKanban { get; set; }
        public int SalesFunnelId { get; set; }
        public string SalesFunneName { get; set; }

        public bool IsFullAccess { get; set; }
    }
}
