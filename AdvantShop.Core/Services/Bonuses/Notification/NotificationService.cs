using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Notification.Template;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using RazorEngine;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Bonuses.Notification
{

    public class NotificationService
    {
        private static string NormalizeRazor(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var pattern = string.Format( @"({0})(\w+)({0})", BaseNotificationTemplate.ModelTag);
            var replaced = Regex.Replace(text, pattern, "@Raw(Model.$2)");
            return replaced;
        }

        public static bool Valid(string text, ENotifcationType type)
        {
            try
            {
                var viewModel = BaseNotificationTemplate.Factory(type);
                Engine.Razor.RunCompile(NormalizeRazor(text), GetMd5Hash(text), null, viewModel.Prepare());
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return false;
        }

        public static void Process(Guid cardId, ENotifcationType notificationType, BaseNotificationTemplate model)
        {
            var notificationMethods = BonusSystem.EnabledNotificationMethods;
            if (notificationMethods.Count == 0)
                return;

            var text = string.Empty;
            var customer = CustomerService.GetCustomer(cardId);
            if (customer == null)
                return;
            
            var notificationTemplates = NotificationTemplateService.Get(notificationType);
            if (notificationTemplates == null || notificationTemplates.Count == 0) 
                return;
            
            try
            {
                string state = string.Empty;
                if (customer.StandardPhone.HasValue && notificationMethods.Contains(EBonusNotificationMethod.Sms))
                {
                    var notification = notificationTemplates
                        .FirstOrDefault(notificationTemplate => notificationTemplate.NotificationMethod == EBonusNotificationMethod.Sms);
                    if (notification == null)
                        return;
                    text = Engine.Razor.RunCompile(NormalizeRazor(notification.NotificationBody), GetMd5Hash(notification.NotificationBody),  null, model.Prepare());
                    state = SmsNotifier.SendSmsNowWithResult(customer.StandardPhone.Value, text, isInternal: true);
                    NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = state, Contact = customer.StandardPhone.ToString(), Created = DateTime.Now, ContactType = EContactType.SMS });
                }
				if(!string.IsNullOrEmpty(customer.EMail) && notificationMethods.Contains(EBonusNotificationMethod.Email))
				{
                    var notification = notificationTemplates
                        .FirstOrDefault(notificationTemplate => notificationTemplate.NotificationMethod == EBonusNotificationMethod.Email);
                    if (notification == null)
                        return;
                    text = Engine.Razor.RunCompile(NormalizeRazor(notification.NotificationBody), GetMd5Hash(notification.NotificationBody),  null, model.Prepare());
                    state = MailService.SendMailNow(customer.Id, customer.EMail, "Бонусная программа", text, false) ? "Письмо отправлено" : "Письмо не отправлено";
                    NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = state, Contact = customer.EMail, Created = DateTime.Now, ContactType = EContactType.EMail });
                }
                var fcmToken = CustomerService.GetFcmToken(cardId);
                if (string.IsNullOrEmpty(fcmToken) && notificationMethods.Contains(EBonusNotificationMethod.Push))
                {
                    var notification = notificationTemplates
                        .FirstOrDefault(notificationTemplate => notificationTemplate.NotificationMethod == EBonusNotificationMethod.Push);
                    if (notification == null)
                        return;
                    text = Engine.Razor.RunCompile(NormalizeRazor(notification.NotificationBody), GetMd5Hash(notification.NotificationBody),  null, model.Prepare());
                    NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = "Уведомление не отправлено, у покупателя не установлено мобильное приложение",
                        Contact = customer.StandardPhone.ToString(), Created = DateTime.Now, ContactType = EContactType.Push });
                    state = SendAdditionalNotificationForPush(text, customer);
                }
                else if (notificationMethods.Contains(EBonusNotificationMethod.Push))
                {
                    var notification = notificationTemplates
                        .FirstOrDefault(notificationTemplate => notificationTemplate.NotificationMethod == EBonusNotificationMethod.Sms);
                    if (notification == null)
                        return;
                    text = Engine.Razor.RunCompile(NormalizeRazor(notification.NotificationBody), GetMd5Hash(notification.NotificationBody),  null, model.Prepare());
                    state = MobileApp.NotificationService.SendNotification(new MobileApp.Notification
                    {
                        CustomerId = cardId,
                        Body = text
                    });
                    NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = string.IsNullOrEmpty(state) ? "Уведомление отправлено" : state, Contact = customer.StandardPhone.ToString(), Created = DateTime.Now, ContactType = EContactType.Push });

                    if (state.IsNotEmpty())
                    {
                        state = SendAdditionalNotificationForPush(text, customer);
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = "error " + ex.Message, Contact = String.Join(";", new List<string> { customer.EMail, customer.StandardPhone.ToString() }), Created = DateTime.Now });
            }
        }

        private static string GetMd5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        private static string SendAdditionalNotificationForPush(string text, Customer customer)
        {
            if (!BonusSystem.AdditionalNotification.HasValue)
            {
                return null;
            }
            
            string state;
            
            if (customer.StandardPhone.HasValue && BonusSystem.AdditionalNotification == EBonusNotificationMethod.Sms)
            {
                state = SmsNotifier.SendSmsNowWithResult(customer.StandardPhone.Value, text, isInternal: true);
                NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = state, Contact = customer.StandardPhone.ToString(), Created = DateTime.Now, ContactType = EContactType.SMS });
            }
            else
            {
                state = MailService.SendMailNow(customer.Id, customer.EMail, "Бонусная программа", text, false) ? "Письмо отправлено" : "Письмо не отправлено";
                NotificationTemplateService.AddNotificationLog(new NotificationLog { Body = text, State = state, Contact = customer.EMail, Created = DateTime.Now, ContactType = EContactType.EMail });
            }

            return state;
        }
    }
}
