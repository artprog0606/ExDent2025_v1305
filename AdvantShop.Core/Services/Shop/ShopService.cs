using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Core.Services.Shop
{
    public class ShopService
    {
        #region WorkingTime

        private static WorkingTime GetWorkingTimeFromReader(IDataReader reader)
        {
            return new WorkingTime
            {
                DayOfWeek = (DayOfWeek)SQLDataHelper.GetInt(reader, "DayOfWeek"),
                EndTime = SQLDataHelper.GetDateTime(reader, "EndTime").TimeOfDay,
                StartTime = SQLDataHelper.GetDateTime(reader, "StartTime").TimeOfDay,
            };
        }

        public static List<WorkingTime> GetWorkingTimes()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[WorkingTime]",
                CommandType.Text,
                GetWorkingTimeFromReader);
        }

        public static List<WorkingTime> GetWorkingTime(DayOfWeek dayOfWeek)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[WorkingTime] WHERE DayOfWeek = @DayOfWeek",
                CommandType.Text,
                GetWorkingTimeFromReader,
                new SqlParameter("@DayOfWeek", (int)dayOfWeek));
        }

        public static void AddWorkingTime(WorkingTime workingTime)
        {
            var date = DateTime.Parse("2023.01.01");
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Settings].[WorkingTime] (DayOfWeek, StartTime, EndTime) VALUES (@DayOfWeek, @StartTime, @EndTime)",
                CommandType.Text,
                new SqlParameter("@DayOfWeek", (int)workingTime.DayOfWeek),
                new SqlParameter("@StartTime", date.Add(workingTime.StartTime)),
                new SqlParameter("@EndTime", workingTime.EndTime == TimeSpan.Zero ? date.AddDays(1) : date.Add(workingTime.EndTime)));
        }

        public static void DeleteWorkingTimes()
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[WorkingTime]", CommandType.Text);
        }

        #endregion

        #region AdditionalWorkingTime

        private static AdditionalWorkingTime GetAdditionalWorkingTimeFromReader(IDataReader reader)
        {
            return new AdditionalWorkingTime
            {
                EndTime = SQLDataHelper.GetDateTime(reader, "EndTime"),
                StartTime = SQLDataHelper.GetDateTime(reader, "StartTime"),
                IsWork = SQLDataHelper.GetBoolean(reader, "IsWork")
            };
        }

        public static List<AdditionalWorkingTime> GetAdditionalWorkingTimes()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[AdditionalWorkingTime]",
                CommandType.Text,
                GetAdditionalWorkingTimeFromReader);
        }

        public static List<AdditionalWorkingTime> GetAdditionalWorkingTime(DateTime date)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[AdditionalWorkingTime] WHERE [StartTime] >= @StartTime AND [EndTime] <= @EndTime",
                CommandType.Text,
                GetAdditionalWorkingTimeFromReader,
                new SqlParameter("@StartTime", date.Date),
                new SqlParameter("@EndTime", date.Date.AddDays(1)));
        }

        public static void AddAdditionalWorkingTime(AdditionalWorkingTime workingTime)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Settings].[AdditionalWorkingTime] (StartTime, EndTime, IsWork) VALUES (@StartTime, @EndTime, @IsWork)",
                CommandType.Text,
                new SqlParameter("@StartTime", workingTime.StartTime),
                new SqlParameter("@EndTime", workingTime.EndTime),
                new SqlParameter("@IsWork", workingTime.IsWork));
        }

        public static void DeleteAdditionalWorkingTimes(DateTime dateStart, DateTime dateEnd)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[AdditionalWorkingTime] WHERE StartTime >= @StartTime AND StartTime < @EndTime", 
                CommandType.Text,
                new SqlParameter("@StartTime", dateStart),
                new SqlParameter("@EndTime", dateEnd));
        }

        #endregion

        public static bool AllowCheckoutNow()
        {
            if (SettingsCheckout.TypeStoreWorkMode == EStoreWorkMode.AroundTheClock)
                return true;

            var currentDateTimeWithOffset = DateTime.UtcNow.GetDateTimeOffset();
            if (!currentDateTimeWithOffset.HasValue)
                throw new BlException("Invalid datetime with offset");

            var currentDateTime = currentDateTimeWithOffset.Value.DateTime;

            var additionalTimes = GetAdditionalWorkingTime(currentDateTime);
            if (additionalTimes.Count != 0)
                if (additionalTimes.Any(x => x.IsWork && x.StartTime <= currentDateTime && x.EndTime > currentDateTime))
                    return true;
                else 
                    return false;

            var workingTimes = GetWorkingTime(currentDateTime.DayOfWeek);
            if (workingTimes.Any(x => x.StartTime <= currentDateTime.TimeOfDay && (x.EndTime == TimeSpan.Zero || x.EndTime > currentDateTime.TimeOfDay)))
                return true;

            return false;
        }
    }
}
