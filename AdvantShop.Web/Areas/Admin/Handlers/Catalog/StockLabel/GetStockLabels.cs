using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses.StockLabel;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.StockLabel
{
    public class GetStockLabels
    {
        private readonly StockLabelsFilterModel _filterModel;
        private SqlPaging _paging;

        public GetStockLabels(StockLabelsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }
        
        public StockLabelsFilterResult Execute()
        {
            var result = new StockLabelsFilterResult();
        
            GetPaging();

            result.TotalItemsCount = _paging.TotalRowsCount;
            result.TotalPageCount = _paging.PageCount();

            if (result.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return result;
            }

            result.DataItems = _paging.PageItemsList<StockLabelGridModel>();

            return result;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("StockLabel.Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "StockLabel.Id".AsSqlField("LabelId"),
                "StockLabel.Name",
                "StockLabel.ClientName",
                "StockLabel.Color",
                "StockLabel.AmountUpTo");

            _paging.From("[Catalog].[StockLabel]");

            Sorting();
            Filter();
        }
 
        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
                _paging.Where("(StockLabel.Name LIKE '%'+{0}+'%' OR StockLabel.ClientName LIKE '%'+{0}+'%')", _filterModel.Search);

            if (!string.IsNullOrWhiteSpace(_filterModel.Name))
                _paging.Where("StockLabel.Name LIKE '%'+{0}+'%'", _filterModel.Name);

            if (_filterModel.AmountUpToFrom.HasValue)
                _paging.Where("StockLabel.AmountUpTo >= {0}", _filterModel.AmountUpToFrom.Value);

            if (_filterModel.AmountUpToTo.HasValue)
                _paging.Where("StockLabel.AmountUpTo <= {0}", _filterModel.AmountUpToTo.Value);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    new SqlCritera("StockLabel.AmountUpTo", "", SqlSort.Asc)
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