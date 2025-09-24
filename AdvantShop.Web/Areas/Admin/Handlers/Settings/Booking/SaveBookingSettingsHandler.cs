using AdvantShop.Configuration;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Booking
{
    public class SaveBookingSettingsHandler
    {
        private readonly BookingSettingsModel _model;

        public SaveBookingSettingsHandler(BookingSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            SettingsPictureSize.BookingCategoryImageWidth = _model.CategoryImageWidth;
            SettingsPictureSize.BookingCategoryImageHeight = _model.CategoryImageHeight;

            SettingsPictureSize.BookingReservationResourceImageWidth = _model.ReservationResourceImageWidth;
            SettingsPictureSize.BookingReservationResourceImageHeight = _model.ReservationResourceImageHeight;

            SettingsPictureSize.BookingServiceImageWidth = _model.ServiceImageWidth;
            SettingsPictureSize.BookingServiceImageHeight = _model.ServiceImageHeight;

            SettingsMain.BookingActive = _model.BookingActive;

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_EditSettings);
        }
    }
}
