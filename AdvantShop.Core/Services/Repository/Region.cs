//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Repository
{
    public class Region
    {
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string RegionCode { get; set; }
        public int SortOrder { get; set; }

        private RegionAdditionalSettings _additionalSettings;
        public RegionAdditionalSettings AdditionalSettings => _additionalSettings ?? (_additionalSettings = RegionService.GetAdditionalSettings(RegionId));
    }

    public class RegionAdditionalSettings
    {
        public int RegionId { get; set; }
        public string FiasId { get; set; }
        public string KladrId { get; set; }
    }
}