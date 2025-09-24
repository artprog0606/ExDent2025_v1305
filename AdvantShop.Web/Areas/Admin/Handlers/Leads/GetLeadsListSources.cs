using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Crm;

namespace AdvantShop.Web.Admin.Handlers.Leads
{

    public class GetLeadsListSources
    {
        private readonly int _leadsListId;

        public GetLeadsListSources(int leadsListId)
        {
            _leadsListId = leadsListId;
        }

        public List<LeadsListSourceItem> Execute()
        {

            var totalCountLeadsInList = LeadService.GetLeadsCountByListId(_leadsListId);

            var model = new List<LeadsListSourceItem>();
            foreach (var source in LeadService.GetLeadsListSources(_leadsListId))
            {
                model.Add(new LeadsListSourceItem
                {
                    OrderSourceId = source.OrderSourceId,
                    Name = source.Name,
                    LeadsCount = source.LeadsCount,
                    PercentLeads = totalCountLeadsInList > 0 ? Convert.ToInt32(source.LeadsCount / ( totalCountLeadsInList / 100f)) : 0
                });
            }

            return model;
        }
    }
}
