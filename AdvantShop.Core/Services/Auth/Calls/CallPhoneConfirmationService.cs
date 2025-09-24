using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.CallsAuth;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public sealed class CallPhoneConfirmationService : IPhoneConfirmationService
    {
        private const int CodeLength = 4;
        private const string CodeChars = "0123456789";
        private const string CallAuthPhoneConfirmedCookie = "customer_phone";

        public string SendCode(long phone, bool? addHash)
        {
            var code = _getAuthCode(phone, SettingsAuthCall.AuthCallMode, throwError: true);

            new CallCodeConfirmationRepository().AddCallCode(phone, code);
            return code;
        }
        
        public bool ConfirmPhoneByCode(long phone, string code)
        {
            var service = new CallCodeConfirmationRepository();
            
            var codeConfirmation = service.GetCallCode(phone, code);
            if (codeConfirmation == null)
                throw new BlException("Не правильный sms-код");
            
            service.SetSmsCodeUsed(codeConfirmation.Id);
            
            if ((codeConfirmation.DateAdded - DateTime.Now).TotalSeconds > 60*10)
                throw new BlException("sms-код устарел");
            
            return true;
        }
        
        public void SetPhoneConfirmedState(long phone, Guid customerId)
        {
            var value = GetPhoneConfirmedCookieValue(phone, customerId);
            CommonHelper.SetCookie(CallAuthPhoneConfirmedCookie, value, new TimeSpan(1, 0, 0, 0), true);
        }
        
        public bool IsPhoneConfirmed(long phone, Guid customerId)
        {
            var cookieValue = CommonHelper.GetCookieString(CallAuthPhoneConfirmedCookie);
            if (string.IsNullOrWhiteSpace(cookieValue))
                return false;
            
            var value = GetPhoneConfirmedCookieValue(phone, customerId);

            return cookieValue == value;
        }
        
        public bool IsBannedByPhoneOrIp(long phone, string ip)
        {
            return CallBanService.IsBannedByPhoneOrIp(phone, ip);
        }

        public void Ban(long? phone, string ip, DateTime untilDate)
        {
            CallBanService.Ban(phone, ip, untilDate);
        }
        
        public Result IsModuleActive()
        {
            if (GetActiveAuthCallModule() == null)
                return Result.Failure(new Error("Модуль для аутентификационного звонка не установлен"));
                    
            if (SettingsAuthCall.AuthCallMode == 0)
                return Result.Failure(new Error("Не выбран метод аутентификационного звонка"));
            
            return Result.Success();
        }

        public string GetHintDescription()
        {
            switch (SettingsAuthCall.AuthCallMode)
            {
                case EAuthCall.Flash:
                    return LocalizationService.GetResource("User.CodeDescription.Call.Flash");
                case EAuthCall.Voice:
                    return LocalizationService.GetResource("User.CodeDescription.Call.Voice");
                default:
                    return null;
            }
        }

        public IAuthCallService GetActiveAuthCallModule()
        {
            if (SettingsAuthCall.ActiveAuthCallModule == "-1")
                return null;

            var list = new List<IAuthCallService>();

            foreach (var moduleType in AttachedModules.GetModules<IAuthCallService>())
            {
                var module = (IAuthCallService)Activator.CreateInstance(moduleType, null);

                if (module.ModuleStringId == SettingsAuthCall.ActiveAuthCallModule)
                    return module;
                
                list.Add(module);
            }

            return list.Count > 0 ? list[0] : null;
        }

        public IAuthCallService GetAuthCallModuleByStringId(string stringId)
        {
            return AttachedModules.GetModules<IAuthCallService>()
                .Select(moduleType => (IAuthCallService)Activator.CreateInstance(moduleType, null))
                .FirstOrDefault(module => module.ModuleStringId == stringId);
        }

        public List<SelectListItem> GetAuthCallModsByModuleStringId(string moduleStringId)
        {
            var list = new List<SelectListItem>() {new SelectListItem() {Text = LocalizationService.GetResource("Core.Settings.AuthCall.AuthCallMods.DefaultMode"), Value = "0"}};
            
            if (moduleStringId == "-1" || moduleStringId == null) return list;
            
            var module = GetAuthCallModuleByStringId(moduleStringId);

            if (module == null) return list;
            
            if (module.FlashCallEnable())
                list.Add(new SelectListItem() {Text = EAuthCall.Flash.Localize(), Value = ((int)EAuthCall.Flash).ToString()});
            if (module.VoiceCallEnable())
                list.Add(new SelectListItem() {Text = EAuthCall.Voice.Localize(), Value = ((int)EAuthCall.Voice).ToString()});

            return list;
        }
        
        public List<IAuthCallService> GetAllAuthCallModules()
        {
            return AttachedModules.GetModules<IAuthCallService>()
                .Select(moduleType => (IAuthCallService) Activator.CreateInstance(moduleType, null))
                .ToList();
        }
        
        private string GetPhoneConfirmedCookieValue(long phone, Guid customerId)
        {
            return $"{customerId}{phone}{SettingsLic.LicKey}".Md5(false);
        }
        
        private string GenerateCallCode()
        {
            var smsCode = string.Empty;
            var rnd = new Random();

            for (var i = 1; i <= CodeLength; i++)
            {
                smsCode += CodeChars[(int)(CodeChars.Length * rnd.NextDouble())];
            }

            return smsCode;
        }

        private string _getAuthCode(long phone, EAuthCall type, Guid? customerId = null, bool throwError = false, bool isInternal = false)
        {
            var ip = !isInternal ? HttpContext.Current.TryGetIp() : null;

            if (!new CallValidationService(isInternal).Validate(phone, ip, throwError))
                return null;

            return _getAuthCode(phone, type, customerId, throwError, ip);
        }

        private string _getAuthCode(long phone, EAuthCall type, Guid? customerId, bool throwError, string ip)
        {
            var result = string.Empty;

            var status = CallAuthStatus.Error;
            try
            {
                var moduleAuthCall = GetActiveAuthCallModule();
                if (moduleAuthCall == null)
                {
                    if (throwError) throw new BlException("Не подключен модуль авторизационного звонка");
                    return string.Empty;
                }

                result
                    = moduleAuthCall is IAuthCallWithCodeGenService moduleAuthCallWithCodeGen
                        ? moduleAuthCallWithCodeGen.GetAuthCode(phone, GenerateCallCode(), type)
                        : ((IAuthCallWithoutCodeGenService)moduleAuthCall).GetAuthCode(phone, type);

                status = CallAuthStatus.Sent;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Error(error);

                                    if (throwError) throw;
                                }
                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Error($"SendCall error: {phone} {type}", ex);

                if (throwError) throw;
            }

            CallLogger.Log(new CallLogData(phone, ip, customerId, status));
            
            return result;
        }
    }
}