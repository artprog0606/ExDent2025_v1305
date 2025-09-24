using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Models.User;
using AdvantShop.ViewModel.Auth;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.User
{
    public class SendCode : AbstractCommandHandler<SendCodeResponse>
    {
        private readonly SendCodeModel _model;

        private string _phoneStr;
        private long _phone;
        private readonly IPhoneConfirmationService _phoneConfirmationService; 

        public SendCode(SendCodeModel model)
        {
            _model = model;
            _phoneConfirmationService = new PhoneConfirmationService();
        }

        protected override void Validate()
        {
            if (!SettingsOAuth.AuthByCodeActive)
                throw new BlException("Авторизация по коду не активна");
            
            if (string.IsNullOrEmpty(_model.Phone))
                throw new BlException("Пустой номер телефона");
            
            _phoneStr = HttpUtility.HtmlEncode(_model.Phone);

            _phone = StringHelper.ConvertToStandardPhone(_phoneStr) ?? 0;
            if (_phone == 0)
                throw new BlException("Введите корректный номер телефона");

            if (!_model.SignUp) // если авторизация, то проверяем, что пользователь существует
            {
                var customer = CustomerService.GetCustomerByPhone(_phoneStr, _phone);
                if (customer == null)
                    throw new BlException($"Пользователь с телефоном {_phoneStr} не найден");
            }
            
            var result = _phoneConfirmationService.IsModuleActive();
            if (!result.IsSuccess)
                throw new BlException(result.Error.Message);
        }

        protected override SendCodeResponse Handle()
        {
            try
            {
                _phoneConfirmationService.SendCode(_phone, null);
            }
            catch (BlException ex)
            {
                ConfirmCode.IncreaseErrorCount(_phone, _phoneConfirmationService);
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException("Ошибка при отправке кода аутентификации");
            }
            
            return new SendCodeResponse() {SecondsToRetry = PhoneConfirmationConfig.SecondsPerPhoneBetweenMessage};
        }
    }
}