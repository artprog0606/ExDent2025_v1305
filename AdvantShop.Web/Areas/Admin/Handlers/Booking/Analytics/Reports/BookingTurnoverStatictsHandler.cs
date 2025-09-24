using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports
{
    public class BookingTurnoverStatictsHandler : AnalyticsBaseHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        private readonly bool? _isPaid;
        private readonly BookingStatus? _status;
        private readonly int? _affiliateId;
        private readonly bool _showByAccess;
        private readonly string _groupFormatString;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;
        private EGroupDateBy _groupBy;


        public BookingTurnoverStatictsHandler(DateTime dateFrom, DateTime dateTo, bool? isPaid, BookingStatus? status, string groupFormatString, int? affiliateId, bool showByAccess = true)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _isPaid = isPaid;
            _status = status;
            _affiliateId = affiliateId;
            _showByAccess = showByAccess;
            _groupFormatString = groupFormatString ?? "dd";

            _groupBy = Filter(_groupFormatString);
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public ChartDataJsonModel GetOrdersSum()
        {
            var bookingStatuses = Enum.GetValues(typeof(BookingStatus)).Cast<BookingStatus>().Where(x => !_status.HasValue || x == _status.Value).ToDictionary(x => x, x => new Dictionary<DateTime, float>());

            var list = BookingStatisticsService.GetBookingsSum(_groupFormatString, _dateFrom, _dateTo, _isPaid, _status, _affiliateId, _showByAccess && !_currentCustomer.IsAdmin && _currentManager != null ? _currentManager.ManagerId : (int?)null);

            foreach (var tuple in list)
                bookingStatuses[tuple.Item2].Add(tuple.Item1, tuple.Item3);

            var data = Enum.GetValues(typeof(BookingStatus)).Cast<BookingStatus>().Where(x => !_status.HasValue || x == _status.Value).ToDictionary(x => x, x => new Dictionary<DateTime, float>());

            foreach (var bookingStatusData in bookingStatuses)
            {
                switch (_groupBy)
                {
                    case EGroupDateBy.Day:
                        data[bookingStatusData.Key] = GetByDays(bookingStatusData.Value, _dateFrom, _dateTo);
                        break;
                    case EGroupDateBy.Week:
                        data[bookingStatusData.Key] = GetByWeeks(bookingStatusData.Value, _dateFrom, _dateTo);
                        break;
                    case EGroupDateBy.Month:
                        data[bookingStatusData.Key] = GetByMonths(bookingStatusData.Value, _dateFrom, _dateTo);
                        break;
                }
            }

            var model = new ChartDataJsonModel()
            {
                Data = data.Values.Select(val => (object)val.Select(x => x.Value)).ToList(),
                Labels = data.First().Value.Keys.Select(x => x.ToString("d MMM")).ToList(),
                Series = data.Keys.Select(GetStatusName).ToList(),/*new List<string>() { "Брони" }*/
                Colors = data.Keys.Select(GetStatusColor).ToList()
            };


            if (!_status.HasValue)
            {
                var sum = new SortedDictionary<DateTime, float>();

                foreach (var status in data.Keys.Where(x => x != BookingStatus.Cancel))
                foreach (var kv in data[status])
                    if (!sum.ContainsKey(kv.Key))
                        sum.Add(kv.Key, kv.Value);
                    else
                        sum[kv.Key] += kv.Value;

                model.Data.Insert(0, sum.Values.Select(x => x));
                model.Series.Insert(0, "Оборот без отмененных");
                model.Colors.Insert(0, "#ff9900");
            }
            else
            {
                model.Data.Insert(0, new float[]{});
                model.Series.Insert(0, null);
                model.Colors.Insert(0, "#ff9900");
            }

            return model;
        }

        public ChartDataJsonModel GetOrdersCount()
        {
            var bookingStatuses = Enum.GetValues(typeof(BookingStatus)).Cast<BookingStatus>().Where(x => !_status.HasValue || x == _status.Value).ToDictionary(x => x, x => new Dictionary<DateTime, float>());

            var list = BookingStatisticsService.GetBookingsCount(_groupFormatString, _dateFrom, _dateTo, _isPaid, _status, _affiliateId, _showByAccess && !_currentCustomer.IsAdmin && _currentManager != null ? _currentManager.ManagerId : (int?)null);

            foreach (var tuple in list)
                bookingStatuses[tuple.Item2].Add(tuple.Item1, tuple.Item3);

            var data = Enum.GetValues(typeof(BookingStatus)).Cast<BookingStatus>().Where(x => !_status.HasValue || x == _status.Value).ToDictionary(x => x, x => new Dictionary<DateTime, float>());

            foreach (var bookingStatusData in bookingStatuses)
            {
                switch (_groupBy)
                {
                    case EGroupDateBy.Day:
                        data[bookingStatusData.Key] = GetByDays(bookingStatusData.Value, _dateFrom, _dateTo);
                        break;
                    case EGroupDateBy.Week:
                        data[bookingStatusData.Key] = GetByWeeks(bookingStatusData.Value, _dateFrom, _dateTo);
                        break;
                    case EGroupDateBy.Month:
                        data[bookingStatusData.Key] = GetByMonths(bookingStatusData.Value, _dateFrom, _dateTo);
                        break;
                }
            }

            var model = new ChartDataJsonModel()
            {
                Data = data.Values.Select(val => (object)val.Select(x => x.Value)).ToList(),
                Labels = data.First().Value.Keys.Select(x => x.ToString("d MMM")).ToList(),
                Series = data.Keys.Select(GetStatusName).ToList(),/*new List<string>() { "Количество броней" }*/
                Colors = data.Keys.Select(GetStatusColor).ToList()
            };


            if (!_status.HasValue)
            {
                var sum = new SortedDictionary<DateTime, float>();

                foreach (var status in data.Keys.Where(x => x != BookingStatus.Cancel))
                foreach (var kv in data[status])
                    if (!sum.ContainsKey(kv.Key))
                        sum.Add(kv.Key, kv.Value);
                    else
                        sum[kv.Key] += kv.Value;

                model.Data.Insert(0, sum.Values.Select(x => x));
                model.Series.Insert(0, "Количество без отмененных");
                model.Colors.Insert(0, "#ff9900");
            }
            else
            {
                model.Data.Insert(0, new float[] { });
                model.Series.Insert(0, null);
                model.Colors.Insert(0, "#ff9900");
            }

            return model;
        }

        private string GetStatusName(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.New:
                    return "Новые";
                case BookingStatus.Confirmed:
                    return "Подтвержденные";
                case BookingStatus.Completed:
                    return "Завершенные";
                case BookingStatus.Cancel:
                    return "Отмененные";
                default:
                    return status.Localize();
            }
        }

        private string GetStatusColor(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.New:
                    return "#cc6600";
                case BookingStatus.Confirmed:
                    return "#008000";
                case BookingStatus.Completed:
                    return "#0066cc";
                case BookingStatus.Cancel:
                    return "#cccccc";
                default:
                    return "#000000";
            }
        }
    }
}
