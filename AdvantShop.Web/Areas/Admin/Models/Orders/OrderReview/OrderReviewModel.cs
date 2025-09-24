namespace AdvantShop.Web.Admin.Models.Orders.OrderReview
{
    public class OrderReviewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public float Ratio { get; set; }
        public string Text { get; set; }
    }
}
