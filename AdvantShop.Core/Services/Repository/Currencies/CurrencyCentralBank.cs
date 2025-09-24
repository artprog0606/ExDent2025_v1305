namespace AdvantShop.Repository.Currencies
{
    public class CurrencyCentralBank
    {
        /// <summary>
        /// CharCode
        /// </summary>
        public string Iso3 { get; set; }

        /// <summary>
        /// NumCode
        /// </summary>
        public int NumIso3 { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public float Rate { get; set; }

        public float Nominal { get; set; }
    }
}
