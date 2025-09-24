using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Partners
{
    public class PayoutReportService
    {
        public static PayoutReport GetPayoutReport(int id)
        {
            return SQLDataAccess.Query<PayoutReport>("SELECT * FROM [Partners].[PayoutReport] WHERE Id = @Id", new { id }).FirstOrDefault();
        }

        public static void AddPayoutReport(PayoutReport payoutReport)
        {
            payoutReport.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Partners].[PayoutReport] (FileName, PeriodTo, DateCreated) VALUES (@FileName, @PeriodTo, @DateCreated);" +
                "SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@FileName", payoutReport.FileName),
                new SqlParameter("@PeriodTo", payoutReport.PeriodTo),
                new SqlParameter("@DateCreated", DateTime.Now)
            );
        }

        public static void DeletePayoutReport(int id)
        {
            var report = GetPayoutReport(id);
            if (report == null)
                return;
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PartnerPayoutReports, report.FileName);
            if (File.Exists(filePath))
                FileHelpers.DeleteFile(filePath);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Partners].[PayoutReport] WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }
    }
}
