using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Warehouses
{
    public class GetWarehouses
    {
        private readonly WarehousesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetWarehouses(WarehousesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public WarehousesFilterResult Execute()
        {
            var result = new WarehousesFilterResult();
        
            GetPaging();

            result.TotalItemsCount = _paging.TotalRowsCount;
            result.TotalPageCount = _paging.PageCount();

            if (result.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return result;
            }

            result.DataItems = _paging.PageItemsList<WarehouseGridModel>();

            return result;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Warehouse.Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            var countWarehouse = WarehouseService.GetList().Count;
            
            _paging.Select(
                "Warehouse.Id".AsSqlField("WarehouseId"),
                "Warehouse.Name".AsSqlField("WarehouseName"),
                "Warehouse.SortOrder",
                "Warehouse.Enabled",
                "(SELECT ISNULL(SUM([Quantity]),0) FROM [Catalog].[WarehouseStocks] WHERE [WarehouseStocks].[WarehouseId]=Warehouse.Id)".AsSqlField("Amount"),
                countWarehouse.ToString().AsSqlField("CountWarehouses"),
                "(CASE WHEN EXISTS (SELECT * FROM [Order].[WarehouseOrderItem] WHERE [WarehouseOrderItem].[WarehouseId] = Warehouse.Id) THEN 1 ELSE  0 END)".AsSqlField("UseInOrders"),
                Configuration.SettingsCatalog.DefaultWarehouse.ToString().AsSqlField("DefaultWarehouse"),
                "Warehouse.TypeId".AsSqlField("TypeId"),
                "TypeWarehouse.Name".AsSqlField("TypeName"),
                "City.CityID".AsSqlField("CityId"),
                "City.CityName".AsSqlField("CityName"));

            _paging.From("[Catalog].[Warehouse]");
            _paging.Left_Join("[Catalog].[TypeWarehouse] On [TypeWarehouse].Id = [Warehouse].TypeId");
            _paging.Left_Join("[Customers].[City] On [City].CityID = [Warehouse].CityId");

            Sorting();
            Filter();
        }
 
        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
                _filterModel.WarehouseName = _filterModel.Search;

            if (!string.IsNullOrWhiteSpace(_filterModel.WarehouseName))
                _paging.Where("Warehouse.Name LIKE '%'+{0}+'%'", _filterModel.WarehouseName);

            if (_filterModel.CityId.HasValue)
                _paging.Where("[Warehouse].CityId = {0}", _filterModel.CityId);

            if (_filterModel.TypeId.HasValue)
                _paging.Where("[Warehouse].TypeId = {0}", _filterModel.TypeId);

            if (_filterModel.Enabled != null)
                _paging.Where("Warehouse.Enabled = {0}", _filterModel.Enabled.Value ? "1" : "0");

            if (_filterModel.SortingFrom != 0)
                _paging.Where("Warehouse.SortOrder >= {0}", _filterModel.SortingFrom);

            if (_filterModel.SortingTo != 0)
                _paging.Where("Warehouse.SortOrder <= {0}", _filterModel.SortingTo);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("SortOrder");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                    _paging.OrderBy(sorting);
                else
                    _paging.OrderByDesc(sorting);
            }
        }
    }
}