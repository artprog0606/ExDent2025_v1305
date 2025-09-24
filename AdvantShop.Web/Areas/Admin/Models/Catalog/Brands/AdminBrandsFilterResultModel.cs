using System.Collections.Generic;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Brands
{
    public class AdminBrandsFilterResult : FilterResult<AdminBrandModel>
    {
        public List<Country> Countries { get; set; }
    }
}