using System;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadsListAnalytics
    {
        private readonly int _leadsListId;

        public GetLeadsListAnalytics(int leadsListId)
        {
            _leadsListId = leadsListId;
        }

        public LeadsAnalyticsModel Execute()
        {
            var model = new LeadsAnalyticsModel
            {
                LeadsListId = _leadsListId,
                LeadsCount = LeadService.GetLeadsCountByListId(_leadsListId),
                DealsInWorkCount = LeadService.GetLeadsListDealsCount(_leadsListId, SalesFunnelStatusType.None),
                DealsDoneCount = LeadService.GetLeadsListDealsCount(_leadsListId, SalesFunnelStatusType.FinalSuccess),
                DealsRejectedCount = LeadService.GetLeadsListDealsCount(_leadsListId, SalesFunnelStatusType.Canceled),

                DealsInWorkPrice = LeadService.GetLeadsListDealsSum(_leadsListId, SalesFunnelStatusType.None),
                DealsDonePrice = LeadService.GetLeadsListDealsSum(_leadsListId, SalesFunnelStatusType.FinalSuccess)
            };

            return model;
        }
    }
}
