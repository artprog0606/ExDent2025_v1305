using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Orders
{
    public class OrderReview
    {
        public int OrderId { get; set; }
        public float Ratio { get; set; }
        public string Text { get; set; }
        public bool Readonly => Ratio != 0 || Text.IsNotEmpty();
    }
}
