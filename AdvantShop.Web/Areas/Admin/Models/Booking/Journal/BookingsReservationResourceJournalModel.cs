using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsReservationResourceJournalModel
    {
        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }
        public List<BookingJournalModel> Events { get; set; }
    }
}
