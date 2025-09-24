using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Customers;

namespace AdvantShop.Web.Admin.Handlers.Customers
{
    public class GetCustomer
    {
        private readonly int _orderId;
        private readonly int? _partnerId;
        private readonly Guid _customerId;
        private readonly string _clientCode;
        private readonly bool _isEditMode;
        
        public GetCustomer(Guid customerId, string clientCode = null)
        {
            _customerId = customerId;
            _clientCode = clientCode;
            _isEditMode = true;
        }

        public GetCustomer(AddCustomerModel addCustomerModel)
        {
            if (addCustomerModel.OrderId != null)
                _orderId = addCustomerModel.OrderId.Value;
            _partnerId = addCustomerModel.PartnerId;
        }

        public CustomersModel Execute()
        {
            var model = new CustomersModel()
            {
                CustomerId = _customerId,
                IsEditMode = _isEditMode,
                PartnerId = _partnerId,
                OrderId = _orderId
            };

            if (_orderId != 0)
            {
                var orderCustomer = OrderService.GetOrderCustomer(_orderId);
                if (orderCustomer != null)
                {
                    model.AllowForceSave = true;
                    model.CustomerId = orderCustomer.CustomerID;
                    model.Customer = (Customer)orderCustomer;

                    if (model.Customer.Contacts != null && model.Customer.Contacts.Count > 0)
                        model.CustomerContact = model.Customer.Contacts[0];
                }
            }

            if (!_isEditMode)
            {
                if (model.Customer == null)
                    model.Customer = new Customer();

                var manager = ManagerService.GetManager(CustomerContext.CurrentCustomer.Id);
                if (manager != null)
                {
                    var m = model.Managers.Find(x => x.Value == manager.ManagerId.ToString());
                    if (m != null)
                        model.Customer.ManagerId = manager.ManagerId;
                }
                model.IsEnabledBothTypesOfCustomers = SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity;
                if (!model.IsEnabledBothTypesOfCustomers)
                    model.CustomerType = SettingsCustomers.IsRegistrationAsPhysicalEntity ? CustomerType.PhysicalEntity : CustomerType.LegalEntity;
                else
                    model.CustomerType = model.Customer.CustomerType;

                return model;
            }

            var customer = CustomerService.GetCustomer(_customerId);
            if (customer == null && _clientCode.IsNotEmpty())
            {
                customer = ClientCodeService.GetCustomerByCode(_clientCode, _customerId);
                customer.Code = _clientCode;
                model.IsEditMode = false;
            }

            if (customer == null || !CustomerService.CheckAccess(customer))
                return null;

            model.Customer = customer;

            if (customer.Contacts != null && customer.Contacts.Count > 0)
                model.CustomerContact = customer.Contacts[0];

            model.ShoppingCart = ShoppingCartService.GetShoppingCartFromDb(ShoppingCartType.ShoppingCart, customer.Id, false);
            
            model.VkUser = VkService.GetUser(customer.Id);
            model.InstagramUser = InstagramService.GetUserByCustomerId(customer.Id);
            model.FacebookUser = FacebookService.GetUser(customer.Id);
            model.OkUser = OkService.GetUser(customer.Id);

            model.IsEnabledBothTypesOfCustomers = SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity;
            model.CustomerType = customer.CustomerType;

            return model;
        }
    }
}
