//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Repository;
using AdvantShop.SEO;

namespace AdvantShop.Catalog
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BriefDescription { get; set; }

        private BrandPhoto _brandLogo;
        public BrandPhoto BrandLogo
        {
            get => _brandLogo ?? (_brandLogo = PhotoService.GetPhotoByObjId<BrandPhoto>(BrandId, PhotoType.Brand));
            set => _brandLogo = value;
        }

        //public string BrandLogo { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public int CountryId { get; set; }
        public int CountryOfManufactureId { get; set; }
        public string BrandSiteUrl { get; set; }

        private Country _brandCountry;

        public Country BrandCountry => _brandCountry ?? (_brandCountry = CountryId == 0 ? null : CountryService.GetCountry(CountryId));

        private Country _brandCountryOfManufacture;

        public Country BrandCountryOfManufacture =>
            _brandCountryOfManufacture ??
            (_brandCountryOfManufacture = CountryOfManufactureId == 0 ? null : CountryService.GetCountry(CountryOfManufactureId));

        private string _urlPath;
        public string UrlPath
        {
            get => _urlPath;
            set => _urlPath = value.ToLower();
        }

        private MetaInfo _meta;

        public MetaInfo Meta
        {
            get =>
                _meta ??
                (_meta =
                    MetaInfoService.GetMetaInfo(BrandId, MetaType) ??
                    MetaInfoService.GetDefaultMetaInfo(MetaType, Name));
            set => _meta = value;
        }

        public MetaType MetaType => MetaType.Brand;

        public int ID => BrandId;
    }
}