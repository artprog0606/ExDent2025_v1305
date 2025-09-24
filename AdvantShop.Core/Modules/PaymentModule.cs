namespace AdvantShop.Core.Modules
{
    public class PaymentModule
    {
        public int ModuleId { get; set; }
        public string ModuleStringId { get; set; }
        public string ModuleVersion { get; set; }
        public string PaymentKey { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string PriceString { get; set; }
        public string Icon { get; set; }
    }
}