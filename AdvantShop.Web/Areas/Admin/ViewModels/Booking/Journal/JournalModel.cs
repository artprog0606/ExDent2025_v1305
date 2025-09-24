using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.ViewModels.Booking.Journal
{
    public enum JournalViewMode
    {
        Sheduler,
        ShedulerCompact,
        Grid,
        ShedulerMultiDays
    }

    public class JournalModel
    {
        public Affiliate SelectedAffiliate { get; set; }
        public bool AccessToEditing { get; set; }
        public bool AccessToViewBooking { get; set; }
        public JournalViewMode ViewMode { get; set; }
        public Dictionary<int, string> BookingStatuses
        {
            get
            {
                return Enum.GetValues(typeof (BookingStatus))
                    .Cast<BookingStatus>()
                    .ToDictionary(status => (int) status, status => GetStatusName(status) ?? status.Localize());
            }
        }

        private string GetStatusName(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.New:
                    return "Новые";
                case BookingStatus.Confirmed:
                    return "Подтвержденные";
                case BookingStatus.Completed:
                    return "Завершенные";
                case BookingStatus.Cancel:
                    return "Отмененные";
            }
            return null;
        }

        public string GetViewModeValue(JournalViewMode viewMode)
        {
            switch (viewMode)
            {
                case JournalViewMode.Sheduler:
                    return "sheduler";
                case JournalViewMode.ShedulerCompact:
                    return "sheduler-short";
                case JournalViewMode.Grid:
                    return "grid";
                case JournalViewMode.ShedulerMultiDays:
                    return "sheduler-days";
            }
            return null;
        }
    }
}
