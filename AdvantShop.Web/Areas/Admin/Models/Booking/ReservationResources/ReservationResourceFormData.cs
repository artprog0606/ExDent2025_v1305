using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class ReservationResourceFormData
    {
        public List<SelectItemModel> Managers { get; set; }
        public int? AffiliateBookingIntervalMinutes { get; set; }

        public List<object> BookingIntervals
        {
            get
            {
                return new List<object>()
                {
                    new {Text = "5 мин", Value = 5},
                    new {Text = "10 мин", Value = 10},
                    new {Text = "15 мин", Value = 15},
                    new {Text = "20 мин", Value = 20},
                    new {Text = "30 мин", Value = 30},
                    new {Text = "45 мин", Value = 45},
                    new {Text = "1 ч", Value = 60},
                    new {Text = "1 ч 30 мин", Value = 90},
                    new {Text = "2 ч", Value = 120},
                };
            }
        }
        public List<string> Times { get; set; }

        public List<string> MondayWorkTimes { get; set; }
        public List<string> TuesdayWorkTimes { get; set; }
        public List<string> WednesdayWorkTimes { get; set; }
        public List<string> ThursdayWorkTimes { get; set; }
        public List<string> FridayWorkTimes { get; set; }
        public List<string> SaturdayWorkTimes { get; set; }
        public List<string> SundayWorkTimes { get; set; }

        public List<string> MondayTimes { get; set; }
        public List<string> TuesdayTimes { get; set; }
        public List<string> WednesdayTimes { get; set; }
        public List<string> ThursdayTimes { get; set; }
        public List<string> FridayTimes { get; set; }
        public List<string> SaturdayTimes { get; set; }
        public List<string> SundayTimes { get; set; }

        public List<string> Tags { get; set; }
    }
}
