using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.SEO;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class WarehouseService
    {
        public const string CookieName = "warehouses";
        private const string WarehouseCacheKey = "Warehouses_";
        
        #region CRUD

        public static int Add(Warehouse warehouse)
        {
            if (warehouse is null) throw new ArgumentNullException(nameof(warehouse));
            
            if (SaasDataService.IsSaasEnabled && GetCount() >= SaasDataService.CurrentSaasData.WarehouseAmount)
                return 0;

            warehouse.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Catalog].[Warehouse]
	                ([Name],[UrlPath],[Description],[TypeId],[SortOrder],[Enabled],[CityId],[Address],[Longitude],[Latitude],[AddressComment],[Phone],[Phone2],[Email],[DateAdded],[DateModified],[ModifiedBy])
                VALUES
	                (@Name,@UrlPath,@Description,@TypeId,@SortOrder,@Enabled,@CityId,@Address,@Longitude,@Latitude,@AddressComment,@Phone,@Phone2,@Email,@DateAdded,@DateModified,@ModifiedBy);
                SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", warehouse.Name ?? (object) DBNull.Value),
                new SqlParameter("@UrlPath", warehouse.UrlPath ?? (object) DBNull.Value),
                new SqlParameter("@Description", warehouse.Description ?? (object) DBNull.Value),
                new SqlParameter("@TypeId", warehouse.TypeId ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", warehouse.SortOrder),
                new SqlParameter("@Enabled", warehouse.Enabled),
                new SqlParameter("@CityId", warehouse.CityId ?? (object) DBNull.Value),
                new SqlParameter("@Address", warehouse.Address ?? (object) DBNull.Value),
                new SqlParameter("@Longitude", warehouse.Longitude ?? (object) DBNull.Value),
                new SqlParameter("@Latitude", warehouse.Latitude ?? (object) DBNull.Value),
                new SqlParameter("@AddressComment", warehouse.AddressComment ?? (object) DBNull.Value),
                new SqlParameter("@Phone", warehouse.Phone ?? (object) DBNull.Value),
                new SqlParameter("@Phone2", warehouse.Phone2 ?? (object) DBNull.Value),
                new SqlParameter("@Email", warehouse.Email ?? (object) DBNull.Value),
                new SqlParameter("@DateAdded", DateTime.Now),
                new SqlParameter("@DateModified", DateTime.Now),
                new SqlParameter("@ModifiedBy", warehouse.ModifiedBy ?? (object) DBNull.Value));

            if (warehouse.Meta != null)
            {
                if (!warehouse.Meta.Title.IsNullOrEmpty() 
                    || !warehouse.Meta.MetaKeywords.IsNullOrEmpty() 
                    || !warehouse.Meta.MetaDescription.IsNullOrEmpty() 
                    || !warehouse.Meta.H1.IsNullOrEmpty())
                {
                    warehouse.Meta.ObjId = warehouse.Id;
                    MetaInfoService.SetMeta(warehouse.Meta);
                }
            }
            
            CacheManager.RemoveByPattern(WarehouseCacheKey);

            return warehouse.Id;
        }

        public static void Update(Warehouse warehouse)
        {
            if (warehouse is null) throw new ArgumentNullException(nameof(warehouse));
            
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Warehouse]
                   SET [Name] = @Name
                      ,[UrlPath] = @UrlPath
                      ,[Description] = @Description
                      ,[TypeId] = @TypeId
                      ,[SortOrder] = @SortOrder
                      ,[Enabled] = @Enabled
                      ,[CityId] = @CityId
                      ,[Address] = @Address
                      ,[Longitude] = @Longitude
                      ,[Latitude] = @Latitude
                      ,[AddressComment] = @AddressComment
                      ,[Phone] = @Phone
                      ,[Phone2] = @Phone2
                      ,[Email] = @Email
                      ,[DateModified] = @DateModified
                      ,[ModifiedBy] = @ModifiedBy
                 WHERE [Id] = @Id", 
                CommandType.Text,
                new SqlParameter("@Id", warehouse.Id),
                new SqlParameter("@Name", warehouse.Name ?? (object) DBNull.Value),
                new SqlParameter("@UrlPath", warehouse.UrlPath ?? (object) DBNull.Value),
                new SqlParameter("@Description", warehouse.Description ?? (object) DBNull.Value),
                new SqlParameter("@TypeId", warehouse.TypeId ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", warehouse.SortOrder),
                new SqlParameter("@Enabled", warehouse.Enabled),
                new SqlParameter("@CityId", warehouse.CityId ?? (object) DBNull.Value),
                new SqlParameter("@Address", warehouse.Address ?? (object) DBNull.Value),
                new SqlParameter("@Longitude", warehouse.Longitude ?? (object) DBNull.Value),
                new SqlParameter("@Latitude", warehouse.Latitude ?? (object) DBNull.Value),
                new SqlParameter("@AddressComment", warehouse.AddressComment ?? (object) DBNull.Value),
                new SqlParameter("@Phone", warehouse.Phone ?? (object) DBNull.Value),
                new SqlParameter("@Phone2", warehouse.Phone2 ?? (object) DBNull.Value),
                new SqlParameter("@Email", warehouse.Email ?? (object) DBNull.Value),
                new SqlParameter("@DateModified", DateTime.Now),
                new SqlParameter("@ModifiedBy", warehouse.ModifiedBy ?? (object) DBNull.Value));
       
            if (warehouse.Meta != null)
            {
                if (warehouse.Meta.Title.IsNullOrEmpty() 
                    && warehouse.Meta.MetaKeywords.IsNullOrEmpty() 
                    && warehouse.Meta.MetaDescription.IsNullOrEmpty() 
                    && warehouse.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(warehouse.Id, warehouse.MetaType))
                    {
                        MetaInfoService.DeleteMetaInfo(warehouse.Id, warehouse.MetaType);
                    }
                }
                else
                    MetaInfoService.SetMeta(warehouse.Meta);
            }
            
            CacheManager.RemoveByPattern(WarehouseCacheKey);
            CacheManager.RemoveByPattern(DomainGeoLocationService.CacheKey);
            CacheManager.RemoveByPattern(WarehouseCityService.CachePrefix);
        }
        
        public static Warehouse Get(int id)
        {
            return CacheManager.Get(WarehouseCacheKey + "id_" + id, () =>
                SQLDataAccess.ExecuteReadOne(
                    "SELECT TOP 1 * FROM [Catalog].[Warehouse] WHERE [Id] = @Id",
                    CommandType.Text,
                    GetFromReader,
                    new SqlParameter("@Id", id))
            );
        }
        
        public static Warehouse GetByUrl(string url)
        {
            if (url is null) throw new ArgumentNullException(nameof(url));

            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[Warehouse] WHERE [UrlPath] = @UrlPath",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@UrlPath", url ?? (object) DBNull.Value));
        }
  
        public static void Delete(int id)
        {
            var warehouses = GetList();
            if (warehouses.Count == 1
                && warehouses[0].Id == id)
                throw new BlException("Нельзя удалять единственный склад");

            if (WarehouseStocksService.SumStocksOfWarehouse(id) > 0f)
                throw new BlException("Склад с имеющимися остатками нельзя удалять");

            if (Orders.DistributionOfOrderItemService.HasWarehouseInOrderItems(id))
                throw new BlException("Нельзя удалить склад. Есть заказы ссылающиеся на этот склад.");

            if (id == SettingsCatalog.DefaultWarehouse)
                throw new BlException("Нельзя удалить запасной склад.");

            WarehouseStocksService.DeleteZeroStocksByWarehouse(id);
            
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[Warehouse] WHERE [Id] = @Id",
                CommandType.Text, 
                new SqlParameter("@Id", id));
            
            CacheManager.RemoveByPattern(WarehouseCacheKey);
            CacheManager.RemoveByPattern(DomainGeoLocationService.CacheKey);
            CacheManager.RemoveByPattern(WarehouseCityService.CachePrefix);
        }
     
        public static Warehouse GetFromReader(SqlDataReader reader)
        {
            return new Warehouse
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                TypeId = SQLDataHelper.GetNullableInt(reader, "TypeId"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                CityId = SQLDataHelper.GetNullableInt(reader, "CityId"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                Longitude = SQLDataHelper.GetNullableFloat(reader, "Longitude"),
                Latitude = SQLDataHelper.GetNullableFloat(reader, "Latitude"),
                AddressComment = SQLDataHelper.GetString(reader, "AddressComment"),
                Phone = SQLDataHelper.GetString(reader, "Phone"),
                Phone2 = SQLDataHelper.GetString(reader, "Phone2"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                DateAdded = SQLDataHelper.GetDateTime(reader, "DateAdded"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
                ModifiedBy = SQLDataHelper.GetString(reader, "ModifiedBy"),
            };
        }

        #endregion

        public static int GetCount(bool? enabled = null)
        {
            var parameters = new List<SqlParameter>();
            if (enabled.HasValue)
                parameters.Add(new SqlParameter("@Enabled", enabled.Value));
            
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Catalog].[Warehouse] " +
                (enabled.HasValue ? "WHERE [Enabled] = @Enabled " : null),
                CommandType.Text, 
                parameters.ToArray());
        }
                
        public static List<Warehouse> GetList(bool? enabled = null)
        {
            var parameters = new List<SqlParameter>();
            if (enabled.HasValue)
                parameters.Add(new SqlParameter("@Enabled", enabled.Value));

            return CacheManager.Get(WarehouseCacheKey + "list" + enabled, () =>
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Catalog].[Warehouse] " +
                    (enabled.HasValue ? "WHERE [Enabled] = @Enabled " : null) +
                    "ORDER BY [SortOrder] DESC",
                    CommandType.Text,
                    GetFromReader,
                    parameters.ToArray())
            );
        }
                
        public static List<int> GetListIds(bool? enabled = null)
        {
            var parameters = new List<SqlParameter>();
            if (enabled.HasValue)
                parameters.Add(new SqlParameter("@Enabled", enabled.Value));
            
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT [Id] FROM [Catalog].[Warehouse] " +
                (enabled.HasValue ? "WHERE [Enabled] = @Enabled " : null) +
                "ORDER BY [SortOrder] DESC",
                CommandType.Text,
                "Id",
                parameters.ToArray());
        }

        public static void SetActive(int id, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Catalog].[Warehouse] Set [Enabled] = @Enabled, [DateModified] = @DateModified Where [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@Enabled", active),
                new SqlParameter("@DateModified", DateTime.Now));
            
            CacheManager.RemoveByPattern(WarehouseCacheKey);
            CacheManager.RemoveByPattern(DomainGeoLocationService.CacheKey);
            CacheManager.RemoveByPattern(WarehouseCityService.CachePrefix);
        }

        public static bool Exists(int id, bool onlyEnabled = false)
        {
            return
                CacheManager.Get(WarehouseCacheKey + "exists_" + id + "_" + onlyEnabled, () =>
                    SQLDataAccess.ExecuteScalar<int>(
                        "IF EXISTS (SELECT 1 FROM [Catalog].[Warehouse] WHERE [Id] = @Id " +
                        (onlyEnabled ? "and Enabled = 1" : "") + @")
                    SELECT 1
                    ELSE
                    SELECT 0",
                        CommandType.Text,
                        new SqlParameter("@Id", id))
                    == 1
                );
        }

        public static void DeactivateStorageMoreThan(int activeWarehouseCount)
        {
            if (activeWarehouseCount <= 0)
                return;

            SQLDataAccess.ExecuteNonQuery(
                @"if(Select (Count(*)) From [Catalog].[Warehouse] Where Warehouse.Enabled = 1 ) > @activeWarehouseCount
                  BEGIN
                    ;WITH storageToDeactivate AS 
                    ( 
	                    Select 
	                    Top(Select (Count(*) - @activeWarehouseCount) From [Catalog].[Warehouse] Where Warehouse.Enabled = 1) Warehouse.Id 
	                    From [Catalog].[Warehouse]
	                    Where Warehouse.Enabled = 1 
	                    Order by [SortOrder], Warehouse.DateAdded Desc
                    ) 
                    UPDATE [Catalog].[Warehouse] SET Enabled = 0 Where Id in (Select Id From storageToDeactivate)
                  END",
                CommandType.Text,
                new SqlParameter("@activeWarehouseCount", activeWarehouseCount));
        }

        public static void SetCookie(List<int> currentWarehouseIds)
        {
            CommonHelper.SetCookie(CookieName, string.Join(",", currentWarehouseIds), true, setOnMainDomain: true);
        }
    }
}