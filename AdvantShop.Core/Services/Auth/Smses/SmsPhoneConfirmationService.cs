using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Auth.Smses
{
    public sealed class SmsPhoneConfirmationService : IPhoneConfirmationService
    {
        private const int SmsCodeLength = 4;
        private const string SmsCodeChars = "0123456789";
        private const string SmsPhoneConfirmedCookie = "customer_phone";

        private static string GenerateSmsCode()
        {
            var smsCode = string.Empty;
            var rnd = new Random();

            for (int i = 1; i <= SmsCodeLength; i++)
            {
                smsCode += SmsCodeChars[(int)(SmsCodeChars.Length * rnd.NextDouble())];
            }

            return smsCode;
        }

        public string SendCode(long phone, bool? addHash)
        {
            var code = SettingsSms.SmsTesting ? "1234" :  GenerateSmsCode();
            var text = LocalizationService.GetResourceFormat("SignInByPhone.SmsCodeTemplate", code);
            
            if (addHash != null && addHash.Value)
                text += "\n" + SettingsApiAuth.SmsRetrieverHashCode;

            SmsNotifier.SendSmsNowWithResult(phone, text, throwError: true, isInternal: false, isAuthCode: true);
            
            new SmsCodeConfirmationRepository().AddSmsCode(phone, code);

            return code;
        }

        public bool ConfirmPhoneByCode(long phone, string code)
        {
            var service = new SmsCodeConfirmationRepository();
            
            var smsCodeConfirmation = service.GetSmsCode(phone, code);
            if (smsCodeConfirmation == null)
                throw new BlException("Не правильный sms-код");
            
            service.SetSmsCodeUsed(smsCodeConfirmation.Id);
            
            if ((smsCodeConfirmation.DateAdded - DateTime.Now).TotalSeconds > 60*10)
                throw new BlException("sms-код устарел");
            
            return true;
        }

        public void SetPhoneConfirmedState(long phone, Guid customerId)
        {
            var value = GetPhoneConfirmedCookieValue(phone, customerId);
            CommonHelper.SetCookie(SmsPhoneConfirmedCookie, value, new TimeSpan(1, 0, 00, 0), true);
        }

        public bool IsPhoneConfirmed(long phone, Guid customerId)
        {
            var cookieValue = CommonHelper.GetCookieString(SmsPhoneConfirmedCookie);
            if (string.IsNullOrWhiteSpace(cookieValue))
                return false;
            
            var value = GetPhoneConfirmedCookieValue(phone, customerId);

            return cookieValue == value;
        }

        public bool IsBannedByPhoneOrIp(long phone, string ip)
        {
            return SmsBanService.IsBannedByPhoneOrIp(phone, ip);
        }
        
        public void Ban(long? phone, string ip, DateTime untilDate)
        {
            SmsBanService.Ban(phone, ip, untilDate);
        }

        public Result IsModuleActive()
        {
            return SmsNotifier.GetActiveSmsModule() == null
                ? Result.Failure(new Error("Модуль для отправки SMS не установлен"))
                : Result.Success();
        }
        
        public string GetHintDescription() => LocalizationService.GetResource("User.CodeDescription.Sms");

        private static string GetPhoneConfirmedCookieValue(long phone, Guid customerId)
        {
            return $"{customerId}{phone}{SettingsLic.LicKey}".Md5(false);
        }
    }
}
