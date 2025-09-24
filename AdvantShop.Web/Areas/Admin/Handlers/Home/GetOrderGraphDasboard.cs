using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.ViewModels.Home;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetOrderGraphDashboard
    {
        private readonly DateTime _dateTo;
        private DateTime _nowDate;

        public GetOrderGraphDashboard()
        {
            _nowDate = DateTime.Now.Date;
            _dateTo = new DateTime(_nowDate.Year, _nowDate.Month, _nowDate.Day, 23, 59, 59);
        }

        public OrderGraphDasboardViewModel Execute()
        {
            return new OrderGraphDasboardViewModel()
            {
                ChartWeek = GetDataByDays(_nowDate.AddDays(-7)),
                ChartMonth = GetDataByDays(_nowDate.AddMonths(-1)),
                ChartYear = GetDataByMonths(_nowDate.AddYears(-1)),
            };
        }

        private OrdersChartDataModel GetDataByDays(DateTime dateFrom)
        {
            var listProfit = OrderStatisticsService.GetOrdersProfitByDays(dateFrom, _dateTo);
            var listSum = OrderStatisticsService.GetOrdersSumByDays(dateFrom, _dateTo);

            var chartData = new OrdersChartDataModel();

            var profitValues = new List<string>();
            var orderValues = new List<string>();
            var labels = new List<string>();

            for (int i = 0; i < (_dateTo - dateFrom).Days; i++)
            {
                var date = dateFrom.AddDays(i);
                profitValues.Add(listProfit.TryGetValue(date, out var value) ? value.ToString("F2").Replace(",", ".") : "0");
                orderValues.Add(listSum.TryGetValue(date, out var value1) ? value1.ToString("F2").Replace(",", ".") : "0");
                labels.Add($"'{date:d MMM}'");
            }
            chartData.Data = $"[[{profitValues.AggregateString(',')}],[{orderValues.AggregateString(',')}]]";
            chartData.Labels = $"[{labels.AggregateString(',')}]";

            return chartData;
        }

        private OrdersChartDataModel GetDataByMonths(DateTime dateFrom)
        {
            dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, 1);
            var listProfit = OrderStatisticsService.GetOrdersProfitByDays(dateFrom, _dateTo);
            var listSum = OrderStatisticsService.GetOrdersSumByDays(dateFrom, _dateTo);

            var chartData = new OrdersChartDataModel();

            var profitValues = new List<string>();
            var orderValues = new List<string>();
            var labels = new List<string>();
            
            while (dateFrom < _nowDate)
            {
                var dateRangeTo = dateFrom.AddMonths(1);
                if (dateRangeTo >= _nowDate)
                    dateRangeTo = _nowDate.AddDays(1);

                var profitAverage = listProfit.Where(x => x.Key >= dateFrom && x.Key < dateRangeTo).DefaultIfEmpty().Sum(x => x.Value);
                profitValues.Add(profitAverage.ToString("F2").Replace(",", "."));

                var ordersAverage = listSum.Where(x => x.Key >= dateFrom && x.Key < dateRangeTo).DefaultIfEmpty().Sum(x => x.Value);
                orderValues.Add(ordersAverage.ToString("F2").Replace(",", "."));

                labels.Add($"'{dateFrom:MMM}'");

                dateFrom = dateRangeTo;
            }
            chartData.Data = $"[[{profitValues.AggregateString(',')}],[{orderValues.AggregateString(',')}]]";
            chartData.Labels = $"[{labels.AggregateString(',')}]";

            return chartData;
        }
    }
}
