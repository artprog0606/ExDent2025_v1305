using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class GetUserMe : AbstractCommandHandler<GetCustomerResponse>
    {
        private Customer _customer;

        protected override void Validate()
        {
            _customer = CustomerContext.CurrentCustomer;
            
            if (!_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
        }

        protected override GetCustomerResponse Handle()
        {
            return new GetCustomerResponse(_customer);
        }
    }
}