using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Web.Admin.Models.Landings;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.ViewModels.Landings
{
    public class FunnelDetailsViewModel
    {
        public LpFunnelCategory Category { get; set; }
        public int? ProductId { get; set; }
        public LpFunnelModel FunnelModel { get; set; }
        public List<LpTemplateModel> Templates { get; set; }
    }
}
