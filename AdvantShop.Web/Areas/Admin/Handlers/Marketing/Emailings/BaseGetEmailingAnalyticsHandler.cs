using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public abstract class BaseGetEmailingAnalyticsHandler<TResult> : ICommandHandler<TResult>
    {
        public abstract TResult Execute();

        protected EmailingAnalyticsModel GetModel(EmailingStatistics statistics)
        {
            var model = new EmailingAnalyticsModel();
            if (statistics == null) return model;
            if (!statistics.Statuses.Any() || !statistics.Statuses.SelectMany(x => x.Data).Any())
                return model;

            var allData = statistics.Statuses.SelectMany(x => x.Data).ToList();
            var dateFrom = allData.Min(x => x.Date);
            var dateTo = allData.Max(x => x.Date);

            // добавление отсутствующих статусов
            foreach (EmailStatus status in Enum.GetValues(typeof(EmailStatus)))
            {
                if (status == EmailStatus.None)
                    continue;
                if (!statistics.Statuses.Any(x => x.Status == status))
                    statistics.Statuses.Add(new EmailStatusStatistics { StatusStr = status.ToString() });
            }

            // добавление нулевых точек для пропущенных дат
            foreach (var statusData in statistics.Statuses)
            {
                var date = dateFrom;
                while (date <= dateTo)
                {
                    if (!statusData.Data.Any(x => x.Date == date))
                    {
                        statusData.Data.Add(new EmailDailyStatistics { Date = date });
                    }
                    date = date.AddDays(1);
                }
                statusData.Data = statusData.Data.OrderBy(x => x.Date).ToList();
            }
            
            // подсчет значений по статусам
            foreach (EmailStatus status in new List<EmailStatus> { EmailStatus.Sent, EmailStatus.Delivered, EmailStatus.Opened })
            {
                var statusData = statistics.Statuses.First(x => x.Status == status);
                foreach (var data in statusData.Data)
                {
                    var sumStatuses = AdvantShopMailService.GetSumStatuses(statusData.Status);
                    data.Count += statistics.Statuses.Where(x => sumStatuses.Contains(x.Status)).SelectMany(x => x.Data.Where(y => y.Date == data.Date)).ToList().Sum(x => x.Count);
                }
            }

            statistics.Statuses.RemoveAll(x => x.Data.All(y => y.Count == 0));

            // сортировка статусов по порядку в enum
            statistics.Statuses = statistics.Statuses.OrderBy(x => (int)x.Status).ToList();

            var chartData = new ChartDataJsonModel
            {
                Series = statistics.Statuses.Select(x => x.StatusName).ToList(),
                Labels = statistics.Statuses[0].Data.Select(x => x.Date.ToString("d MMM")).ToList(),
                Data = statistics.Statuses.Select(s => (object)s.Data.Select(d => d.Count).ToList()).ToList()
            };
            model.ChartData = chartData;

            model.StatusesData = new List<EmailStatusAnalyticsModel>();
            foreach (EmailStatus status in Enum.GetValues(typeof(EmailStatus)))
            {
                if (status == EmailStatus.None)
                    continue;
                var statusData = statistics.Statuses.FirstOrDefault(x => x.Status == status) ?? new EmailStatusStatistics();
                var count = statusData.Data.Sum(x => x.Count);
                model.StatusesData.Add(new EmailStatusAnalyticsModel
                {
                    Status = status.ToString(),
                    StatusName = status.Localize(),
                    Count = count,
                    Percent = (int)Math.Round((decimal)count / statistics.Count * 100, 0)
                });
            }

            return model;
        }
    }
}
