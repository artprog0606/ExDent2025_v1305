using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadModel : IValidatableObject
    {
        public int Id { get; set; }

        public Lead Lead { get; set; }

        public Currency LeadCurrency { get; set; }

        public List<SelectItemModel> Statuses { get; set; }
        public string SerializedStatuses
        {
            get
            {
                return JsonConvert.SerializeObject(this.Statuses);
            }
        }
        public int FinalStatusId { get; set; }
        public int CanceledStatusId { get; set; }

        public List<SelectListItem> LeadSources { get; set; }

        private List<SelectListItem> _managers;
        public List<SelectListItem> Managers
        {
            get
            {
                if (Lead == null || !Lead.ManagerId.HasValue)
                    return _managers;

                Manager manager;
                if (!_managers.Any(x => x.Value == Lead.ManagerId.Value.ToString()) && (manager = ManagerService.GetManager(Lead.ManagerId.Value)) != null)
                    _managers.Add(new SelectListItem { Text = manager.FullName, Value = manager.ManagerId.ToString() });

                return _managers;
            }
        }

        public long? StandardPhone
        {
            get
            {
                if (Lead != null)
                    return !string.IsNullOrEmpty(Lead.Phone)
                        ? StringHelper.ConvertToStandardPhone(Lead.Phone, true, true)
                        : (Lead.CustomerId != null && Lead.Customer != null
                            ? StringHelper.ConvertToStandardPhone(Lead.Customer.Phone, true, true)
                            : null);

                return null;
            }
        }

        public List<ISmsService> SmsModules { get; private set; }

        private List<CustomerFieldWithValue> _customerFields;
        public List<CustomerFieldWithValue> CustomerFields
        {
            get
            {
                if (_customerFields != null)
                    return _customerFields;

                _customerFields = new List<CustomerFieldWithValue>();

                if (Lead != null && Lead.CustomerId != null)
                {
                    _customerFields = CustomerFieldService.GetCustomerFieldsWithValue(Lead.CustomerId.Value) ??
                                      new List<CustomerFieldWithValue>();
                }

                return _customerFields;
            }
            set { _customerFields = value; }
        }
        public Dictionary<string, CustomerFieldWithValue> CustomerFieldsJs { get; set; }


        private List<LeadFieldWithValue> _leadFields;
        public List<LeadFieldWithValue> LeadFields
        {
            get { return _leadFields ?? (_leadFields = LeadFieldService.GetLeadFieldsWithValue(Lead != null ? Lead.Id : (int?)null)); }
            set { _leadFields = value; }
        }
        public Dictionary<string, LeadFieldWithValue> LeadFieldsJs { get; set; }

        public OrderTrafficSource TrafficSource { get; set; }
        public Order Order { get; set; }

        public VkUser VkUser { get; set; }
        public InstagramUser InstagramUser { get; set; }
        public FacebookUser FacebookUser { get; set; }
        public TelegramUser TelegramUser { get; set; }
        public OkUser OkUser { get; set; }

        public bool ShowTasks { get; private set; }
        public bool ShowVk { get; private set; }
        public bool ShowFacebook { get; private set; }
        public bool ShowInstagram { get; private set; }
        public bool ShowTelegram { get; private set; }
        public bool ShowOk { get; private set; }
        public bool ShowCustomerType { get; private set; }
        public SalesFunnel SalesFunnel { get; set; }
        public List<SelectListItem> SalesFunnels { get; private set; }

        public bool ShowViberDesktopAppNotification
        {
            get { return SettingsAdmin.ShowViberDesktopAppNotification; }
        }
        public bool ShowWhatsAppDesktopAppNotification
        {
            get { return SettingsAdmin.ShowWhatsAppDesktopAppNotification; }
        }
        public List<SelectListItem> CustomerTypes { get; private set; }

        public LeadModel()
        {
            LeadSources =
                OrderSourceService.GetOrderSources()
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                    .ToList();

            _managers = new List<SelectListItem>() { new SelectListItem { Text = "-", Value = "" } };
            _managers.AddRange(ManagerService.GetManagers(RoleAction.Crm).Select(x => new SelectListItem { Text = x.FullName, Value = x.ManagerId.ToString() }));

            SalesFunnels =
                SalesFunnelService.GetList()
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                    .ToList();

            LeadCurrency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

            SmsModules = SmsNotifier.GetAllSmsModules();

            ShowTasks = SettingsTasks.TasksActive;
            ShowVk = VkApiService.IsVkActive();
            ShowFacebook = new FacebookApiService().IsActive();
            ShowInstagram = Instagram.Instance.IsActive();
            ShowTelegram = new TelegramApiService().IsActive();
            ShowOk = OkApiService.IsActive();
            ShowCustomerType = SettingsCustomers.IsRegistrationAsPhysicalEntity && SettingsCustomers.IsRegistrationAsLegalEntity;
            CustomerTypes =
                Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Where(x => x != CustomerType.All).Select(x =>
                            new SelectListItem()
                            {
                                Text = x.Localize(),
                                Value = x.ToString()
                            }).ToList();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Lead == null)
                yield return new ValidationResult("Лид не найден");
        }
    }
}
