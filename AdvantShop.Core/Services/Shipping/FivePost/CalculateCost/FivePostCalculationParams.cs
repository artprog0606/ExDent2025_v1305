using AdvantShop.Shipping.FivePost.Api;

namespace AdvantShop.Shipping.FivePost.CalculateCost
{
    public class FivePostCalculationParams
    {
        public float InsureValue { get; set; }
        public FivePostRate Rate { get; set; }
        public float Weight { get; set; }

        public bool WithInsure { get; set; }
        public string WarehouseId { get; set; }
    }

    public class FivePostCalculationResult
    {
        public float DeliveryCost { get; set; }
        public float DeliveryCostWithInsure { get; set; }
    }
}
