using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class TimeOfWorkService
    {
        public static int Add(TimeOfWork timeOfWork)
        {
            if (timeOfWork is null) throw new ArgumentNullException(nameof(timeOfWork));

            timeOfWork.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Catalog].[WarehouseTimeOfWork]
	                ([WarehouseId],[DayOfWeeks],[OpeningTime],[ClosingTime],[BreakStartTime],[BreakEndTime])
                VALUES
	                (@WarehouseId,@DayOfWeeks,@OpeningTime,@ClosingTime,@BreakStartTime,@BreakEndTime);
                SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@WarehouseId", timeOfWork.WarehouseId),
                new SqlParameter("@DayOfWeeks", (byte)timeOfWork.DayOfWeeks),
                new SqlParameter("@OpeningTime", ((short?)timeOfWork.OpeningTime?.TotalMinutes) ?? (object) DBNull.Value),
                new SqlParameter("@ClosingTime", ((short?)timeOfWork.ClosingTime?.TotalMinutes) ?? (object) DBNull.Value),
                new SqlParameter("@BreakStartTime", ((short?)timeOfWork.BreakStartTime?.TotalMinutes) ?? (object) DBNull.Value),
                new SqlParameter("@BreakEndTime", ((short?)timeOfWork.BreakEndTime?.TotalMinutes) ?? (object) DBNull.Value));

            return timeOfWork.Id;
        }

        public static void Update(TimeOfWork timeOfWork)
        {
            if (timeOfWork is null) throw new ArgumentNullException(nameof(timeOfWork));
            
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[WarehouseTimeOfWork]
                   SET [DayOfWeeks] = @DayOfWeeks
                      ,[OpeningTime] = @OpeningTime
                      ,[ClosingTime] = @ClosingTime
                      ,[BreakStartTime] = @BreakStartTime
                      ,[BreakEndTime] = @BreakEndTime
                 WHERE [Id] = @Id", 
                CommandType.Text,
                new SqlParameter("@Id", timeOfWork.Id),
                // new SqlParameter("@WarehouseId", timeOfWork.WarehouseId),
                new SqlParameter("@DayOfWeeks", (byte)timeOfWork.DayOfWeeks),
                new SqlParameter("@OpeningTime", ((short?)timeOfWork.OpeningTime?.TotalMinutes) ?? (object) DBNull.Value),
                new SqlParameter("@ClosingTime", ((short?)timeOfWork.ClosingTime?.TotalMinutes) ?? (object) DBNull.Value),
                new SqlParameter("@BreakStartTime", ((short?)timeOfWork.BreakStartTime?.TotalMinutes) ?? (object) DBNull.Value),
                new SqlParameter("@BreakEndTime", ((short?)timeOfWork.BreakEndTime?.TotalMinutes) ?? (object) DBNull.Value));
        }
          
        public static TimeOfWork Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[WarehouseTimeOfWork] WHERE [Id] = @Id",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@Id", id));
        }
          
        public static List<TimeOfWork> GetWarehouseTimeOfWork(int warehouseId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[WarehouseTimeOfWork] WHERE [WarehouseId] = @WarehouseId",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@WarehouseId", warehouseId));
        }
        
        public static TimeOfWork GetFromReader(SqlDataReader reader)
        {
            int index;
            return new TimeOfWork
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                WarehouseId = SQLDataHelper.GetInt(reader, "WarehouseId"),
                DayOfWeeks = (FlagDayOfWeek) SQLDataHelper.GetInt(reader, "DayOfWeeks"),
                OpeningTime = reader.IsDBNull(index = reader.GetOrdinal("OpeningTime"))
                    ? (TimeSpan?) null
                    : TimeSpan.FromMinutes(reader.GetInt16(index)),
                ClosingTime = reader.IsDBNull(index = reader.GetOrdinal("ClosingTime"))
                    ? (TimeSpan?) null
                    : TimeSpan.FromMinutes(reader.GetInt16(index)),
                BreakStartTime = reader.IsDBNull(index = reader.GetOrdinal("BreakStartTime"))
                    ? (TimeSpan?) null
                    : TimeSpan.FromMinutes(reader.GetInt16(index)),
                BreakEndTime = reader.IsDBNull(index = reader.GetOrdinal("BreakEndTime"))
                    ? (TimeSpan?) null
                    : TimeSpan.FromMinutes(reader.GetInt16(index)),
            };
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[WarehouseTimeOfWork] WHERE [Id] = @Id",
                CommandType.Text, 
                new SqlParameter("@Id", id));
        }

        public static void DeleteWarehouseTimeOfWork(int warehouseId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[WarehouseTimeOfWork] WHERE [WarehouseId] = @WarehouseId",
                CommandType.Text, 
                new SqlParameter("@WarehouseId", warehouseId));
        }

        public static string FormatTimeOfWork(TimeOfWork timeOfWork)
            => FormatTimeOfWork(timeOfWork, AdvantShop.Localization.Culture.GetCulture());
        
        public static string FormatTimeOfWork(TimeOfWork timeOfWork, CultureInfo culture)
        {
            var cultureIsRu = culture.Name.Equals("ru-ru", StringComparison.OrdinalIgnoreCase);
            var days = string.Join(", ",
                Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .OrderBy(x => cultureIsRu && x == DayOfWeek.Sunday) // для России воскресенье ставим в конец
                    .Where(x => timeOfWork.DayOfWeeks.HasDayOfWeek(x))
                    .Select(x => culture.DateTimeFormat.GetShortestDayName(x)));

            string timeWork =
                LocalizationService.GetResource("AdvantShop.Core.Services.Catalog.Warehouses.TimeOfWork.Weekend");
            var timeFormat = culture.DateTimeFormat.ShortTimePattern;
            
            // для России часы выводим как HH
            if (cultureIsRu
                && timeFormat.Count(c => c == 'H') == 1)
                timeFormat = timeFormat.Replace("H", "HH"); 
            
            if (timeOfWork.OpeningTime.HasValue
                && timeOfWork.ClosingTime.HasValue)
            {
                if (timeOfWork.BreakStartTime.HasValue
                    && timeOfWork.BreakEndTime.HasValue)
                {
                    timeWork = string.Format("{0} - {1}, {2} - {3}", 
                        (DateTime.Now.Date + timeOfWork.OpeningTime.Value).ToString(timeFormat, culture),
                        (DateTime.Now.Date + timeOfWork.BreakStartTime.Value).ToString(timeFormat, culture), 
                        (DateTime.Now.Date + timeOfWork.BreakEndTime.Value).ToString(timeFormat, culture),
                        (DateTime.Now.Date + timeOfWork.ClosingTime.Value).ToString(timeFormat, culture));
                }
                else
                {
                    timeWork = string.Format("{0} - {1}", 
                        (DateTime.Now.Date + timeOfWork.OpeningTime.Value).ToString(timeFormat, culture), 
                        (DateTime.Now.Date + timeOfWork.ClosingTime.Value).ToString(timeFormat, culture));
                }
            }

            return $"{days}: {timeWork}";
        }
    }
}