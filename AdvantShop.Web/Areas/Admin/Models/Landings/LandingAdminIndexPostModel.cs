using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class LandingAdminIndexPostModel
    {
        public int? SiteId { get; set; }
        public string Template { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Enabled { get; set; }

        public LpFunnelType Type { get; set; }
    }
}
