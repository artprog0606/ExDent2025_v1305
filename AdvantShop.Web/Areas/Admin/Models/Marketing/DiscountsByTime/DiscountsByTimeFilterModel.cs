using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Marketing.DiscountsByTime
{
    public class DiscountsByTimeFilterModel : BaseFilterModel
    {
        public bool? Enabled { get; set; }
        public float? DiscountFrom {  get; set; }
        public float? DiscountTo { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
    }
}
