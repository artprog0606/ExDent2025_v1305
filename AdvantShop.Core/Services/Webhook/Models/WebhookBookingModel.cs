namespace AdvantShop.Core.Services.Webhook.Models
{
    public class WebhookBookingModel : WebhookModel
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public float Sum { get; set; }

        public static explicit operator WebhookBookingModel(Booking.Booking booking)
        {
            return new WebhookBookingModel
            {
                Id = booking.Id,
                AffiliateId = booking.AffiliateId,
                ReservationResourceId = booking.ReservationResourceId,
                Sum = booking.Sum
            };
        }
    }
}
