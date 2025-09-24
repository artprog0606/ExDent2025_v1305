using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Security;
using AdvantShop.ViewModel.User;
using AdvantShop.Core.Services.Mails;

namespace AdvantShop.Handlers.User
{
    public class RegistrationHandler
    {
        /// <summary>
        /// Is valid model
        /// </summary>
        /// <returns>null - on success, error string - on error</returns>
        public string IsValid(RegistrationViewModel model)
        {
            var isValid = true;

            if (SettingsMain.RegistrationIsProhibited)
                return LocalizationService.GetResource("User.Registration.ErrorRegistrationIsProhibited");

            if (SettingsCheckout.IsShowEmail || !SettingsOAuth.AuthByCodeActive)
            {
                isValid &=
                    SettingsCheckout.IsRequiredEmail
                        ? ValidationHelper.IsValidEmail(model.Email)
                        : model.Email.IsNullOrEmpty() || ValidationHelper.IsValidEmail(model.Email);
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && CustomerService.IsEmailExist(model.Email))
            {
                return string.Format(LocalizationService.GetResource("User.Registration.ErrorCustomerExist"), "forgotpassword");
            }

            isValid &= !String.IsNullOrWhiteSpace(model.PasswordConfirm) && !String.IsNullOrWhiteSpace(model.Password) &&
                       model.Password == model.PasswordConfirm;

            if (!isValid)
                return LocalizationService.GetResource("User.Registration.ErrorPasswordNotMatch");

            isValid &= model.Password.Length >= 6;
            if (!isValid)
                return LocalizationService.GetResource("User.Registration.PasswordLenght");

            if (SettingsCheckout.IsShowPhone && SettingsCheckout.IsRequiredPhone && String.IsNullOrWhiteSpace(model.Phone))
                isValid = false;

            if (SettingsCheckout.IsShowPhone && !String.IsNullOrWhiteSpace(model.Phone))
            {
                var standardPhone = StringHelper.ConvertToStandardPhone(HttpUtility.HtmlEncode(model.Phone));

                if (CustomerService.IsPhoneExist(model.Phone, standardPhone))
                    return LocalizationService.GetResource("User.Registration.ErrorCustomerPhoneExist");

                if (SettingsOAuth.AuthByCodeActive
                    && !new PhoneConfirmationService().IsPhoneConfirmed(standardPhone ?? 0, CustomerContext.CustomerId))
                {
                    return LocalizationService.GetResource("User.Registration.ErrorPhoneNotConfirmed");
                }
            }

            if (SettingsCheckout.IsShowLastName && SettingsCheckout.IsRequiredLastName && String.IsNullOrWhiteSpace(model.LastName))
                isValid = false;

            if (SettingsCheckout.IsShowPatronymic && SettingsCheckout.IsRequiredPatronymic && String.IsNullOrWhiteSpace(model.Patronymic))
                isValid = false;

            if (SettingsCheckout.IsShowBirthDay && SettingsCheckout.IsRequiredBirthDay && model.BirthDay == null)
                isValid = false;

            isValid &= !String.IsNullOrWhiteSpace(model.FirstName);
            
            if (SettingsCheckout.IsShowUserAgreementText && !model.Agree)
                return LocalizationService.GetResource("User.Registration.ErrorAgreement");

            if (BonusSystem.IsActive && model.WantBonusCard)
            {
                var bonusCard = BonusSystemService.GetCard(CustomerContext.CurrentCustomer.Id);
                if (bonusCard != null)
                    return "Бонусная карта уже используется";
            }

            if (!isValid)
                return LocalizationService.GetResource("User.Registration.Error");
            
            return null;
        }

        public void Register(RegistrationViewModel model)
        {
            var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
            {
                Id = CustomerContext.CustomerId,
                Password = HttpUtility.HtmlEncode(model.Password),
                FirstName = HttpUtility.HtmlEncode(model.FirstName),
                LastName = SettingsCheckout.IsShowLastName ? HttpUtility.HtmlEncode(model.LastName)  : "",
                Patronymic = SettingsCheckout.IsShowPatronymic ? HttpUtility.HtmlEncode(model.Patronymic)  : "",
                Phone = SettingsCheckout.IsShowPhone ? HttpUtility.HtmlEncode(model.Phone) : "",
                StandardPhone = StringHelper.ConvertToStandardPhone(SettingsCheckout.IsShowPhone ? HttpUtility.HtmlEncode(model.Phone) : ""),
                EMail = HttpUtility.HtmlEncode(model.Email),
                BirthDay = SettingsCheckout.IsShowBirthDay ? model.BirthDay : null,
                CustomerRole = Role.User,
                CustomerType = model.CustomerType,
                IsAgreeForPromotionalNewsletter = 
                    SettingsDesign.ShowUserAgreementForPromotionalNewsletter
                        ? model.UserAgreementForPromotionalNewsletter
                        : SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
            };

            CustomerService.InsertNewCustomer(customer, model.CustomerFields);
            if (!string.IsNullOrWhiteSpace(customer.EMail))
            {
                AuthorizeService.SignIn(customer.EMail, customer.Password, false, true);
            }
            else
            {
                AuthorizeService.SignInByPhone(customer.StandardPhone, customer.Password, false, true);
            }
            
            if (BonusSystem.IsActive && model.WantBonusCard)
            {
                try
                {
                    customer.BonusCardNumber = BonusSystemService.AddCard(new Card {CardId = customer.Id});
                    CustomerService.UpdateCustomer(customer);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            if (!CustomerContext.CurrentCustomer.IsVirtual)
            {
                var mail = new RegistrationMailTemplate(customer);
                MailService.SendMailNow(customer.Id, customer.EMail, mail);
                MailService.SendMailNow(SettingsMail.EmailForRegReport, mail, replyTo: customer.EMail);
            }

            ModulesExecuter.Registration(customer);
        }
    }
}