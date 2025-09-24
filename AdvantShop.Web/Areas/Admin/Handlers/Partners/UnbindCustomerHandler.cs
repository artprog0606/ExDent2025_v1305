using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class UnbindCustomerHandler : AbstractCommandHandler
    {
        private readonly Guid _customerId;
        private Customer _customer;

        public UnbindCustomerHandler(Guid customerId)
        {
            _customerId = customerId;
        }

        protected override void Load()
        {
            _customer = CustomerService.GetCustomer(_customerId);
        }

        protected override void Validate()
        {
            if (_customer == null)
                throw new BlException(T("Admin.Partner.Errors.CustomerNotFound"));
            if (TransactionService.CustomerHasTransactions(_customerId))
                throw new BlException("Удаление невозможно. Имеются транзакции");
        }

        protected override void Handle()
        {
            PartnerService.DeleteBindedCustomer(_customerId);
        }
    }
}
