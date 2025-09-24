using System;
using System.Web;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Auth.Smses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class SmsPhoneConfirmation : AbstractCommandHandler<SmsPhoneConfirmationResponse>
    {
        private readonly string _phone;
        private readonly bool? _addHash;
        private long? _standardPhone;

        public SmsPhoneConfirmation(string phone, bool? addHash)
        {
            _phone = phone;
            _addHash = addHash;
        }
        
        public SmsPhoneConfirmation(Customer customer, bool? addHash)
        {
            if (customer != null)
            {
                _phone = customer.Phone;
                _standardPhone = customer.StandardPhone;
            }
            _addHash = addHash;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_phone))
                throw new BlException($"Укажите телефон");

            if (_standardPhone == null)
                _standardPhone = StringHelper.ConvertToStandardPhone(HttpUtility.HtmlEncode(_phone), true, true);
            
            if (_standardPhone == null || _standardPhone == 0)
                throw new BlException($"Не валидный телефон {_standardPhone}");
        }

        protected override SmsPhoneConfirmationResponse Handle()
        {
            if (SettingsApi.TestAccountCustomerId != null &&
                CustomerContext.CurrentCustomer != null && 
                CustomerContext.CurrentCustomer.Id == SettingsApi.TestAccountCustomerId.Value)
            {
                return new SmsPhoneConfirmationResponse() {IsCodeSended = true};
            }
            
            try
            {
                new SmsPhoneConfirmationService().SendCode(_standardPhone.Value, _addHash);
                
                return new SmsPhoneConfirmationResponse() {IsCodeSended = true};
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