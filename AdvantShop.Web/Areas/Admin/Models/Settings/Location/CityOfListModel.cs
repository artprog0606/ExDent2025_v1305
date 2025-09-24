using System.Text;

namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class CityOfListModel
    {
        public int CityId { get; set; }
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public string District { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(CityName);

            if (!string.IsNullOrWhiteSpace(District)
                && !string.Equals(District, CityName, System.StringComparison.OrdinalIgnoreCase))
            {
                sb.Append(", ");
                sb.Append(District);
            }

            if (!string.IsNullOrWhiteSpace(CountryName)
                || !string.IsNullOrWhiteSpace(RegionName))
            {
                if (!string.IsNullOrWhiteSpace(RegionName))
                {
                    sb.Append(", ");
                    sb.Append(RegionName);
                }
                if (!string.IsNullOrWhiteSpace(CountryName))
                {
                    sb.Append(", ");
                    sb.Append(CountryName);
                }
            }

            return sb.ToString();
        }
    }
}