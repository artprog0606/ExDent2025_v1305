using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Marketing.Emailings
{
    public class ManualEmailingsFilterModel : BaseFilterModel<Guid>
    {
        public string Subject { get; set; }

        public int? TotalCountFrom { get; set; }
        public int? TotalCountTo { get; set; }

        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
    }
}
