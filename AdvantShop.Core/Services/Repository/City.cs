//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Repository
{
    public class City
    {
        public int CityId { get; set; }
        public int RegionId { get; set; }
        public string Name { get; set; }
        public string District { get; set; }
        public int CitySort { get; set; }
        public bool DisplayInPopup { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Zip { get; set; }

        private CityAdditionalSettings _additionalSettings;
        public CityAdditionalSettings AdditionalSettings => _additionalSettings ?? (_additionalSettings = CityService.GetAdditionalSettings(CityId));
    }

    public class CityAdditionalSettings
    {
        public int CityId { get; set; }
        public string ShippingZones { get; set; }
        public string ShippingZonesIframe { get; set; }
        public string CityAddressPoints { get; set; }
        public string CityAddressPointsIframe { get; set; }
        public string CityDescription { get; set; }
        public string FiasId { get; set; }
        public string KladrId { get; set; }
    }
}