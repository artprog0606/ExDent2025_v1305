namespace AdvantShop.Shipping.Dpd
{
    public class DpdCalculateOption
    {
        /// <summary>
        /// Самопривоз на терминал
        /// </summary>
        public bool SelfPickup { get; set; }

        /// <summary>
        /// Доставка до терминала. Самовывоз с терминала.
        /// </summary>
        public bool SelfDelivery { get; set; }

        /// <summary>
        /// Код услуги DPD
        /// </summary>
        public string ServiceCode { get; set; }
    }
}
