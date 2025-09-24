namespace AdvantShop.Shipping.Sdek
{
    public class SdekCalculateOption
    {
        public int TariffId { get; set; }
        public bool? WithInsure { get; set; }
        public bool? AllowInspection { get; set; }
        public bool? PartialDelivery { get; set; }
        public bool? TryingOn { get; set; }
    }
}
