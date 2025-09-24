namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class ReservationResourceGridModel : ReservationResourceModel
    {
        public int? AffiliateId { get; set; }
        public bool BindAffiliate { get; set; }
        public int? BookingIntervalMinutes { get; set; }
    }
}
