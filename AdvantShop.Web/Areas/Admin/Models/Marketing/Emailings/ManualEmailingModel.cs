using System;

namespace AdvantShop.Web.Admin.Models.Marketing.Emailings
{
    public class ManualEmailingModel
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public int TotalCount { get; set; }
        public DateTime DateCreated { get; set; }
        public string DateCreatedFormatted { get { return DateCreated.ToString("dd.MM.yy HH:mm"); } }
    }
}
