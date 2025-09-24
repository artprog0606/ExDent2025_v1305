namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class CityAdditionalSettingsModel
    {
        public int CityId { get; set; }
        public string ShippingZones { get; set; }
        public string ShippingZonesIframe { get; set; }
        public string CityAddressPoints { get; set; }
        public string CityAddressPointsIframe { get; set; }
        public string CityDescription { get; set; }
        public string FiasId { get; set; }
        public string KladrId { get; set; }
        public string CountryIso2 { get; set; }
    }
}
