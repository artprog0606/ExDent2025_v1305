using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Auth.Calls;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Settings.SettingsMail;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class GetNotifyEmailsSettings
    {
        public MailSettingsModel Execute()
        {
            var model = new MailSettingsModel
            {
                EmailForOrders = SettingsMail.EmailForOrders,
                EmailForProductDiscuss = SettingsMail.EmailForProductDiscuss,
                EmailForRegReport = SettingsMail.EmailForRegReport,
                EmailForFeedback = SettingsMail.EmailForFeedback,
                EmailForLeads = SettingsMail.EmailForLeads,
                EmailForBookings = SettingsMail.EmailForBookings,
                EmailForPartners = SettingsMail.EmailForPartners,
                EmailForMissedCall = SettingsMail.EmailForMissedCall,
                SMTP = SettingsMail.SMTP,
                Port = SettingsMail.Port != default ? SettingsMail.Port : 25,
                From = SettingsMail.From,
                Login = SettingsMail.Login,
                Password = SettingsMail.Password,
                PasswordCompare = string.IsNullOrWhiteSpace(SettingsMail.Password)
                    ? string.Empty
                    : DateTime.Now.ToString("ddhhmmss"),
                SSL = SettingsMail.SSL,
                SenderName = SettingsMail.SenderName,
                ImapHost =
                    SettingsMail.ImapHost ??
                    (SettingsMail.SMTP != null && SettingsMail.SMTP.StartsWith("smtp.")
                        ? SettingsMail.SMTP.Replace("smtp.", "imap.")
                        : ""),
                ImapPort = SettingsMail.ImapPort != 0 ? SettingsMail.ImapPort : 993,
                UseAdvantshopMail = SettingsMail.UseAdvantshopMail,
                AllowSendTestEmail = SettingsMail.IsAdvantshopTrialFreeEmailEnabled,

                AdminPhone = SettingsSms.AdminPhone,
                SmsBanLevel = SettingsSms.SmsBanLevel,
                SendSmsToCustomerOnNewOrder = SettingsSms.SendSmsToCustomerOnNewOrder,
                SendSmsToAdminOnNewOrder = SettingsSms.SendSmsToAdminOnNewOrder,
                SmsTextOnNewOrder = SettingsSms.SmsTextOnNewOrder,
                SendSmsToCustomerOnOrderStatusChanging = SettingsSms.SendSmsToCustomerOnOrderStatusChanging,
                SendSmsToAdminOnOrderStatusChanging = SettingsSms.SendSmsToAdminOnOrderStatusChanging,
                SendSmsToCustomerOnOrderPayStatusChanging = SettingsSms.SendSmsToCustomerOnOrderPayStatusChanging,
                SendSmsToAdminOnOrderPayStatusChanging = SettingsSms.SendSmsToAdminOnOrderPayStatusChanging,
                SmsTextOnOrderPayStatusChanging = SettingsSms.SmsTextOnOrderPayStatusChanging,
                SendSmsToAdminOnNewLead = SettingsSms.SendSmsToAdminOnNewLead,
               
                SmsTextOnNewLead = SettingsSms.SmsTextOnNewLead
            };

            model.ActiveSmsModule = SettingsSms.ActiveSmsModule;
            model.SmsModules =
                SmsNotifier.GetAllSmsModules()
                    .Select(x => new SelectListItem() {Text = x.ModuleName, Value = x.ModuleStringId})
                    .ToList();

            if (model.ActiveSmsModule != "-1")
            {
                var selectedSmsModuleItem = model.SmsModules.Find(x => x.Value == model.ActiveSmsModule);
                if (selectedSmsModuleItem == null)
                {
                    var item = model.SmsModules.FirstOrDefault();
                    if (item != null)
                        model.ActiveSmsModule = item.Value;
                }
            }

            model.SmsModules.Insert(0, new SelectListItem() { Text = "Выберите SMS-модуль", Value = "-1" });

            model.ActiveAuthCallModule = SettingsAuthCall.ActiveAuthCallModule;
            model.AuthCallModules =
                new CallPhoneConfirmationService().GetAllAuthCallModules()
                    .Select(x => new SelectListItem() { Text = x.ModuleName, Value = x.ModuleStringId })
                    .ToList();
            model.AuthCallModules.Insert(0, new SelectListItem() { Text = LocalizationService.GetResource("Core.Settings.AuthCall.AuthCallModules.DefaultModule"), Value = "-1" });

            var authCallMods = new CallPhoneConfirmationService().GetAuthCallModsByModuleStringId(model.ActiveAuthCallModule);
            
            model.AuthCallMods = JsonConvert.SerializeObject(authCallMods);
            model.AuthCallMode = authCallMods.Count > 1 
                                 && authCallMods.Any(mode => 
                                     mode.Value == ((int)SettingsAuthCall.AuthCallMode).ToString())
                ? (int)SettingsAuthCall.AuthCallMode
                : 0;

            try
            {
                model.FromEmail = CapShopSettings.FromEmail;
                model.FromName = CapShopSettings.FromName;
                model.ConfirmDateEmail = CapShopSettings.ConfirmDate;
                model.HtmlMessage = CapShopSettings.HtmlMessage;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return model;
        }
    }
}
