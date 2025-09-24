using System;
using System.Linq;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class GetCustomerFields : AbstractCommandHandler<GetCustomerFieldsResponse>
    {
        private readonly Guid _id;
        private readonly CustomerType? _type;

        public GetCustomerFields(Guid id, CustomerType? type)
        {
            _id = id;
            _type = type;
        }

        protected override GetCustomerFieldsResponse Handle()
        {
            var customer = CustomerService.GetCustomer(_id);

            var fields = CustomerFieldService.GetCustomerFieldsWithValue(customer?.Id ?? Guid.Empty);

            if (_type != null)
                fields = fields.Where(x => x.CustomerType == _type).ToList();
            

            var result = fields.Select(x => new CustomerFieldModel(x)).ToList();

            return new GetCustomerFieldsResponse(result);
        }
    }
}