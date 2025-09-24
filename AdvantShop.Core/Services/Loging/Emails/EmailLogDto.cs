using System;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public sealed class EmailLogDto
    {
        public Guid CustomerId { get; } 
        public string Email { get; } 
        public string Subject { get; } 
        public string Body { get; } 
        public DateTime CreatedOnUtc { get; } 
        public EmailStatus Status { get; }

        public EmailLogDto(Guid customerId,
            string email,
            string subject,
            string body,
            DateTime createdOnUtc,
            EmailStatus status)
        {
            CustomerId = customerId;
            Email = email;
            Subject = subject;
            Body = body;
            CreatedOnUtc = createdOnUtc;
            Status = status;
        }
    }
}