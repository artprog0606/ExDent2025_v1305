using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Web.Admin.Models.Booking.Analytics.Reports
{
    public class CommonStatisticData
    {
        public float? SumAllBookings { get; set; }
        public string SumAllBookingsFormated
        {
            get { return SumAllBookings.HasValue ? SumAllBookings.Value.FormatPrice() : "-"; }
        }
        public float? SumPaidBookings { get; set; }
        public string SumPaidBookingsFormated
        {
            get { return SumPaidBookings.HasValue ? SumPaidBookings.Value.FormatPrice() : "-"; }
        }
        public int? CountAllBookings { get; set; }
        public float? AverageSumBookings { get; set; }
        public string AverageSumBookingsFormated
        {
            get { return AverageSumBookings.HasValue ? AverageSumBookings.Value.FormatPrice() : "-"; }
        }
        public float? SumCancelledBookings { get; set; }
        public string SumCancelledBookingsFormated
        {
            get { return SumCancelledBookings.HasValue ? SumCancelledBookings.Value.FormatPrice() : "-"; }
        }
        public float? SumCancelledAndPaidBookings { get; set; }
        public string SumCancelledAndPaidBookingsFormated
        {
            get { return SumCancelledAndPaidBookings.HasValue ? SumCancelledAndPaidBookings.Value.FormatPrice() : "-"; }
        }
        public int? CountCancelledBookings { get; set; }

    }
}
