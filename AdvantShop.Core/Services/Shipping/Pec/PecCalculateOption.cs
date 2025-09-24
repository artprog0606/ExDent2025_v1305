namespace AdvantShop.Shipping.Pec
{
    public class PecCalculateOption
    {
        public int TransportingType { get; set; }

        /// <summary>
        /// Курьерская доставка
        /// </summary>
        public bool WithDelivery { get; set; }
    }
}
