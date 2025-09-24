using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Shipping.RangePriceAndDistanceOption
{
    public class RangePriceAndDistanceOption : BaseShippingOption
    {
        public RangePriceAndDistanceOption() { }
        public RangePriceAndDistanceOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }
        public float Distance { get; set; }
        public int MaxDistance { get; set; }

        public override string TemplateName
        {
            get { return "RangePriceAndDistanceOption.html"; }
        }
    }
}