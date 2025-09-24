using System;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class GetMonthFreeDaysModel
    {
        public int AffiliateId { get; set; }
        public int? SelectedReservationResourceId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool LoadPrevMonth { get; set; }
        public bool LoadCurrentMonth { get; set; }
        public bool LoadNextMonth { get; set; }
    }
}
