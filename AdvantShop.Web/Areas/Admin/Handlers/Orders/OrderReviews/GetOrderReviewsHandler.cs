using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Orders.OrderReview;
using AdvantShop.Web.Infrastructure.Admin;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Orders.OrderReviews
{
    public class GetOrderReviewsHandler
    {
        private readonly OrderReviewFilterModel _filterModel;
        private SqlPaging _paging;

        public GetOrderReviewsHandler(OrderReviewFilterModel model)
        {
            _filterModel = model;
        }

        public FilterResult<OrderReviewModel> Execute()
        {
            var model = new FilterResult<OrderReviewModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<OrderReviewModel>();

            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "[Order].[OrderId]",
                "[Order].[Number]".AsSqlField("OrderNumber"),
                "Ratio",
                "Text"
                );

            _paging.From("[Order].[OrderReview]");
            _paging.Inner_Join("[Order].[Order] ON [Order].[OrderId] = [OrderReview].[OrderId]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
                _paging.Where("Number LIKE '%'+{0}+'%'", _filterModel.Search);

            if (_filterModel.OrderNumber.IsNotEmpty())
                _paging.Where("[Order].[Number] LIKE '%'+{0}+'%'", _filterModel.OrderNumber);

            if (_filterModel.Text.IsNotEmpty())
                _paging.Where("Text LIKE '%'+{0}+'%'", _filterModel.Text);

            if (_filterModel.RatioFrom.HasValue)
                _paging.Where("Ratio >= {0}", _filterModel.RatioFrom);

            if (_filterModel.RatioTo.HasValue)
                _paging.Where("Ratio <= {0}", _filterModel.RatioTo);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

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