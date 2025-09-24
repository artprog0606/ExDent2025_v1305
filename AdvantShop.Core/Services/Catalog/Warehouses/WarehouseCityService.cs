using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class WarehouseCityService
    {
        public const string CachePrefix = "WarehouseCity_";  
        private const string WarehouseIdsByCityCachePrefix = CachePrefix + "WarehouseIdsByCity_";
        private const string WarehousesByCitiesCachePrefix = CachePrefix + "WarehousesByCities_";
        
        public static void Add(WarehouseCity warehouseCity)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Catalog].[Warehouse_City] (WarehouseId, CityId, SortOrder) Values (@WarehouseId, @CityId, @SortOrder)",
                CommandType.Text,
                new SqlParameter("@WarehouseId", warehouseCity.WarehouseId),
                new SqlParameter("@CityId", warehouseCity.CityId),
                new SqlParameter("@SortOrder", warehouseCity.SortOrder)
            );
            
            CacheManager.RemoveByPattern(CachePrefix);
        }
        
        public static void Update(WarehouseCity warehouseCity)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Catalog].[Warehouse_City] Set SortOrder = @SortOrder Where  WarehouseId = @WarehouseId and CityId = @CityId",
                CommandType.Text,
                new SqlParameter("@WarehouseId", warehouseCity.WarehouseId),
                new SqlParameter("@CityId", warehouseCity.CityId),
                new SqlParameter("@SortOrder", warehouseCity.SortOrder)
            );
            
            CacheManager.RemoveByPattern(CachePrefix);
        }
        
        public static void Delete(int warehouseId, int cityId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Catalog].[Warehouse_City] Where WarehouseId = @WarehouseId and CityId = @CityId",
                CommandType.Text,
                new SqlParameter("@WarehouseId", warehouseId),
                new SqlParameter("@CityId", cityId)
            );
            
            CacheManager.RemoveByPattern(CachePrefix);
        }
        
        public static int GetMaxSortOrder(int warehouseId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select isnull(Max(SortOrder), 0) From [Catalog].[Warehouse_City] Where WarehouseId = @WarehouseId",
                CommandType.Text,
                new SqlParameter("@WarehouseId", warehouseId)
            );
        }
        
        public static WarehouseCity Get(int warehouseId, int cityId)
        {
            return SQLDataAccess
                .Query<WarehouseCity>(
                    "Select * From [Catalog].[Warehouse_City] Where  WarehouseId = @warehouseId and CityId = @cityId", 
                    new { warehouseId, cityId })
                .FirstOrDefault();
        }
        
        public static List<CityOfWarehouse> GetCities(int warehouseId)
        {
            return SQLDataAccess
                .Query<CityOfWarehouse>(
                    "Select c.CityId, c.CityName " +
                    "From [Catalog].[Warehouse_City] as wc " +
                    "Inner Join [Customers].[City] as c  on c.CityId = wc.CityId " +
                    "Where WarehouseId = @warehouseId " +
                    "Order by SortOrder", 
                    new { warehouseId })
                .ToList();
        }
        
        public static List<int> GetWarehouseIds(int cityId, bool onlyEnabled = true)
        {
            return CacheManager.Get(WarehouseIdsByCityCachePrefix + cityId, () =>
                SQLDataAccess
                    .Query<int>(
                        "Select wc.WarehouseId " +
                        "From [Catalog].[Warehouse_City] as wc " +
                        "Inner Join [Catalog].[Warehouse] as w  on w.Id = wc.WarehouseId " +
                        "Where wc.CityId = @cityId " + (onlyEnabled ? " and w.Enabled = 1 " : "") +
                        "Order By wc.SortOrder",
                        new { cityId })
                    .ToList()
            );
        }
        
        public static List<WarehouseCity> GetWarehouseCities(List<int> cityIds, bool onlyEnabled = true)
        {
            if (cityIds == null || cityIds.Count == 0)
                return new List<WarehouseCity>();
            
            var ids = string.Join(",", cityIds);

            return CacheManager.Get(WarehousesByCitiesCachePrefix + ids, () =>
                SQLDataAccess
                    .Query<WarehouseCity>(
                        "Select wc.* " +
                        "From [Catalog].[Warehouse_City] as wc " +
                        "Inner Join [Catalog].[Warehouse] as w on w.Id = wc.WarehouseId " +
                        "Where wc.CityId in (" + ids + ") " + (onlyEnabled ? " and w.Enabled = 1 " : "") 
                    )
                    .ToList()
            );
        }
        
        public static List<WarehouseByCity> GetWarehouses(int cityId, bool onlyEnabled = true)
        {
            return SQLDataAccess
                .Query<WarehouseByCity>(
                    "Select w.Id, w.Name " +
                    "From [Catalog].[Warehouse_City] as wc " +
                    "Inner Join [Catalog].[Warehouse] as w  on w.Id = wc.WarehouseId " +
                    "Where wc.CityId = @cityId " + (onlyEnabled ? " and w.Enabled = 1 " : "") +
                    "Order By wc.SortOrder", 
                    new { cityId })
                .ToList();
        }
    }
}