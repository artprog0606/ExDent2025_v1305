using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class LandingAdminSiteModel
    {
        public LpSite Site { get; set; }
        //public List<Lp> Landings { get; set; }

        public bool UseDomainsManager { get; set; }

        public LandingAdminSiteSettings Settings { get; set; }

    }
}
