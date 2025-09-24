using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings.Location;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class GetCitiesList : ICommandHandler<CitiesListFilterResult>
    {
        private readonly CitiesListFilterModel _filter;
        private SqlPaging _paging;

        public GetCitiesList(CitiesListFilterModel filter)
        {
            _filter = filter;
        }

        public CitiesListFilterResult Execute()
        {
            var result = new CitiesListFilterResult();
        
            GetPaging();

            result.TotalItemsCount = _paging.TotalRowsCount;
            result.TotalPageCount = _paging.PageCount();

            if (result.TotalPageCount < _filter.Page && _filter.Page > 1)
            {
                return result;
            }

            result.DataItems = _paging.PageItemsList<CityOfListModel>();

            return result;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("City.CityID");
        }

        public CityOfListModel GetItem(int cityId)
        {
            GetPaging();

            _paging.Where("City.CityID = {0}", cityId);

            return _paging.PageItemsList<CityOfListModel>().FirstOrDefault();
        }

        private void GetPaging()
        {
            _paging = new SqlPaging
            {
                ItemsPerPage = _filter.ItemsPerPage,
                CurrentPageIndex = _filter.Page
            };

            _paging
               .Select(
                    "City.CityID".AsSqlField("CityId"),
                    "City.CityName".AsSqlField("CityName"),
                    "City.District".AsSqlField("District"),
                    "City.DisplayInPopup".AsSqlField("DisplayInPopup"),
                    "Region.RegionID".AsSqlField("RegionId"),
                    "Region.RegionName".AsSqlField("RegionName"),
                    "Country.CountryID".AsSqlField("CountryId"),
                    "Country.CountryName"
                )
               .From("[Customers].[City]")
               .Left_Join("[Customers].[Region] On [Region].[RegionID] = [City].[RegionID]")
               .Left_Join("[Customers].[Country] On [Country].[CountryID] = [Region].[CountryID]");

            Sorting();
            Filter();
        }
 
        private void Filter()
        {
            if (_filter.CountryId != null && _filter.RegionId == null)
                _paging.Where("Country.CountryId = {0}", _filter.CountryId);
            
            if (_filter.RegionId != null)
                _paging.Where("Region.RegionId = {0}", _filter.RegionId);
            
            if (_filter.CityName.IsNotEmpty())
                _paging.Where("CityName Like {0}+'%'", _filter.CityName);
            
            if (_filter.Search.IsNotEmpty())
                _paging.Where("City.CityName LIKE '%'+{0}+'%'", _filter.Search);
        }
   
        private void Sorting()
        {
            if (_filter.Sorting == "SelectCityInModal")
            {
                if (_filter.Search.IsNullOrEmpty() && (_filter.CountryId != null || _filter.RegionId != null))
                    _paging.OrderByDesc("City.DisplayInPopup");
                
                _paging.OrderBy("City.CityId");
                return;
            }
            
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(new SqlCritera("City.CityName", "", SqlSort.Asc));
                return;
            }

            var sorting = _filter.Sorting.ToLower();

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
                    _paging.OrderBy(sorting);
                else
                    _paging.OrderByDesc(sorting);
            }

            _paging.OrderBy(new SqlCritera("City.CityName", "", SqlSort.Asc));
        }
    }
}