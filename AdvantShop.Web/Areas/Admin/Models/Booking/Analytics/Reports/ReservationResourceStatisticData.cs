using System;

namespace AdvantShop.Web.Admin.Models.Booking.Analytics.Reports
{
    public class ReservationResourceStatisticData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BookingsCount { get; set; }
        public float BookingsSum { get; set; }
        public float AvgCheck {
            get { return BookingsCount > 0 ? (float)Math.Round(BookingsSum / BookingsCount, 2) : 0f;}
        }
        public int? CountSlots { get; set; }
        public float? FillingPlace
        {
            get { return CountSlots.HasValue ? (float)Math.Round(BookingsCount / (float)CountSlots.Value * 100f, 2) : (float?)null; }
        }
    }
}
