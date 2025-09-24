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
    public class ActReportService
    {
        public static ActReport GetActReport(int id)
        {
            return SQLDataAccess.Query<ActReport>("SELECT * FROM [Partners].[ActReport] WHERE Id = @Id", new { id }).FirstOrDefault();
        }

        public static bool ActReportExists(int partnerId, DateTime periodFrom, DateTime periodTo)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [Partners].[ActReport] WHERE PartnerId = @PartnerId AND PeriodFrom = @PeriodFrom AND PeriodTo = @PeriodTo", CommandType.Text,
                new SqlParameter("@PartnerId", partnerId),
                new SqlParameter("@PeriodFrom", periodFrom),
                new SqlParameter("@PeriodTo", periodTo)) > 0;
        }

        public static void AddActReport(ActReport actReport)
        {
            actReport.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Partners].[ActReport] (PartnerId, FileName, PeriodFrom, PeriodTo, DateCreated) " +
                "VALUES (@PartnerId, @FileName, @PeriodFrom, @PeriodTo, @DateCreated); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@PartnerId", actReport.PartnerId),
                new SqlParameter("@FileName", actReport.FileName),
                new SqlParameter("@PeriodFrom", actReport.PeriodFrom),
                new SqlParameter("@PeriodTo", actReport.PeriodTo),
                new SqlParameter("@DateCreated", DateTime.Now)
            );
        }

        public static void DeleteActReport(int id)
        {
            var report = GetActReport(id);
            if (report == null)
                return;
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PartnerActReports, report.FileName);
            if (File.Exists(filePath))
                FileHelpers.DeleteFile(filePath);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Partners].[ActReport] WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }
    }
}
