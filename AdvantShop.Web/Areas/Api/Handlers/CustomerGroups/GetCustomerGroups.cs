using System.Linq;
using AdvantShop.Areas.Api.Models.CustomerGroups;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.CustomerGroups
{
    public class GetCustomerGroups : AbstractCommandHandler<GetCustomerGroupsResponse>
    {
        protected override GetCustomerGroupsResponse Handle()
        {
            var groups =
                CustomerGroupService.GetCustomerGroupList()
                    .Select(x => new CustomerGroupModel(x))
                    .ToList();

            return new GetCustomerGroupsResponse(groups);
        }
    }
}