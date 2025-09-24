using System.Collections.Generic;
using AdvantShop.Core.Services.Landing.Templates;

namespace AdvantShop.Web.Admin.Models.Dashboard
{
    public class CreateSiteModel
    {
        public List<CreateSiteCategory> Categories { get; set; }
        public string Mode { get; set; }
    }

    public class CreateSiteCategory
    {
        public string Name { get; set; }
        public LpSiteCategory Type { get; set; }
    }
}
