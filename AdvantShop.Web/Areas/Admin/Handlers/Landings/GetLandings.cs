using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Landings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetLandings
    {
        private readonly LandingsAdminFilterModel _filterModel;
        private SqlPaging _paging;

        public GetLandings(LandingsAdminFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public LandingsFilterResult Execute()
        {
            var model = new LandingsFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;

            model.DataItems = _paging.PageItemsList<LandingSiteAdminItemModel>();
            
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
                "Id",
                "Name",
                "Enabled",
                "DomainUrl",
                "Url".AsSqlField("LandingSiteUrl"),
                "(Select Count(Id) From [CMS].[Landing] Where Landing.LandingSiteId = LandingSite.Id)".AsSqlField("LandingsCount"),
                "CreatedDate"
                );

            _paging.From("[CMS].[LandingSite]");

            Sorting();
            Filter();
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Id");
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("Name LIKE '%'+{0}+'%' OR DomainUrl LIKE '%'+{0}+'%' OR Url LIKE '%'+{0}+'%'", _filterModel.Search);
            }

            if (_filterModel.Enabled != null)
            {
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value ? "1" : "0");
            }

            DateTime from, to;

            if (!string.IsNullOrWhiteSpace(_filterModel.CreatedDateFrom) && DateTime.TryParse(_filterModel.CreatedDateFrom, out from))
            {
                _paging.Where("CreatedDate >= {0}", from);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.CreatedDateTo) && DateTime.TryParse(_filterModel.CreatedDateTo, out to))
            {
                _paging.Where("CreatedDate <= {0}", to);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("CreatedDate");
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
