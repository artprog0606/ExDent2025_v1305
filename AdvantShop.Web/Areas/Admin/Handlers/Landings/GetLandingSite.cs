using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Saas;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Landings;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetLandingSite
    {
        private readonly int _id;
        private readonly LpSiteService _lpSiteService;

        public GetLandingSite(int id)
        {
            _id = id;
            _lpSiteService = new LpSiteService();
        }

        public LandingAdminSiteModel Execute()
        {
            var site = _lpSiteService.Get(_id);
            if (site == null)
                return null;

            var model = new LandingAdminSiteModel() {
                Site = site,
                UseDomainsManager = SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled,
                Settings = new GetLandingSiteSettings(_id).Execute()
            };

            return model;
        }
    }
}
