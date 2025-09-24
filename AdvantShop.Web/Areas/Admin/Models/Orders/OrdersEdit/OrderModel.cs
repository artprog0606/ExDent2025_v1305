using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Attachments;
using AdvantShop.Web.Admin.Models.Settings.TemplatesDocxSettings;

namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class OrderModel : IValidatableObject
    {
        public OrderModel()
        {
            OrderStatuses =
                OrderStatusService.GetOrderStatuses()
                    .Select(x => new SelectListItem() { Text = x.StatusName, Value = x.StatusID.ToString() })
                    .ToList();

            OrderSources =
                OrderSourceService.GetOrderSources()
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                    .ToList();

            _managers = new List<SelectListItem>() { new SelectListItem() { Text = "-", Value = "" } };
            _managers.AddRange(ManagerService.GetManagers(RoleAction.Orders).Select(x => new SelectListItem() { Text = x.FullName, Value = x.ManagerId.ToString() }));

            OrderCurrency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

            var suggestionsModule = AttachedModules.GetModules<ISuggestions>().Select(x => (ISuggestions)Activator.CreateInstance(x)).FirstOrDefault();
            UseAddressSuggestions = suggestionsModule != null && suggestionsModule.SuggestAddressInAdmin;
            AddressSuggestionsUrl = suggestionsModule != null ? suggestionsModule.SuggestAddressUrl : null;

            TemplatesDocx = TemplatesDocxServices.GetList<OrderTemplateDocx>().Select(x => (TemplatesDocxModel)x).ToList();

            ShowTasks = SettingsTasks.TasksActive;
            ShowCustomerType = SettingsCustomers.IsRegistrationAsPhysicalEntity && SettingsCustomers.IsRegistrationAsLegalEntity;
            if(ShowCustomerType)
                CustomerTypes = Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Where(x => x != CustomerType.All).Select(x =>
                            new SelectListItem()
                            {
                                Text = x.Localize(),
                                Value = x.ToString()
                            }).ToList();
            
        }
        
        public Guid ContactId
        {
            get
            {
                if (Customer == null || Customer.Contacts.Count == 0)
                {
                    return Guid.Empty;
                }
                
                var contact = Customer.Contacts.FirstOrDefault(x => x.Country == Order.OrderCustomer.Country
                                                                    && x.Region == Order.OrderCustomer.Region
                                                                    && x.City == Order.OrderCustomer.City
                                                                    && x.District == Order.OrderCustomer.District);

                return contact != null ? contact.ContactId : Guid.Empty;
            }
        }

        public bool IsOrderContactExistInAddressList => ContactId != Guid.Empty;

        public int OrderId { get; set; }

        public bool IsPayed { get; set; }

        public Order Order { get; set; }

        public Currency OrderCurrency { get; set; }

        public Customer Customer { get; set; }
        
        public Customer LinkedCustomer { get; set; }

        public bool IsEditMode { get; set; }

        public OrderTrafficSource OrderTrafficSource { get; set; }

        public bool UseAddressSuggestions { get; private set; }
        public string AddressSuggestionsUrl { get; private set; }

        public float ProductsCost { get; set; }
        public float ProductsDiscountPrice { get; set; }
        public float CouponPrice { get; set; }


        public List<SelectListItem> OrderStatuses { get; set; }

        public List<SelectListItem> OrderSources { get; set; }

        private List<SelectListItem> _managers;
        public List<SelectListItem> Managers
        {
            get
            {
                if (Order == null || !Order.ManagerId.HasValue)
                    return _managers;

                Manager manager = null;
                if (!_managers.Any(x => x.Value == Order.ManagerId.Value.ToString()) && (manager = ManagerService.GetManager(Order.ManagerId.Value)) != null)
                    _managers.Add(new SelectListItem { Text = manager.FullName, Value = manager.ManagerId.ToString() });

                return _managers;
            }
        }

        public bool ShowManager
        {
            get
            {
                var c = CustomerContext.CurrentCustomer;
                return c.IsAdmin || (c.IsModerator && c.HasRoleAction(RoleAction.Crm));
            }
        }

        public Card BonusCard { get; set; }

        //public Purchase BonusCardPurchase { get; set; }
        //public bool UseBonuses { get; set; }

        public int? NextOrderId { get; set; }
        public int? PrevOrderId { get; set; }


        public bool ShowCrm
        {
            get
            {
                return SettingsCrm.CrmActive && (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm);
            }
        }

        public bool EnableMangers => !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm;

        public bool ShowTasks { get; private set; }

        //public bool ShowActivity
        //{
        //    get
        //    {
        //        return !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog;
        //    }
        //}

        public bool ShowTelelephony
        {
            get
            {
                return SettingsTelephony.PhonerLiteActive &&
                       (!SaasDataService.IsSaasEnabled ||
                        (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTelephony));
            }
        }

        public long? StandardPhone
        {
            get
            {
                return Order != null && Order.OrderCustomer != null ? Order.OrderCustomer.StandardPhone : null;
            }
        }

        public List<ISmsService> SmsModules
        {
            get { return SmsNotifier.GetAllSmsModules(); }
        }

        private List<CustomerFieldWithValue> _customerFields;

        public List<CustomerFieldWithValue> CustomerFields
        {
            get
            {
                if (_customerFields != null)
                    return _customerFields;
                
                if (Order != null && Order.OrderCustomer != null)
                {
                    var customer = CustomerService.GetCustomer(Order.OrderCustomer.CustomerID);
                    _customerFields = CustomerFieldService.GetCustomerFieldsWithValue(customer?.Id ?? Guid.Empty);
                }

                return _customerFields;
            }
            set { _customerFields = value; }
        }

        public string LpLink { get; set; }
  
        public List<TemplatesDocxModel> TemplatesDocx { get; set; }

        public bool ShowViberDesktopAppNotification
        {
            get { return SettingsAdmin.ShowViberDesktopAppNotification; }
        }
        public bool ShowWhatsAppDesktopAppNotification
        {
            get { return SettingsAdmin.ShowWhatsAppDesktopAppNotification; }
        }        
        public bool ShowTelegramDesktopAppNotification
        {
            get { return SettingsAdmin.ShowTelegramDesktopAppNotification; }
        }
        public bool ShowCustomerType { get; set; }
        public List<SelectListItem> CustomerTypes { get; set; }
        public string FilesHelpText { get; set; }
        public string AllowedFileExtensions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsEditMode && Order == null)
            {
                yield return new ValidationResult("Заказ не найден", new[] { "Order" });
            }
        }
    }
}