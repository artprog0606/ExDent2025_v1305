using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.SQL;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Orders
{
    public class DeferredMailService
    {
        public static void Add(DeferredMail mail)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Order].[DeferredMail] ([EntityId], [EntityType], [CreatedDate]) Values (@EntityId, @EntityType, getDate())",
                CommandType.Text,
                new SqlParameter("@EntityId", mail.EntityId),
                new SqlParameter("@EntityType", (int)mail.EntityType));
        }

        public static void Delete(int entityId, DeferredMailType entityType)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Order].[DeferredMail] Where EntityId=@EntityId and EntityType=@EntityType",
                CommandType.Text,
                new SqlParameter("@EntityId", entityId),
                new SqlParameter("@EntityType", (int) entityType));
        }

        public static DeferredMail Get(int entityId, DeferredMailType entityType)
        {
            return
                SQLDataAccess.Query<DeferredMail>(
                    "Select * From [Order].[DeferredMail] Where EntityId=@entityId and EntityType=@EntityType",
                    new {entityId, entityType = (int) entityType}).FirstOrDefault();
        }

        public static List<DeferredMail> GetListByDate(DateTime date)
        {
            return
                SQLDataAccess.Query<DeferredMail>(
                    "Select * From [Order].[DeferredMail] Where CreatedDate <= @date",
                    new { date }).ToList();
        }

        public static void SendMailByOrder(Order order, float? newBonusAmount = null)
        {
            OrderMailService.SendMail(order, newBonusAmount);

            Delete(order.OrderID, DeferredMailType.Order);
        }

        public static void SendMailByLead(Lead lead)
        {
            //OrderService.SendOrderMail(order, order.TotalDiscount, newBonusAmount ?? 0, order.ArchivedShippingName, order.ArchivedPaymentName);

            Delete(lead.Id, DeferredMailType.Lead);
        }


    }
}
