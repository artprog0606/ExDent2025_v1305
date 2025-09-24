//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using Newtonsoft.Json;

namespace AdvantShop.Customers
{
    [Serializable]
    public class Customer : ICustomer, ITriggerObject
    {
        public Customer()
        {
            Id = Guid.Empty;
            Enabled = true;
        }

        public Customer(int groupId) : this()
        {
            CustomerGroupId = groupId;
        }

        public Customer(bool isRegistered) : this()
        {
            RegistredUser = isRegistered;
        }

        public Guid Id { get; set; }

        public int InnerId { get; set; }

        private int _customerGroupId = 0;

        [Compare("Core.Customers.Customer.CustomerGroup", ChangeHistoryParameterType.CustomerGroup)]
        public int CustomerGroupId
        {
            get
            {
                if (_customerGroupId != 0)
                    return _customerGroupId;
                return (_customerGroupId = CustomerGroupService.DefaultCustomerGroup);
            }
            set => _customerGroupId = value;
        }

        public string Password { get; set; }

        [Compare("Core.Customers.Customer.FirstName")]
        public string FirstName { get; set; }

        [Compare("Core.Customers.Customer.LastName")]
        public string LastName { get; set; }

        [Compare("Core.Customers.Customer.Patronymic")]
        public string Patronymic { get; set; }

        [Compare("Core.Customers.Customer.Organization")]
        public string Organization { get; set; }

        [Compare("Core.Customers.Customer.Phone")]
        public string Phone { get; set; }

        [Compare("Core.Customers.Customer.StandardPhone")]
        public long? StandardPhone { get; set; }

        public DateTime RegistrationDateTime { get; set; }

        [Compare("Core.Customers.Customer.Email")]
        public string EMail { get; set; }

        private bool? _subscribedForNews;

        [Obsolete]
        public bool SubscribedForNews
        {
            get => IsAgreeForPromotionalNewsletter;
            set => IsAgreeForPromotionalNewsletter = value;
        }

        [Compare("Core.Customers.Customer.IsAgreeForPromotionalNewsletter")]
        public bool IsAgreeForPromotionalNewsletter
        {
            // логика как у соглашения на рассылки — но через функционал подписок на новости.
            get => _subscribedForNews ?? (_subscribedForNews = SubscriptionService.IsSubscribe(EMail)).Value;
            set => _subscribedForNews = value;
        }
        
        private Subscription _subscriptionForNews;
        [JsonIgnore]
        public Subscription SubscriptionForNews => 
            _subscriptionForNews ?? (_subscriptionForNews = SubscriptionService.GetSubscription(EMail));

        [Compare("Core.Customers.Customer.BonusCardNumber")]
        public long? BonusCardNumber { get; set; }

        [Compare("Core.Customers.Customer.AdminComment")]
        public string AdminComment { get; set; }

        [Compare("Core.Customers.Customer.Rating")]
        public int Rating { get; set; }

        [Compare("Core.Customers.Customer.Role")]
        public Role CustomerRole { get; set; }

        private CustomerGroup _customerGroup;
        
        public CustomerGroup CustomerGroup =>
            _customerGroup ??
            (_customerGroup = CustomerGroupService.GetCustomerGroup(CustomerGroupId)) ??
            (_customerGroup = CustomerGroupService.GetDefaultCustomerGroup()) ??
            new CustomerGroup() {CustomerGroupId = CustomerGroupService.DefaultCustomerGroup};

        private List<CustomerContact> _contacts;
        
        public List<CustomerContact> Contacts
        {
            get => _contacts ?? (_contacts = CustomerService.GetCustomerContacts(Id));
            set => _contacts = value;
        }
        
        public bool IsAdmin => CustomerRole == Role.Administrator;

        public bool IsModerator => CustomerRole == Role.Moderator;

        private bool? _isManager;
        
        [Compare("Core.Customers.Customer.IsManager")]
        public bool IsManager
        {
            get
            {
                if (!_isManager.HasValue)
                    _isManager = ManagerService.CustomerIsManager(Id);

                return _isManager.Value;
            }
        }

        public bool HasRoleAction(RoleAction key)
        {
            return IsAdmin || IsVirtual || (IsModerator && RoleActionService.HasCustomerRoleAction(Id, key));
        }

        public bool IsBuyer => (!RegistredUser && !IsVirtual) || (!IsAdmin && !IsModerator && !IsManager);

        public bool CanBeDeleted => CustomerService.CanDelete(Id);

        public bool RegistredUser { get; protected set; }
        public void SetRegistered() => RegistredUser = true;

        public bool IsVirtual { get; set; }

        /// <summary>
        /// Customer's manager ID
        /// </summary>
        [Compare("Core.Customers.Customer.Manager", ChangeHistoryParameterType.Manager)]
        public int? ManagerId { get; set; }

        [Compare("Core.Customers.Customer.Enabled")]
        public bool Enabled { get; set; }

        public Guid? HeadCustomerId { get; set; }

        [Compare("Core.Customers.Customer.BirthDay")]
        public DateTime? BirthDay { get; set; }

        public string BirthDayFormatted => BirthDay != null ? Culture.ConvertDateWithoutHours(BirthDay.Value) : null;

        public string City { get; set; }

        public string Avatar { get; set; }

        [Compare("Core.Customers.Customer.Code")]
        public string Code { get; set; }
        
        private Manager _manager;

        /// <summary>
        /// Customer's manager
        /// </summary>
        [JsonIgnore]
        public Manager Manager => _manager ?? (_manager = ManagerId.HasValue ? ManagerService.GetManager(ManagerId.Value) : null);

        [Compare("Core.Customers.Customer.SortOrder")]
        public int SortOrder { get; set; }

        [Compare("Core.Customers.Customer.ClientStatus")]
        public CustomerClientStatus ClientStatus { get; set; }
        
        public CustomerType CustomerType { get; set; }

        public string RegisteredFrom { get; set; }
        public string RegisteredFromIp { get; set; }
        
        public string FcmToken { get; set; }

        public TriggerProcessObject GetTriggerProcessObject()
        {
            return new TriggerProcessObject()
            {
                EntityId = InnerId,
                Email = EMail,
                Phone = StandardPhone ?? StringHelper.ConvertToStandardPhone(Phone) ?? 0,
                CustomerId = Id,
            };
        }

        private IList<Tag> _tag;
        public IList<Tag> Tags
        {
            get => _tag ?? (_tag = TagService.Gets(Id,  true));
            set => _tag = value;
        }
        
        
        private string _customerCompanyName;
        public string CustomerCompanyName 
        { 
            get 
            {
                if (!string.IsNullOrEmpty(_customerCompanyName))
                    return _customerCompanyName;
                
                if (Id == Guid.Empty || CustomerType != CustomerType.LegalEntity)
                    return string.Empty;
                
                var customerField = CustomerFieldService.GetCustomerFieldWithValueByFieldAssignment(Id, CustomerFieldAssignment.CompanyName);

                return _customerCompanyName = customerField?.Value;  
            }
        }
    }


    public enum CustomerClientStatus
    {
        None = 0,
        Vip = 1,
        Bad = 2
    }
}