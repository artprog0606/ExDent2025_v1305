using System.Collections.Generic;
using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class ProfilingActionFilterProvider : IFilterProvider
    {
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return new[] { new Filter(new ProfilingActionFilter(), FilterScope.Global, 0) };
        }
    }
}
