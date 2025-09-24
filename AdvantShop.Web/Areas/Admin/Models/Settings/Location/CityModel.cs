using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class CityModel
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
        public List<RegionModel> Regions { get; set; }
    }
}