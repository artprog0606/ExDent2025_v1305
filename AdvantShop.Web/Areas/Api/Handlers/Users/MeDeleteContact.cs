using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class MeDeleteContact : AbstractCommandHandler<ApiResponse>
    {
        private readonly Guid _contactId;
        private readonly Customer _customer;
        
        public MeDeleteContact(Guid contactId)
        {
            _contactId = contactId;
            _customer = CustomerContext.CurrentCustomer;
        }
        
        protected override void Validate()
        {
            if (_customer == null || !_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
            
            var contact = CustomerService.GetCustomerContact(_contactId);
                
            if (contact == null || contact.CustomerGuid != _customer.Id)
                throw new BlException("Контакт не найден");
        }

        protected override ApiResponse Handle()
        {
            CustomerService.DeleteContact(_contactId);

            return new ApiResponse();
        }
    }
}