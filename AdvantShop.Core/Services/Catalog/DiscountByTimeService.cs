using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.Jobs;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Catalog
{
    public class DiscountByTimeService
    {
        public const string DiscountByTimeCacheName = "DiscountByTime_";

        #region Discount by time

        public static List<DiscountByTime> GetCurrentDiscountsByTime()
        {
            if (!CacheManager.TryGetValue(DiscountByTimeCacheName + "Current", out List<DiscountByTime> discountsByTime))
            {
                var now = DateTime.Now;
                discountsByTime = GetByDatetime(now);

                if (discountsByTime != null) 
                {
                    var discount = discountsByTime.OrderBy(x => x.TimeTo).FirstOrDefault();
                    
                    var cacheMinutes = GetCacheMinutes(discount, now);
                    
                    CacheManager.Insert(DiscountByTimeCacheName + "Current", discountsByTime, cacheMinutes);
                }
            }
            return discountsByTime;
        }

        private static double GetCacheMinutes(DiscountByTime discount, DateTime now)
        {
            if (discount == null)
                return GetList(true).Count == 0 ? 60 * 24 : 2;

            if (discount.TimeTo > discount.TimeFrom)
                return discount.TimeTo.TotalMinutes - now.TimeOfDay.TotalMinutes;

            if (now.TimeOfDay > discount.TimeFrom)
                return
                    discount.DaysOfWeek.Contains(now.AddDays(1).DayOfWeek)
                        ? 24 * 60 - now.TimeOfDay.TotalMinutes + discount.TimeTo.TotalMinutes
                        : 24 * 60 - now.TimeOfDay.TotalMinutes;
            
            return discount.TimeTo.TotalMinutes - now.TimeOfDay.TotalMinutes;
        }

        private static DiscountByTime GetDiscountByTimeWithFromReader(IDataReader reader)
        {
            return new DiscountByTime
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                TimeFrom = SQLDataHelper.GetDateTime(reader, "TimeFrom").TimeOfDay,
                TimeTo = SQLDataHelper.GetDateTime(reader, "TimeTo").TimeOfDay,
                ShowPopup = SQLDataHelper.GetBoolean(reader, "ShowPopup"),
                PopupText = SQLDataHelper.GetString(reader, "PopupText"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static List<DiscountByTime> GetAll(bool? enabled = null)
        {
            var condition = string.Empty;
            if (enabled.HasValue)
                condition = " WHERE Enabled = " + enabled.Value.ToInt();
            return SQLDataAccess.ExecuteReadList(
                $"SELECT * FROM [Catalog].[DiscountByTime]{condition} ORDER BY [SortOrder]",
                CommandType.Text,
                GetDiscountByTimeWithFromReader);
        }

        public static DiscountByTime Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP(1) * FROM [Catalog].[DiscountByTime] WHERE Id = @Id",
                CommandType.Text,
                GetDiscountByTimeWithFromReader,
                new SqlParameter("@Id", id));
        }
        
        public static List<DiscountByTime> GetList(bool enabled = true)
        {
            return CacheManager.Get(DiscountByTimeCacheName + "list" + enabled, 24*60,
                () =>
                    SQLDataAccess.ExecuteReadList(
                        @"SELECT * FROM [Catalog].[DiscountByTime] " + 
                        (enabled ? "WHERE Enabled = 1" : "") +
                        " ORDER BY [SortOrder]",
                        CommandType.Text,
                        GetDiscountByTimeWithFromReader)
            );
        }

        public static List<DiscountByTime> GetByDatetime(DateTime dateTime)
        {
            var timeOfDay = dateTime.TimeOfDay;

            var list =
                GetList(enabled: true)
                    .Where(x => 
                        (x.TimeTo > x.TimeFrom 
                            ? x.TimeFrom <= timeOfDay && timeOfDay <= x.TimeTo  // 09 <= time <= 18 
                            : x.TimeFrom <= timeOfDay || timeOfDay <= x.TimeTo) // 22 <= time <= 09
                        
                        && x.DaysOfWeek.Contains(dateTime.DayOfWeek))
                    .ToList();

            return list;
            /*
            var date = DateTime.Parse("2024.01.01");
            return SQLDataAccess.ExecuteReadList(
                @"SELECT * FROM [Catalog].[DiscountByTime] 
                WHERE Enabled = 1 
                        AND TimeFrom <= @Time 
                        AND TimeTo >= @Time 
                        AND EXISTS (SELECT 1 FROM [Catalog].[DiscountByTimeDayOfWeek] day WHERE DiscountByTime.Id = day.DiscountByTimeId AND day.DayOfWeek = @DayOfWeek) 
                ORDER BY [SortOrder]",
                CommandType.Text,
                GetDiscountByTimeWithFromReader,
                new SqlParameter("@Time", date.Add(dateTime.TimeOfDay)),
                new SqlParameter("@DayOfWeek", (int)dateTime.DayOfWeek));
            */
        }

        public static int Add(DiscountByTime data)
        {
            var date = DateTime.Parse("2024.01.01");
            data.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Catalog].[DiscountByTime] ([Enabled],[TimeFrom],[TimeTo],[Discount],[ShowPopup],[PopupText],[SortOrder])
                    VALUES (@Enabled,@TimeFrom,@TimeTo,@Discount,@ShowPopup,@PopupText,@SortOrder)
                SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Enabled", data.Enabled),
                new SqlParameter("@TimeFrom", date.Add(data.TimeFrom)),
                new SqlParameter("@TimeTo", date.Add(data.TimeTo)),
                new SqlParameter("@Discount", data.Discount),
                new SqlParameter("@ShowPopup", data.ShowPopup),
                new SqlParameter("@PopupText", data.PopupText ?? (object)DBNull.Value),
                new SqlParameter("@SortOrder", data.SortOrder));
            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
            return data.Id;
        }

        public static void Update(DiscountByTime data)
        {
            if (data.Id == 0)
                return;
            var date = DateTime.Parse("2024.01.01");

            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[DiscountByTime]
                    SET [Enabled] = @Enabled
                        ,[TimeFrom] = @TimeFrom
                        ,[TimeTo] = @TimeTo
                        ,[Discount] = @Discount
                        ,[ShowPopup] = @ShowPopup
                        ,[PopupText] = @PopupText
                        ,[SortOrder] = @SortOrder
                WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", data.Id),
                new SqlParameter("@Enabled", data.Enabled),
                new SqlParameter("@TimeFrom", date.Add(data.TimeFrom)),
                new SqlParameter("@TimeTo", date.Add(data.TimeTo)),
                new SqlParameter("@Discount", data.Discount),
                new SqlParameter("@ShowPopup", data.ShowPopup),
                new SqlParameter("@PopupText", data.PopupText ?? (object)DBNull.Value),
                new SqlParameter("@SortOrder", data.SortOrder));

            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
        }

        public static void Delete(int id)
        {
            var deletedDiscount = Get(id);
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[DiscountByTime] WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
            RemoveDiscountByTimeTasks(deletedDiscount);
            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
        }

        /// <remarks>Необходимо передавать productId, чтобы получить скидку с учетом скидок категорий</remarks>
        public static float GetCurrentDiscount(int? productId = null)
        {
            var discountsByTime = GetCurrentDiscountsByTime();
            if (discountsByTime == null || discountsByTime.Count == 0)
                return 0;

            if (productId.HasValue)
            {
                var productCategories = ProductService.GetCategoriesIDsByProductId(productId.Value, false).ToList();
                // DiscountByTime берется из кэша, поэтому список ApplyDiscountCategories уже будет загружен
                var discountByCategory = discountsByTime.FirstOrDefault(discountByTime => discountByTime.ApplyDiscountCategories 
                                                                                            .Any(category => productCategories.Contains(category.CategoryId)));
                if (discountByCategory != null)
                    return discountByCategory.Discount;
            }

            var discountForAllCategories = discountsByTime.FirstOrDefault(discountByTime => discountByTime.ApplyDiscountCategories.Count == 0);
            return discountForAllCategories?.Discount ?? 0;
        }

        #endregion

        #region Discount by time day of week

        public static List<DayOfWeek> GetDiscountByTimeDaysOfWeek(int discountByTimeId)
        {
            return SQLDataAccess.ExecuteReadList(
                @"SELECT * FROM [Catalog].[DiscountByTimeDayOfWeek] WHERE DiscountByTimeId = @DiscountByTimeId",
                CommandType.Text,
                (reader) => (DayOfWeek)SQLDataHelper.GetInt(reader, "DayOfWeek"),
                new SqlParameter("@DiscountByTimeId", discountByTimeId));
        }

        public static void AddDiscountByTimeDayOfWeek(int discountByTimeId, DayOfWeek dayOfWeek)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Catalog].[DiscountByTimeDayOfWeek] ([DiscountByTimeId],[DayOfWeek])
                        VALUES (@DiscountByTimeId,@DayOfWeek)",
                CommandType.Text,
                new SqlParameter("@DiscountByTimeId", discountByTimeId),
                new SqlParameter("@DayOfWeek", (int)dayOfWeek));
            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
        }

        public static void DeleteDiscountByTimeDaysOfWeek(int discountByTimeId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[DiscountByTimeDayOfWeek] WHERE DiscountByTimeId = @DiscountByTimeId",
                CommandType.Text,
                new SqlParameter("@DiscountByTimeId", discountByTimeId));
            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
        }

        #endregion

        #region Distount by time categories

        private static DiscountByTimeCategory GetDistountByTimeCategoryFromReader(IDataReader reader)
        { 
            return new DiscountByTimeCategory
            {
                DiscountByTimeId = SQLDataHelper.GetInt(reader, "DiscountByTimeId"),
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                ApplyDiscount = SQLDataHelper.GetBoolean(reader, "ApplyDiscount"),
                ActiveByTime = SQLDataHelper.GetBoolean(reader, "ActiveByTime")
            };
        }

        public static List<DiscountByTimeCategory> GetDiscountCategories(int discountByTimeId, bool? applyDiscount = null, bool? activeByTime = null)
        {
            var conditions = new List<string>
            {
                "DiscountByTimeId = @DiscountByTimeId"
            };
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DiscountByTimeId", discountByTimeId)
            };
            if (applyDiscount.HasValue)
            {
                conditions.Add("ApplyDiscount = @ApplyDiscount");
                parameters.Add(new SqlParameter("@ApplyDiscount", applyDiscount.Value));
            }
            if (activeByTime.HasValue)
            {
                conditions.Add("ActiveByTime = @ActiveByTime");
                parameters.Add(new SqlParameter("@ActiveByTime", activeByTime.Value));
            }
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[DiscountByTimeCategory] WHERE " + string.Join(" AND ", conditions),
                CommandType.Text,
                GetDistountByTimeCategoryFromReader,
                parameters.ToArray());
        }

        public static void AddOrUpdateDiscountCategory(int discountByTimeId, int categoryId, bool? activeByTime = null, bool? applyDiscount = null)
        {
            if (!activeByTime.HasValue && !applyDiscount.HasValue)
                return;
            var args = new List<string>();
            if (applyDiscount.HasValue)
                args.Add("ApplyDiscount = @ApplyDiscount");
            if (activeByTime.HasValue)
                args.Add("ActiveByTime = @ActiveByTime");
            SQLDataAccess.ExecuteNonQuery(
                @"IF EXISTS(SELECT * FROM [Catalog].[DiscountByTimeCategory] WHERE DiscountByTimeId = @DiscountByTimeId AND CategoryId = @CategoryId)
                    UPDATE [Catalog].[DiscountByTimeCategory] SET " + string.Join(", ", args) + " WHERE DiscountByTimeId = @DiscountByTimeId AND CategoryId = @CategoryId " +
                @"ELSE
                    INSERT INTO [Catalog].[DiscountByTimeCategory] ([DiscountByTimeId],[CategoryId],[ActiveByTime],[ApplyDiscount])
                        VALUES (@DiscountByTimeId,@CategoryId,@ActiveByTime,@ApplyDiscount)",
                CommandType.Text,
                new SqlParameter("@DiscountByTimeId", discountByTimeId),
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@ActiveByTime", activeByTime ?? false),
                new SqlParameter("@ApplyDiscount", applyDiscount ?? false));
            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
        }

        public static void DeleteDiscountCategories(int discountByTimeId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[DiscountByTimeCategory] WHERE DiscountByTimeId = @DiscountByTimeId",
                CommandType.Text,
                new SqlParameter("@DiscountByTimeId", discountByTimeId));
            CacheManager.RemoveByPattern(DiscountByTimeCacheName);
        }

        #endregion

        public static void ChangeCategoryEnabled(List<int> categoryIds, bool enabled)
        {
            if (categoryIds == null)
                return;
            foreach (var categoryId in categoryIds)
                CategoryService.SetActive(categoryId, enabled, true);
            CategoryService.RecalculateProductsCountManual();
        }

        public static List<TaskSetting> GetAllDiscountByTimeTaskSettings()
        {
            var settings = new List<TaskSetting>();
            foreach (var discountByTime in GetAll(true))
                settings.AddRange(GetDiscountByTimeTaskSetting(discountByTime));
            return settings;
        }

        public static void AddUpdateDiscountByTimeTasks(DiscountByTime discountByTime)
        {
            var taskManagerInstance = TaskManager.TaskManagerInstance();
            GetDiscountByTimeTaskSetting(discountByTime).ForEach(x => taskManagerInstance.AddUpdateTask(x, TaskManager.TaskGroup));
        }
        
        public static void RemoveDiscountByTimeTasks(DiscountByTime discountByTime)
        {
            var taskManagerInstance = TaskManager.TaskManagerInstance();
            GetDiscountByTimeTaskSetting(discountByTime).ForEach(x => taskManagerInstance.RemoveTask(x.GetUniqueName(), TaskManager.TaskGroup));
        }

        private static List<TaskSetting> GetDiscountByTimeTaskSetting(DiscountByTime discountByTime)
        {
            return new List<TaskSetting>
            {
                new TaskSetting
                {
                    Enabled = discountByTime.Enabled,
                    JobType = typeof(DiscountByTimeJob).ToString(),
                    TimeHours = discountByTime.TimeFrom.Hours,
                    TimeMinutes = discountByTime.TimeFrom.Minutes,
                    TimeType = TimeIntervalType.Days,
                    DataMap = discountByTime.Id + ",True" //enable
                },
                new TaskSetting
                {
                    Enabled = discountByTime.Enabled,
                    JobType = typeof(DiscountByTimeJob).ToString(),
                    TimeHours = discountByTime.TimeTo.Hours,
                    TimeMinutes = discountByTime.TimeTo.Minutes,
                    TimeType = TimeIntervalType.Days,
                    DataMap = discountByTime.Id + ",False" // disable
                }
            };
        }
    }
}