using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class LandingAdminIndexModel
    {
        public List<Lp> Landings { get; set; }


        public IEnumerable<SelectListItem> Templates { get; set; } 
    }
}
