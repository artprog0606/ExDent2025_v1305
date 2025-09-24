using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Triggers.DeferredDatas
{
    /// <summary>
    /// Сервис хранит данные, которые будем обрабатывать по расписанию
    /// </summary>
    public class TriggerDeferredDataService
    {
        public static List<TriggerDeferredData> GetList()
        {
            return SQLDataAccess.Query<TriggerDeferredData>("Select * From [CRM].[TriggerDeferredData]").ToList();
        }

        public static int Add(TriggerDeferredData data)
        {
            data.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into [CRM].[TriggerDeferredData] (EntityId, TriggerActionId, TriggerObjectType, DateCreated) Values (@EntityId, @TriggerActionId, @TriggerObjectType, getdate()); Select scope_identity(); ",
                CommandType.Text,
                new SqlParameter("@EntityId", data.EntityId),
                new SqlParameter("@TriggerActionId", data.TriggerActionId),
                new SqlParameter("@TriggerObjectType", (int)data.TriggerObjectType)
                );

            return data.Id;
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [CRM].[TriggerDeferredData] Where Id=@id", CommandType.Text, new SqlParameter("@id", id));
        }

        public static void Delete(ETriggerObjectType objectType, int entityId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [CRM].[TriggerDeferredData] Where EntityId=@EntityId and TriggerObjectType=@TriggerObjectType",
                CommandType.Text,
                new SqlParameter("@EntityId", entityId),
                new SqlParameter("@TriggerObjectType", (int)objectType));
        }
    }
}
