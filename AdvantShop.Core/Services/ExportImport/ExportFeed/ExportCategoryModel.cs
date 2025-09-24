namespace AdvantShop.ExportImport
{
    public class ExportCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentCategory { get; set; }
    }

    public class ExportFeedSelectedCategory
    {
        public int ExportFeedId { get; set; }
        public int CategoryId { get; set; }
        public bool Opened { get; set; }
    }
}