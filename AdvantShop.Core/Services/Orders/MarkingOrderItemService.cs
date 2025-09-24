using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Orders
{
    public class MarkingOrderItemService
    {
        public static List<MarkingOrderItem> GetMarkingItems(int orderItemId)
        {
            return SQLDataAccess
                .Query<MarkingOrderItem>("Select * From [Order].[MarkingOrderItem] Where OrderItemId = @orderItemId",
                    new {orderItemId}).ToList();
        }

        public static void Add(MarkingOrderItem item)
        {
            SQLDataAccess.ExecuteNonQuery("Insert Into [Order].[MarkingOrderItem] (OrderItemId, Code) Values (@OrderItemId, @Code)", CommandType.Text,
                new SqlParameter("@OrderItemId", item.OrderItemId),
                new SqlParameter("@Code", item.Code ?? ""));
        }

        public static void Delete(int orderItemId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Order].[MarkingOrderItem] Where OrderItemId = @OrderItemId", CommandType.Text,
                new SqlParameter("@OrderItemId", orderItemId));
        }

        public static void DeleteById(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Order].[MarkingOrderItem] Where Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static void RemoveExcessMarkingItems(OrderItem orderItem)
        {
            var markingItems = GetMarkingItems(orderItem.OrderItemID);
            if (markingItems.Count <= 0)
                return;
            
            var amount = (int)Math.Round(orderItem.Amount, MidpointRounding.AwayFromZero);
            if (markingItems.Count > amount)
            {
                foreach (var item in markingItems.Skip(amount))
                {
                    DeleteById(item.Id);
                }
            }
        }
    }
}
