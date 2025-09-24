using AdvantShop.Areas.Api.Models.MetaInfos;
using AdvantShop.Catalog;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Categories
{
    public interface ICategoryApi
    {
        int SortingValue { get; set; }
        
        string Sorting { get; set; }
    }
    
    public class CategoryBaseModel : ICategoryApi
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public int ParentCategoryId { get; set; }

        public string Name { get; set; }
        
        public string Url { get; set; }

        public string Description { get; set; }

        public string BriefDescription { get; set; }

        public bool Enabled { get; set; }

        public bool Hidden { get; set; }

        public int SortOrder { get; set; }
        
        public string Sorting { get; set; }
        
        [JsonIgnore]
        public int SortingValue { get; set; }

        public ECategoryDisplayStyle ShowMode { get; set; }

        public bool ShowBrandsInMenu { get; set; }

        public bool ShowSubCategoriesInMenu { get; set; }

        public bool ShowOnMainPage { get; set; }
        
        public string ModifiedBy { get; set; }
    }

    public class CategoryModel : CategoryBaseModel
    {
        [JsonProperty(Order = 1)]
        public SeoMetaInformation SeoMetaInformation { get; set; }

        [JsonProperty(Order = 2)]
        public string PictureUrl { get; set; }

        [JsonProperty(Order = 3)]
        public string MiniPictureUrl { get; set; }

        [JsonProperty(Order = 4)]
        public string MenuIconPictureUrl { get; set; }
    }
}