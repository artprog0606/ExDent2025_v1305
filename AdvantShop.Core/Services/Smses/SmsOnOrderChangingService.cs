using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Smses
{
    public class SmsOnOrderChangingService
    {
        public static List<SmsTemplateOnOrderChanging> GetList()
        {
            return
                SQLDataAccess.Query<SmsTemplateOnOrderChanging>("Select * From Settings.SmsTemplateOnOrderChanging")
                    .ToList();
        }

        public static SmsTemplateOnOrderChanging Get(int id)
        {
            return
                SQLDataAccess.Query<SmsTemplateOnOrderChanging>(
                    "Select * From Settings.SmsTemplateOnOrderChanging Where Id=@id", new {id}).FirstOrDefault();
        }

        public static SmsTemplateOnOrderChanging GetByOrderStatusId(int orderStatusId)
        {
            return
                SQLDataAccess.Query<SmsTemplateOnOrderChanging>(
                    "Select * From Settings.SmsTemplateOnOrderChanging Where OrderStatusId=@orderStatusId and Enabled = 1", new { orderStatusId }).FirstOrDefault();
        }

        public static int Add(SmsTemplateOnOrderChanging template)
        {
            template.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into Settings.SmsTemplateOnOrderChanging (OrderStatusId, SmsText, Enabled) Values (@OrderStatusId, @SmsText, @Enabled); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@OrderStatusId", template.OrderStatusId),
                    new SqlParameter("@SmsText", template.SmsText ?? ""),
                    new SqlParameter("@Enabled", template.Enabled));

            return template.Id;
        }

        public static void Update(SmsTemplateOnOrderChanging template)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Settings.SmsTemplateOnOrderChanging Set OrderStatusId=@OrderStatusId, SmsText=@SmsText, Enabled=@Enabled Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", template.Id),
                new SqlParameter("@OrderStatusId", template.OrderStatusId),
                new SqlParameter("@SmsText", template.SmsText ?? ""),
                new SqlParameter("@Enabled", template.Enabled));
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From Settings.SmsTemplateOnOrderChanging Where Id=@Id",
                CommandType.Text, new SqlParameter("@Id", id));
        }
    }
}
