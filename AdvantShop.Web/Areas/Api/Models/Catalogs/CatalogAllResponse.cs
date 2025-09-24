using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class CatalogAllResponse : IApiResponse
    {
        public List<CategoryItem> Categories { get; set; }
    }

    public class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string BriefDescription { get; set; }
        public string PictureUrl { get; set; }
        public string MiniPictureUrl { get; set; }
        public int SortOrder { get; set; }
        public bool HasSubCategories { get; set; }
        
        public List<CatalogProductItem> Products { get; set; }
        public int ProductsTotalCount { get; set; }
        public int ProductsTotalPageCount { get; set; }

        public CategoryItem(Category category)
        {
            Id = category.CategoryId;
            Name = category.Name;
            Url = category.UrlPath;
            BriefDescription = !string.IsNullOrWhiteSpace(category.BriefDescription) ? category.BriefDescription : null;
            SortOrder = category.SortOrder;
            HasSubCategories = category.HasChild;

            PictureUrl = category.Picture != null && !string.IsNullOrEmpty(category.Picture.PhotoName)
                ? category.Picture.ImageSrcBig()
                : null;
            MiniPictureUrl = category.MiniPicture?.ImageSrcSmall();
        }
    }
}