using System;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class GetBookingFormModel
    {
        public int? Id { get; set; }
        public int? AffiliateId { get; set; }
        public int? SelectedReservationResourceId { get; set; }
        public DateTime? SelectedDate { get; set; }
    }
}
