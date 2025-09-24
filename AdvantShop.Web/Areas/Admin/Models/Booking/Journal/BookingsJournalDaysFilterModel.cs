using System;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsJournalDaysFilterModel
    {
        public int? AffiliateFilterId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
