using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingFormData
    {
        public List<string> Times { get; set; }
        public List<string> WorkTimes { get; set; }
        public List<SelectItemModel> ReservationResources { get; set; }
        public int? CurrentManager { get; set; }
        public List<SelectItemModel> Managers { get; set; }
        public int BookingSourceNone { get; set; }
        public List<SelectItemModel> BookingSources { get; set; }
    }
}
