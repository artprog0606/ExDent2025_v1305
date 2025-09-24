namespace AdvantShop.Shipping.Dpd.GeographyServices
{
    public class ParcelShop
    {
        public ParcelShop()
        {
            CityName = string.Empty;
            RegionName = string.Empty;
            CountryCode = string.Empty;
            Address = string.Empty;
            AddressDescription = string.Empty;
            SelfDeliveryTimes = string.Empty;
            ExtraServices = string.Empty;
            Services = string.Empty;
            Type = string.Empty;
        }

        public string Code { get; set; }
        public long CityId { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string CountryCode { get; set; }
        public string Address { get; set; }
        public string AddressDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsSelfPickup { get; set; }
        public bool IsSelfDelivery { get; set; }
        public string SelfDeliveryTimes { get; set; }
        public string ExtraServices { get; set; }
        public string Services { get; set; }
        public double? MaxWeight { get; set; }
        public double? DimensionSum { get; set; }
        public double? MaxHeight { get; set; }
        public double? MaxWidth { get; set; }
        public double? MaxLength { get; set; }
        public string Type { get; set; }
    }
}
