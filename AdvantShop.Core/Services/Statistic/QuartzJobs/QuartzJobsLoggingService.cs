using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Statistic.QuartzJobs
{
    internal static class QuartzJobsLoggingService
    {
        #region QuartzJobRun

        private static QuartzJobRun GetQuartzJobRunFromReader(IDataReader reader)
        {
            return new QuartzJobRun
            {
                Id = SQLDataHelper.GetString(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Group = SQLDataHelper.GetString(reader, "Group"),
                Initiator = SQLDataHelper.GetString(reader, "Initiator"),
                Status = SQLDataHelper.GetString(reader, "Status").TryParseEnum<EQuartzJobStatus>(),
                StartDate = SQLDataHelper.GetDateTime(reader, "StartDate"),
                EndDate = SQLDataHelper.GetDateTime(reader, "EndDate")
            };
        }

        public static void AddQuartzJobRun(QuartzJobRun jobRun)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jobRun.Id))
                    return;

                SQLDataAccess.ExecuteNonQuery("INSERT INTO [Settings].[QuartzJobRuns] (Id, \"Name\", \"Group\", Initiator, Status, StartDate) " +
                                              "VALUES (@Id, @Name, @Group, @Initiator, @Status, @StartDate)",
                    CommandType.Text,
                    new SqlParameter("@Id", jobRun.Id),
                    new SqlParameter("@Name", jobRun.Name),
                    new SqlParameter("@Group", jobRun.Group),
                    new SqlParameter("@Initiator", jobRun.Initiator ?? (object)DBNull.Value),
                    new SqlParameter("@Status", jobRun.Status.ToString()),
                    new SqlParameter("@StartDate", jobRun.StartDate));
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
            }
        }

        public static QuartzJobRun GetQuartzJobRun(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM [Settings].[QuartzJobRuns] WHERE Id = @Id",
                    CommandType.Text,
                    GetQuartzJobRunFromReader,
                    new SqlParameter("@Id", id));
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
                return null;
            }
        }

        public static void UpdateQuartzJobRun(QuartzJobRun jobRun)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jobRun.Id))
                    return;

                SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[QuartzJobRuns] SET Status = @Status, EndDate = @EndDate WHERE Id = @Id",
                    CommandType.Text,
                    new SqlParameter("@Id", jobRun.Id),
                    new SqlParameter("@Status", jobRun.Status.ToString()),
                    new SqlParameter("@EndDate", jobRun.EndDate ?? (object)DBNull.Value));
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
            }
        }

        public static bool IsExistsQuartzJobRun(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return false;

                return SQLDataAccess2.ExecuteScalar<bool>(
                    @"SELECT CASE WHEN EXISTS ( 
                            SELECT *
                            FROM [Settings].[QuartzJobRuns]
                            WHERE Id = @Id
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END",
                    new { Id = id });
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
                return false;
            }
        }

        public static void ClearExpiredLogs()
        {
            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandTimeout = 60 * 10;// 10 mins
                    db.cmd.CommandText = "DELETE FROM [Settings].[QuartzJobRuns] WHERE StartDate < @date";
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now.AddDays(-7)));

                    db.cnOpen();
                    db.cmd.ExecuteNonQuery();
                    db.cnClose();
                }
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
            }
        }

        public static void FinalizeDeadJobs()
        {
            try
            {
                SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[QuartzJobRuns] SET Status = @BrokenStatus, EndDate = @EndDate WHERE Status = @RunningStatus",
                    CommandType.Text,
                    new SqlParameter("@BrokenStatus", EQuartzJobStatus.BrokenOnAppRestart.ToString()),
                    new SqlParameter("@EndDate", DateTime.Now),
                    new SqlParameter("@RunningStatus", EQuartzJobStatus.Running.ToString()));
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
            }
        }

        #endregion

        #region QuartzJobRunLog

        private static QuartzJobRunLog GetQuartzJobRunLogFromReader(IDataReader reader)
        {
            return new QuartzJobRunLog
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                JobRunId = SQLDataHelper.GetString(reader, "JobRunId"),
                Event = SQLDataHelper.GetString(reader, "EventName").TryParseEnum<EQuartzJobEvent>(),
                Message = SQLDataHelper.GetString(reader, "Message"),
                AddDate = SQLDataHelper.GetDateTime(reader, "AddDate")
            };
        }

        public static void AddQuartzJobRunLog(QuartzJobRunLog jobRunLog)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jobRunLog.JobRunId))
                    return;

                SQLDataAccess.ExecuteNonQuery("INSERT INTO [Settings].[QuartzJobRunLogs] (JobRunId, Event, Message, AddDate) " +
                                              "VALUES (@JobRunId, @Event, @Message, @AddDate)",
                    CommandType.Text,
                    new SqlParameter("@JobRunId", jobRunLog.JobRunId),
                    new SqlParameter("@Event", jobRunLog.Event.ToString()),
                    new SqlParameter("@Message", jobRunLog.Message ?? (object)DBNull.Value),
                    new SqlParameter("@AddDate", DateTime.Now));
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
            }
        }

        public static List<QuartzJobRunLog> GetQuartzJobRunLogsByRunId(string runId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(runId))
                    return null;

                return SQLDataAccess.ExecuteReadList("SELECT * FROM [Settings].[QuartzJobRunLogs] WHERE JobRunId = @JobRunId",
                    CommandType.Text,
                    GetQuartzJobRunLogFromReader,
                    new SqlParameter("@JobRunId", runId));
            }
            catch (Exception exception)
            {
                Debug.Log.Error(exception);
                return null;
            }
        }

        #endregion
    }
}
