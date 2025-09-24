using System.Collections.Generic;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Infrastructure.Api
{
    public abstract class EntitiesHandler<TFilter, TResult> : ICommandHandler<EntitiesFilterResult<TResult>> where TFilter : EntitiesFilterModel
    {
        private SqlPaging _paging;
        protected readonly TFilter FilterModel;

        protected EntitiesHandler(TFilter filterModel)
        {
            FilterModel = filterModel;
        }

        public EntitiesFilterResult<TResult> Execute()
        {
            var model = new EntitiesFilterResult<TResult>();
            _paging = _paging != null ? _paging : GetPaging();

            model.PageIndex = FilterModel.Page;

            model.ItemsPerPage = _paging.ItemsPerPage;
            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            if (model.TotalPageCount < FilterModel.Page && FilterModel.Page > 1)
                return model;

            model.DataItems = FillItems(_paging);

            return model;
        }

        protected string T(string resourceKey)
        {
            return LocalizationService.GetResource(resourceKey);
        }

        protected string T(string resourceKey, params object[] parameters)
        {
            return LocalizationService.GetResourceFormat(resourceKey, parameters);
        }

        private SqlPaging GetPaging()
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

        protected virtual List<TResult> FillItems(SqlPaging paging)
        {
            return _paging.PageItemsList<TResult>();
        }
    }
}
