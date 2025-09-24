using System.Collections.Generic;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvCategory
    {
        public string CategoryId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ParentCategory { get; set; }
        public string SortOrder { get; set; }
        public string Enabled { get; set; }
        public string Hidden { get; set; }
        public string BriefDescription { get; set; }
        public string Description { get; set; }
        public string DisplayStyle { get; set; }
        public string Sorting { get; set; }
        public string DisplayBrandsInMenu { get; set; }
        public string DisplaySubCategoriesInMenu { get; set; }
        public string Tags { get; set; }
        public string Picture { get; set; }
        public string MiniPicture { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string H1 { get; set; }
        public string PropertyGroups { get; set; }
        public string CategoryHierarchy { get; set; }
        public string ShowOnMainPage { get; set; }
        public string RelatedCategories { get; set; }
        public string SimilarCategories { get; set; }
        public Dictionary<string, string> RelatedProperties { get; set; }
        public IEnumerable<string> RelatedPropertyNames { get; set; }
        public Dictionary<string, string> SimilarProperties { get; set; }
        public IEnumerable<string> SimilarPropertyNames { get; set; }
        public string SizeChart { get; set; }
    }
}