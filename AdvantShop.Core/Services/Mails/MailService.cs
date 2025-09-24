using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Mails
{
    public class MailService
    {
        private static IEmailLogger Loger { get { return LoggingManager.GetEmailLogger(); } }

        /// <summary>
        /// run in task
        /// </summary>
        public static bool SendMailNow(
            Guid customerIdTo, 
            string strTo, 
            string strSubject, 
            string strText, 
            bool isBodyHtml, 
            string replyTo = null, 
            int lettercount = 1, 
            bool logging = true)
        {
            return SendMailNow(customerIdTo, strTo, strSubject, strText, isBodyHtml, emailingId: null, replyTo: replyTo, lettercount: lettercount, logging: logging);
        }

        /// <summary>
        /// directly executed
        /// </summary>
        public static bool SendMail(
            Guid customerIdTo, 
            string strTo, 
            string strSubject, 
            string strText,
            bool isBodyHtml,
            string replyTo = null, 
            bool needretry = true, 
            int lettercount = 1, 
            bool logging = true)
        {
            return SendMail(customerIdTo, strTo, strSubject, strText, isBodyHtml, null, null, replyTo, needretry, lettercount, logging);
        }

        /// <summary>
        /// run in task
        /// </summary>
        public static bool SendMailNow(
            Guid customerIdTo, 
            string strTo, 
            string strSubject, 
            string strText, 
            bool isBodyHtml, 
            Guid? emailingId, 
            string replyTo = null, 
            int lettercount = 1, 
            bool logging = true)
        {
            Task.Factory.StartNew(() => SendMail(customerIdTo, strTo, strSubject, strText, isBodyHtml, emailingId, null, replyTo, lettercount: lettercount, logging: logging));
            
            return true;
        }

        public static bool SendMailNow(
            Guid customerIdTo, 
            string strTo, 
            string strSubject, 
            string strText, 
            bool isBodyHtml, 
            int? formatId, 
            string replyTo = null, 
            int lettercount = 1, 
            bool logging = true)
        {
            Task.Factory.StartNew(() => SendMail(customerIdTo, strTo, strSubject, strText, isBodyHtml, null, formatId, replyTo, lettercount: lettercount, logging: logging));
            
            return true;
        }

        public static bool SendMail(
            Guid customerIdTo, 
            string strTo, 
            MailTemplate template, 
            Guid? emailingId = null, 
            string replyTo = null, 
            bool needretry = true, 
            int lettercount = 1, 
            bool logging = true)
        {
            template.BuildMail();
            
            return SendMail(customerIdTo, strTo, template.Subject, template.Body, true, emailingId, (int)template.Type, replyTo, needretry, lettercount, logging: logging);
        }

        public static bool SendMailNow(
            string strTo, 
            MailTemplate template, 
            Guid? emailingId = null, 
            string replyTo = null, 
            int lettercount = 1)
        {
            SendMailNow(Guid.Empty, strTo, template, emailingId, replyTo,  lettercount, logging: false);
            
            return true;
        }

        public static bool SendMailNow(
            Guid customerIdTo, 
            string strTo, 
            MailTemplate template,
            Guid? emailingId = null, 
            string replyTo = null, 
            int lettercount = 1, 
            bool logging = true)
        {
            template.BuildMail();
            
            Task.Factory.StartNew(() => SendMail(customerIdTo, strTo, template.Subject, template.Body, true, emailingId, (int)template.Type, replyTo, lettercount: lettercount, logging: logging));
            
            return true;
        }

        /// <summary>
        /// directly executed
        /// </summary>
        public static bool SendMail(
            Guid customerIdTo, 
            string emailTo, 
            string subject, 
            string body, 
            bool isBodyHtml, 
            Guid? emailingId, 
            int? formatId, 
            string replyTo = null, 
            bool needretry = true, 
            int lettercount = 1,
            bool logging = true)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body) || string.IsNullOrWhiteSpace(emailTo))
                return false;

            body = PrepareMessage(body);

            var status = EmailStatus.Sent;
            try
            {
                if (SettingsMail.UseAdvantshopMail && !SettingsMail.IsAdvantshopTrialFreeEmailEnabled)
                {
                    AdvantShopMailService.Send(customerIdTo, emailTo, subject, body, isBodyHtml, replyTo, lettercount, emailingId, formatId);
                }
                else
                {
                    SmtpMailService.Send(emailTo, subject, body, isBodyHtml, replyTo, needretry);
                }
            }
            catch (Exception e)
            {
                status = EmailStatus.Error;

                if (e.Message.Equals("Достигнут лимит отправки писем с пробного аккаунта", StringComparison.OrdinalIgnoreCase))
                {
                    AdminInformerService.Add(new AdminInformer
                    {
                        Body = "Достигнут лимит отправки писем с пробного аккаунта, подключите свой почтовый ящик.",
                        Type = AdminInformerType.Error
                    });
                }
                
                if (SettingsMail.UseAdvantshopMail)
                {
                    if (e.Message.ToLower().Contains("ограничение отправки для триала в день"))
                    {
                        AdminInformerService.Add(new AdminInformer
                        {
                            Body = "Ошибка при отправке писем: ограничение писем в день для триалального режима. Подключите свою почту в Настройках почты.",
                            Type = AdminInformerType.Error
                        });
                    }
                }

                if (needretry)
                {
                    Debug.Log.Warn(e.Message + " " + emailTo, e);
                }
                else
                {
                    throw new BlException(e.Message + " " + emailTo);
                }
            }
            finally
            {
                if (logging && !SettingsMail.UseAdvantshopMail)
                {
                    Loger.LogEmail(new EmailLog(customerIdTo, emailTo, subject, body, status));
                }
            }

            return true;
        }

        private static string PrepareMessage(string message)
        {
            var res = message.Replace("\"userfiles/", $"\"{SettingsMain.SiteUrl.Trim('/')}/userfiles/");

            if (SettingsMail.IsAdvantshopTrialFreeEmailEnabled) 
                res += SettingsMail.AdvantshopTrialFreeEmailPostfix;

            return res;
        }

        public static void Save(string fromEmail, string fromName)
        {
            if (SettingsMail.UseAdvantshopMail)
            {
                AdvantShopMailService.SaveSender(fromEmail, fromName);
                CapSettingProvider.Reset();
            }
        }

        public static bool SendValidate()
        {
            if (SettingsMail.UseAdvantshopMail)
            {
                var res = AdvantShopMailService.SendValidate();
                CapSettingProvider.Reset();
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_BindAdvantshopMailService);
                return res;
            }
            throw new BlException("validation only for UseAdvantshopMail");
        }

        public static void Reset()
        {
            if (SettingsMail.UseAdvantshopMail)
                CapSettingProvider.Reset();
        }

        public static List<EmailLogItem> GetLast(Guid customerId)
        {
            if (SettingsMail.UseAdvantshopMail)
                return AdvantShopMailService.GetLast(customerId);

            return null;
        }

        public static string ValidateMailSettingsBeforeSending()
        {
            string error = null;

            if (SettingsMail.UseAdvantshopMail &&
                !string.IsNullOrEmpty(CapShopSettings.FromEmail) && CapShopSettings.FromEmail.Contains("adv-mail"))
            {
                error =
                    LocalizationService.GetResourceFormat("Core.Services.Mails.MailService.NeedActivateYourEmailError",
                        UrlService.GetUrl("adminv2/settingsmail#?notifyTab=emailsettings"));
            }

            return error;
        }
    }
}
