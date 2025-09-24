using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Booking.Services
{
    public class ServicesFilterModel : BaseFilterModel<int>
    {
        public bool? HasAffiliate { get; set; }
        public int? LeftJoinAffiliateId { get; set; }
        public int? AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public int CategoryFilterId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public bool? Enabled { get; set; }
        public bool? HasPhoto { get; set; }
        public int? SortingFrom { get; set; }
        public int? SortingTo { get; set; }
    }
}
