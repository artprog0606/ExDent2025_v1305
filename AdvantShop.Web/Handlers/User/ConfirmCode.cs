using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Security;
using AdvantShop.ViewModel.User;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.User
{
    public class ConfirmCode : AbstractCommandHandler<ConfirmCodeModel>
    {
        private readonly string _phoneStr;
        private readonly string _code;
        private readonly bool _signUp;
        private readonly UrlHelper _urlHelper;
        private readonly IPhoneConfirmationService _phoneCodeConfirmationService;
        
        private long _phone;
        
        public const string WrongConfirmCount = "login_auth_count";
        
        public ConfirmCode(string phoneStr, string code, bool signUp)
        {
            _phoneStr = phoneStr;
            _code = code;
            _signUp = signUp;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _phoneCodeConfirmationService = new PhoneConfirmationService();
        }

        protected override void Validate()
        {
            if (!SettingsOAuth.AuthByCodeActive)
                throw new BlException("Авторизация по коду не активна");
            
            _phone = StringHelper.ConvertToStandardPhone(_phoneStr) ?? 0;
            if (_phone == 0)
                throw new BlException("Введите корректный номер телефона");
            
            if (!_signUp)  // если авторизация, то проверяем, что пользователь существует
            {
                var customer = CustomerService.GetCustomerByPhone(_phoneStr, _phone);
                if (customer == null)
                {
                    IncreaseErrorCount(_phone);
                    throw new BlException($"Пользователь с телефоном {_phoneStr} не найден");
                }
            }

            if (string.IsNullOrWhiteSpace(_code))
            {
                IncreaseErrorCount(_phone);
                throw new BlException("Не правильный код");
            }
            
            if (_phoneCodeConfirmationService.IsBannedByPhoneOrIp(_phone, HttpContext.Current.TryGetIp()))
                throw new BlException("Слишком много попыток");
        }

        protected override ConfirmCodeModel Handle()
        {
            try
            {
                _phoneCodeConfirmationService.ConfirmPhoneByCode(_phone, _code);
                
                HttpContext.Current.Session[WrongConfirmCount] = null;
                
                if (!_signUp)
                    return AuthByPhone();
                
                _phoneCodeConfirmationService.SetPhoneConfirmedState(_phone, CustomerContext.CustomerId);
                
                return new ConfirmCodeModel();
                
            }
            catch (BlException)
            {
                IncreaseErrorCount(_phone);
                throw;
            }
        }

        private ConfirmCodeModel AuthByPhone()
        {
            var customersByPhone = CustomerService.GetCustomersByPhone(_phoneStr);
            if (customersByPhone.Count > 1)
                customersByPhone = customersByPhone
                    .OrderByDescending(x => CustomerService.GetCustomerLastOrderDate(x.Id))
                    .ThenByDescending(x => x.RegistrationDateTime)
                    .ToList();

            var customer = customersByPhone.FirstOrDefault();

            if (customer == null)
                return new ConfirmCodeModel()
                {
                    RedirectTo = _urlHelper.AbsoluteRouteUrl("registration")
                };

            if (customer.Password.IsNullOrEmpty())
            {
                var password = StringHelper.GeneratePassword(8);
                customer.Password = SecurityHelper.GetPasswordHash(password);
                CustomerService.UpdateCustomerPassword(customer.Id, customer.Password);
            }

            AuthorizeService.SignInByPhone(customer.StandardPhone, customer.Password, true, true);

            return new ConfirmCodeModel()
            {
                RedirectTo = !customer.IsAdmin && !customer.IsModerator
                    ? _urlHelper.AbsoluteRouteUrl("myaccount")
                    : null
            };
        }

        private void IncreaseErrorCount(long phone)
        {
            IncreaseErrorCount(phone, _phoneCodeConfirmationService);
        }

        public static void IncreaseErrorCount(long phone, IPhoneConfirmationService phoneConfirmationService)
        {
            var count = Convert.ToInt32(HttpContext.Current.Session[WrongConfirmCount]) + 1;
            HttpContext.Current.Session[WrongConfirmCount] = count;

            if (count > 15)
            {
                phoneConfirmationService.Ban(phone, HttpContext.Current.TryGetIp(), DateTime.Now.AddHours(1));
                
                HttpContext.Current.Session[WrongConfirmCount] = null;
            }
        }
    }
}