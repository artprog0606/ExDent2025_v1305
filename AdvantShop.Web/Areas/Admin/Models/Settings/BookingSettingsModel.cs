namespace AdvantShop.Web.Admin.Models.Settings
{
    public class BookingSettingsModel
    {
        public int CategoryImageWidth { get; set; }
        public int CategoryImageHeight { get; set; }

        public int ReservationResourceImageWidth { get; set; }
        public int ReservationResourceImageHeight { get; set; }

        public int ServiceImageWidth { get; set; }
        public int ServiceImageHeight { get; set; }

        public bool BookingActive { get; set; }
    }
}
