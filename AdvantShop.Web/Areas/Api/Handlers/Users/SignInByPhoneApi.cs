using System;
using System.Web;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class SignInByPhoneApi : AbstractCommandHandler<SignInByPhoneResponse>
    {
        private readonly SignInByPhoneModel _model;
        private long _phone;

        public SignInByPhoneApi(SignInByPhoneModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model.SignUp && SettingsMain.RegistrationIsProhibited)
                throw new BlException(LocalizationService.GetResource("User.Registration.ErrorRegistrationIsProhibited"));
            
            if (string.IsNullOrWhiteSpace(_model.Phone))
                throw new BlException("Укажите телефон");

            var phoneStr = HttpUtility.HtmlEncode(_model.Phone);
            
            var standardPhone = StringHelper.ConvertToStandardPhone(phoneStr, true, true);

            if (standardPhone == null || standardPhone == 0)
                throw new BlException("Не валидный телефон");

            _phone = standardPhone.Value;

            if (!_model.SignUp)
            {
                var customer = CustomerService.GetCustomerByPhone(phoneStr, _phone);
                if (customer == null)
                    throw new BlException($"Пользователь с телефоном {phoneStr} не зарегистрирован");
            }
        }

        protected override SignInByPhoneResponse Handle()
        {
            if (SettingsApi.TestAccountCustomerId != null)
            {
                var customer = CustomerService.GetCustomerByPhone(_model.Phone, _phone);
                
                if (customer != null && customer.Id == SettingsApi.TestAccountCustomerId.Value)
                {
                    return new SignInByPhoneResponse() {IsCodeSended = true};
                }
            }

            try
            {
                new PhoneConfirmationService().SendCode(_phone, _model.AddHash);
                
                return new SignInByPhoneResponse() { IsCodeSended = true };
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException("Ошибка при отправки смс");
            }
        }
    }
}