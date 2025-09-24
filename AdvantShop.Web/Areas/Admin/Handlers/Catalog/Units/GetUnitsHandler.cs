using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Units;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Units
{
    public class GetUnitsHandler
    {
        private readonly UnitsFilterModel _filterModel;
        private SqlPaging _paging;

        public GetUnitsHandler(UnitsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<UnitModel> Execute()
        {
            var model = new FilterResult<UnitModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<UnitModel>();
            
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
                "DisplayName",
                "SortOrder",
                "(Select count(*) From [Catalog].[Product] Where Product.[Unit] = Units.Id)".AsSqlField("ProductsCount")
            );

            _paging.From("[Catalog].[Units]");
            
            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
                _paging.Where("(Name LIKE '%'+{0}+'%' OR DisplayName LIKE '%'+{0}+'%')", _filterModel.Search);

            if (!string.IsNullOrEmpty(_filterModel.Name))
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Name);

            if (!string.IsNullOrEmpty(_filterModel.DisplayName))
                _paging.Where("DisplayName LIKE '%'+{0}+'%'", _filterModel.DisplayName);
            
            if (_filterModel.WithOutMeasureType is true)
                _paging.Where("MeasureType IS NULL");
            
            if (_filterModel.WithOutMeasureType is false)
                _paging.Where("MeasureType IS NOT NULL");
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                _paging.OrderBy("Name");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("formatted", "");

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