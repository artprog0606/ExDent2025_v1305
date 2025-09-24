using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadsAnalyticsModel
    {
        public int LeadsListId { get; set; }

        public int LeadsCount { get; set; }
        public int DealsInWorkCount { get; set; }
        public int DealsDoneCount { get; set; }
        public int DealsRejectedCount { get; set; }

        public float DealsInWorkPrice { get; set; }
        public float DealsDonePrice { get; set; }
        
        public List<string> LeadsSources { get; set; }
    }
}
