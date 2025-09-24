using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Shipping
{
    public class ShippingWarehouseMappingService
    {
        public static void Add(int methodId, int warehouseId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS (SELECT * FROM [Order].[ShippingWarehouse] WHERE [MethodId] = @MethodId AND [WarehouseId] = @WarehouseId)
                BEGIN
                    INSERT INTO [Order].[ShippingWarehouse] ([MethodId],  [WarehouseId]) Values (@MethodId, @WarehouseId)
                END",
                CommandType.Text,
                new SqlParameter("@MethodId", methodId),
                new SqlParameter("@WarehouseId", warehouseId));
        }

        public static void Delete(int methodId, int warehouseId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Order].[ShippingWarehouse] WHERE [MethodId] = @MethodId AND [WarehouseId] = @WarehouseId",
                CommandType.Text,
                new SqlParameter("@MethodId", methodId),
                new SqlParameter("@WarehouseId", warehouseId));
        }

        public static List<int> GetByMethod(int methodId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                @"SELECT [ShippingWarehouse].[WarehouseId] 
                FROM [Order].[ShippingWarehouse] 
                      INNER JOIN [Catalog].[Warehouse] ON [Warehouse].[Id] = [ShippingWarehouse].[WarehouseId] 
                WHERE [MethodId] = @MethodId
                ORDER BY [Warehouse].[SortOrder] DESC",
                CommandType.Text,
                "WarehouseId",
                new SqlParameter("@MethodId", methodId));
        }
    }
}