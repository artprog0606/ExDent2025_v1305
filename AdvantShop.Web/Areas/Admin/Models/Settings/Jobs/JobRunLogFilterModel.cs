using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.Jobs
{
    public class JobRunLogFilterModel : BaseFilterModel<int>
    {
        public string Event { get; set; }
        public DateTime? AddDateFrom { get; set; }
        public DateTime? AddDateTo { get; set; }
    }
}
