using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public class CustomersModel : IValidatableObject
    {
        public CustomersModel()
        {
            CustomerContact = new CustomerContact();

            CustomerGroups =
                CustomerGroupService.GetCustomerGroupList().Select(x =>
                            new SelectListItem()
                            {
                                Text = string.Format("{0} - {1}%", x.GroupName, x.GroupDiscount),
                                Value = x.CustomerGroupId.ToString()
                            }).ToList();

            CustomerTypes =
                Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Where(x => x != CustomerType.All).Select(x =>
                            new SelectListItem()
                            {
                                Text = x.Localize(),
                                Value = x.ToString()
                            }).ToList();

            _managers = new List<SelectListItem>() { new SelectListItem() { Text = "-", Value = "" } };
            _managers.AddRange(ManagerService.GetManagers(RoleAction.Customers).Select(x => new SelectListItem() { Text = x.FullName, Value = x.ManagerId.ToString() }));

            var suggestionsModule = AttachedModules.GetModules<ISuggestions>().Select(x => (ISuggestions)Activator.CreateInstance(x)).FirstOrDefault();
            UseAddressSuggestions = suggestionsModule != null && suggestionsModule.SuggestAddressInAdmin;
            AddressSuggestionsUrl = suggestionsModule != null ? suggestionsModule.SuggestAddressUrl : null;

            ShowTelelephony = SettingsTelephony.PhonerLiteActive &&
                              (!SaasDataService.IsSaasEnabled ||
                               (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTelephony));
        }

        public Guid CustomerId { get; set; }

        public bool IsEditMode { get; set; }

        public bool AllowForceSave { get; set; }

        public Customer Customer { get; set; }

        public CustomerContact CustomerContact { get; set; }

        public List<SelectListItem> CustomerGroups { get; set; }

        public int? PartnerId { get; set; }

        public int? OrderId { get; set; }

        private List<SelectListItem> _managers;
        public List<SelectListItem> Managers
        {
            get
            {
                if (Customer == null || !Customer.ManagerId.HasValue)
                    return _managers;

                Manager manager = null;
                if (!_managers.Any(x => x.Value == Customer.ManagerId.Value.ToString()) && (manager = ManagerService.GetManager(Customer.ManagerId.Value)) != null)
                    _managers.Add(new SelectListItem { Text = manager.FullName, Value = manager.ManagerId.ToString() });

                return _managers;
            }
        }

        public ShoppingCart ShoppingCart { get; set; }

        public bool UseAddressSuggestions { get; private set; }
        public string AddressSuggestionsUrl { get; private set; }

        public bool ShowTelelephony { get; private set; }
        
        public bool ShowManager
        {
            get
            {
                var c = CustomerContext.CurrentCustomer;
                return c.IsAdmin || (c.IsModerator && c.HasRoleAction(RoleAction.Crm));
            }
        }
        

        private List<CustomerFieldWithValue> _customerFields;

        public List<CustomerFieldWithValue> CustomerFields
        {
            get
            {
                if (_customerFields != null)
                    return _customerFields;

                _customerFields = CustomerFieldService.GetCustomerFieldsWithValue(CustomerId) ??
                                  new List<CustomerFieldWithValue>();

                return _customerFields;
            }
            set { _customerFields = value; }
        }

        public Dictionary<string, CustomerFieldWithValue> CustomerFieldsJs { get; set; }
        
        public VkUser VkUser { get; set; }
        public InstagramUser InstagramUser { get; set; }
        public FacebookUser FacebookUser { get; set; }
        public OkUser OkUser { get; set; }

        public List<string> Tags { get; set; }
        public bool IsEnabledBothTypesOfCustomers { get;set; }
        public CustomerType CustomerType { get; set; }
        public List<SelectListItem> CustomerTypes { get; set; }
        
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Customer == null)
                yield return new ValidationResult("Пользователь не найден.");

            if (IsEditMode && Customer != null)
            {
                var c = CustomerService.GetCustomer(CustomerId);
                if (c == null)
                    yield return new ValidationResult("Пользователь не найден");

                if (c != null)
                {
                    if (!string.IsNullOrEmpty(Customer.EMail) && Customer.EMail != c.EMail && CustomerService.ExistsEmail(Customer.EMail))
                        yield return new ValidationResult("Введенный Email занят");

                    if (!string.IsNullOrWhiteSpace(Customer.Phone) && Customer.Phone != c.Phone)
                    {
                        var standardPhone = StringHelper.ConvertToStandardPhone(Customer.Phone, true, true);

                        if (standardPhone != null && CustomerService.IsPhoneExist(Customer.Phone, standardPhone))
                            yield return new ValidationResult("Пользователь с телефоном \"" + Customer.Phone + "\" уже существует");
                    }

                    if (Customer.BonusCardNumber.HasValue && BonusSystem.IsActive &&
                        Customer.BonusCardNumber != c.BonusCardNumber && BonusSystemService.GetCard(Customer.BonusCardNumber) == null)
                        yield return new ValidationResult("Бонусной карты с таким номером не существует");
                }
            }

            if (!IsEditMode && Customer != null)
            {
                if (!string.IsNullOrWhiteSpace(Customer.EMail) && CustomerService.ExistsEmail(Customer.EMail))
                {
                    yield return new ValidationResult("Пользователь с почтой \"" + Customer.EMail + "\" уже существует");
                }

                if (!string.IsNullOrWhiteSpace(Customer.Phone))
                {
                    var standardPhone = StringHelper.ConvertToStandardPhone(Customer.Phone, true, true);

                    if (standardPhone != null && CustomerService.IsPhoneExist(Customer.Phone, standardPhone))
                        yield return new ValidationResult("Пользователь с телефоном \"" + Customer.Phone + "\" уже существует");
                }
            }
        }
    }
}
