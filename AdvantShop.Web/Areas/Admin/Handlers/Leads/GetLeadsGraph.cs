using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Web.Admin.ViewModels.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadsGraph
    {
        private DateTime _dateTo;
        private DateTime _dateFrom;
        private const int _chartYearStep = 12;
        private readonly int _leadsListId;

        public GetLeadsGraph(int leadsListId, DateTime? dateFrom, DateTime? dateTo)
        {
            _dateTo = dateTo.HasValue ? dateTo.Value.AddDays(1).Date : DateTime.Now.AddDays(2).Date;
            _dateFrom = dateFrom.HasValue ? dateFrom.Value : DateTime.Now.AddDays(-30);

            _leadsListId = leadsListId;
        }

        public LeadsGraphViewModel Execute()
        {
            return new LeadsGraphViewModel()
            {
                Chart = GetDataByDays(),
                DateFrom = _dateFrom,
                DateTo = _dateTo
            };
        }

        private LeadsChartDataModel GetDataByDays()
        {
            var listLeads = LeadService.GetLeadsCountByDays(_dateFrom, _dateTo, _leadsListId);

            var chartData = new LeadsChartDataModel();

            var leadsCountsByDay = new List<string>();
            var labels = new List<string>();

            for (int i = 0; i < (_dateTo - _dateFrom).Days; i++)
            {
                var date = _dateFrom.AddDays(i);
                leadsCountsByDay.Add(listLeads.ContainsKey(date.Date) ? listLeads[date.Date].ToString() : "0");

                labels.Add(string.Format("{0}", date.ToString("d MMM")));
            }

            chartData.Data = new List<object> {
                leadsCountsByDay.Select(x => x)
            };

            chartData.Labels = labels;

            return chartData;
        }

    }
}
