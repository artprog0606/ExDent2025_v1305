using System.Collections.Generic;
using AdvantShop.Core.Services.Loging.Emails;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class LandingTriggerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EventTypeName { get; set; }

        public List<LandingTriggerEmailingModel> Emailings { get; set; }
    }

    public class LandingTriggerEmailingModel
    {
        public string EmailSubject { get; set; }

        public Dictionary<EmailStatus, int> EmailsCount { get; set; }
    }
}
