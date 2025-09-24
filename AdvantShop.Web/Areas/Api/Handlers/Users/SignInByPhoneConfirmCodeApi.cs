using System;
using System.Web;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Auth.Smses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class SignInByPhoneConfirmCodeApi : AbstractCommandHandler<SignInResponse>
    {
        private readonly SignInByPhoneConfirmCodeModel _model;
        private long _phone;
        private string _phoneStr;
        private Customer _customer;

        public SignInByPhoneConfirmCodeApi(SignInByPhoneConfirmCodeModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.Code))
                throw new BlException("Укажите sms-код");
            
            if (string.IsNullOrWhiteSpace(_model.Phone))
                throw new BlException("Укажите телефон");
            
            _phoneStr = HttpUtility.HtmlEncode(_model.Phone);
            
            var standardPhone = StringHelper.ConvertToStandardPhone(_phoneStr, true, true);

            if (standardPhone == null || standardPhone == 0)
                throw new BlException("Не валидный телефон");

            _phone = standardPhone.Value;

            _customer = CustomerService.GetCustomerByPhone(_phoneStr, _phone);
            
            if (!_model.SignUp)
            {
                if (_customer == null)
                    throw new BlException($"Пользователь с телефоном {_phoneStr} не зарегистрирован");
            }
        }

        protected override SignInResponse Handle()
        {
            var isTestCustomer = SettingsApi.TestAccountCustomerId != null &&
                                 _customer != null && _customer.Id == SettingsApi.TestAccountCustomerId.Value &&
                                 SettingsApi.TestAccountSmsVerificationCode == _model.Code;

            if (!isTestCustomer)
            {
                new PhoneConfirmationService().ConfirmPhoneByCode(_phone, _model.Code);

                if (_model.SignUp && _customer == null)
                {
                    if (SettingsMain.RegistrationIsProhibited)
                        throw new BlException(LocalizationService.GetResource("User.Registration.ErrorRegistrationIsProhibited"));
                    
                    _customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        CustomerRole = Role.User,
                        StandardPhone = _phone,
                        Phone = _phoneStr,
                        Password = StringHelper.GeneratePassword(8),
                        EMail = $"{_phone}{SmsCodeConfirmationRepository.TestEmailPrefix}"
                    };
                    CustomerService.InsertNewCustomer(_customer, processTriggers: false);

                    _customer = _customer.Id != Guid.Empty ? CustomerService.GetCustomer(_customer.Id) : null;

                    if (_customer == null)
                    {
                        Debug.Log.Error($"SignInByPhoneConfirmCodeApi не удалось зарегистрировать покупателя {_phone} {_phoneStr}");
                        throw new BlException("Ошибка при регистрации");
                    }
                }

                if (_customer != null && _customer.RegistredUser && string.IsNullOrEmpty(_customer.Password))
                {
                    var password = StringHelper.GeneratePassword(8);
                    _customer.Password = SecurityHelper.GetPasswordHash(password);
                    
                    CustomerService.UpdateCustomerPassword(_customer.Id, _customer.Password);
                }
            }

            new ApiAuthorizeService().SignIn(_customer, out string userKey, out string userId);

            return new SignInResponse()
            {
                UserKey = userKey,
                UserId = userId,
                Customer = new GetCustomerResponse(_customer)
            };
        }
    }
}