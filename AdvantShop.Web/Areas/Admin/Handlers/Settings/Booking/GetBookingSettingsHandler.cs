using AdvantShop.Configuration;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Booking
{
    public class GetBookingSettingsHandler
    {
        public BookingSettingsModel Execute()
        {
            return new BookingSettingsModel
            {
                CategoryImageWidth = SettingsPictureSize.BookingCategoryImageWidth,
                CategoryImageHeight = SettingsPictureSize.BookingCategoryImageHeight,

                ReservationResourceImageWidth = SettingsPictureSize.BookingReservationResourceImageWidth,
                ReservationResourceImageHeight = SettingsPictureSize.BookingReservationResourceImageHeight,

                ServiceImageWidth = SettingsPictureSize.BookingServiceImageWidth,
                ServiceImageHeight = SettingsPictureSize.BookingServiceImageHeight,

                BookingActive = SettingsMain.BookingActive

            };
        }
    }
}
