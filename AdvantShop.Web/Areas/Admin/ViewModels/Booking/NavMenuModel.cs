using System.Collections.Generic;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.ViewModels.Booking
{
    public class NavMenuModel
    {
        public List<AffiliateMenu> Affiliates { get; set; }
        public Affiliate SelectedAffiliate { get; set; }
        public bool AccessToEditing { get; set; }
        public bool AccessToSettings { get; set; }
        public bool AccessToAnalytic { get; set; }
    }

    public class AffiliateMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public int CountNewBooking { get; set; }

    }
}
