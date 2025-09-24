using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class ReservationResourcesFilterModel : BaseFilterModel<int>
    {
        public int AffiliateFilterId { get; set; }
        public bool? Enabled { get; set; }
        public bool? HasPhoto { get; set; }
        public bool? HasAffiliate { get; set; }
        public int? SortingFrom { get; set; }
        public int? SortingTo { get; set; }
    }
}
