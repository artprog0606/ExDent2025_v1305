using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Auth.Calls;
using AdvantShop.Core.Services.Auth.Smses;
using AdvantShop.Core.Services.Smses;

namespace AdvantShop.Core.Services.Auth
{
    public sealed class PhoneConfirmationService : IPhoneConfirmationService
    {
        private readonly IPhoneConfirmationService _phoneConfirmationService;
        
        public PhoneConfirmationService()
        {
            switch (SettingsOAuth.AuthByCodeMethod)
            {
                case EAuthByCodeMode.Sms:
                    _phoneConfirmationService = new SmsPhoneConfirmationService();
                    break;
                    
                case EAuthByCodeMode.Call:
                    _phoneConfirmationService = new CallPhoneConfirmationService();
                    break;
                
                default:
                    throw new NotImplementedException($"Method {SettingsOAuth.AuthByCodeMethod} not implemented in PhoneConfirmationService");
            }
            
        }
        
        public string SendCode(long phone, bool? addHash)
        {
            return _phoneConfirmationService.SendCode(phone, addHash);
        }
        
        public bool ConfirmPhoneByCode(long phone, string code)
        {
            return _phoneConfirmationService.ConfirmPhoneByCode(phone, code);
        }

        public void SetPhoneConfirmedState(long phone, Guid customerId)
        {
            _phoneConfirmationService.SetPhoneConfirmedState(phone, customerId);
        }

        public bool IsPhoneConfirmed(long phone, Guid customerId)
        {
            return _phoneConfirmationService.IsPhoneConfirmed(phone, customerId);
        }
        
        public bool IsBannedByPhoneOrIp(long phone, string ip)
        {
            return _phoneConfirmationService.IsBannedByPhoneOrIp(phone, ip);
        }
        
        public void Ban(long? phone, string ip, DateTime untilDate)
        {
            _phoneConfirmationService.Ban(phone, ip, untilDate);
        }

        public Result IsModuleActive()
        {
            return _phoneConfirmationService.IsModuleActive();
        }
        
        public string GetHintDescription()
        {
            return _phoneConfirmationService.GetHintDescription();
        }
    }
}