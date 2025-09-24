using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Customers.Subscription;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Customers.Subscription
{
    public class GetSubscription
    {

        private SubscriptionFilterModel _filterModel;
        private SqlPaging _paging;

        public GetSubscription(SubscriptionFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<SubscriptionFilterResultModel> Execute()
        {
            var model = new FilterResult<SubscriptionFilterResultModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Subscribe.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<SubscriptionFilterResultModel>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Id");
        }

        private void GetPaging()
        {
            _paging =
                new SqlPaging()
                    {
                        ItemsPerPage = _filterModel.ItemsPerPage, 
                        CurrentPageIndex = _filterModel.Page
                    }
                    .Select(
                        "Id",
                        "Email",
                        "Subscribe".AsSqlField("Enabled"),
                        "SubscribeDate",
                        "SubscribeFromPage",
                        "SubscribeFromIp",
                        "UnsubscribeDate")
                    .From("[Customers].[Subscription]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
            {
                _filterModel.Email = _filterModel.Search;
            }
            
            if (!string.IsNullOrWhiteSpace(_filterModel.Email))
            {
                _paging.Where("Email LIKE '%'+{0}+'%'", _filterModel.Email);
            }

            if (_filterModel.Enabled != null)
            {
                _paging.Where("Subscribe = {0}", (bool)_filterModel.Enabled ? "1" : "0");
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.UnSubscribeFrom) && DateTime.TryParse(_filterModel.UnSubscribeFrom, out var unsFrom))
            {
                _paging.Where("UnsubscribeDate >= {0}", unsFrom);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.UnsubscribeTo) && DateTime.TryParse(_filterModel.UnsubscribeTo, out var unsTo))
            {
                _paging.Where("UnsubscribeDate <= {0}", unsTo);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.SubscribeFrom) && DateTime.TryParse(_filterModel.SubscribeFrom, out var subFrom))
            {
                _paging.Where("SubscribeDate >= {0}", subFrom);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.SubscribeTo) && DateTime.TryParse(_filterModel.SubscribeTo, out var subTo))
            {
                _paging.Where("SubscribeDate <= {0}", subTo);
            }

            if (_filterModel.SubscribeFrom != null)
            {
                _paging.Where("SubscribeFromPage = {0}", _filterModel.SubscribeFrom);
            }

            if (_filterModel.SubscribeFromIp != null)
            {
                _paging.Where("SubscribeFromIp = {0}", _filterModel.SubscribeFromIp);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(new SqlCritera("Email", "", SqlSort.Asc));
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

            if (sorting == "subscribedatestr")
                sorting = "subscribedate";

            if (sorting == "unsubscribedatestr")
                sorting = "unsubscribedate";

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
