using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Areas.Api.Models.MetaInfos;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.SQL2;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class GetCategories : AbstractCommandHandler<GetCategoriesResponse>
    {
        private readonly CategoriesFilterModel _filter;
        private SqlPaging _paging;

        public GetCategories(CategoriesFilterModel filter)
        {
            _filter = filter;
        }
        
        protected override void Validate()
        {
            if (_filter.IsDefaultItemsPerPage)
                _filter.ItemsPerPage = 100;
            
            if (_filter.ItemsPerPage > 500 || _filter.ItemsPerPage <= 0)
                _filter.ItemsPerPage = 100;

            if (_filter.Page < 0)
                throw new BlException("page can't less than 0");
        }
        
        protected override GetCategoriesResponse Handle()
        {
            GetPaging();

            var model = new GetCategoriesResponse()
            {
                Pagination = new ApiPagination()
                {
                    CurrentPage = _paging.CurrentPageIndex,
                    TotalCount = _paging.TotalRowsCount,
                    TotalPageCount = _paging.PageCount()
                },
                Categories = new List<ICategoryApi>()
            };

            if (model.Pagination.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;

            var categories =
                _filter.Extended != null && _filter.Extended.Value
                    ? _paging.PageItemsList<GetCategoriesItemExtended>().Select(x => (ICategoryApi) x).ToList()
                    : _paging.PageItemsList<GetCategoriesItem>().Select(x => (ICategoryApi) x).ToList();

            if (_filter.Extended != null && _filter.Extended.Value)
            {
                foreach(GetCategoriesItemExtended item in categories)
                {
                    var picture = PhotoService.GetPhotoByObjId<CategoryPhoto>(item.Id, PhotoType.CategoryBig);
                    if (picture != null && !string.IsNullOrEmpty(picture.PhotoName))
                        item.PictureUrl = picture.ImageSrcBig();
                    
                    picture = PhotoService.GetPhotoByObjId<CategoryPhoto>(item.Id, PhotoType.CategorySmall);
                    if (picture != null && !string.IsNullOrEmpty(picture.PhotoName))
                        item.MiniPictureUrl = picture.ImageSrcSmall();
                    
                    picture = PhotoService.GetPhotoByObjId<CategoryPhoto>(item.Id, PhotoType.CategoryIcon);
                    if (picture != null && !string.IsNullOrEmpty(picture.PhotoName))
                        item.MenuIconPictureUrl = picture.IconSrc();

                    var meta = MetaInfoService.GetMetaInfo(item.Id, MetaType.Category) ??
                               MetaInfoService.GetDefaultMetaInfo(MetaType.Category, item.Name);
                    
                    item.SeoMetaInformation = new SeoMetaInformation(meta); 
                }
            }
            
            foreach (var category in categories)
            {
                category.Sorting = ((ESortOrder) category.SortingValue).ToString();
            }

            model.Categories = categories;
            model.Pagination.Count = model.Categories.Count;
            
            return model;
        }
        
        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filter.ItemsPerPage,
                CurrentPageIndex = _filter.Page
            };

            _paging.Select(
                "CategoryId".AsSqlField("Id"),
                "ExternalId",
                "Name",
                "UrlPath".AsSqlField("Url"),
                "ParentCategory".AsSqlField("ParentCategoryId"),
                "BriefDescription",
                "Description",
                "SortOrder",
                "Enabled",
                "DisplayStyle".AsSqlField("ShowMode"),
                "DisplayBrandsInMenu".AsSqlField("ShowBrandsInMenu"),
                "DisplaySubCategoriesInMenu".AsSqlField("ShowSubCategoriesInMenu"),
                "ShowOnMainPage",
                "Sorting".AsSqlField("SortingValue"),
                "Hidden",
                "ModifiedBy",
                "CatLevel".AsSqlField("Level"),
                "(SELECT Count(c.CategoryId) FROM [Catalog].[Category] c WHERE c.ParentCategory = [Category].CategoryId)".AsSqlField("ChildCategoriesCount"),
                "Products_Count".AsSqlField("ProductsCount"),
                "Current_Products_Count".AsSqlField("ProductsCountInCategory")
            );
            _paging.From("[Catalog].[Category]");

            Filter();
            Sorting();            
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filter.Search))
            {
                _paging.Where("Name LIKE '%'+{0}+'%'", _filter.Search);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Name))
            {
                _paging.Where("Name LIKE '%'+{0}+'%'", _filter.Name);
            }

            if (_filter.ParentCategoryId != null)
            {
                _paging.Where("ParentCategory = {0}", _filter.ParentCategoryId.Value);
                
                if (_filter.ParentCategoryId.Value == 0)
                    _paging.Where("CategoryId <> 0");
            }
            
            if (_filter.Enabled != null)
            {
                _paging.Where("Enabled = {0}", _filter.Enabled.Value);
            }
            
            if (_filter.Hidden != null)
            {
                _paging.Where("Hidden = {0}", _filter.Hidden.Value);
            }
        }


        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                _paging.OrderBy("Name");
                return;
            }

            var sorting = _filter.Sorting.ToLower().Replace("formatted", "");

            if (sorting == "sorting")
                sorting = "sortingvalue";
            
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}