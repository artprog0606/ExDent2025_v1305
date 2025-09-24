using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class TypeWarehouseService
    {
        #region CRUD

        public static int Add(TypeWarehouse typeWarehouse)
        {
            if (typeWarehouse is null) throw new ArgumentNullException(nameof(typeWarehouse));

            typeWarehouse.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Catalog].[TypeWarehouse]
	                ([Name],[SortOrder],[Enabled],[DateAdded],[DateModified])
                VALUES
	                (@Name,@SortOrder,@Enabled,@DateAdded,@DateModified);
                SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", typeWarehouse.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", typeWarehouse.SortOrder),
                new SqlParameter("@Enabled", typeWarehouse.Enabled),
                new SqlParameter("@DateAdded", DateTime.Now),
                new SqlParameter("@DateModified", DateTime.Now));

            return typeWarehouse.Id;
        }

        public static void Update(TypeWarehouse typeWarehouse)
        {
            if (typeWarehouse is null) throw new ArgumentNullException(nameof(typeWarehouse));
            
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[TypeWarehouse]
                   SET [Name] = @Name
                      ,[SortOrder] = @SortOrder
                      ,[Enabled] = @Enabled
                      ,[DateModified] = @DateModified
                 WHERE [Id] = @Id", 
                CommandType.Text,
                new SqlParameter("@Id", typeWarehouse.Id),
                new SqlParameter("@Name", typeWarehouse.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", typeWarehouse.SortOrder),
                new SqlParameter("@Enabled", typeWarehouse.Enabled),
                new SqlParameter("@DateModified", DateTime.Now));
        }
        
        public static TypeWarehouse Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[TypeWarehouse] WHERE [Id] = @Id",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@Id", id));
        }
  
        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[TypeWarehouse] WHERE [Id] = @Id",
                CommandType.Text, 
                new SqlParameter("@Id", id));
        }
     
        public static TypeWarehouse GetFromReader(SqlDataReader reader)
        {
            return new TypeWarehouse
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                DateAdded = SQLDataHelper.GetDateTime(reader, "DateAdded"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
            };
        }

        #endregion
        
        public static void SetActive(int id, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Catalog].[TypeWarehouse] Set [Enabled] = @Enabled, [DateModified] = @DateModified Where [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@Enabled", active),
                new SqlParameter("@DateModified", DateTime.Now));
        }
    }
}