using System;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public class SmsLogDto
    {
        public long Phone { get; }
        public string Message { get; }
        public DateTime CreatedOnUtc { get; }
        public SmsStatus Status { get; }

        public SmsLogDto(long phone, string message, DateTime createdOnUtc, SmsStatus status)
        {
            Phone = phone;
            Message = message;
            CreatedOnUtc = createdOnUtc;
            Status = status;
        }
    }
}