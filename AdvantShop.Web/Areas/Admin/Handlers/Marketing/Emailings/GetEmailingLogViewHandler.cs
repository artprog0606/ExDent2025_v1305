using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Admin.ViewModels.Emailings;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetEmailingLogViewHandler
    {
        private Guid _emailingId;
        private UrlHelper _urlHelper;
        private string _statuses;

        public GetEmailingLogViewHandler(Guid emailingId, string statuses, UrlHelper urlHelper)
        {
            _emailingId = emailingId;
            _urlHelper = urlHelper;
            _statuses = statuses;
        }

        public EmailingLogViewModel Execute()
        {
            var model = new EmailingLogViewModel
            {
                EmailingId = _emailingId,
                BreadCrumbs = new List<KeyValuePair<string, string>>()
            };
            var triggerAction = TriggerActionService.GetTriggerAction(_emailingId);
            if (triggerAction != null)
            {
                var trigger = TriggerRuleService.GetTrigger(triggerAction.TriggerRuleId);
                model.BreadCrumbs.AddRange(new[] {
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("Index", "Triggers"), LocalizationService.GetResource("Admin.Triggers.Index.Title")),
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("Edit", "Triggers", new { id = trigger.Id }), string.Format("Триггер {0}", trigger.Name)),
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("TriggerEmailings", "Emailings", new { id = trigger.Id }), LocalizationService.GetResource("Admin.Analytics.EmailingAnalytics")),
                    new KeyValuePair<string, string>(string.Empty, LocalizationService.GetResource("Admin.EmailingLog.Title"))
                });
                model.EmailSubject = triggerAction.SendEmailData.EmailSubject;
                model.TriggerId = triggerAction.TriggerRuleId;
                model.BackUrl = _urlHelper.AbsoluteActionUrl("TriggerEmailings", "Emailings", new { id = trigger.Id });
                model.Statuses = _statuses;
                return model;
            }
            var manualEmailing = ManualEmailingService.GetManualEmailing(_emailingId);
            if (manualEmailing != null)
            {
                model.BreadCrumbs.AddRange(new[] {
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("ManualEmailings", "Emailings"), LocalizationService.GetResource("Admin.ManualEmailings.Title")),
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("ManualEmailing", "Emailings", new { id = _emailingId }), LocalizationService.GetResource("Admin.Analytics.EmailingAnalytics")),
                    new KeyValuePair<string, string>(string.Empty, LocalizationService.GetResource("Admin.EmailingLog.Title"))
                });
                model.EmailSubject = manualEmailing.Subject;
                model.BackUrl = _urlHelper.AbsoluteActionUrl("ManualEmailing", "Emailings", new { id = _emailingId });
                model.Statuses = _statuses;
                return model;
            }

            return null;
        }
    }
}
