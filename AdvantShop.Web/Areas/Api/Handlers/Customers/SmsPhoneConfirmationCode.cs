using System.Web;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Auth.Smses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class SmsPhoneConfirmationCode : AbstractCommandHandler<SmsPhoneConfirmationCodeResponse>
    {
        private readonly SmsPhoneConfirmationCodeModel _model;
        private long _phone;

        public SmsPhoneConfirmationCode(SmsPhoneConfirmationCodeModel model)
        {
            _model = model;
        }

        public SmsPhoneConfirmationCode(Customer customer, string code)
        {
            if (customer != null)
            {
                _phone = customer.StandardPhone ?? -1;
                _model = new SmsPhoneConfirmationCodeModel()
                {
                    Phone = _phone.ToString(),
                    Code = code
                };
            }
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.Code))
                throw new BlException("Укажите sms-код");
            
            if (string.IsNullOrWhiteSpace(_model.Phone))
                throw new BlException("Укажите телефон");
            
            if (_phone == -1)
                throw new BlException("Не валидный телефон");

            if (_phone == 0)
            {
                var phoneStr = HttpUtility.HtmlEncode(_model.Phone);
                var standardPhone = StringHelper.ConvertToStandardPhone(phoneStr, true, true);

                if (standardPhone == null || standardPhone == 0)
                    throw new BlException("Не валидный телефон");

                _phone = standardPhone.Value;
            }
        }

        protected override SmsPhoneConfirmationCodeResponse Handle()
        {
            if (SettingsApi.TestAccountCustomerId != null &&
                CustomerContext.CurrentCustomer != null && 
                CustomerContext.CurrentCustomer.Id == SettingsApi.TestAccountCustomerId.Value)
            {
                if (SettingsApi.TestAccountSmsVerificationCode == _model.Code)
                    return new SmsPhoneConfirmationCodeResponse(true);
            }
            
            new SmsPhoneConfirmationService().ConfirmPhoneByCode(_phone, _model.Code);

            return new SmsPhoneConfirmationCodeResponse(true);
        }
    }
}