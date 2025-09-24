using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Sizes;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Sizes
{
    public class GetSizeNameForCategoriesHandler
    {
        private readonly SizesForCategoryFilterModel _filterModel;
        private SqlPaging _paging;

        public GetSizeNameForCategoriesHandler(SizesForCategoryFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<SizeForCategoryModel> Execute()
        {
            var model = new FilterResult<SizeForCategoryModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<SizeForCategoryModel>();
            
            return model;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                _filterModel.CategoryId.ToString().AsSqlField("CategoryId"),
                "Size.SizeId",
                "SizeName",
                "SortOrder",
                "SizeNameForCategory"
                );

            _paging.From("[Catalog].[Size]");
            _paging.Left_Join($"[Catalog].[Category_Size] as catSize ON Size.SizeID = catSize.SizeId AND catSize.CategoryId = {_filterModel.CategoryId}");
            
            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
                _paging.Where("(SizeName LIKE '%'+{0}+'%' OR SizeNameForCategory LIKE '%'+{0}+'%')", _filterModel.Search);

            if (!string.IsNullOrEmpty(_filterModel.SizeName))
                _paging.Where("SizeName LIKE '%'+{0}+'%'", _filterModel.SizeName);

            if (!string.IsNullOrEmpty(_filterModel.SizeNameForCategory))
                _paging.Where("SizeNameForCategory LIKE '%'+{0}+'%'", _filterModel.SizeNameForCategory);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                _paging.OrderBy("SizeName");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
                if (_filterModel.SortingType == FilterSortingType.Asc)
                    _paging.OrderBy(sorting);
                else
                    _paging.OrderByDesc(sorting);
        }
    }
}