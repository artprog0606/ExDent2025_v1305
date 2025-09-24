using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Partners
{
    public class PaymentTypeService
    {
        public static List<PaymentType> GetPaymentTypes()
        {
            return SQLDataAccess.Query<PaymentType>("SELECT * FROM [Partners].[PaymentType] ORDER BY SortOrder").ToList();
        }

        public static PaymentType GetPaymentType(int id)
        {
            return SQLDataAccess.Query<PaymentType>("SELECT * FROM [Partners].[PaymentType] WHERE Id = @Id", new { Id = id }).FirstOrDefault();
        }

        public static int AddPaymentType(PaymentType paymentType)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Partners].[PaymentType] (Name,SortOrder) VALUES (@Name,@SortOrder);" +
                "SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", paymentType.Name),
                new SqlParameter("@SortOrder", paymentType.SortOrder)
            );
        }

        public static void UpdatePaymentType(PaymentType paymentType)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Partners].[PaymentType] SET Name=@Name, SortOrder=@SortOrder WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", paymentType.Id),
                new SqlParameter("@Name", paymentType.Name),
                new SqlParameter("@SortOrder", paymentType.SortOrder)
                );
        }

        public static void DeletePaymentType(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Partners].[PaymentType] WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }

        public static bool IsPaymentTypeInUse(int id)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Partners].[NaturalPerson] WHERE PaymentTypeId = @PaymentTypeId",
                CommandType.Text, new SqlParameter("@PaymentTypeId", id)) > 0;
        }
    }
}
