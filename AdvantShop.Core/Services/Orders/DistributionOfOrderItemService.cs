using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Orders
{
    public class DistributionOfOrderItemService
    {
        public static void AddOrUpdateDistributionOfOrderItem(DistributionOfOrderItem distributionOfOrderItem)
        {
            distributionOfOrderItem = distributionOfOrderItem ?? throw new ArgumentNullException(nameof(distributionOfOrderItem));

            SQLDataAccess.ExecuteNonQuery(
                @"IF EXISTS (SELECT * FROM [Order].[WarehouseOrderItem] WHERE [OrderItemId] = @OrderItemId AND [WarehouseId] = @WarehouseId)
                BEGIN
	                UPDATE [Order].[WarehouseOrderItem]
	                   SET [Amount] = @Amount
		                  ,[DecrementedAmount] = @DecrementedAmount
	                WHERE [OrderItemId] = @OrderItemId AND [WarehouseId] = @WarehouseId
                END
                ELSE
                BEGIN
	                INSERT INTO [Order].[WarehouseOrderItem] ([OrderItemId],[WarehouseId],[Amount],[DecrementedAmount])
	                VALUES (@OrderItemId,@WarehouseId,@Amount,@DecrementedAmount)
                END",
                CommandType.Text,
                new SqlParameter("@OrderItemId", distributionOfOrderItem.OrderItemId),
                new SqlParameter("@WarehouseId", distributionOfOrderItem.WarehouseId),
                new SqlParameter("@Amount", distributionOfOrderItem.Amount),
                new SqlParameter("@DecrementedAmount", distributionOfOrderItem.DecrementedAmount)
            );
        }

        // public static void DeleteDistributionOfOrderItem(int orderItemId, int warehouseId)
        // {
        //     SQLDataAccess.ExecuteNonQuery(
        //         @"DELETE FROM [Order].[WarehouseOrderItem] WHERE [OrderItemId] = @OrderItemId AND [WarehouseId] = @WarehouseId",
        //         CommandType.Text,
        //         new SqlParameter("@OrderItemId", orderItemId),
        //         new SqlParameter("@WarehouseId", warehouseId)
        //     );
        //     
        // }

        public static bool HasWarehouseInOrderItems(int warehouseId)
        {
            return 
                SQLDataAccess.ExecuteScalar<int>(@"IF EXISTS (SELECT * FROM [Order].[WarehouseOrderItem] WHERE [WarehouseId] = @WarehouseId)
                    SELECT 1
                    ELSE
                    SELECT 0",
                    CommandType.Text,
                    new SqlParameter("@WarehouseId", warehouseId))
                == 1;
        }

        public static List<DistributionOfOrderItem> GetDistributionOfOrderItems(int orderId)
        {
            return SQLDataAccess.ExecuteReadList<DistributionOfOrderItem>(
                @"SELECT [WarehouseOrderItem].*
                FROM [Order].[WarehouseOrderItem]
	                INNER JOIN [Order].[OrderItems] ON [WarehouseOrderItem].[OrderItemId] = [OrderItems].[OrderItemID]
                WHERE [OrderItems].[OrderID] = @OrderId",
                CommandType.Text, GetDistributionOfOrderItemFromReader,
                new SqlParameter("@OrderId", orderId));
        }

        public static bool HasDistribution(int orderId)
        {
            return 
                SQLDataAccess.ExecuteScalar<int>(@"IF EXISTS (SELECT * FROM [Order].[WarehouseOrderItem] INNER JOIN [Order].[OrderItems] ON [WarehouseOrderItem].[OrderItemId] = [OrderItems].[OrderItemID] WHERE [OrderItems].[OrderID] = @OrderId)
                    SELECT 1
                    ELSE
                    SELECT 0",
                    CommandType.Text,
                    new SqlParameter("@OrderId", orderId))
                == 1;
        }

        public static List<DistributionOfOrderItem> GetDistributionOfOrderItem(int orderItemId)
        {
            return SQLDataAccess.ExecuteReadList<DistributionOfOrderItem>(
                @"SELECT * FROM [Order].[WarehouseOrderItem] WHERE [OrderItemId] = @OrderItemId",
                CommandType.Text, GetDistributionOfOrderItemFromReader,
                new SqlParameter("@OrderItemId", orderItemId));
        }

        public static DistributionOfOrderItem GetDistributionOfOrderItemFromReader(
            IDataReader reader)
        {
            return new DistributionOfOrderItem
            {
                OrderItemId = SQLDataHelper.GetInt(reader, "OrderItemId"),
                WarehouseId = SQLDataHelper.GetInt(reader, "WarehouseId"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                DecrementedAmount = SQLDataHelper.GetFloat(reader, "DecrementedAmount")
            };
        }
    }
}