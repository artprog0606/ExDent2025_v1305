using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Booking.Affiliate.SmsTemplate
{
    public class SmsTemplatesFilterModel : BaseFilterModel<int>
    {
        public int? AffiliateFilterId { get; set; }
        public BookingStatus? Status { get; set; }
        public bool? Enabled { get; set; }
    }
}
