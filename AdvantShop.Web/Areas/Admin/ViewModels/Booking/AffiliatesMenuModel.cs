using System.Collections.Generic;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.ViewModels.Booking
{
    public class AffiliatesMenuModel
    {
        public List<AffiliateMenu> Affiliates { get; set; }
        public Affiliate SelectedAffiliate { get; set; }
    }
}
