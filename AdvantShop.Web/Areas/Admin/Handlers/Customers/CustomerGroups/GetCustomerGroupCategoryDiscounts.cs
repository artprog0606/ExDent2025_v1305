using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Customers.CustomerGroups;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerGroups
{
    public class GetCustomerGroupCategoryDiscounts
    {
        private readonly CustomerGroupCategoryDiscountFilter _filter;
        private SqlPaging _paging;

        public GetCustomerGroupCategoryDiscounts(CustomerGroupCategoryDiscountFilter filter)
        {
            _filter = filter;
        }
        
        public FilterResult<CustomerGroupCategoryDiscountModel> Execute()
        {
            var model = new FilterResult<CustomerGroupCategoryDiscountModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<CustomerGroupCategoryDiscountModel>();
            
            return model;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
                {
                    ItemsPerPage = _filter.ItemsPerPage,
                    CurrentPageIndex = _filter.Page
                }
                .Select(
                    "Category.CategoryId",
                    "Category.Name",
                    "cg_cat.Discount",
                    _filter.CustomerGroupId.ToString().AsSqlField("CustomerGroupId")
                )
                .From("[Catalog].[Category]")
                .Left_Join("[Customers].[CustomerGroup_Category] as cg_cat ON Category.CategoryId = cg_cat.CategoryId AND cg_cat.CustomerGroupId = {0}", _filter.CustomerGroupId)
                .Where("Category.CategoryId <> 0 and Category.ParentCategory = {0}", _filter.CategoryId);
            
            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filter.Search))
                _paging.Where("(Name LIKE '%'+{0}+'%')", _filter.Search);

            if (!string.IsNullOrEmpty(_filter.Category))
                _paging.Where("Name LIKE '%'+{0}+'%'", _filter.Category);
            
            if (_filter.Discount != null)
                _paging.Where("cg_cat.Discount = {0}", _filter.Discount.Value);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                return;
            }

            var sorting = _filter.Sorting.ToLower();

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
                if (_filter.SortingType == FilterSortingType.Asc)
                    _paging.OrderBy(sorting);
                else
                    _paging.OrderByDesc(sorting);
        }
    }
}