using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class UpdateSummaryModel
    {
        public int BookingId { get; set; }
        public List<BookingItemModel> CurrentItems { get; set; }
        public BookingSummaryModel Summary { get; set; }
        public bool UpdateStatusBillingLink { get; set; }
    }
}
