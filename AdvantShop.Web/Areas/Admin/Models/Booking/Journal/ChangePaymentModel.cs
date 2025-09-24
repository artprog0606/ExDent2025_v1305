using AdvantShop.Payment;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class ChangePaymentModel
    {
        public BasePaymentOption Payment { get; set; }
        public BookingSummaryModel Summary { get; set; }
    }
}
