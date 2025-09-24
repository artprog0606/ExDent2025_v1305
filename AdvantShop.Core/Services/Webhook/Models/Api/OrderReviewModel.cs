using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderReviewModel
    {
        public float Ratio { get; }
        public string Text { get; }
        
        public OrderReviewModel(OrderReview review)
        {
            Ratio = review.Ratio;
            Text = review.Text;
        }
    }
}