using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.SalesFunnels;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class CrmNavMenuModel
    {
        public string Selected { get; set; }
        public List<SalesFunnel> SaleFunnels { get; set; }
        public bool IsFullAccess { get; set; }
        public int? ExcludeLeadListId { get; set; }
    }
}
