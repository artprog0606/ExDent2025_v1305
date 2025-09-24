using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Triggers
{
    public class TriggerFilterModel : BaseFilterModel
    {
        public bool? Enabled { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
    }
}
