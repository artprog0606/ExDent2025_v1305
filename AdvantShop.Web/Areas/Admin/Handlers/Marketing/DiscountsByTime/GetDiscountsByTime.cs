using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Marketing.DiscountsByTime;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Marketing.DiscountsPriceRanges
{
    public class GetDiscountsByTime
    {
        private DiscountsByTimeFilterModel _filterModel;
        private SqlPaging _paging;

        public GetDiscountsByTime(DiscountsByTimeFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<DiscountsByTimeGridModel> Execute()
        {
            var model = new FilterResult<DiscountsByTimeGridModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<DiscountsByTimeGridModel>();
            
            return model;
        }

        public List<T> GetItemsIds<T>()
        {
            GetPaging();

            return _paging.ItemsIds<T>("Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Id",
                "Enabled",
                "TimeFrom",
                "TimeTo",
                "Discount",
                "SortOrder");

            _paging.From("[Catalog].[DiscountByTime]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
                _paging.Where("Discount LIKE '%'+{0}+'%'", _filterModel.Search);

            if (_filterModel.Enabled.HasValue)
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value);

            if (_filterModel.DiscountFrom.HasValue)
                _paging.Where("Discount >= {0}", _filterModel.DiscountFrom.Value);

            if (_filterModel.DiscountTo.HasValue)
                _paging.Where("Discount <= {0}", _filterModel.DiscountTo.Value);

            var date = DateTime.Parse("2024.01.01");

            if (_filterModel.TimeFrom.IsNotEmpty())
                _paging.Where("TimeFrom >= {0}", date.Add(_filterModel.TimeFrom.TryParseTimeSpan()));

            if (_filterModel.TimeTo.IsNotEmpty())
                _paging.Where("TimeTo <= {0}", date.Add(_filterModel.TimeTo.TryParseTimeSpan()));
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();
            if (sorting == "time")
                sorting = "timefrom";
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    if (sorting == "timefrom")
                        _paging.OrderBy("timefrom", "timeto");
                    else
                        _paging.OrderBy(sorting);
                }
                else
                {
                    if (sorting == "timefrom")
                        _paging.OrderByDesc("timefrom", "timeto");
                    else
                        _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}