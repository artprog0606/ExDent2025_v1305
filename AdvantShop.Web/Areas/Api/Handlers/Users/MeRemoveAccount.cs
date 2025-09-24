using System;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Core;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class MeRemoveAccount : AbstractCommandHandler<RemoveAccountResponse>
    {
        private readonly Guid _customerId;
        private Customer _customer;

        public MeRemoveAccount(Guid customerId)
        {
            _customerId = customerId;
        }

        protected override void Validate()
        {
            _customer = CustomerService.GetCustomer(_customerId);
            
            if (_customer == null)
                throw new BlException("Пользователь не найден");
            
            if (_customer.Id != CustomerContext.CustomerId)
                throw new BlException("Не достаточно прав для удаления");
        }

        protected override RemoveAccountResponse Handle()
        {
            CustomerService.DeleteCustomer(_customerId,
                changedBy: new ChangedBy("Mobile app by API") {CustomerId = _customerId});
            
            return new RemoveAccountResponse(true);
        }
    }
}