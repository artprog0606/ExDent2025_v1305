using System.Linq;
using AdvantShop.Areas.Api.Models.Managers;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Managers
{
    public class GetManagers : AbstractCommandHandler<GetManagersResponse>
    {
        protected override GetManagersResponse Handle()
        {
            var managers = ManagerService.GetManagersList()
                            .Select(x => new ManagerModel() {Id = x.ManagerId, Name = x.FullName})
                            .ToList();

            return new GetManagersResponse(managers);
        }
    }
}