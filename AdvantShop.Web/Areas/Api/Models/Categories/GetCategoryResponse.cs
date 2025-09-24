using AdvantShop.Areas.Api.Models.MetaInfos;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Categories
{
    public class GetCategoryResponse : CategoryModel, IApiResponse
    {
        public GetCategoryResponse()
        { 
        }

        public GetCategoryResponse(Category category)
        {
            Id = category.CategoryId;
            ExternalId = category.ExternalId;
            ParentCategoryId = category.ParentCategoryId;
            Name = category.Name;
            Url = category.UrlPath;
            Description = category.Description;
            BriefDescription = category.BriefDescription;
            Enabled = category.Enabled;
            Hidden = category.Hidden;
            SortOrder = category.SortOrder;
            Sorting = category.Sorting.ToString();
            ShowMode = category.DisplayStyle;
            ShowBrandsInMenu = category.DisplayBrandsInMenu;
            ShowSubCategoriesInMenu = category.DisplaySubCategoriesInMenu;
            ShowOnMainPage = category.ShowOnMainPage;
            ModifiedBy = category.ModifiedBy;
            
            SeoMetaInformation = category.Meta != null ? new SeoMetaInformation(category.Meta) : null;

            PictureUrl = category.Picture != null && !string.IsNullOrEmpty(category.Picture.PhotoName)
                ? category.Picture.ImageSrcBig()
                : null;
            MiniPictureUrl = category.MiniPicture != null && !string.IsNullOrEmpty(category.MiniPicture.PhotoName)
                ? category.MiniPicture.ImageSrcSmall()
                : null;
            MenuIconPictureUrl = category.Icon != null && !string.IsNullOrEmpty(category.Icon.PhotoName) 
                ? category.Icon.IconSrc()
                : null;
        }
    }
}