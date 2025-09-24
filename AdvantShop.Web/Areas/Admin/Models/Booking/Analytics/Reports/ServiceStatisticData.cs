namespace AdvantShop.Web.Admin.Models.Booking.Analytics.Reports
{
    public class ServiceStatisticData
    {
        public int Id { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public float Sum { get; set; }
        public string ReservationResourceId { get; set; }//for grouping
        public string ReservationResourceName { get; set; }//for grouping
    }
}
