using System;
using System.Linq;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports;
using AdvantShop.Web.Admin.Models.Marketing.Analytics;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Core.SQL2;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class GetSearchQueriesStatistic
    {
        private readonly SearchQueriesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetSearchQueriesStatistic(SearchQueriesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<SearchQueriesModel> Execute()
        {
            var model = new FilterResult<SearchQueriesModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено товаров: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<SearchQueriesModel>();

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
                "Request",
                "ResultCount",
                "Date",
                "SearchTerm",
                "Description",
                "[SearchStatistic].[CustomerID]",
                "FirstName",
                "LastName"
                );

            _paging.From("[Statistic].[SearchStatistic]");
            _paging.Left_Join("[Customers].[Customer] ON [Customers].[Customer].[CustomerID] = [Statistic].[SearchStatistic].[CustomerID]");

            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("SearchTerm like {0} + '%'", _filterModel.Search);
            }

            if (!string.IsNullOrEmpty(_filterModel.SearchTerm))
            {
                _paging.Where("SearchTerm like {0} + '%'", _filterModel.SearchTerm);
            }

            if (_filterModel.ResultCountFrom >= 0)
            {
                _paging.Where("ResultCount >= {0}", _filterModel.ResultCountFrom.Value);
            }

            if (_filterModel.ResultCountTo >= 0)
            {
                _paging.Where("ResultCount <= {0}", _filterModel.ResultCountTo.Value);
            }

            if (_filterModel.DateFrom != null)
            {
                _paging.Where("Date >= {0}", _filterModel.DateFrom.Value);
            }

            if (_filterModel.DateTo != null)
            {
                _paging.Where("Date <= {0}", _filterModel.DateTo.Value);
            }

            if (!string.IsNullOrEmpty(_filterModel.CustomerFIO))
            {
                _paging.Where("FirstName  like {0} + '%' or LastName  like {0} + '%'", _filterModel.CustomerFIO);
            }
        }

        private void Sorting()
        {
            if (!string.IsNullOrEmpty(_filterModel.Sorting))
            {
                var sorting = _filterModel.Sorting.ToLower();

                if (sorting == "customerid")
                {
                    sorting = "lastname";
                }

                var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
                if (field != null)
                {
                    if (_filterModel.SortingType == FilterSortingType.Asc)
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

        public List<T> GetItemsIds<T>(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<T>(fieldName);
        }

    }
}
