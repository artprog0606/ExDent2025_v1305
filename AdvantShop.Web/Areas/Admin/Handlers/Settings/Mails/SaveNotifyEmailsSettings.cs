using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Auth.Calls;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings.SettingsMail;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class SaveNotifyEmailsSettings
    {
        private readonly MailSettingsModel _model;

        public SaveNotifyEmailsSettings(MailSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            SettingsMail.EmailForOrders = _model.EmailForOrders.DefaultOrEmpty();
            SettingsMail.EmailForProductDiscuss = _model.EmailForProductDiscuss.DefaultOrEmpty();
            SettingsMail.EmailForRegReport = _model.EmailForRegReport.DefaultOrEmpty();
            SettingsMail.EmailForFeedback = _model.EmailForFeedback.DefaultOrEmpty();
            SettingsMail.EmailForLeads = _model.EmailForLeads.DefaultOrEmpty();
            SettingsMail.EmailForBookings = _model.EmailForBookings.DefaultOrEmpty();
            SettingsMail.EmailForPartners = _model.EmailForPartners.DefaultOrEmpty();
            SettingsMail.EmailForMissedCall = _model.EmailForMissedCall.DefaultOrEmpty();

            SettingsMail.SMTP = _model.SMTP.DefaultOrEmpty();
            SettingsMail.Port = _model.Port ?? 0;
            SettingsMail.From = _model.From.DefaultOrEmpty();

            var imapSettingsChanged =
                SettingsMail.Login != _model.Login.DefaultOrEmpty() ||
                SettingsMail.ImapHost != _model.ImapHost.DefaultOrEmpty() ||
                SettingsMail.ImapPort != _model.ImapPort;

            SettingsMail.Login = _model.Login.DefaultOrEmpty();

            if (_model.Password != _model.PasswordCompare)
                SettingsMail.Password = _model.Password.DefaultOrEmpty();

            SettingsMail.SSL = _model.SSL;
            SettingsMail.SenderName = _model.SenderName.DefaultOrEmpty();

            SettingsMail.ImapHost = _model.ImapHost.DefaultOrEmpty();
            SettingsMail.ImapPort = _model.ImapPort ?? 0;

            if (imapSettingsChanged)
                SettingsMail.ImapLastUpdateLetterId = 0;

            SettingsMail.UseAdvantshopMail = _model.UseAdvantshopMail;
            if (_model.UseAdvantshopMail)
            {
                if (string.IsNullOrWhiteSpace(_model.FromEmail)) throw new BlException("Поле email обязательно к заполнению");
                if (string.IsNullOrWhiteSpace(_model.FromName)) throw new BlException("Поле имя отправителя обязательно к заполнению");

                MailService.Save(_model.FromEmail, _model.FromName);
            }

            // sms 
            var phone = !string.IsNullOrEmpty(_model.AdminPhone)
                ? StringHelper.ConvertToStandardPhone(_model.AdminPhone, true, true)
                : null;

            if (_model.ActiveSmsModule == "-1")
            {
                SettingsSms.ActiveSmsModule = _model.ActiveSmsModule;
            }
            else
            {
                var smsModules = SmsNotifier.GetAllSmsModules();
                var selectedSmsModule = smsModules.Find(x => x.ModuleStringId == _model.ActiveSmsModule);

                SettingsSms.ActiveSmsModule = selectedSmsModule != null
                    ? selectedSmsModule.ModuleStringId
                    : (smsModules.Count > 0 ? smsModules[0].ModuleStringId : null);
            }

            SettingsSms.AdminPhone = phone != null ? phone.ToString() : "";
            SettingsSms.SendSmsToCustomerOnNewOrder = _model.SendSmsToCustomerOnNewOrder;
            SettingsSms.SendSmsToAdminOnNewOrder = _model.SendSmsToAdminOnNewOrder;
            SettingsSms.SmsTextOnNewOrder = _model.SmsTextOnNewOrder;
            SettingsSms.SendSmsToCustomerOnOrderStatusChanging = _model.SendSmsToCustomerOnOrderStatusChanging;
            SettingsSms.SendSmsToAdminOnOrderStatusChanging = _model.SendSmsToAdminOnOrderStatusChanging;
            SettingsSms.SmsTextOnOrderPayStatusChanging = _model.SmsTextOnOrderPayStatusChanging;
            SettingsSms.SendSmsToCustomerOnOrderPayStatusChanging = _model.SendSmsToCustomerOnOrderPayStatusChanging;
            SettingsSms.SendSmsToAdminOnOrderPayStatusChanging = _model.SendSmsToAdminOnOrderPayStatusChanging;

            SettingsSms.SendSmsToAdminOnNewLead = _model.SendSmsToAdminOnNewLead;
            SettingsSms.SmsTextOnNewLead = _model.SmsTextOnNewLead;

            SettingsAuthCall.ActiveAuthCallModule = _model.ActiveAuthCallModule;
            SettingsAuthCall.AuthCallMode = (EAuthCall)_model.AuthCallMode;
            
            SettingsSms.SmsBanLevel = _model.SmsBanLevel;
        }
    }
}

