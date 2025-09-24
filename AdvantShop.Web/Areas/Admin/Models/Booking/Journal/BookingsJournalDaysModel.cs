using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsJournalDaysModel
    {
        public string Name { get; set; }
        public long ElapsedMilliseconds { get; set; }

        public List<Week> Weeks { get; set; }
    }

    public class Week
    {
        public List<Day> Days { get; set; }
    }

    public class Day
    {
        public DateTime? Date { get; set; }

        public string DateString
        {
            get { return Date.HasValue ? Date.Value.ToShortDateString() : null; }
        }

        public string DayOfWeek
        {
            get { return Date.HasValue ? Date.Value.ToString("dddd") : null; }
        }

        public List<ReservationResourceStatistic> ReservationResources { get; set; }
    }

    public class ReservationResourceStatistic
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int BookingsCount { get; set; }
        public int? CountSlots { get; set; }
        public float? FillingPlace
        {
            get { return CountSlots.HasValue ? (float) Math.Round(BookingsCount/(float) CountSlots.Value*100f, 2) : (float?)null; }
        }
        public bool CheckErros { get; set; }
    }
}
