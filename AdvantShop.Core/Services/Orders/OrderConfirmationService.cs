using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Orders
{
    public class OrderConfirmationService
    {
        public static bool IsExist(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(CustomerId) from [Order].OrderConfirmation where CustomerId=@CustomerId", CommandType.Text,
                new SqlParameter("@CustomerId", customerId)) > 0;
        }

        public static CheckoutData Get(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadOne("Select TOP 1 * from [Order].OrderConfirmation where CustomerId=@CustomerId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@CustomerId", customerId));
        }

        public static void Add(Guid customerId, CheckoutData item)
        {
            var set = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            SQLDataAccess.ExecuteNonQuery(
                "Insert into [Order].OrderConfirmation ([CustomerId],[OrderConfirmationData],LastUpdate) values (@CustomerId,@OrderConfirmationData,GetDate())",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@OrderConfirmationData", JsonConvert.SerializeObject(item, set)));
        }

        public static void Update(Guid customerId, CheckoutData item)
        {
            var set = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].OrderConfirmation set OrderConfirmationData=@OrderConfirmationData,LastUpdate=GetDate() where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@OrderConfirmationData", JsonConvert.SerializeObject(item, set)));
        }

        public static void Delete(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].OrderConfirmation Where CustomerId=@CustomerId",
                CommandType.Text, new SqlParameter("@CustomerId", customerId));
            AttachmentService.DeleteAttachments<CheckoutAttachment>(customerId.GetHashCode());
        }

        public static void DeleteExpired()
        {
            var expiredCustomerIds = SQLDataAccess.ExecuteReadList(
                "SELECT CustomerId FROM [Order].OrderConfirmation WHERE DATEADD(month, 1, LastUpdate) < GetDate()",
                CommandType.Text,
                (reader) => SQLDataHelper.GetGuid(reader, "CustomerId"));
            // необходимо удалять фото загруженные при оформлении заказа, для этого нужно знать CustomerId у которого просрочились данные
            foreach (var expiredCustomerId in expiredCustomerIds)
                Delete(expiredCustomerId);
        }

        private static CheckoutData GetFromReader(SqlDataReader reader)
        {
            return
                JsonConvert.DeserializeObject<CheckoutData>(
                    SQLDataHelper.GetString(reader, "OrderConfirmationData"),
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

        }
    }
}