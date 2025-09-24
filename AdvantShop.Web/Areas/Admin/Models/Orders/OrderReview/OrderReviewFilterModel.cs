using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Orders.OrderReview
{
    public class OrderReviewFilterModel : BaseFilterModel
    {
        public string OrderNumber { get; set; }
        public float? RatioFrom { get; set; }
        public float? RatioTo { get; set; }
        public string Text { get; set; }
    }
}
