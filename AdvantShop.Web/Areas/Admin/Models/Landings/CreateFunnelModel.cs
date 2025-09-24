using AdvantShop.Core.Services.Landing;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class CreateFunnelModel
    {
        public int? ProductId { get; set; }
        public List<LpFunnelModel> FunnelTypes { get; set; }
    }
}
