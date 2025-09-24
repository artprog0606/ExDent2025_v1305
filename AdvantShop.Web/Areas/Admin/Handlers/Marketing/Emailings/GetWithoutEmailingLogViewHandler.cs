using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Mails;
using AdvantShop.Web.Admin.ViewModels.Emailings;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetWithoutEmailingLogViewHandler
    {
        private int? _formatId;
        private UrlHelper _urlHelper;
        private string _datefrom;
        private string _dateto;
        private string _statuses;

        public GetWithoutEmailingLogViewHandler(int? formatId, string datefrom, string dateto, string statuses, UrlHelper urlHelper)
        {
            _formatId = formatId;
            _urlHelper = urlHelper;
            _datefrom = datefrom;
            _dateto = dateto;
            _statuses = statuses;
        }

        public EmailingLogViewModel Execute()
        {
            var model = new EmailingLogViewModel
            {
                //EmailingId = _emailingId,
                FormatId = _formatId,
                BreadCrumbs = new List<KeyValuePair<string, string>>()
            };

            var dto = new GetWithoutEmailLogsDto();
            dto.FormatId = _formatId;
            dto.Page = 1;
            dto.ItemsPerPage = 10;
            var manualEmailing = AdvantShopMailService.GetWithoutEmailing(dto);
            if (manualEmailing != null)
            {
                var types = MailFormatService.GetMailFormatTypes();

                var emailSubject = "";

                if (dto.FormatId.HasValue)
                {
                    var t = types.FirstOrDefault(x => x.MailFormatTypeId == dto.FormatId.Value);
                    emailSubject = t != null ? t.TypeName : "не указан тип";
                }
                else
                {
                    emailSubject = "не указан тип";
                }


                model.BreadCrumbs.AddRange(new[] {
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("ManualEmailings", "Emailings"), LocalizationService.GetResource("Admin.ManualEmailings.Title")),
                    new KeyValuePair<string, string>(_urlHelper.AbsoluteActionUrl("ManualWithoutEmailing", "Emailings", new { id = _formatId }), LocalizationService.GetResource("Admin.Analytics.EmailingAnalytics")),
                    new KeyValuePair<string, string>(string.Empty, LocalizationService.GetResource("Admin.EmailingLog.Title"))
                });
                model.EmailSubject = emailSubject;
                model.BackUrl = _urlHelper.AbsoluteActionUrl("ManualWithoutEmailing", "Emailings", new { id = _formatId });
                model.DateFrom = _datefrom;
                model.DateTo = _dateto;
                model.Statuses = _statuses;
                return model;
            }

            return null;
        }
    }
}