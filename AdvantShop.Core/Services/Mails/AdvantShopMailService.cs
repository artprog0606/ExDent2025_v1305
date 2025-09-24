using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Mails
{
    public class EmailSendDto
    {
        public Guid CustomerShopId { get; set; }
        public string LicKey { get; set; }
        public List<string> Emails { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public int EmailTotalCount { get; set; }
        public Guid? EmailingId { get; set; }
        public int? FormatId { get; set; }
    }

    public class EmailRegistDto
    {
        public string LicKey { get; set; }
        public string Email { get; set; }
        public string FromName { get; set; }
    }

    public class EmailLastDto
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public EmailStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class AdvantShopMailService
    {
        private static string urlHost = SettingsLic.EmailServiceUrl;
        private const string lic = "lic";
        private static Dictionary<string, string> headers = new Dictionary<string, string> { { lic, SettingsLic.LicKey } };

        public static string Send(Guid customerShopId, List<string> strTo, string strSubject, string strText,
                                  bool isBodyHtml, string replyTo, int lettercount,
                                  Guid? emailingId, int? emailFormatId)
        {
            if (string.IsNullOrEmpty(CapShopSettings.FromEmail))
                throw new BlException("Поле E-mail отправителя отправителя не заполнено. Измените настройки почты.");

            if (CapShopSettings.ConfirmDate == null)
                throw new BlException("E-mail отправителя не подтверджен. Пожалуйста подтвердите E-mail в настройках почты.");

            if (string.IsNullOrEmpty(strSubject))
                throw new BlException("Укажите тему письма");

            if (string.IsNullOrEmpty(strText))
                throw new BlException("Укажите текст письма");

            var urlAction = "v1/email/send";

            var request = new EmailSendDto
            {
                LicKey = SettingsLic.LicKey,
                CustomerShopId = customerShopId,
                Emails = strTo,
                Message = strText,
                Subject = strSubject,
                EmailTotalCount = lettercount,
                EmailingId = emailingId,
                FormatId = emailFormatId
            };
            var result = RequestHelper.MakeRequest<string>(urlHost + urlAction, request, headers);
            return result;
        }

        public static string Send(Guid customerShopId, string strTo, string strSubject, string strText, bool isBodyHtml, string replyTo, int lettercount, Guid? emailingId, int? emailFormatId)
        {
            if (strTo.Contains(";") || strTo.Contains(","))
            {
                var temp = strTo.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                var trimtemp = temp.Select(x => x.Trim()).Where(mail => ValidationHelper.IsValidEmail(mail)).Distinct().ToList();

                return Send(customerShopId, trimtemp, strSubject, strText, isBodyHtml, replyTo, lettercount, emailingId, emailFormatId);
            }
            return Send(customerShopId, new List<string> { strTo }, strSubject, strText, isBodyHtml, replyTo, lettercount, emailingId, emailFormatId);
        }

        public static string SaveSender(string fromEmail, string fromName)
        {
            var urlAction = "v1/email/savesender";

            var request = new EmailRegistDto();
            request.LicKey = SettingsLic.LicKey;
            request.Email = fromEmail;
            request.FromName = fromName;
            var result = RequestHelper.MakeRequest<string>(urlHost + urlAction, request, headers);
            return result;
        }

        public static List<EmailLogItem> GetLast(Guid customerId)
        {
            var urlAction = "v1/email/last";

            var result = RequestHelper.MakeRequest<List<EmailLastDto>>(urlHost + urlAction, new { lickey = SettingsLic.LicKey, customerid = customerId, count = 100 }, headers);
            var temp = result.Select(x => new EmailLogItem
            {
                Body = x.Message,
                EmailAddress = x.Email,
                Subject = x.Subject,
                Status = x.Status,
                CreateOn = x.Created,
                Updated = x.Updated
            }).ToList();
            return temp;
        }

        public static bool SendValidate()
        {
            var urlAction = "v1/email/sendvalidate/" + SettingsLic.LicKey;

            var result = RequestHelper.MakeRequest<bool>(urlHost + urlAction, SettingsLic.LicKey, headers);
            return result;
        }

        public static string Subscribe(string email)
        {
            var urlAction = "v1/email/subscribe/";
            var request = new
            {
                LicKey = SettingsLic.LicKey,
                Email = email
            };
            var result = RequestHelper.MakeRequest<string>(urlHost + urlAction, request, headers);
            return result;
        }

        #region Analytics

        public static List<EmailingStatistics> GetTriggerAnalytics(int triggerId, DateTime dateFrom, DateTime dateTo)
        {
            var emailingIds =
                Triggers.TriggerActionService.GetTriggerActions(triggerId)
                    .Where(x => x.ActionType == Triggers.ETriggerActionType.Email)
                    .Select(x => x.EmailingId)
                    .ToList();

            return GetEmailingAnalytics(emailingIds, dateFrom, dateTo);
        }

        public static EmailingStatistics GetEmailingAnalytics(Guid emailingId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var result = GetEmailingAnalytics(new List<Guid> { emailingId }, dateFrom, dateTo) ?? new List<EmailingStatistics>();
            return result.FirstOrDefault();
        }

        public static List<EmailingStatistics> GetEmailingAnalytics(List<Guid> emailingIds, DateTime? dateFrom, DateTime? dateTo)
        {
            var urlAction = "v1/email/getEmailingAnalytics";

            var request = new GetEmailingAnalyticsDto()
            {
                LicKey = SettingsLic.LicKey,
                EmailingIds = emailingIds,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var result = RequestHelper.MakeRequest<List<EmailingStatistics>>(urlHost + urlAction, request, headers, ERequestMethod.POST, timeoutSeconds:60*2);
            return result;
        }


        public static EmailingStatistics GetWithoutEmailingAnalytics(int? formatId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var result = GetWithoutEmailingAnalytics(new List<int?> { formatId }, dateFrom, dateTo) ?? new List<EmailingStatistics>();
            return result.FirstOrDefault();
        }

        public static List<EmailingStatistics> GetWithoutEmailingAnalytics(List<int?> formatIds, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var urlAction = "v1/email/getWithoutEmailingAnalytics";

            var request = new GetEmailingWithoutAnalyticsDto
            {
                LicKey = SettingsLic.LicKey,
                FormatIds = formatIds,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var result = RequestHelper.MakeRequest<List<EmailingStatistics>>(urlHost + urlAction, request, headers, ERequestMethod.POST, timeoutSeconds: 60*2);
            return result;
        }

        public static EmailingLog GetEmailingLog(GetEmailingLogDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto");

            dto.LicKey = SettingsLic.LicKey;
            AddSumStatuses(dto);

            var cacheName = CacheNames.AdvantShopMail + "Log_" + dto.GetHashCode();

            return CacheManager.Get(cacheName, () =>
            {
                var urlAction = "v1/email/getEmailLogs";

                var result = RequestHelper.MakeRequest<EmailingLog>(urlHost + urlAction, dto, headers, ERequestMethod.POST, timeoutSeconds: 60 * 2);

                return result;
            });
        }

        public static AdvantShopMail GetEmailLogs(int id)
        {
            var cacheName = CacheNames.AdvantShopMail + "Email_" + id;

            return CacheManager.Get(cacheName, () =>
            {
                var urlAction = "v1/email/getEmailLog";

                var dto = new GetAdvantShopMailDto
                {
                    LicKey = SettingsLic.LicKey,
                    Id = id
                };
                var result = RequestHelper.MakeRequest<AdvantShopMail>(urlHost + urlAction, dto, headers, ERequestMethod.POST);

                return result;
            });
        }

        public static List<SubjectWithoutEmailingResultDto> GetSubjectWithoutEmailing()
        {
            var cacheName = CacheNames.AdvantShopMail + "GetSubjectWithoutEmailing";

            return CacheManager.Get(cacheName, () =>
            {
                var urlAction = "v1/email/getSubjectWithoutEmailing";

                var dto = new GetSubjectWithoutEmailingDto
                {
                    LicKey = SettingsLic.LicKey,
                };

                var result = RequestHelper.MakeRequest<List<SubjectWithoutEmailingResultDto>>(urlHost + urlAction, dto, headers, ERequestMethod.POST);

                return result;
            });
        }

        public static EmailingLog GetWithoutEmailing(GetWithoutEmailLogsDto dto)
        {
            dto.LicKey = SettingsLic.LicKey;
            dto.Email = ValidationHelper.IsValidEmail(dto.Email) ? dto.Email : null;

            AddSumStatuses(dto);

            var cacheName = CacheNames.AdvantShopMail + "getWithoutEmailing" + dto.GetHashCode();
            return CacheManager.Get(cacheName, () =>
            {
                var urlAction = "v1/email/getWithoutEmailing";

                var result = RequestHelper.MakeRequest<EmailingLog>(urlHost + urlAction, dto, headers, ERequestMethod.POST);
                return result;
            });
        }

        private static void AddSumStatuses(BaseGetEmailLogsDto dto)
        {
            if (dto.Statuses == null)
                return;

            if (dto.Statuses.Any(x => x == EmailStatus.Sent))
                dto.Statuses.AddRange(GetSumStatuses(EmailStatus.Sent));
            else if (dto.Statuses.Any(x => x == EmailStatus.Delivered))
                dto.Statuses.AddRange(GetSumStatuses(EmailStatus.Delivered));
            else if (dto.Statuses.Any(x => x == EmailStatus.Opened))
                dto.Statuses.AddRange(GetSumStatuses(EmailStatus.Opened));
        }

        public static List<EmailStatus> GetSumStatuses(EmailStatus status)
        {
            switch (status)
            {
                case EmailStatus.Sent:
                    return new List<EmailStatus>
                    {
                        EmailStatus.Delivered,
                        EmailStatus.Opened,
                        EmailStatus.Clicked,
                        EmailStatus.Unsubscribed,
                        EmailStatus.SoftBounced,
                        EmailStatus.HardBounced,
                        EmailStatus.Spam
                    };
                case EmailStatus.Delivered:
                    return new List<EmailStatus>
                    {
                        EmailStatus.Opened,
                        EmailStatus.Clicked,
                        EmailStatus.Unsubscribed,
                        EmailStatus.Spam
                    };
                case EmailStatus.Opened:
                    return new List<EmailStatus>
                    {
                        EmailStatus.Clicked
                    };
                default:
                    return new List<EmailStatus>();
            }
        }

        #endregion
    }
}
