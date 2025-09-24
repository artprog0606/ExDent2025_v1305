using System.Collections.Generic;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadsFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public string LeadStatus { get; set; }
        public int SalesFunnelId { get; set; }
        public int? DealStatusId { get; set; }
        public string CreatedDateFrom { get; set; }
        public string CreatedDateTo { get; set; }
        public float? SumFrom { get; set; }
        public float? SumTo { get; set; }
        public int? ManagerId { get; set; }
        public string CustomerId { get; set; }
        public int? OrderSourceId { get; set; }
        public int? FunnelId { get; set; }
        public string Organization { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string ProductNameArtNo { get; set; }

        public Dictionary<string, CustomerFiledFilterModel> CustomerFields { get; set; }
        public Dictionary<string, CustomerFiledFilterModel> LeadFields { get; set; }
        
        public bool? CreateOrderOnFinalSuccess { get; set; }
    }
}
