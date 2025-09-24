namespace AdvantShop.Core.Services.Booking.Sms
{
    public class SmsTemplate
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public BookingStatus Status { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; }
    }
}
