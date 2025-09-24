using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.ShippingReplaceGeo
{
    public class ShippingReplaceGeoFilterModel : BaseFilterModel<int>
    {
        public string ShippingType { get; set; }
        public string InCountryName { get; set; }
        public string InCountryISO2 { get; set; }
        public string InRegionName { get; set; }
        public string InCityName { get; set; }
        public string InDistrict { get; set; }
        public string InZip { get; set; }
        public string OutCountryName { get; set; }
        public string OutRegionName { get; set; }
        public string OutCityName { get; set; }
        public string OutDistrict { get; set; }
        public bool? OutDistrictClear { get; set; }
        public string OutZip { get; set; }
        public bool? Enabled { get; set; }
        //public string Comment { get; set; }
    }
}
