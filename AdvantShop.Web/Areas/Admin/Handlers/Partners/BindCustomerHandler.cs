using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class BindCustomerHandler : AbstractCommandHandler
    {
        private readonly int _id;
        private readonly Guid _customerId;
        private Partner _partner;
        private Customer _customer;

        public BindCustomerHandler(int partnerId, Guid customerId)
        {
            _id = partnerId;
            _customerId = customerId;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
            _customer = CustomerService.GetCustomer(_customerId);
        }

        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
            if (_customer == null)
                throw new BlException(T("Admin.Partner.Errors.CustomerNotFound"));
            var bindedPartner = PartnerService.GetCustomersPartner(_customerId);
            if (bindedPartner != null)
                throw new BlException(T("Admin.Partner.Errors.CustomerAlreadyBinded") + 
                    string.Format(" <a href=\"partners/view/{0}\" target=\"_blank\">{1}</a>", bindedPartner.Id, bindedPartner.Name));
        }

        protected override void Handle()
        {
            PartnerService.AddBindedCustomer(new BindedCustomer { PartnerId = _id, CustomerId = _customerId });
        }
    }
}
