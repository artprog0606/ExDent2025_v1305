using System;

namespace AdvantShop.Shipping.RangePriceAndDistanceOption
{
    [Serializable]
    public class PriceLimit
    {
        public float Price { get; set; }
        public float OrderPrice { get; set; }
        public int Distance { get; set; }
        public bool PerUnit { get; set; }
        public float PriceDistance { get; set; }
    }
}