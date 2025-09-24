using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Admin.Models.Landings;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetSiteTriggerEmails : AbstractCommandHandler<List<LandingTriggerModel>>
    {
        private readonly int _orderSourceId;
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public GetSiteTriggerEmails(int orderSourceId, DateTime dateFrom, DateTime dateTo)
        {
            _orderSourceId = orderSourceId;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        protected override List<LandingTriggerModel> Handle()
        {
            var triggers = new List<TriggerRule>();
            foreach (var objectType in new List<ETriggerObjectType> { ETriggerObjectType.Lead, ETriggerObjectType.Order })
            {
                switch (objectType)
                {
                    case ETriggerObjectType.Lead:
                        triggers.AddRange(TriggerRuleService.GetTriggersByObjectType(objectType).Where(
                            t => t.Actions.Any(a => a.ActionType == ETriggerActionType.Email) &&
                                t.Filter != null &&
                                (t.Filter as LeadFilter).Comparers.Any(c => c.FieldType == ELeadFieldType.Source && c.FieldComparer != null && c.FieldComparer.ValueObjId == _orderSourceId)));
                        break;
                    case ETriggerObjectType.Order:
                        triggers.AddRange(TriggerRuleService.GetTriggersByObjectType(objectType).Where(
                            t => t.Actions.Any(a => a.ActionType == ETriggerActionType.Email) &&
                                t.Filter != null &&
                                (t.Filter as OrderFilter).Comparers.Any(c => c.FieldType == EOrderFieldType.OrderSource && c.FieldComparer != null && c.FieldComparer.ValueObjId == _orderSourceId)));
                        break;
                }
            }

            var data = new List<LandingTriggerModel>();
            foreach (var trigger in triggers)
            {
                var model = new LandingTriggerModel
                {
                    Id = trigger.Id,
                    Name = trigger.Name,
                    EventTypeName = trigger.EventType.Localize(),
                    Emailings = new List<LandingTriggerEmailingModel>()
                };
                var emailingsData = (SettingsMail.UseAdvantshopMail 
                    ? AdvantShopMailService.GetTriggerAnalytics(trigger.Id, _dateFrom, _dateTo) 
                    : null) ?? new List<EmailingStatistics>();
                foreach (var action in trigger.Actions.Where(a => a.ActionType == ETriggerActionType.Email))
                {
                    var emailingModel = new LandingTriggerEmailingModel
                    {
                        EmailSubject = action.SendEmailData.EmailSubject,
                        EmailsCount = new Dictionary<EmailStatus, int>()
                    };
                    foreach (EmailStatus status in Enum.GetValues(typeof(EmailStatus)))
                    {
                        if (status == EmailStatus.None)
                            continue;
                        emailingModel.EmailsCount.Add(status, 0);

                        var emailingData = emailingsData.FirstOrDefault(x => x.EmailingId == action.EmailingId);
                        if (emailingData != null && emailingData.Statuses != null)
                        {
                            var sumStatuses = AdvantShopMailService.GetSumStatuses(status);
                            emailingModel.EmailsCount[status] = emailingData.Statuses
                                .Where(x => x.Status == status || sumStatuses.Contains(x.Status))
                                .Sum(statusData => statusData.Data.Sum(dailyData => dailyData.Count));
                        }
                    }

                    model.Emailings.Add(emailingModel);
                }
                data.Add(model);
            }

            return data;
        }
    }
}
