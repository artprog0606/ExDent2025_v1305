using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Affiliate.Settings
{
    public class SettingsTimesOfBooking
    {
        public List<string> Times { get; set; }
        public List<string> MondayTimes { get; set; }
        public List<string> TuesdayTimes { get; set; }
        public List<string> WednesdayTimes { get; set; }
        public List<string> ThursdayTimes { get; set; }
        public List<string> FridayTimes { get; set; }
        public List<string> SaturdayTimes { get; set; }
        public List<string> SundayTimes { get; set; }
    }
}
