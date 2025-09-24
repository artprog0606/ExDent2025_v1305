using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings.ShippingReplaceGeo;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingReplaceGeo
{
    public class GetListShippingReplaceGeoHandler
    {
        private readonly ShippingReplaceGeoFilterModel _filterModel;
        private SqlPaging _paging;

        public GetListShippingReplaceGeoHandler(ShippingReplaceGeoFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<ShippingReplaceGeoModel> Execute()
        {
            var model = new FilterResult<ShippingReplaceGeoModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено замен: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<ShippingReplaceGeoModel>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Id");
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
                "ShippingType",
                "InCountryName",
                "InCountryISO2",
                "InRegionName",
                "InCityName",
                "InDistrict",
                "InZip",
                "OutCountryName",
                "OutRegionName",
                "OutCityName",
                "OutDistrict",
                "OutDistrictClear",
                "OutZip",
                "Enabled",
                "Sort",
                "Comment");

            _paging.From("[Order].[ShippingReplaceGeo]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
                _paging.Where("Comment LIKE '%'+{0}+'%'", _filterModel.Search);

            if (_filterModel.ShippingType.IsNotEmpty())
                _paging.Where("ShippingType = {0}", _filterModel.ShippingType);
            if (_filterModel.InCountryName.IsNotEmpty())
                _paging.Where("InCountryName LIKE '%'+{0}+'%'", _filterModel.InCountryName);
            if (_filterModel.InCountryISO2.IsNotEmpty())
                _paging.Where("InCountryISO2 LIKE '%'+{0}+'%'", _filterModel.InCountryISO2);
            if (_filterModel.InRegionName.IsNotEmpty())
                _paging.Where("InRegionName LIKE '%'+{0}+'%'", _filterModel.InRegionName);
            if (_filterModel.InCityName.IsNotEmpty())
                _paging.Where("InCityName LIKE '%'+{0}+'%'", _filterModel.InCityName);
            if (_filterModel.InDistrict.IsNotEmpty())
                _paging.Where("InDistrict LIKE '%'+{0}+'%'", _filterModel.InDistrict);
            if (_filterModel.InZip.IsNotEmpty())
                _paging.Where("InZip LIKE '%'+{0}+'%'", _filterModel.InZip);
            if (_filterModel.OutCountryName.IsNotEmpty())
                _paging.Where("OutCountryName LIKE '%'+{0}+'%'", _filterModel.OutCountryName);
            if (_filterModel.OutRegionName.IsNotEmpty())
                _paging.Where("OutRegionName LIKE '%'+{0}+'%'", _filterModel.OutRegionName);
            if (_filterModel.OutCityName.IsNotEmpty())
                _paging.Where("OutCityName LIKE '%'+{0}+'%'", _filterModel.OutCityName);
            if (_filterModel.OutDistrict.IsNotEmpty())
                _paging.Where("OutDistrict LIKE '%'+{0}+'%'", _filterModel.OutDistrict);
            if (_filterModel.OutDistrictClear.HasValue)
                _paging.Where("OutDistrictClear = {0}", _filterModel.OutDistrictClear.Value);
            if (_filterModel.OutZip.IsNotEmpty())
                _paging.Where("OutZip LIKE '%'+{0}+'%'", _filterModel.OutZip);
            if (_filterModel.Enabled.HasValue)
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value);
            //if (_filterModel.Comment.IsNotEmpty())
            //    _paging.Where("Comment LIKE '%'+{0}+'%'", _filterModel.Comment);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("Sort");
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
