using System;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Areas.Api.Services
{
    public class ApiAuthorizeService
    {
        public bool SignIn(string email, string password, out Customer customer, out string userKey, out string userId)
        {
            userKey = null;
            userId = null;
            
            customer = CustomerService.GetCustomerByEmailAndPassword(email, password, false);
            if (customer == null)
                return false;

            return SignIn(customer, out userKey, out userId);
        }
        
        public bool SignIn(Customer customer, out string userKey, out string userId)
        {
            userKey = CreateUserKey(customer);
            userId = customer.Id.ToString();
            
            if (CustomerContext.CurrentCustomer != null)
                ShoppingCartService.MergeShoppingCarts(CustomerContext.CustomerId, customer.Id);
            
            return true;
        }

        public string CreateUserKey(Customer customer)
        {
            return SecurityHelper.EncodeWithHmac($"{customer.Id}_{customer.Password}_{customer.InnerId}", SettingsLic.LicKey ?? "saltAuthApi");
        }

        public Customer GetNotExistCustomer()
        {
            var customerId = Guid.NewGuid();
            while (CustomerService.ExistsCustomer(customerId))
                customerId = Guid.NewGuid();

            return new Customer { Id = customerId, CustomerRole = Role.Guest };
        }
    }
}