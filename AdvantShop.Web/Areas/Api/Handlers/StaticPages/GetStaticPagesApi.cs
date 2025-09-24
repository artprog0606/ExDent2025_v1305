using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.StaticPages;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.StaticPages
{
    public class GetStaticPagesApi : AbstractCommandHandler<GetStaticPagesResponse>
    {
        private readonly StaticPagesFilter _filter;
        private SqlPaging _paging;

        public GetStaticPagesApi(StaticPagesFilter filter)
        {
            _filter = filter;
        }
        
        protected override void Validate()
        {
            if (_filter.IsDefaultItemsPerPage)
                _filter.ItemsPerPage = 100;

            if (_filter.ItemsPerPage > 100 || _filter.ItemsPerPage <= 0)
                _filter.ItemsPerPage = 100;

            if (_filter.Page < 0)
                throw new BlException("page can't less than 0");
        }

        protected override GetStaticPagesResponse Handle()
        {
            GetPaging();

            var model = new GetStaticPagesResponse()
            {
                Pagination = new ApiPagination()
                {
                    CurrentPage = _paging.CurrentPageIndex,
                    TotalCount = _paging.TotalRowsCount,
                    TotalPageCount = _paging.PageCount()
                },
                StaticPages = new List<StaticPageItem>()
            };

            if (model.Pagination.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;

            model.StaticPages = _paging.PageItemsList<StaticPageItem>();
            model.Pagination.Count = model.StaticPages.Count;

            return model;
        }

        private void GetPaging()
        {
            _paging =
                new SqlPaging() {ItemsPerPage = _filter.ItemsPerPage, CurrentPageIndex = _filter.Page}
                    .Select(
                        "Id",
                        "Title",
                        "Icon",
                        "ShowInProfile"
                    )
                    .From("[CMS].[StaticPageApi]");

            if (_filter.LoadText != null && _filter.LoadText.Value)
                _paging.Select("Text");

            Filter();
            Sorting();            
        }

        private void Filter()
        {
            _paging.Where("Enabled = 1");
            
            if (!string.IsNullOrEmpty(_filter.Search))
                _paging.Where("(Title LIKE '%'+{0}+'%' OR Text LIKE '%'+{0}+'%')", _filter.Search);

            if (!string.IsNullOrWhiteSpace(_filter.Title))
                _paging.Where("Title LIKE '%'+{0}+'%'", _filter.Title);

            if (_filter.ShowInProfile != null)
                _paging.Where("ShowInProfile = {0}", _filter.ShowInProfile.Value ? 1 : 0);
        }


        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                return;
            }

            var sorting = _filter.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
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