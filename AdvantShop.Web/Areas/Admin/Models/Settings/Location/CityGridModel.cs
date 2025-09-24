namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class CityGridModel
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
    }
}
