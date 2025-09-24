using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class AdminMenuModel
    {
        public int SiteId { get; set; }
        public int LandingId { get; set; }
        public bool Inplace { get; set; }
        public List<Lp> Items { get; set; }
    }
}
