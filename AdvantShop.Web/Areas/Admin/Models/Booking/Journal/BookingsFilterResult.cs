using System.Collections.Generic;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsFilterResult : FilterResult<BookingFilteredResultModel>
    {
        public BookingsFilterResult()
        {
            BookingsCount = new Dictionary<int, int>();
        }

        public Dictionary<int, int> BookingsCount { get; set; }
    }
}
