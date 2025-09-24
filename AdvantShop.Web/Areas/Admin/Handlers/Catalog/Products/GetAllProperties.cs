
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Properties;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetAllProperties
    {
        private readonly PropertiesFilterModel _filter;
        private SqlPaging _paging;

        public GetAllProperties(PropertiesFilterModel filterModel)
        {
            _filter = filterModel;
        }

        public FilterResult<PropertyShortModel> Execute()
        {
            var model = new FilterResult<PropertyShortModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<PropertyShortModel>();

            return model;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
                {
                    ItemsPerPage = _filter.ItemsPerPage,
                    CurrentPageIndex = _filter.Page
                }
                .Select(
                    "[Property].[PropertyId]",
                    "Name",
                    "Type"
                )
                .From("[Catalog].[Property]")
                .OrderBy("Name");

            
            if (_filter.UseInDetails != null)
                _paging.Where("UseInDetails = {0}", _filter.UseInDetails.Value);
            
            if (!string.IsNullOrWhiteSpace(_filter.Search))
                _paging.Where("Name like '%'+{0}+'%'", _filter.Search);
            
            if (_filter.ProductId.HasValue)
            {
                _paging.Where(@"CASE WHEN [Property].[Type] <> 4 
                                    THEN NULL 
                                    ELSE (SELECT ProductID from Catalog.ProductPropertyValue 
                                            WHERE ProductID = {0} AND PropertyValueID IN (SELECT PropertyValueID FROM Catalog.PropertyValue WHERE PropertyValue.PropertyID = Property.PropertyID)) 
                                END IS NULL", _filter.ProductId);
            }
        }
    }
}
