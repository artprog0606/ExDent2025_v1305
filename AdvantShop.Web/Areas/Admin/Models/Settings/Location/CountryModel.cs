namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class CountryModel
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public bool DisplayInPopup { get; set; }
        public int SortOrder { get; set; }
        public int? DialCode { get; set; }
    }
}
