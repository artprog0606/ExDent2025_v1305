using System;
using AdvantShop.SEO;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class Warehouse
    {
        public int Id { get; set; }

        public string Name { get; set; }


        private string _urlPath;
        public string UrlPath
        {
            get => _urlPath;
            set => _urlPath = value.ToLower();
        }

        public string Description { get; set; }

        public int? TypeId { get; set; }

        public int SortOrder { get; set; }

        public bool Enabled { get; set; }

        public int? CityId { get; set; }

        public string Address { get; set; }

        public float? Longitude { get; set; }

        public float? Latitude { get; set; }

        public string AddressComment { get; set; }

        public string Phone { get; set; }

        public string Phone2 { get; set; }

        public string Email { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }
    
        private MetaInfo _meta;

        public MetaInfo Meta
        {
            get =>
                _meta ??
                (_meta =
                    MetaInfoService.GetMetaInfo(Id, MetaType) ??
                    MetaInfoService.GetDefaultMetaInfo(MetaType, Name));
            set => _meta = value;
        }

        public MetaType MetaType => MetaType.Warehouse;
    }
}