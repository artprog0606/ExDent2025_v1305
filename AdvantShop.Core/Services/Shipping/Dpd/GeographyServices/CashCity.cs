namespace AdvantShop.Shipping.Dpd.GeographyServices
{
    public class CashCity
    {
        public CashCity()
        {
            CityName = string.Empty;
            RegionName = string.Empty;
            CountryCode = string.Empty;
            Abbreviation = string.Empty;
        }

        public long CityId { get; set; }
        public string CityName { get; set; }
        public string Abbreviation { get; set; }
        public string RegionName { get; set; }
        public string CountryCode { get; set; }
    }
}
