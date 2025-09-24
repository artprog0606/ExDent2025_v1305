namespace AdvantShop.Shipping.Hermes
{
    public class HermesCalculateOption
    {
        public string BusinessUnitCode { get; set; }
        public bool SourcePointIsDistributionCenter { get; set; }
        public string SourcePointCode { get; set; }
        public bool WithInsure { get; set; }
    }
}
