using System;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Models.MyAccount;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.MyAccount
{
    public class SaveUserInfo : AbstractCommandHandler
    {
        private readonly UserInfoModel _userInfo;

        public SaveUserInfo(UserInfoModel userInfo)
        {
            _userInfo = userInfo;
        }

        protected override void Validate()
        {
            var valid = !string.IsNullOrWhiteSpace(_userInfo.FirstName);

            if (SettingsCheckout.IsRequiredLastName && string.IsNullOrWhiteSpace(_userInfo.LastName))
                valid = false;
            
            if (SettingsCheckout.IsRequiredPatronymic && string.IsNullOrWhiteSpace(_userInfo.Patronymic))
                valid = false;
            
            if (SettingsCheckout.IsRequiredPhone && string.IsNullOrWhiteSpace(_userInfo.Phone))
                valid = false;
            
            if (SettingsCheckout.IsRequiredBirthDay && _userInfo.BirthDay == null)
                valid = false;

            if (!valid)
                throw new BlException(LocalizationService.GetResource("MyAccount.SaveUserInfo.ErrorRequiredFields"));
            
            var customer = CustomerContext.CurrentCustomer;

            if (!string.IsNullOrWhiteSpace(_userInfo.Phone) && customer.Phone != _userInfo.Phone)
            {
                var standardPhone = StringHelper.ConvertToStandardPhone(_userInfo.Phone, true, true);

                if (standardPhone != null && CustomerService.IsPhoneExist(_userInfo.Phone, standardPhone))
                    throw new BlException("Пользователь с телефоном \"" + _userInfo.Phone + "\" уже существует");
            }
        }

        protected override void Handle()
        {
            try
            {
                var customer = CustomerContext.CurrentCustomer;
            
                customer.FirstName = HttpUtility.HtmlEncode(_userInfo.FirstName);
                customer.LastName = HttpUtility.HtmlEncode(_userInfo.LastName);
                customer.Patronymic = HttpUtility.HtmlEncode(_userInfo.Patronymic);
                customer.Phone = HttpUtility.HtmlEncode(_userInfo.Phone);
                customer.StandardPhone = StringHelper.ConvertToStandardPhone(_userInfo.Phone);
                customer.BirthDay = _userInfo.BirthDay;
                customer.IsAgreeForPromotionalNewsletter =
                    SettingsDesign.ShowUserAgreementForPromotionalNewsletter
                        ? _userInfo.IsAgreeForPromotionalNewsletter
                        : SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked;

                if (_userInfo.CustomerFields != null)
                {
                    foreach (var customerField in _userInfo.CustomerFields)
                    {
                        var field = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id, customerField.Id);
                        if (field == null || !field.Enabled || (field.DisableCustomerEditing && !string.IsNullOrEmpty(field.Value)))
                            continue;

                        CustomerFieldService.AddUpdateMap(customer.Id, customerField.Id, customerField.Value ?? "");
                    }
                }

                CustomerService.UpdateCustomer(customer);

                //в адресной книге редактировать имя нельзя, при оформлении данные берутся именно из контакта
                if (customer.Contacts.Any())
                {
                    foreach (var contact in customer.Contacts)
                    {
                        contact.Name = customer.GetFullName();
                        CustomerService.UpdateContact(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException(LocalizationService.GetResource("MyAccount.SaveUserInfo.Error"));
            }
        }
    }
}