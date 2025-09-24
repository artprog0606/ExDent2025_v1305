//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Booking
{
    public class AffiliateService
    {
        #region Get Add Update Delete

        public static Affiliate Get(int id)
        {
            return SQLDataAccess.Query<Affiliate>("SELECT * FROM [Booking].[Affiliate] WHERE Id = @Id", new {Id = id})
                .FirstOrDefault();
        }

        public static List<Affiliate> GetList()
        {
            return SQLDataAccess.Query<Affiliate>("SELECT * FROM [Booking].[Affiliate] Order by SortOrder").ToList();
        }

        public static List<int> GetListId()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[Affiliate]", CommandType.Text, "Id");
        }

        public static int Add(Affiliate affiliate)
        {
            affiliate.Id = SQLDataAccess.ExecuteScalar<int>(
                " INSERT INTO [Booking].[Affiliate] " +
                " ([CityId], [Name], [Description], [Address], [Phone], [SortOrder], [Enabled], [BookingIntervalMinutes], [AccessForAll], " +
                "   [IsActiveSmsNotification], [ForHowManyMinutesToSendSms], [SmsTemplateBeforeStartBooiking], [AnalyticsAccessForAll], " +
                "   [AccessToViewBookingForResourceManagers], [CancelUnpaidViaMinutes]) " +
                " VALUES (@CityId, @Name, @Description, @Address, @Phone, @SortOrder, @Enabled, @BookingIntervalMinutes, @AccessForAll, " +
                "   @IsActiveSmsNotification, @ForHowManyMinutesToSendSms, @SmsTemplateBeforeStartBooiking, @AnalyticsAccessForAll, " +
                "   @AccessToViewBookingForResourceManagers, @CancelUnpaidViaMinutes);" +
                " SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@CityId", affiliate.CityId ?? (object) DBNull.Value),
                new SqlParameter("@Name", affiliate.Name ?? (object) DBNull.Value),
                new SqlParameter("@Description", affiliate.Description ?? (object) DBNull.Value),
                new SqlParameter("@Address", affiliate.Address ?? (object) DBNull.Value),
                new SqlParameter("@Phone", affiliate.Phone ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", affiliate.SortOrder),
                new SqlParameter("@Enabled", affiliate.Enabled),
                new SqlParameter("@BookingIntervalMinutes", affiliate.BookingIntervalMinutes),
                new SqlParameter("@AccessForAll", affiliate.AccessForAll),
                new SqlParameter("@AnalyticsAccessForAll", affiliate.AnalyticsAccessForAll),
                new SqlParameter("@AccessToViewBookingForResourceManagers", affiliate.AccessToViewBookingForResourceManagers),
                new SqlParameter("@IsActiveSmsNotification", affiliate.IsActiveSmsNotification),
                new SqlParameter("@ForHowManyMinutesToSendSms", affiliate.ForHowManyMinutesToSendSms ?? (object)DBNull.Value),
                new SqlParameter("@SmsTemplateBeforeStartBooiking", affiliate.SmsTemplateBeforeStartBooiking ?? (object)DBNull.Value),
                new SqlParameter("@CancelUnpaidViaMinutes", affiliate.CancelUnpaidViaMinutes ?? (object)DBNull.Value)
                );

            if (!affiliate.AccessForAll)
                affiliate.ManagerIds.ForEach(x => AddManager(affiliate.Id, x));

            if (!affiliate.AnalyticsAccessForAll)
                affiliate.AnalyticManagerIds.ForEach(x => AddAnalyticManager(affiliate.Id, x));

            return affiliate.Id;
        }

        public static void Update(Affiliate affiliate)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[Affiliate] SET [CityId] = @CityId, [Name] = @Name, [Description] = @Description, [Address] = @Address," +
                " [Phone] = @Phone, [SortOrder] = @SortOrder, [Enabled] = @Enabled, [BookingIntervalMinutes] = @BookingIntervalMinutes, [AccessForAll] = @AccessForAll," +
                " [IsActiveSmsNotification] = @IsActiveSmsNotification, [ForHowManyMinutesToSendSms] = @ForHowManyMinutesToSendSms, [SmsTemplateBeforeStartBooiking] = @SmsTemplateBeforeStartBooiking," +
                " [AnalyticsAccessForAll] = @AnalyticsAccessForAll, [AccessToViewBookingForResourceManagers] = @AccessToViewBookingForResourceManagers, [CancelUnpaidViaMinutes] = @CancelUnpaidViaMinutes" +
                " WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", affiliate.Id),
                new SqlParameter("@CityId", affiliate.CityId ?? (object) DBNull.Value),
                new SqlParameter("@Name", affiliate.Name ?? (object) DBNull.Value),
                new SqlParameter("@Description", affiliate.Description ?? (object) DBNull.Value),
                new SqlParameter("@Address", affiliate.Address ?? (object) DBNull.Value),
                new SqlParameter("@Phone", affiliate.Phone ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", affiliate.SortOrder),
                new SqlParameter("@Enabled", affiliate.Enabled),
                new SqlParameter("@BookingIntervalMinutes", affiliate.BookingIntervalMinutes),
                new SqlParameter("@AccessForAll", affiliate.AccessForAll),
                new SqlParameter("@AnalyticsAccessForAll", affiliate.AnalyticsAccessForAll),
                new SqlParameter("@AccessToViewBookingForResourceManagers", affiliate.AccessToViewBookingForResourceManagers),
                new SqlParameter("@IsActiveSmsNotification", affiliate.IsActiveSmsNotification),
                new SqlParameter("@ForHowManyMinutesToSendSms", affiliate.ForHowManyMinutesToSendSms ?? (object)DBNull.Value),
                new SqlParameter("@SmsTemplateBeforeStartBooiking", affiliate.SmsTemplateBeforeStartBooiking ?? (object)DBNull.Value),
                new SqlParameter("@CancelUnpaidViaMinutes", affiliate.CancelUnpaidViaMinutes ?? (object)DBNull.Value)
                );

            if (!affiliate.AccessForAll)
            {
                var currentManagers = GetManagerIds(affiliate.Id);

                // delete
                currentManagers.Where(current => !affiliate.ManagerIds.Contains(current))
                    .ForEach(x => DeleteManager(affiliate.Id, x));

                // add
                affiliate.ManagerIds.Where(x => !currentManagers.Contains(x))
                    .ForEach(x => AddManager(affiliate.Id, x));
            }
            else
                DeleteManagers(affiliate.Id);

            if (!affiliate.AnalyticsAccessForAll)
            {
                var currentManagers = GetAnalyticManagerIds(affiliate.Id);

                // delete
                currentManagers.Where(current => !affiliate.AnalyticManagerIds.Contains(current))
                    .ForEach(x => DeleteAnalyticManager(affiliate.Id, x));

                // add
                affiliate.AnalyticManagerIds.Where(x => !currentManagers.Contains(x))
                    .ForEach(x => AddAnalyticManager(affiliate.Id, x));
            }
            else
                DeleteAnalyticManagers(affiliate.Id);
        }

        public static void Delete(int id)
        {
            BeforeDelete(id);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[Affiliate] WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        private static void BeforeDelete(int id)
        {
            CategoryService.DeleteRefByAffiliate(id);
        }

        #endregion

        #region Managers

        public static List<int> GetManagerIds(int affiliateId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    "SELECT ManagerId FROM [Booking].[AffiliateManager] WHERE [AffiliateId] = @AffiliateId",
                    CommandType.Text,
                    "ManagerId",
                    new SqlParameter("@AffiliateId", affiliateId));
        }

        public static bool ExistManager(int affiliateId, int managerId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] WHERE [AffiliateId] = @AffiliateId AND [ManagerId] = @ManagerId)
                begin 
                    SELECT 1
                end
                else
                begin 
                    SELECT 0
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId)
                );
        }

        public static void AddManager(int affiliateId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] WHERE [AffiliateId] = @AffiliateId AND [ManagerId] = @ManagerId)
                begin 
                    INSERT INTO [Booking].[AffiliateManager] ([AffiliateId],[ManagerId]) VALUES (@AffiliateId, @ManagerId)
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId)
                );
        }

        public static void DeleteManagers(int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateManager] WHERE AffiliateId = @AffiliateId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId));
        }

        public static void DeleteManager(int affiliateId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateManager] WHERE AffiliateId = @AffiliateId AND ManagerId = @ManagerId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId));
        }

        #endregion

        #region AnalyticManagers

        public static List<int> GetAnalyticManagerIds(int affiliateId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    "SELECT ManagerId FROM [Booking].[AffiliateAnalyticManager] WHERE [AffiliateId] = @AffiliateId",
                    CommandType.Text,
                    "ManagerId",
                    new SqlParameter("@AffiliateId", affiliateId));
        }

        public static bool ExistAnalyticManager(int affiliateId, int managerId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS(SELECT 1 FROM [Booking].[AffiliateAnalyticManager] WHERE [AffiliateId] = @AffiliateId AND [ManagerId] = @ManagerId)
                begin 
                    SELECT 1
                end
                else
                begin 
                    SELECT 0
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId)
                );
        }

        public static void AddAnalyticManager(int affiliateId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[AffiliateAnalyticManager] WHERE [AffiliateId] = @AffiliateId AND [ManagerId] = @ManagerId)
                begin 
                    INSERT INTO [Booking].[AffiliateAnalyticManager] ([AffiliateId],[ManagerId]) VALUES (@AffiliateId, @ManagerId)
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId)
                );
        }

        public static void DeleteAnalyticManagers(int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateAnalyticManager] WHERE AffiliateId = @AffiliateId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId));
        }

        public static void DeleteAnalyticManager(int affiliateId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateAnalyticManager] WHERE AffiliateId = @AffiliateId AND ManagerId = @ManagerId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId));
        }

        #endregion

        #region Help
        public static bool CheckAccess(int affiliateId)
        {
            return CheckAccess(Get(affiliateId));
        }

        public static bool CheckAccess(Affiliate affiliate)
        {
            return CheckAccess(affiliate, null, true);
        }

        public static bool CheckAccess(Affiliate affiliate, Manager manager, bool checkByReservationResources)
        {
            if (affiliate.AccessForAll)
                return true;

            if (manager == null)
            {
                var currentCustomer = CustomerContext.CurrentCustomer;

                if (currentCustomer.IsAdmin || currentCustomer.IsVirtual)
                    return true;

                if (currentCustomer.IsManager)
                {
                    manager = ManagerService.GetManager(currentCustomer.Id);

                    if (CheckAccessManager(affiliate, manager, checkByReservationResources))
                        return true;
                }
            }
            else if (CheckAccessManager(affiliate, manager, checkByReservationResources))
                return true;

            return false;
        }

        private static bool CheckAccessManager(Affiliate affiliate, Manager manager, bool checkByReservationResources)
        {
            if (manager != null && manager.Enabled)
            {
                if (manager.Customer.IsAdmin || affiliate.ManagerIds.Contains(manager.ManagerId))
                    return true;

                if (
                    checkByReservationResources &&
                    ReservationResourceService.GetByAffiliate(affiliate.Id, onlyActive: false)
                        .Any(x => x.ManagerId == manager.ManagerId))
                    return true;
            }
            return false;
        }

        public static bool CheckAccessToEditing(int affiliateId, Manager manager = null)
        {
            return CheckAccessToEditing(Get(affiliateId), manager);
        }

        public static bool CheckAccessToEditing(Affiliate affiliate, Manager manager = null)
        {
            return CheckAccess(affiliate, manager, false);
        }

        public static bool CheckAccessToAnalytic(int affiliateId, Manager manager = null)
        {
            return CheckAccessToAnalytic(Get(affiliateId), manager);
        }

        public static bool CheckAccessToAnalytic(Affiliate affiliate, Manager manager = null)
        {
            if (!CheckAccessToEditing(affiliate, manager))
                return false;

            if (affiliate.AnalyticsAccessForAll)
                return true;

            if (manager == null)
            {
                var currentCustomer = CustomerContext.CurrentCustomer;

                if (currentCustomer.IsAdmin || currentCustomer.IsVirtual)
                    return true;

                if (currentCustomer.IsManager)
                {
                    manager = ManagerService.GetManager(currentCustomer.Id);

                    if (CheckAccessManagerToAnalytic(affiliate, manager))
                        return true;
                }
            }
            else
                return CheckAccessManagerToAnalytic(affiliate, manager);

            return false;
        }

        private static bool CheckAccessManagerToAnalytic(Affiliate affiliate, Manager manager)
        {
            if (manager != null && manager.Enabled)
            {
                if (manager.Customer.IsAdmin || affiliate.AnalyticManagerIds.Contains(manager.ManagerId))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Список должен быть строго по одному дню</para>
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="listTimes">Список проверяемого времени 
        /// <para>Список должен быть строго по одному дню</para>
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(int affiliateId, List<Tuple<DateTime, DateTime>> listTimes)
        {
            var dateFirst = listTimes[0].Item1.Date;
            if (listTimes.Any(x => x.Item1.Date != dateFirst))
                throw new ArgumentException("Список должен быть строго по одному дню", "listTimes");

            return listTimes.All(x => ExistDateRangeInTimeOfBooking(affiliateId, x.Item1, x.Item2));
        }

        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Список должен быть строго по одному дню</para>
        /// </summary>
        /// <param name="listTimes">Список проверяемого времени 
        /// <para>Список должен быть строго по одному дню</para>
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(List<Tuple<DateTime, DateTime>> listTimes,
            List<AffiliateAdditionalTime> affiliateAdditionalTime, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            var dateFirst = listTimes[0].Item1.Date;
            if (listTimes.Any(x => x.Item1.Date != dateFirst))
                throw new ArgumentException("Список должен быть строго по одному дню", "listTimes");

            return listTimes.All(x => ExistDateRangeInTimeOfBooking(x.Item1, x.Item2, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek));
        }

        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(int affiliateId, DateTime start, DateTime end)
        {
            return ExistDateRangeInTimeOfBooking(affiliateId, start, end,
                AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliateId, start));
        }


        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(int affiliateId, DateTime start, DateTime end, List<AffiliateAdditionalTime> affiliateAdditionalTime)
        {

            if (affiliateAdditionalTime.Count > 0)
            {
                return ExistDateRangeInTimeOfBooking(start, end, affiliateAdditionalTime);
            }
            else
            {
                return ExistDateRangeInTimeOfBookingByDayOfWeek(affiliateId, start, end);
            }
        }


        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(DateTime start, DateTime end,
            List<AffiliateAdditionalTime> affiliateAdditionalTime, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {

            if (affiliateAdditionalTime.Count > 0)
            {
                return ExistDateRangeInTimeOfBooking(start, end, affiliateAdditionalTime);
            }
            else
            {
                return ExistDateRangeInTimeOfBookingByDayOfWeek(start, end, affiliateTimesOfBookingDayOfWeek);
            }
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBooking(DateTime start, DateTime end, List<AffiliateAdditionalTime> affiliateAdditionalTime)
        {
            /* Если первый элемент рабочий (IsWork), то значит это список
             * с рабочим временем на этот день.
             * Если первый элемент не рабочий, значит это единственный элемент
             * и он покрывает по времени весь день
             */
            return affiliateAdditionalTime[0].IsWork &&
                     affiliateAdditionalTime.Any(
                         additionalTime =>
                             start == additionalTime.StartTime &&
                             end == additionalTime.EndTime);
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, DateTime start, DateTime end)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(affiliateId, start.TimeOfDay, end.TimeOfDay, start.DayOfWeek);
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="listTimes">Список проверяемого времени
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <param name="dayOfWeek">День недели проверяемого времени</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, List<Tuple<TimeSpan, TimeSpan>> listTimes, DayOfWeek dayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(listTimes,
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, dayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, TimeSpan start, TimeSpan end, DayOfWeek dayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(start, end,
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, dayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        /// <param name="listTimes">Список проверяемого времени.
        /// T1 начальное время, T2 время окончания.</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(List<Tuple<TimeSpan, TimeSpan>> listTimes, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            return listTimes.All(x => ExistDateRangeInTimeOfBookingByDayOfWeek(x.Item1, x.Item2, affiliateTimesOfBookingDayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(DateTime start, DateTime end, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(start.TimeOfDay, end.TimeOfDay, affiliateTimesOfBookingDayOfWeek);
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(TimeSpan start, TimeSpan end, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            //var dayOfWeek = affiliateTimesOfBookingDayOfWeek[0].DayOfWeek;
            //if (affiliateTimesOfBookingDayOfWeek.Any(x => x.DayOfWeek != dayOfWeek))
            //    throw new ArgumentException("Список расписанию должне быть по одному дню недели", "affiliateTimesOfBookingDayOfWeek");

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return affiliateTimesOfBookingDayOfWeek.Any(
                timeOfBooking =>
                    start == timeOfBooking.StartTime && end == timeOfBooking.EndTime);
        }


        /// <summary>
        /// Проверка времени на то, что оно рабочее
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <returns></returns>
        public static bool CheckDateRangeIsWork(int affiliateId, DateTime start, DateTime end)
        {
            var oneDay = start.Date == end.Date;

            var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(affiliateId, start.Date, end.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                (oneDay
                    ? AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, start.DayOfWeek)
                    : AffiliateTimeOfBookingService.GetByAffiliate(affiliateId))

                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList())
            );

            return CheckDateRangeIsWork(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
        }

        /// <summary>
        /// Проверка времени на то, что оно рабочее
        /// </summary>
        /// <param name="listTimes">Список проверяемого времени 
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool CheckDateRangeIsWork(List<Tuple<DateTime, DateTime>> listTimes,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTime,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBookingDayOfWeek)
        {
            return listTimes.All(x => CheckDateRangeIsWork(x.Item1, x.Item2, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek));
        }

        /// <summary>
        /// Проверка времени на то, что оно рабочее
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool CheckDateRangeIsWork(DateTime start, DateTime end,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTime,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBookingDayOfWeek)
        {
            if (CheckWeekend(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek))
                return false;

            // Вариант вхождения в рабочии интервалы
            // Алгоритм не проверяет на нерабочие дни, т.к. это делается выше
            List<Tuple<DateTime, DateTime>> listTimes;
            Tuple<DateTime, DateTime> times;
            DateTime? startWorkInterval = null;
            DateTime? prev = null;
            int index;
            DateTime date = end.Date;
            for (DateTime d = start.Date; d <= date; d = d.AddDays(1))// в переменной d всегда начало дня
            {
                index = 0;

                if (affiliateAdditionalTime.ContainsKey(d) && affiliateAdditionalTime[d].Count > 0)
                    listTimes =
                        affiliateAdditionalTime[d].Select(
                            x => new Tuple<DateTime, DateTime>(x.StartTime, x.EndTime)).ToList();
                else
                    listTimes = affiliateTimesOfBookingDayOfWeek.ContainsKey(d.DayOfWeek)
                        ? affiliateTimesOfBookingDayOfWeek[d.DayOfWeek].Select(
                            x => new Tuple<DateTime, DateTime>(d + x.StartTime, d + x.EndTime)).ToList()
                        : new List<Tuple<DateTime, DateTime>>();

                // зайдет на певом шаге
                if (startWorkInterval == null)
                {
                    for (int tempIndex = 0; tempIndex < listTimes.Count; tempIndex++)
                        if (listTimes[tempIndex].Item1 <= start && start < listTimes[tempIndex].Item2)
                        {
                            index = tempIndex;
                            startWorkInterval = listTimes[tempIndex].Item1;
                            break;
                        }

                    // дата начала не пересекается с рабочим временем
                    if (startWorkInterval == null)
                        return false;
                }

                if (listTimes.Count > 0)
                { 
                    for (; index < listTimes.Count; index++)
                    {
                        times = listTimes[index];
                        if (!prev.HasValue)// зайден на первом шаге обоих циклов
                            prev = times.Item1;

                        if (prev != times.Item1 || end <= times.Item2)// || (index == listTimes.Count - 1 && d == date) условие остановки циклов (т.е. последний шаг), как страховка, если не сработает end <= times.Item2
                        {
                            var endWorkInterval = prev == times.Item1 ? times.Item2 : prev.Value;
                            return start >= startWorkInterval && end <= endWorkInterval;
                        }

                        prev = times.Item2;
                    }
                }
                else
                {
                    var endWorkInterval = prev.Value;
                    return start >= startWorkInterval && end <= endWorkInterval;
                }
            }

            return false;
        }

        /// <summary>
        /// Проверка на пересечение с нерабочим днем
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        public static bool CheckWeekend(DateTime start, DateTime end,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTime,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBookingDayOfWeek)
        {
            DateTime date;

            // перенесено в цикл ниже
            // если в указанном времени есть хоть один нерабочий день
            // date - хранит начало дня (BeginDate)
            //if (affiliateAdditionalTime.Where(x => start < (date = x.Key.Date).AddDays(1) && date < end) // start < [EndDate] AND [BeginDate] < end
            //        .Any(x => x.Value.Count > 0 && !x.Value[0].IsWork))
            //    return true;

            // если в указанном времени есть хоть один нерабочий день недели
            //var checkDayOfWeek = new List<DayOfWeek>();
            date = end != end.Date ? end.Date : end.Date.AddDays(-1);//если время оканчания 00:00, тогда день, на который заезжает дата окончания, не нужно проверять
            for (DateTime d = start.Date; d <= date; d = d.AddDays(1))
            {
                if (affiliateAdditionalTime.ContainsKey(d) && affiliateAdditionalTime[d].Count > 0)
                {
                    if (!affiliateAdditionalTime[d][0].IsWork)
                        return true;
                }
                else
                {
                    if (!affiliateTimesOfBookingDayOfWeek.ContainsKey(d.DayOfWeek) ||
                        affiliateTimesOfBookingDayOfWeek[d.DayOfWeek].Count == 0)
                        return true;
                }

                //checkDayOfWeek.Add(d.DayOfWeek);
                //if (checkDayOfWeek.Count >= 7)// проверили все 7 дней недели
                //    break;
            }

            return false;
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно рабочее
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool CheckDateRangeIsWorkByDayOfWeek(TimeSpan start, TimeSpan end, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            //var dayOfWeek = affiliateTimesOfBookingDayOfWeek[0].DayOfWeek;
            //if (affiliateTimesOfBookingDayOfWeek.Any(x => x.DayOfWeek != dayOfWeek))
            //    throw new ArgumentException("Список расписанию должне быть по одному дню недели", "affiliateTimesOfBookingDayOfWeek");

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            // Вариант вхождения в рабочии интервалы
            TimeSpan? startWorkInterval = null;
            TimeSpan? prev = null;

            for (int index = 0; index < affiliateTimesOfBookingDayOfWeek.Count; index++)
            {
                var timeOfBooking = affiliateTimesOfBookingDayOfWeek[index];

                if (!startWorkInterval.HasValue)
                    startWorkInterval = timeOfBooking.StartTime;
                if (!prev.HasValue)
                    prev = timeOfBooking.StartTime;

                if (prev != timeOfBooking.StartTime || index == affiliateTimesOfBookingDayOfWeek.Count - 1)
                {
                    TimeSpan endWorkInterval;
                    bool isWorkAffiliate;

                    if (prev != timeOfBooking.StartTime && index == affiliateTimesOfBookingDayOfWeek.Count - 1)
                    {
                        endWorkInterval = prev.Value;
                        isWorkAffiliate = start >= startWorkInterval && end <= endWorkInterval;
                        if (isWorkAffiliate)
                            return true;

                        startWorkInterval = timeOfBooking.StartTime;
                    }

                    endWorkInterval = index == affiliateTimesOfBookingDayOfWeek.Count - 1 ? timeOfBooking.EndTime : prev.Value;

                    isWorkAffiliate = start >= startWorkInterval && end <= endWorkInterval;
                    if (isWorkAffiliate)
                        return true;

                    startWorkInterval = timeOfBooking.StartTime;
                }

                prev = timeOfBooking.EndTime;
            }
            return false;
        }

        #endregion
    }
}
