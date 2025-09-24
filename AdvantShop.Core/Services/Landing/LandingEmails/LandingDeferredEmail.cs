using System;

namespace AdvantShop.Core.Services.Landing.LandingEmails
{
    public class LandingDeferredEmail
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        
        /// <summary>
        /// Когда отослать письмо
        /// </summary>
        public DateTime SendingDate { get; set; }
    }
}
