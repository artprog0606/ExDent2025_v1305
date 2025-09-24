using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.WarehouseTypes
{
    public class GetWarehouseTypes
    {
        private readonly WarehouseTypesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetWarehouseTypes(WarehouseTypesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }
        
        public WarehouseTypesFilterResult Execute()
        {
            var result = new WarehouseTypesFilterResult();
        
            GetPaging();

            result.TotalItemsCount = _paging.TotalRowsCount;
            result.TotalPageCount = _paging.PageCount();

            if (result.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return result;
            }

            result.DataItems = _paging.PageItemsList<WarehouseTypeGridModel>();

            return result;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("TypeWarehouse.Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "TypeWarehouse.Id".AsSqlField("TypeId"),
                "TypeWarehouse.Name".AsSqlField("TypeName"),
                "TypeWarehouse.SortOrder",
                "TypeWarehouse.Enabled");

            _paging.From("[Catalog].[TypeWarehouse]");

            Sorting();
            Filter();
        }
 
        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
                _filterModel.TypeName = _filterModel.Search;

            if (!string.IsNullOrWhiteSpace(_filterModel.TypeName))
                _paging.Where("TypeWarehouse.Name LIKE '%'+{0}+'%'", _filterModel.TypeName);

            if (_filterModel.Enabled != null)
                _paging.Where("TypeWarehouse.Enabled = {0}", _filterModel.Enabled.Value ? "1" : "0");

            if (_filterModel.SortingFrom != 0)
                _paging.Where("TypeWarehouse.SortOrder >= {0}", _filterModel.SortingFrom);

            if (_filterModel.SortingTo != 0)
                _paging.Where("TypeWarehouse.SortOrder <= {0}", _filterModel.SortingTo);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    new SqlCritera("TypeWarehouse.SortOrder", "", SqlSort.Asc)
                );
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                    _paging.OrderBy(sorting);
                else
                    _paging.OrderByDesc(sorting);
            }
        }
    }
}