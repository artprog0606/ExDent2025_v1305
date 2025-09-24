using AdvantShop.Configuration;
using AdvantShop.Core.Common;
using AdvantShop.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AdvantShop.Core.Services.Mails
{
    public class SmtpMailService
    {
        public static string Send(string strTo, string strSubject, string strText, bool isBodyHtml, string replyTo = null, bool needretry = true)
        {
            var smtp = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreeSMTP
                : SettingsMail.SMTP;
            var login = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreeLogin
                : SettingsMail.Login == string.Empty || SettingsMail.Login == SettingsMail.SIX_STARS
                    ? SettingsMail.InternalDataL
                    : SettingsMail.Login;
            var password = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreePassword
                : SettingsMail.Password == string.Empty || SettingsMail.Password == SettingsMail.SIX_STARS
                    ? SettingsMail.InternalDataP
                    : SettingsMail.Password;
            var port = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreePort
                : SettingsMail.Port;
            var email = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreeEmailFrom
                : SettingsMail.From;
            var ssl = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreeSSL
                : SettingsMail.SSL;
            var senderName = SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                ? SettingsMail.AdvantshopTrialFreeSenderName
                : SettingsMail.SenderName;

            if (string.IsNullOrWhiteSpace(smtp)
                || string.IsNullOrWhiteSpace(login)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Отправка писем по SMTP не настроена, подключите свой почтовый ящик.");
            }

            Process(strTo, strSubject, strText, isBodyHtml, smtp, port, login, password, email, ssl, senderName, replyTo, needretry ? 5 : 1);
            return string.Empty;
        }

        public static void Process(string strTo, string strSubject, string strText, bool isBodyHtml,
                                     string smtpServer, int port, string login, string password, string emailFrom, bool ssl, string senderName,
                                     string replyTo, int retryCount = 5)
        {
            try
            {
                string[] strMails = strTo.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                foreach (string strEmail in strMails)
                {
                    if (SettingsMail.IsAdvantshopTrialFreeEmailEnabled
                        && SettingsMail.AdvantshopTrialFreeEmailSendCount >= SettingsMail.AdvantshopTrialFreeEmailSendLimit)
                    {
                        throw new Exception("Достигнут лимит отправки писем с пробного аккаунта, подключите свой почтовый ящик.");
                    }
                    
                    RetryHelper.Do(() => _send(strEmail, strSubject, strText, isBodyHtml, smtpServer, login, password, port, emailFrom, ssl, senderName, replyTo), TimeSpan.FromSeconds(60), retryCount);
                    SettingsMail.AdvantshopTrialFreeEmailSendCount++;
                }
            }
            catch (AggregateException ex)
            {
                var error = "";
                if (ex.InnerExceptions != null && ex.InnerExceptions.Count > 0)
                {
                    error = ex.InnerExceptions.Aggregate("", (current, innerEx) => current + " " + (innerEx.InnerException != null ? innerEx.InnerException.Message : innerEx.Message));
                }
                else
                {
                    error = ex.Message;
                }
                //Debug.Log.Error(ex);
                throw new Exception(error);
            }
        }

        private static void _send(string strTo, string strSubject, string strText, bool isBodyHtml, string smtpServer, string login, string password, int port, string emailFrom, bool ssl, string senderName, string replyTo)
        {
            using (var emailClient = new SmtpClient(smtpServer))
            {
                emailClient.UseDefaultCredentials = false;
                emailClient.Credentials = new NetworkCredential(login, password);
                emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                emailClient.Port = port;
                emailClient.EnableSsl = ssl;

                string strE = strTo.Trim();
                if (string.IsNullOrEmpty(strE))
                    return;

                if (!ValidationHelper.IsValidEmail(strE))
                    return;

                if (string.IsNullOrEmpty(strText))
                    return;

                using (MailMessage message = new MailMessage(
                                    new MailAddress(emailFrom, string.IsNullOrEmpty(senderName) ? emailFrom : senderName),
                                    new MailAddress(strE, strE)
                        ))
                {
                    message.Subject = strSubject;
                    message.Body = strText;
                    message.IsBodyHtml = isBodyHtml;
                    message.SubjectEncoding = Encoding.UTF8;
                    message.BodyEncoding = Encoding.UTF8;
                    message.HeadersEncoding = Encoding.UTF8;

                    var replyAdresses = replyTo?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => ValidationHelper.IsValidEmail(x));
                    if (replyAdresses != null && replyAdresses.Any())
                    {
                        message.ReplyToList.Add(string.Join(",", replyAdresses));
                    }

                    emailClient.Send(message);
                }
            }
        }
    }
}
