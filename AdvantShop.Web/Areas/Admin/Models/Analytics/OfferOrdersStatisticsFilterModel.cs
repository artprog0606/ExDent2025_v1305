using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Analytics
{
    public class OfferOrdersStatisticsFilterModel : BaseFilterModel
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public int OfferId { get; set; }

        public bool? Paid { get; set; }
    }
}
