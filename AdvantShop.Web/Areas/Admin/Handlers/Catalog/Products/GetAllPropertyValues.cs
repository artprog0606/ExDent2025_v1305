
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Properties;
using AdvantShop.Web.Admin.Models.Catalog.PropertyValues;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetAllPropertyValues
    {
        private readonly PropertyValuesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetAllPropertyValues(PropertyValuesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<PropertyValueModel> Execute()
        {
            var model = new FilterResult<PropertyValueModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<PropertyValueModel>();

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
                "PropertyValueId",
                "PropertyId",
                "Value",
                "NameOfValue".AsSqlField("Name")
            );
            _paging.From("[Catalog].[PropertyValue]");
            _paging.Where("PropertyId = {0}", _filterModel.PropertyId);
            _paging.OrderBy("SortOrder");
            _paging.OrderBy("Value");

            var search = !string.IsNullOrWhiteSpace(_filterModel.Search) ? _filterModel.Search.Replace("[", "").Replace("]", "") : null;

            if (!string.IsNullOrWhiteSpace(search))
            {
               _paging.Where("(Value like '%' + {0} + '%' OR NameOfValue like '%' + {0} + '%')", search);
            }
        }
    }
}
