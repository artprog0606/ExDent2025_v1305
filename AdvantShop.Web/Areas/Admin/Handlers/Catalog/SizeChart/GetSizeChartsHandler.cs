using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.SizeChart;
using AdvantShop.Web.Infrastructure.Admin;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Catalog.SizeChart
{
    public class GetSizeChartsHandler
    {
        private readonly SizeChartFilterModel _filterModel;
        private SqlPaging _paging;

        public GetSizeChartsHandler(SizeChartFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<SizeChartGridModel> Execute()
        {
            var model = new FilterResult<SizeChartGridModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<SizeChartGridModel>();

            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
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
                "Name",
                "LinkText",
                "SortOrder",
                "Enabled"
                );

            _paging.From("[Catalog].[SizeChart]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Name))
                _filterModel.Search = _filterModel.Name;

            if (_filterModel.Search.IsNotEmpty() && _filterModel.Name.IsNotEmpty())
                _paging.Where("(Name LIKE '%'+{0}+'%' OR Name LIKE '%'+{1}+'%')", _filterModel.Name, _filterModel.Search);
            else if (_filterModel.Name.IsNotEmpty())
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Name);
            else if (_filterModel.Search.IsNotEmpty())
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Search);

            if (_filterModel.Enabled.HasValue)
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value);

            if (_filterModel.LinkText.IsNotEmpty())
                _paging.Where("LinkText LIKE '%'+{0}+'%'", _filterModel.LinkText);

            if (_filterModel.SourceType.HasValue)
                _paging.Where("SourceType = {0}", (int)_filterModel.SourceType);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

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
}
