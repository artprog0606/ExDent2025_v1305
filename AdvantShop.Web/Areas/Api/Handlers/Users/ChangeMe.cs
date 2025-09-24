using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class ChangeMe : AbstractCommandHandler<AddUpdateCustomerResponse>
    {
        private readonly AddUpdateCustomerModel _model;
        private Customer _customer;

        public ChangeMe(AddUpdateCustomerModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            _customer = CustomerContext.CurrentCustomer;
            
            if (!_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
        }

        protected override AddUpdateCustomerResponse Handle()
        {
            return new AddUpdateCustomer(_customer.Id, _model).Execute();
        }
    }
}