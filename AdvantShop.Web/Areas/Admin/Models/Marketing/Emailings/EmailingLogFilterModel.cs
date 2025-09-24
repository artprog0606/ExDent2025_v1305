using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Marketing.Emailings
{
    public class EmailingLogFilterModel : BaseFilterModel<int>
    {
        public Guid EmailingId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<EmailStatus> Statuses { get; set; }
    }
}
