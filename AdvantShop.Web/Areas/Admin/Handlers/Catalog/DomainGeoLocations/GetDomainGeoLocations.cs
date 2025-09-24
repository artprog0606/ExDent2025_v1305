using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.DomainOfCities;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.DomainOfCities
{
    public sealed class GetDomainGeoLocations
    {
        private readonly DomainGeoLocationFilterModel _filterModel;
        private SqlPaging _paging;

        public GetDomainGeoLocations(DomainGeoLocationFilterModel filterModel)
        {
            _filterModel = filterModel;
        }
        
        public DomainGeoLocationFilterResult Execute()
        {
            var result = new DomainGeoLocationFilterResult();
        
            GetPaging();

            result.TotalItemsCount = _paging.TotalRowsCount;
            result.TotalPageCount = _paging.PageCount();

            if (result.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return result;
            }

            result.DataItems = _paging.PageItemsList<DomainOfCityGridItem>();

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

            _paging
                .Select(
                    "Id",
                    "Url"
                )
                .From("[Settings].[DomainGeoLocation]");

            Sorting();
            Filter();
        }
 
        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
                _paging.Where("([Url] LIKE '%'+{0}+'%')", _filterModel.Search);

            if (!string.IsNullOrWhiteSpace(_filterModel.Url))
                _paging.Where("[Url] LIKE '%'+{0}+'%'", _filterModel.Url);

            if (_filterModel.CityId.HasValue)
            {
                _paging.Where(
                    "exists (Select 1 From [Settings].[DomainGeoLocation_City] Where [DomainGeoLocation_City].[DomainGeoLocationId] = Id and CityId = {0})", 
                    _filterModel.CityId.Value);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    new SqlCritera("Id", "", SqlSort.Asc)
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