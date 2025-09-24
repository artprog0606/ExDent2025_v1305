using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class OrdersByHandler : AnalyticsBaseHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        private readonly int? _statusId;
        private readonly bool? _paied;

        private EGroupDateBy _groupBy;
        private readonly string _groupFormatString;


        public OrdersByHandler(DateTime dateFrom, DateTime dateTo, int? statusId, bool? paied, string groupFormatString)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _statusId = statusId;
            _paied = paied;
            _groupFormatString = groupFormatString ?? "dd";

            _groupBy = Filter(_groupFormatString);
        }

        public ChartDataJsonModel GetPayments()
        {
            var payments =
                OrderStatisticsService.GetPayments(_dateFrom, _dateTo, _paied, _statusId)
                    .OrderByDescending(x => x.Sum)
                    .ToList();

            var paymentStat = payments.Take(9).ToList();

            if (payments.Count >= 10)
                paymentStat.Add(new StatisticsDataItem()
                {
                    Name = "Другие",
                    Count = payments.Skip(9).Sum(x => x.Count),
                    Sum = payments.Skip(9).Sum(x => x.Sum)
                });

            if (paymentStat.Count == 0)
                return new ChartDataJsonModel()
                {
                    Data = new List<object>() { 0 },
                    Labels = new List<string>() { "n/a" },
                    Series = new List<string>() { "Методы оплаты" }
                };

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { paymentStat.Select(x => x.Sum) },
                Labels = paymentStat.Select(x => !string.IsNullOrWhiteSpace(x.Name) ? x.Name : "n/a").ToList(),
                Series = new List<string>() { "Методы оплаты" }
            };
        }

        public ChartDataJsonModel GetShippings(bool groupByName = false, DateTime? minDeliveryDate = null, DateTime? maxDeliveryDate = null)
        {
            var shippings = groupByName
                ? OrderStatisticsService.GetShippingsGroupedByName(_dateFrom, _dateTo, _paied, _statusId,
                        minDeliveryDate, maxDeliveryDate)
                    .OrderByDescending(x => x.Sum)
                    .ToList()
                : OrderStatisticsService
                    .GetShippings(_dateFrom, _dateTo, _paied, _statusId, minDeliveryDate, maxDeliveryDate)
                    .OrderByDescending(x => x.Sum)
                    .ToList();

            var shippingStat = shippings.Take(9).ToList();

            if (shippings.Count >= 10)
                shippingStat.Add(new StatisticsDataItem()
                {
                    Name = "Другие",
                    Count = shippings.Skip(9).Sum(x => x.Count),
                    Sum = shippings.Skip(9).Sum(x => x.Sum)
                });

            if (shippingStat.Count == 0)
                return new ChartDataJsonModel()
                {
                    Data = new List<object>() { 0 },
                    Labels = new List<string>() { "n/a" },
                    Series = new List<string>() { "Методы доставки" }
                };

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { shippingStat.Select(x => x.Sum) },
                Labels = shippingStat.Select(x => !string.IsNullOrWhiteSpace(x.Name) ? x.Name : "n/a").ToList(),
                Series = new List<string>() { "Методы доставки" }
            };
        }

        public ChartDataJsonModel GetStatuses()
        {
            var statuses =
                OrderStatisticsService.GetOrdersByStatus(_dateFrom, _dateTo, _paied, _statusId)
                    .OrderByDescending(x => x.Value)
                    .ToList();

            if (statuses.Count == 0)
                return new ChartDataJsonModel()
                {
                    Data = new List<object>() { 0 },
                    Labels = new List<string>() { "n/a" },
                    Series = new List<string>() { "Статусы заказов" }
                };

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { statuses.Select(x => x.Value) },
                Labels = statuses.Select(x => !string.IsNullOrWhiteSpace(x.Key) ? x.Key.Reduce(30) : "n/a").ToList(),
                Series = new List<string>() { "Статусы заказов" }
            };
        }

        public ChartDataJsonModel GetCities()
        {
            var cities =
                OrderStatisticsService.GetTopCities()
                    .OrderByDescending(x => x.Value)
                    .ToList();

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { cities.Select(x => x.Value) },
                Labels = cities.Select(x => !string.IsNullOrWhiteSpace(x.Key) ? x.Key.Reduce(30) : "n/a").ToList(),
                Series = new List<string>() { "Города" }
            };
        }

        public object GetOrderTypes()
        {
            var types =
                OrderStatisticsService.GetOrdersByOrderType(_dateFrom, _dateTo, _paied, _statusId)
                    .OrderByDescending(x => x.Value)
                    .ToList();

            if (types.Count == 0)
                return new ChartDataJsonModel()
                {
                    Data = new List<object>() { 0 },
                    Labels = new List<string>() { "n/a" },
                    Series = new List<string>() { "Способы оформления" }
                };
            
            return new ChartDataJsonModel()
            {
                Data = new List<object>() { types.Select(x => x.Value) },
                Labels = types.Select(x => !string.IsNullOrWhiteSpace(x.Key) ? x.Key.Reduce(30) : "n/a").ToList(),
                Series = new List<string>() { "Способы оформления" }
            };
        }

        public object GetRepeatOrders()
        {
            var list = 
                OrderStatisticsService.GetRepeatOrders(_groupFormatString, _dateFrom, _dateTo, _paied, _statusId);

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
                Series = new List<string>() { "Повторные заказы" }
            };
        }
    }
}