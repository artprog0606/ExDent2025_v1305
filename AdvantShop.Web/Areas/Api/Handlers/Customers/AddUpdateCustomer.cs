using System;
using System.Linq;
using AdvantShop.Areas.Api.Handlers.Users;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Auth.Smses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class AddUpdateCustomer : AbstractCommandHandler<AddUpdateCustomerResponse>
    {
        #region Ctor

        private readonly AddUpdateCustomerModel _model;
        
        /// <summary>
        /// Игнорировать активность и редактирование доп полей покупателя?
        /// Если true, то доп поле отредактируется, если null|false, то сначала проверим можно ли его отредактировать
        /// </summary>
        private readonly bool? _ignoreCustomerFieldsConstraints;
        
        private readonly bool _isEditMode;
        private Customer _customer;
        
        public AddUpdateCustomer(AddUpdateCustomerModel model, AddUpdateCustomerCommand command) 
                                                                                : this(Guid.Empty, model, command) { }
        
        public AddUpdateCustomer(Guid id, AddUpdateCustomerModel model, AddUpdateCustomerCommand command) : this(id, model)
        {
            if (command != null)
            {
                _ignoreCustomerFieldsConstraints = command.IgnoreCustomerFieldsConstraints;
            }
        }

        public AddUpdateCustomer(Guid id, AddUpdateCustomerModel model)
        {
            _model = model;
            _model.Id = id;
            _isEditMode = id != Guid.Empty;
        }

        #endregion

        protected override void Validate()
        {
            if (!string.IsNullOrWhiteSpace(_model.Email) && !ValidationHelper.IsValidEmail(_model.Email))
                throw new BlException("Не валидный email");

            if (!_isEditMode)
            {
                if (SettingsMain.RegistrationIsProhibited)
                    throw new BlException(LocalizationService.GetResource("User.Registration.ErrorRegistrationIsProhibited"));
                
                if (!string.IsNullOrWhiteSpace(_model.Email) && CustomerService.IsEmailExist(_model.Email))
                    throw new BlException("Пользователь с таким email уже существует");

                if (_model.Phone != null)
                {
                    var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone.ToString(), true, true);
                    var phone = standardPhone != null ? standardPhone.ToString() : _model.Phone.ToString();

                    if (CustomerService.IsPhoneExist(phone, standardPhone))
                        throw new BlException("Пользователь с таким телефоном уже существует");
                }
            }
            else
            {
                _customer = CustomerService.GetCustomer(_model.Id);
                if (_customer == null)
                    throw new BlException("Покупатель не найден");

                if (_customer.EMail != _model.Email && !string.IsNullOrWhiteSpace(_model.Email) && CustomerService.IsEmailExist(_model.Email))
                    throw new BlException("Пользователь с таким email уже существует");

                if (_model.Phone != null)
                {
                    var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone.ToString(), true, true);
                    var phone = standardPhone != null ? standardPhone.ToString() : _model.Phone.ToString();

                    if (_customer.StandardPhone != standardPhone && CustomerService.IsPhoneExist(phone, standardPhone))
                        throw new BlException("Пользователь с таким телефоном уже существует");
                }
            }
        }

        private AddUpdateCustomerResponse AddUpdate()
        {
            var customer = _customer ??
                           new Customer(CustomerGroupService.DefaultCustomerGroup) {CustomerRole = Role.User};

            var processTriggers = false;
            
            if (!string.IsNullOrEmpty(_model.Email))
            {
                if (customer.EMail.IsNotEmpty() &&
                    customer.EMail != _model.Email &&
                    customer.EMail.Contains(SmsCodeConfirmationRepository.TestEmailPrefix) &&
                    !_model.Email.Contains(SmsCodeConfirmationRepository.TestEmailPrefix))
                {
                    processTriggers = true;
                }
                
                customer.EMail = _model.Email.EncodeOrEmpty();
            }

            if (_model.FirstName != null)
                customer.FirstName = _model.FirstName.EncodeOrEmpty();

            if (_model.LastName != null)
                customer.LastName = _model.LastName.EncodeOrEmpty();

            if (_model.Patronymic != null)
                customer.Patronymic = _model.Patronymic.EncodeOrEmpty();

            if (_model.Organization != null)
                customer.Organization = _model.Organization.EncodeOrEmpty(true);

            if (_model.Phone != null)
            {
                var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone.ToString(), true, true);
                var phone = standardPhone != null ? standardPhone.ToString() : _model.Phone.ToString();
                
                customer.Phone = standardPhone != null ? standardPhone.ToString() : phone;
                customer.StandardPhone = standardPhone;
            }

            if (_model.BirthDay != null)
                customer.BirthDay = _model.BirthDay;

            if (_model.AdminComment != null)
                customer.AdminComment = _model.AdminComment.EncodeOrEmpty(true);

            if (_model.ManagerId != null && ManagerService.GetManager(_model.ManagerId.Value) != null)
                customer.ManagerId = _model.ManagerId;

            if (_model.GroupId != null && CustomerGroupService.GetCustomerGroup(_model.GroupId.Value) != null)
                customer.CustomerGroupId = _model.GroupId.Value;

            if (!string.IsNullOrEmpty(_model.CustomerType) &&
                Enum.TryParse(_model.CustomerType, true, out CustomerType customerType))
            {
                customer.CustomerType = customerType;
            }

            if (_model.IsAgreeForPromotionalNewsletter != null)
                customer.IsAgreeForPromotionalNewsletter = _model.IsAgreeForPromotionalNewsletter.Value;

            if (!_isEditMode)
            {
                customer.Password = _model.Password.DefaultOrEmpty();

                customer.Id = CustomerService.InsertNewCustomer(customer);

                if (customer.Id == Guid.Empty)
                    throw new BlException("Не удалось создать пользователя");

                if (_model.PartnerId.HasValue && PartnerService.GetPartner(_model.PartnerId.Value) != null)
                    PartnerService.AddBindedCustomer(new BindedCustomer { CustomerId = customer.Id, PartnerId = _model.PartnerId.Value });
            }
            else
            {
                CustomerService.UpdateCustomer(customer);
                
                if (processTriggers)
                    TriggerProcessService.ProcessEvent(ETriggerEventType.CustomerCreated, customer);
            }

            if (_model.Fields != null)
            {
                foreach (var field in _model.Fields)
                {
                    var cf = CustomerFieldService.GetCustomerField(field.Id);
                    if (cf == null)
                        continue;

                    if (_ignoreCustomerFieldsConstraints == null || _ignoreCustomerFieldsConstraints == false)
                    {
                        if (!cf.Enabled || cf.DisableCustomerEditing)
                            continue;
                    }

                    CustomerFieldService.AddUpdateMap(customer.Id, field.Id, field.Value ?? "", !_isEditMode);
                }
            }

            if (_model.Contact != null && !IsEmptyContact(_model.Contact))
                new MeAddUpdateContact(customer, _model.Contact, _model.Contact.ContactId == Guid.Empty).Execute();

            if (_model.Contacts != null)
            {
                foreach (var contactModel in _model.Contacts.Where(x => x != null && !IsEmptyContact(x)))
                {
                    new MeAddUpdateContact(customer, contactModel, contactModel.ContactId == Guid.Empty).Execute();
                }
            }

            if (_isEditMode || !string.IsNullOrWhiteSpace(_model.FcmToken))
            {
                CustomerService.UpdateFcmToken(customer.Id, _model.FcmToken);
            }

            return new AddUpdateCustomerResponse() { Id = customer.Id };
        }

        // Проверям только на улицу, дом и тд. потому, что костыль:
        // есть запросы когда приходит пустой contactId и страна, город или пустой contactId и остальные null
        // и на такие запросы не нужно создавать contact
        private bool IsEmptyContact(CustomerContactModel contact)
        {
            return contact.ContactId == Guid.Empty &&
                   string.IsNullOrWhiteSpace(contact.Street) &&
                   string.IsNullOrWhiteSpace(contact.House) &&
                   string.IsNullOrWhiteSpace(contact.Apartment) &&
                   string.IsNullOrWhiteSpace(contact.Structure) &&
                   string.IsNullOrWhiteSpace(contact.Entrance) &&
                   string.IsNullOrWhiteSpace(contact.Floor);
        }

        protected override AddUpdateCustomerResponse Handle()
        {
            try
            {
                return AddUpdate();
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}