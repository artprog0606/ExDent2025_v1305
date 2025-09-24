using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.Booking.Sms;
using AdvantShop.Web.Admin.Models.Booking.Affiliate;

namespace AdvantShop.Web.Admin.ViewModels.Booking.Settings
{
    public class SettingsModel
    {
        public SettingsModel()
        {
            BookingIntervals = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "5 мин", Value = "5"},
                new SelectListItem() {Text = "10 мин", Value = "10"},
                new SelectListItem() {Text = "15 мин", Value = "15"},
                new SelectListItem() {Text = "20 мин", Value = "20"},
                new SelectListItem() {Text = "30 мин", Value = "30"},
                new SelectListItem() {Text = "45 мин", Value = "45"},
                new SelectListItem() {Text = "1 ч", Value = "60"},
                new SelectListItem() {Text = "1 ч 30 мин", Value = "90"},
                new SelectListItem() {Text = "2 ч", Value = "120"},
            };

            SmsTemplateVariables = SmsTemplateService.Variables;
        }

        public AffiliateModel Affiliate { get; set; }
        public bool CanBeDeleting { get; set; }
        public bool CanBeEditAccess { get; set; }

        public List<SelectListItem> BookingIntervals { get; private set; }

        public string MondayTimes { get; set; }
        public string TuesdayTimes { get; set; }
        public string WednesdayTimes { get; set; }
        public string ThursdayTimes { get; set; }
        public string FridayTimes { get; set; }
        public string SaturdayTimes { get; set; }
        public string SundayTimes { get; set; }

        public Dictionary<string,string> SmsTemplateVariables { get; set; }
        public bool IsActiveSmsModule { get; set; }
    }
}
