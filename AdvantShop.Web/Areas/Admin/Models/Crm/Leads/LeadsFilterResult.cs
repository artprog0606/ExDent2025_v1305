using System.Collections.Generic;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadsFilterResult : FilterResult<LeadsFilterResultModel>
    {
        public LeadsFilterResult()
        {
            LeadsCount = new Dictionary<int, int>();
        }

        public Dictionary<int, int> LeadsCount { get; set; }
    }
}
