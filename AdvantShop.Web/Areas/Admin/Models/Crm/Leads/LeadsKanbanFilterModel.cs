using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadsKanbanFilterModel : KanbanFilterModel<LeadsKanbanColumnFilterModel>
    {
        public string Name { get; set; }
        public string LeadStatus { get; set; }
        public int SalesFunnelId { get; set; }
        public int? DealStatusId { get; set; }
        public string Manager { get; set; }
        public string CreatedDateFrom { get; set; }
        public string CreatedDateTo { get; set; }
        public float? SumFrom { get; set; }
        public float? SumTo { get; set; }
        public int? ManagerId { get; set; }
        public string CustomerId { get; set; }
        public int? OrderSourceId { get; set; }
        public string City { get; set; }
        public string Organization { get; set; }
        public string ProductNameArtNo { get; set; } 

        public Dictionary<string, CustomerFiledFilterModel> CustomerFields { get; set; }
        public Dictionary<string, CustomerFiledFilterModel> LeadFields { get; set; }

        public int? StatusId { get; set; }
    }

    public class LeadsKanbanColumnFilterModel : KanbanColumnFilterModel
    {
        public LeadsKanbanColumnFilterModel() : base() { }
        public LeadsKanbanColumnFilterModel(string id) : base(id) { }

        public int Type => Id.TryParseInt();
    }

}
