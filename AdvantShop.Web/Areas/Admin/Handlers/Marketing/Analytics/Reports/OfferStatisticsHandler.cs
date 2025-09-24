using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    //public class AvgChartDataJsonModel : ChartDataJsonModel
    //{
    //    public string AvgCheck { get; set; }
    //}

    public class OfferStatisticsHandler : AnalyticsBaseHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        private readonly int _offerId;
        private readonly bool? _paied;
        
        private EGroupDateBy _groupBy;
        private readonly string _groupFormatString;
        

        public OfferStatisticsHandler(DateTime dateFrom, DateTime dateTo, int offerId, bool? paied, string groupFormatString)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _offerId = offerId;
            _paied = paied;
            _groupFormatString = groupFormatString ?? "dd";

            _groupBy = Filter(_groupFormatString);
        }

        public ChartDataJsonModel GetSum()
        {
            var list = OrderStatisticsService.GetOfferSumStatistics(_groupFormatString, _dateFrom, _dateTo, _offerId, _paied);

            var data = new Dictionary<DateTime, float>();
            switch (_groupBy)
            {
                case EGroupDateBy.Day:
                    data = GetByDays(list, _dateFrom, _dateTo);
                    break;
                case EGroupDateBy.Week:
                    data = GetByWeeks(list, _dateFrom, _dateTo);
                    break;
                case EGroupDateBy.Month:
                    data = GetByMonths(list, _dateFrom, _dateTo);
                    break;
            }

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { data.Values.Select(x => x) },
                Labels = data.Keys.Select(x => x.ToString("d MMM")).ToList(),
                Series = new List<string>() { "Сумма" }
            };
        }

        public ChartDataJsonModel GetCount()
        {
            var list = OrderStatisticsService.GetOfferCountStatistics(_groupFormatString, _dateFrom, _dateTo, _offerId, _paied);

            var data = new Dictionary<DateTime, float>();
            switch (_groupBy)
            {
                case EGroupDateBy.Day:
                    data = GetByDays(list, _dateFrom, _dateTo);
                    break;
                case EGroupDateBy.Week:
                    data = GetByWeeks(list, _dateFrom, _dateTo);
                    break;
                case EGroupDateBy.Month:
                    data = GetByMonths(list, _dateFrom, _dateTo);
                    break;
            }

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { data.Values.Select(x => x) },
                Labels = data.Keys.Select(x => x.ToString("d MMM")).ToList(),
                Series = new List<string>() { "Средний чек" }
            };
        }
    }
}