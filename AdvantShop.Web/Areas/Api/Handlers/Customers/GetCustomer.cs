using System;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class GetCustomer : AbstractCommandHandler<GetCustomerResponse>
    {
        private readonly Guid _id;
        private Customer _customer;

        public GetCustomer(Guid id)
        {
            _id = id;
        }

        protected override void Validate()
        {
            _customer = CustomerService.GetCustomer(_id);
            if (_customer == null)
                throw new BlException("Пользователь не найден");
        }

        protected override GetCustomerResponse Handle()
        {
            return new GetCustomerResponse(_customer);
        }
    }
}