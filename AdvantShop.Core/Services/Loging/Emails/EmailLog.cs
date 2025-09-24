using System;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public class EmailLog
    {
        public Guid CustomerId { get; }
        public string Email { get; }
        public string Subject { get; }
        public string Body { get; } 
        public DateTime CreatedOnUtc { get; }
        public string Status { get; set; }

        public EmailLog(Guid customerId, string email, string subject, string body, EmailStatus status)
        {
            CustomerId = customerId;
            Email = email;
            Subject = subject;
            Body = body;
            CreatedOnUtc = DateTime.UtcNow;
            Status = status.ToString();
        }
    }
}