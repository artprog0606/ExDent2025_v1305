using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.ViewModels.Booking
{
    public class MenuJsonModel
    {
        public Affiliate SelectedAffiliate { get; set; }
        public bool AccessToEditing { get; set; }
        public bool AccessToAnalytic { get; set; }
        public bool AccessToSettings { get; set; }
        public bool IsOpen { get; set; }
    }
}
