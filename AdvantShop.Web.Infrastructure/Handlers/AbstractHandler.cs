using System.Collections.Generic;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Infrastructure.Handlers
{
    public abstract class AbstractHandler<TKey, TIds, TResult> : IHandler<TResult, TIds> where TKey : BaseFilterModel<TIds>
    {
        private SqlPaging _paging;
        protected readonly TKey FilterModel;

        protected AbstractHandler(TKey filterModel)
        {
            FilterModel = filterModel;
        }

        public AbstractFilterResult<TResult> Execute()
        {
            var model = new AbstractFilterResult<TResult>();
            _paging = _paging != null ? _paging : GetPaging();

            model.PageIndex = FilterModel.Page;

            model.ItemsPerPage = _paging.ItemsPerPage;
            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = T("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < FilterModel.Page && FilterModel.Page > 1)
                return model;
            
            model.DataItems = _paging.PageItemsList<TResult>();

            return model;
        }

        public List<TIds> GetItemsIds(string field)
        {
            _paging = _paging != null ? _paging : GetPaging();

            return _paging.ItemsIds<TIds>(field);
        }

        protected string T(string resourceKey)
        {
            return LocalizationService.GetResource(resourceKey);
        }

        protected string T(string resourceKey, params object[] parameters)
        {
            return LocalizationService.GetResourceFormat(resourceKey, parameters);
        }

        private SqlPaging GetPaging( )
        {
            _paging = new SqlPaging
            {
                ItemsPerPage = FilterModel.ItemsPerPage,
                CurrentPageIndex = FilterModel.Page
            };

            _paging = Select(_paging);
            _paging = Filter(_paging);
            _paging = Sorting(_paging);
            return _paging;
        }

        protected virtual SqlPaging Select(SqlPaging paging)
        {
            return paging;
        }

        protected virtual SqlPaging Filter(SqlPaging paging)
        {
            return paging;
        }

        protected virtual SqlPaging Sorting(SqlPaging paging)
        {
            return paging;
        }
    }

    public class AbstractFilterResult<TResult> : FilterResult<TResult>
    {
        public int ItemsPerPage { get; set; }
    }
}