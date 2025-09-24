using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Orders.OrderSources;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Orders.OrderSources
{
    public class GetOrderSourcesHandler
    {
        private readonly OrderSourcesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetOrderSourcesHandler(OrderSourcesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<OrderSourceModel> Execute()
        {
            var model = new FilterResult<OrderSourceModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<OrderSourceModel>();
            
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
                "Id",
                "Name",
                "Main",
                "Type",
                "SortOrder",
                "ObjId",
                "(Select Count(OrderId) From [Order].[Order] Where OrderSourceId = OrderSource.Id)".AsSqlField("OrdersCount"),
                "(Select Count([Lead].Id) From [Order].[Lead] Where OrderSourceId = OrderSource.Id)".AsSqlField("LeadsCount")
                );

            _paging.From("[Order].[OrderSource]");


            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Search);
            if (_filterModel.Name.IsNotEmpty())
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Name);
            if (_filterModel.Type.HasValue)
                _paging.Where("Type = {0}", _filterModel.Type.ToString());
            if (_filterModel.Main.HasValue)
                _paging.Where("Main = {0}", _filterModel.Main.Value);

            if (_filterModel.OrdersCountFrom.HasValue)
                _paging.Where("(Select Count(OrderId) From [Order].[Order] Where OrderSourceId = OrderSource.Id) >= {0}", _filterModel.OrdersCountFrom.Value);
            if (_filterModel.OrdersCountTo.HasValue)
                _paging.Where("(Select Count(OrderId) From [Order].[Order] Where OrderSourceId = OrderSource.Id) <= {0}", _filterModel.OrdersCountTo.Value);

            if (_filterModel.LeadsCountFrom.HasValue)
                _paging.Where("(Select Count([Lead].Id) From [Order].[Lead] Where OrderSourceId = OrderSource.Id) >= {0}", _filterModel.LeadsCountFrom.Value);
            if (_filterModel.LeadsCountTo.HasValue)
                _paging.Where("(Select Count([Lead].Id) From [Order].[Lead] Where OrderSourceId = OrderSource.Id) <= {0}", _filterModel.LeadsCountTo.Value);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
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