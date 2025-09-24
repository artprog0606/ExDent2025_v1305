using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    class GetErrorLog404
    {
        private AdminErrorLog404FilterModel _filterModel;
        private SqlPaging _paging;

        public GetErrorLog404(AdminErrorLog404FilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<PageError404> Execute()
        {
            var model = new FilterResult<PageError404>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено позиций: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<PageError404>().Where(x => !RedirectSeoService.CheckOnSystemUrl(x.Url)).ToList();
            
            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Error404.Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Url",
                "Error404.Id".AsSqlField("ID"),
                "UrlReferer",
                "UserAgent",
                "IpAddress",
                "RedirectTo",
                "DateAdded");

            _paging.From("[Settings].[Error404]");
            _paging.Left_Join("Settings.Redirect ON Error404.Url = Redirect.RedirectFrom");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
            {
                _paging.Where("Url LIKE '%'+{0}+'%' OR UrlReferer LIKE '%'+{0}+'%'", _filterModel.Search.Replace("'","").Replace("\"",""));
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Url))
            {
                _paging.Where("Url LIKE '%'+{0}+'%'", _filterModel.Url);
            }

            if (!string.IsNullOrEmpty(_filterModel.UrlReferer))
            {
                _paging.Where("UrlReferer LIKE '%'+{0}+'%'", _filterModel.UrlReferer);
            }
            if (!string.IsNullOrEmpty(_filterModel.UserAgent))
            {
                _paging.Where("UserAgent LIKE '%'+{0}+'%'", _filterModel.UserAgent);
            }
            if (!string.IsNullOrEmpty(_filterModel.IpAddress))
            {
                _paging.Where("IpAddress LIKE '%'+{0}+'%'", _filterModel.IpAddress);
            }
            if (!string.IsNullOrEmpty(_filterModel.RedirectTo))
            {
                _paging.Where("RedirectTo LIKE '%'+{0}+'%'", _filterModel.RedirectTo);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("Error404.DateAdded");
                _paging.OrderByDesc("Error404.Id");
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
