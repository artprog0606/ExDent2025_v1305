using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.ViewModels.Booking.Analytics
{
    public class AnalyticsModel
    {
        public AnalyticsModel()
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day);

            DateFrom = date.AddMonths(-1);
            DateTo = date;

            PaidItems = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Любой", Value = "null", Selected = true},
                new SelectListItem() {Text = "Да", Value = "true"},
                new SelectListItem() {Text = "Нет", Value = "false"},
            };
            Paid = PaidItems.First(x => x.Selected).Value;

            Statuses =
                Enum.GetValues(typeof(BookingStatus))
                    .Cast<BookingStatus>()
                    .Select(x => new SelectListItem() { Text = x.Localize(), Value = ((int)x).ToString() })
                    .ToList();

            Statuses.Insert(0, new SelectListItem() { Text = "Любой", Value = "null", Selected = true });

            Status = Statuses.First(x => x.Selected).Value;
        }

        public Affiliate SelectedAffiliate { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public List<SelectListItem> PaidItems { get; set; }

        public List<SelectListItem> Statuses { get; set; }
        public string Paid { get; set; }
        public string Status { get; set; }
    }
}
