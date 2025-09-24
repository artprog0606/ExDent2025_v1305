using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Landings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetSiteLandings
    {
        private readonly SiteLandingsAdminFilterModel _filterModel;
        private SqlPaging _paging;

        public GetSiteLandings(SiteLandingsAdminFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<LandingAdminItemModel> Execute()
        {
            var model = new FilterResult<LandingAdminItemModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;

            model.DataItems = _paging.PageItemsList<LandingAdminItemModel>();
            
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
                "Landing.Id",
                "Landing.LandingSiteId",
                "Landing.Name",
                "Landing.Enabled",
                "LandingSite.DomainUrl",
                "LandingSite.Url".AsSqlField("SiteUrl"),
                "Landing.Url".AsSqlField("LpUrl"),
                "Landing.IsMain",
                "Landing.CreatedDate"
                );

            _paging.From("[CMS].[Landing]");
            _paging.Inner_Join("[CMS].[LandingSite] ON [LandingSite].[Id] = [Landing].[LandingSiteId]");
            _paging.Where("Landing.LandingSiteId = {0}", _filterModel.SiteId);

            Sorting();
            Filter();
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Landing.Id");
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("Landing.Name LIKE '%'+{0}+'%'", _filterModel.Search);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("Landing.IsMain");
                _paging.OrderBy("CreatedDate");
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
