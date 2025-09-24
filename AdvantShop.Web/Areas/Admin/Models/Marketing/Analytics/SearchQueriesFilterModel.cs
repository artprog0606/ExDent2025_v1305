using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Marketing.Analytics
{
    public class SearchQueriesFilterModel : BaseFilterModel
    {
        public int? ResultCountFrom { get; set; }
        public int? ResultCountTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SearchTerm { get; set; }
        public string CustomerFIO { get; set; }
    }

    public class SearchQueriesResultCountRangeModel
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }
}
