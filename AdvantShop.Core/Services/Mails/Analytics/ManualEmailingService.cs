using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Mails.Analytics
{
    public class ManualEmailingService
    {
        public static ManualEmailing GetManualEmailing(Guid id)
        {
            return SQLDataAccess.Query<ManualEmailing>("SELECT * FROM Customers.ManualEmailing WHERE Id = @id", new { id }, CommandType.Text).FirstOrDefault();
        }

        public static Guid AddManualEmailing(ManualEmailing emailing)
        {
            emailing.Id = Guid.NewGuid();
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Customers.ManualEmailing (Id, Subject, TotalCount, DateCreated) VALUES (@Id, @Subject, @TotalCount, @DateCreated)",
                CommandType.Text,
                new SqlParameter("@Id", emailing.Id),
                new SqlParameter("@Subject", emailing.Subject),
                new SqlParameter("@TotalCount", emailing.TotalCount),
                new SqlParameter("@DateCreated", DateTime.Now));
            return emailing.Id;
        }

        public static void DeleteManualEmailing(Guid id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.ManualEmailing WHERE Id = @Id",
                CommandType.Text, new SqlParameter("@Id", id));
        }
    }
}
