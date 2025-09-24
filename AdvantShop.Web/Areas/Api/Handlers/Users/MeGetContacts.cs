using System.Linq;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class MeGetContacts : AbstractCommandHandler<GetContactsResponse>
    {
        private readonly Customer _customer;
        
        public MeGetContacts()
        {
            _customer = CustomerContext.CurrentCustomer;
        }

        protected override void Validate()
        {
            if (_customer == null || !_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
        }

        protected override GetContactsResponse Handle()
        {
            var contacts = _customer.Contacts.Select(x => new CustomerContactModel(x)).ToList();

            return new GetContactsResponse(contacts);
        }
    }
}