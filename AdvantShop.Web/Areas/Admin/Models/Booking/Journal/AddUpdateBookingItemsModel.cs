using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class AddUpdateBookingItemsModel
    {
        public int BookingId { get; set; }
        public List<BookingItemModel> CurrentItems { get; set; }
        public List<int> NewServiceIds { get; set; }
    }
}
