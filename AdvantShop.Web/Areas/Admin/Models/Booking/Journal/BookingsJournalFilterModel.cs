using System;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsJournalFilterModel
    {
        public bool CompactView { get; set; }
        public int AffiliateFilterId { get; set; }
        public DateTime Date { get; set; }
        public string Search { get; set; }
        public string ColumnId { get; set; }
    }
}
