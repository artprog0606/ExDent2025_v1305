using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class SubCategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string BriefDescription { get; set; }
        public string PictureUrl { get; set; }
        public string MiniPictureUrl { get; set; }
        public int SortOrder { get; set; }
        public int ProductsCount { get; set; }
        public int ProductsCountWithSubCategories { get; set; }
        public bool HasSubCategories { get; set; }

        public SubCategoryItem(Category category)
        {
            Id = category.CategoryId;
            Name = category.Name;
            Url = category.UrlPath;
            BriefDescription = !string.IsNullOrWhiteSpace(category.BriefDescription) ? category.BriefDescription : null;
            SortOrder = category.SortOrder;
            ProductsCount = category.Current_Products_Count;
            ProductsCountWithSubCategories = category.ProductsCount;
            HasSubCategories = category.HasChild;

            PictureUrl = category.Picture != null && !string.IsNullOrEmpty(category.Picture.PhotoName)
                ? category.Picture.ImageSrcBig()
                : null;
            MiniPictureUrl = category.MiniPicture?.ImageSrcSmall();
        }
    }
}