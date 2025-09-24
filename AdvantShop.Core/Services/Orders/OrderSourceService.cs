using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Helpers;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using System;

namespace AdvantShop.Orders
{
    public class OrderSourceService
    {
        private const int NameColLength = 250;

        public static OrderSource GetOrderSourceFromReader(SqlDataReader reader)
        {
            return new OrderSource
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Main = SQLDataHelper.GetBoolean(reader, "Main"),
                Type = SQLDataHelper.GetString(reader, "Type").TryParseEnum<OrderType>(),
                ObjId = SQLDataHelper.GetNullableInt(reader, "ObjId"),
            };
        }

        public static List<OrderSource> GetOrderSources()
        {
            return SQLDataAccess.ExecuteReadList<OrderSource>(
                "Select * From [Order].[OrderSource] Order By [SortOrder]",
                CommandType.Text,
                GetOrderSourceFromReader);
        }

        public static OrderSource GetOrderSource(int id)
        {
            return SQLDataAccess.ExecuteReadOne<OrderSource>(
                "Select TOP 1 * From [Order].[OrderSource] Where [Id] = @Id",
                CommandType.Text,
                GetOrderSourceFromReader,
                new SqlParameter("@Id", id));
        }

        public static OrderSource GetOrderSource(string name)
        {
            return SQLDataAccess.ExecuteReadOne<OrderSource>(
                "Select TOP(1) * From [Order].[OrderSource] Where [Name] = @Name",
                CommandType.Text,
                GetOrderSourceFromReader,
                new SqlParameter("@Name", name.Reduce(NameColLength)));
        }

        public static OrderSource GetOrderSource(OrderType type, int objId)
        {
            return SQLDataAccess.ExecuteReadOne<OrderSource>(
                "Select TOP(1) * From [Order].[OrderSource] Where [Type] = @Type And [ObjId] = @ObjId",
                CommandType.Text,
                GetOrderSourceFromReader,
                new SqlParameter("@Type", type.ToString()),
                new SqlParameter("@ObjId", objId));
        }

        /// <summary>
        /// get or create OrderSource if not exists
        /// </summary>
        public static OrderSource GetOrderSource(OrderType type)
        {
            var orderSource = SQLDataAccess.ExecuteReadOne<OrderSource>(
                "Select TOP(1) * From [Order].[OrderSource] Where [Type] = @Type And [Main] = 1",
                CommandType.Text,
                GetOrderSourceFromReader,
                new SqlParameter("@Type", type.ToString()));

            return orderSource ?? AddOrderSource(type, main: true);
        }

        /// <summary>
        /// get or create OrderSource if not exists
        /// </summary>
        public static OrderSource GetOrderSource(OrderType type, int objId, string objName)
        {
            var name = PrepareOrderSourceName(type, objName);
            var orderSource = GetOrderSource(type, objId) ?? GetOrderSource(name);

            return orderSource ?? AddOrderSource(type, name: name, objId: objId, objName: objName);
        }

        public static string PrepareOrderSourceName(OrderType type, string objName)
        {
            return type.Localize() + (objName.IsNotEmpty() ? " \"" + objName + "\"" : null);
        }

        /// <summary>
        /// Add and reload OrderSource from db
        /// </summary>
        public static OrderSource AddOrderSource(OrderType type, string name = null, bool main = false, int? objId = null, string objName = null)
        {
            var orderSourceId = AddOrderSource(new OrderSource
            {
                Name = name.IsNotEmpty() ? name : PrepareOrderSourceName(type, objName),
                SortOrder = 0,
                Type = type,
                Main = main,
                ObjId = objId
            });
            return GetOrderSource(orderSourceId);
        }

        public static int AddOrderSource(OrderSource orderSource)
        {
            orderSource.Id =  SQLDataAccess.ExecuteScalar<int>(
                "If @Main=1 Begin Update [Order].[OrderSource] Set [Main]=0 Where [Type]=@Type End;" +
                "Insert Into [Order].[OrderSource] ([Name],[SortOrder],[Main],[Type],[ObjId]) Values (@Name,@SortOrder,@Main,@Type,@ObjId); SELECT scope_identity();" +
                "If (Select Count(Id) From [Order].[OrderSource] Where [Type]=@Type And Main=1) = 0 Begin Update Top (1) [Order].[OrderSource] Set Main=1 Where [Type]=@Type And Id <> scope_identity() End",
                CommandType.Text,
                new SqlParameter("@Name", orderSource.Name.Reduce(NameColLength)),
                new SqlParameter("@SortOrder", orderSource.SortOrder),
                new SqlParameter("@Main", orderSource.Main),
                new SqlParameter("@Type", orderSource.Type.ToString()),
                new SqlParameter("@ObjId", orderSource.ObjId ?? (object)DBNull.Value));

            return orderSource.Id;
        }

        public static void UpdateOrderSource(OrderSource orderSource)
        {
            SQLDataAccess.ExecuteNonQuery(
                "If @Main=1 Begin Update [Order].[OrderSource] Set [Main]=0 Where [Type]=@Type End;" +
                "Update [Order].[OrderSource] Set [Name]=@Name, [SortOrder]=@SortOrder, [Main]=@Main, [Type]=@Type, [ObjId]=@ObjId Where Id=@Id;" +
                "If (Select Count(Id) From [Order].[OrderSource] Where [Type]=@Type And Main=1) = 0 Begin Update Top (1) [Order].[OrderSource] Set Main=1 Where [Type]=@Type And Id<>@Id End",
                CommandType.Text,
                new SqlParameter("@Id", orderSource.Id),
                new SqlParameter("@Name", orderSource.Name),
                new SqlParameter("@SortOrder", orderSource.SortOrder),
                new SqlParameter("@Main", orderSource.Main),
                new SqlParameter("@Type", orderSource.Type.ToString()),
                new SqlParameter("@ObjId", orderSource.ObjId ?? (object)DBNull.Value));
        }

        public static bool DeleteOrderSource(OrderType type, int objId)
        {
            var orderSource = GetOrderSource(type, objId);
            return orderSource == null || DeleteOrderSource(orderSource.Id);
        }

        public static bool DeleteOrderSource(int id)
        {
            if (!CanBeDeleted(id))
                return false;

            var source = GetOrderSource(id);

            if (source != null)
                SQLDataAccess.ExecuteNonQuery(
                    "Delete From [Order].[OrderSource] Where Id=@Id; " +
                    "If (Select Count(Id) From [Order].[OrderSource] Where [Type]=@Type And Main=1) = 0 Begin Update Top (1) [Order].[OrderSource] Set Main=1 Where [Type]=@Type And Id<>@Id End",
                    CommandType.Text,
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Type", source.Type.ToString()));
            
            return true;
        }

        public static bool CanBeDeleted(int id)
        {
            var ordersCount = SQLDataAccess.Query<int>("Select Count(OrderId) From [Order].[Order] Where OrderSourceId = @id", new {id}).FirstOrDefault();

            if (ordersCount != 0)
                return false;

            var leadsCount = SQLDataAccess.Query<int>("Select Count([Lead].Id) From [Order].[Lead] Where OrderSourceId = @id", new { id }).FirstOrDefault();

            if (leadsCount != 0)
                return false;

            return true;
        }
    }
}
