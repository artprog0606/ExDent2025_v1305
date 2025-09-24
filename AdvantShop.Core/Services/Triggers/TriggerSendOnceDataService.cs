using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerSendOnceDataService
    {
        public static void Add(TriggerSendOnceData data)
        {
            SQLDataAccess.ExecuteNonQuery(
                "if not Exists(Select * From [CRM].[TriggerSendOnceData] Where TriggerId=@TriggerId and EntityId=@EntityId and CustomerId=@CustomerId and TriggerType=@TriggerType) " +
                "Insert Into [CRM].[TriggerSendOnceData] (TriggerId, EntityId, CustomerId, TriggerType, CustomerMail, CustomerPhone) Values (@TriggerId, @EntityId, @CustomerId, @TriggerType, @CustomerMail, @CustomerPhone)",
                CommandType.Text,
                new SqlParameter("@TriggerId", data.TriggerId),
                new SqlParameter("@EntityId", data.EntityId),
                new SqlParameter("@CustomerId", data.CustomerId),
                new SqlParameter("@TriggerType", data.TriggerType),
                new SqlParameter("@CustomerMail", data.CustomerMail),
                new SqlParameter("@CustomerPhone", data.CustomerPhone));
        }

        public static bool IsExist(int triggerId, int entityId, Guid customerId, int triggerType, string customerMail, long customerPhone)
        {
            int exists;
            
            if (triggerType != (int)ETriggerEventType.CustomerCreated && triggerType != (int)ETriggerEventType.InstallMobileApp)
            {
                exists = SQLDataAccess.ExecuteScalar<int>(
                        "if Exists(Select * From [CRM].[TriggerSendOnceData] Where (TriggerId=@TriggerId AND EntityId=@EntityId) and (CustomerId=@CustomerId OR CustomerMail=@CustomerMail OR CustomerPhone=@CustomerPhone)) " +
                        "Select 1 " +
                        "else " +
                        "Select 0",
                        CommandType.Text,
                        new SqlParameter("@TriggerId", triggerId),
                        new SqlParameter("@EntityId", entityId),
                        new SqlParameter("@CustomerId", customerId),
                        new SqlParameter("@CustomerMail", customerMail),
                        new SqlParameter("@CustomerPhone", customerPhone));
            }
            else
            {
                exists = SQLDataAccess.ExecuteScalar<int>(
                    "if Exists(Select * From [CRM].[TriggerSendOnceData] Where (TriggerId=@TriggerId OR TriggerType=@TriggerType OR EntityId=@EntityId) and (CustomerId=@CustomerId OR CustomerMail=@CustomerMail OR CustomerPhone=@CustomerPhone)) " +
                    "Select 1 " +
                    "else " +
                    "Select 0",
                    CommandType.Text,
                    new SqlParameter("@TriggerId", triggerId),
                    new SqlParameter("@EntityId", entityId),
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@TriggerType", triggerType),
                    new SqlParameter("@CustomerMail", customerMail),
                    new SqlParameter("@CustomerPhone", customerPhone));
            }

            return exists == 1;
        }

    }
}
