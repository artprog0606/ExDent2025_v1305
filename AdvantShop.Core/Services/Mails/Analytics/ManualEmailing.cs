using System;

namespace AdvantShop.Core.Services.Mails.Analytics
{
    public class ManualEmailing
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public int TotalCount { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
