using System;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public class SmsLog
    {
        public Guid CustomerId { get; }
        public long Phone { get; }
        public string Message { get; }
        public DateTime CreatedOnUtc { get; }
        public string Status { get; }

        public SmsLog(Guid customerId, long phone, string message, SmsStatus status)
        {
            CustomerId = customerId;
            Phone = phone;
            Message = message;
            CreatedOnUtc = DateTime.UtcNow;
            Status = status.ToString();
        }
    }
}
