
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Web.Admin.Models.Booking.Analytics.Reports
{
    public class PaymentMethodStatisticData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float BookingsSum { get; set; }

        public string BookingsSumFormat
        {
            get { return BookingsSum.FormatPrice(); }
        }

        public int BookingsCount { get; set; }
        public float Percent { get; set; }
    }
}
