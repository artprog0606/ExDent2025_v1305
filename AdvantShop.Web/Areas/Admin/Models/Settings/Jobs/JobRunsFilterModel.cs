using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.Jobs
{
    public class JobRunsFilterModel : BaseFilterModel<string>
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public int? ExecutionTimeFrom { get; set; }
        public int? ExecutionTimeTo { get; set; }
    }
}
