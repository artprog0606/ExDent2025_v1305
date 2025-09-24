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
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class ReservationResourceService
    {
        #region Get Add Update Delete

        public static ReservationResource Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne<ReservationResource>(
                "SELECT * FROM [Booking].[ReservationResource] WHERE Id = @Id", CommandType.Text,
                GetReservationResourceFromReader, new SqlParameter("@Id", id));
        }

        public static List<ReservationResource> GetList()
        {
            return
                SQLDataAccess.ExecuteReadList<ReservationResource>(
                    "SELECT * FROM [Booking].[ReservationResource] ORDER BY SortOrder, Name",
                    CommandType.Text, GetReservationResourceFromReader);
        }

        public static List<ReservationResource> GetByAffiliate(int affiliateId, bool onlyActive = true)
        {
            return
                SQLDataAccess.ExecuteReadList<ReservationResource>(
                    String.Format(
                        "SELECT ReservationResource.*" +
                        " FROM [Booking].[ReservationResource]" +
                        " INNER JOIN [Booking].[AffiliateReservationResource] ON [AffiliateReservationResource].ReservationResourceId = [ReservationResource].Id" +
                        " WHERE [AffiliateReservationResource].[AffiliateId] = @AffiliateId{0}" +
                        " ORDER BY SortOrder, Name",
                        onlyActive ? " AND [ReservationResource].[Enabled] = 1" : String.Empty),
                    CommandType.Text, GetReservationResourceFromReader, new SqlParameter("@AffiliateId", affiliateId));
        }

        public static List<int> GetListId()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[ReservationResource]", CommandType.Text, "Id");
        }

        public static List<int> GetListIdByAffiliate(int affiliateId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT [Id] FROM [Booking].[ReservationResource]" +
                " INNER JOIN [Booking].[AffiliateReservationResource] ON [AffiliateReservationResource].ReservationResourceId = [ReservationResource].Id" +
                " WHERE [AffiliateReservationResource].[AffiliateId] = @AffiliateId",
                CommandType.Text, "Id", new SqlParameter("@AffiliateId", affiliateId));
        }

        private static ReservationResource GetReservationResourceFromReader(SqlDataReader reader)
        {
            return new ReservationResource
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                ManagerId = SQLDataHelper.GetNullableInt(reader, "ManagerId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                Image = SQLDataHelper.GetString(reader, "Image"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };

        }

        public static int Add(ReservationResource reservationresource)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                " INSERT INTO [Booking].[ReservationResource] " +
                " ([ManagerId], [Name], [Description], [Image], [Enabled], [SortOrder]) " +
                " VALUES (@ManagerId, @Name, @Description, @Image, @Enabled, @SortOrder); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@ManagerId", reservationresource.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@Name", reservationresource.Name ?? (object)DBNull.Value),
                new SqlParameter("@Description", reservationresource.Description ?? (object)DBNull.Value),
                new SqlParameter("@Image", reservationresource.Image ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", reservationresource.Enabled),
                new SqlParameter("@SortOrder", reservationresource.SortOrder));
        }

        public static void Update(ReservationResource reservationresource)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[ReservationResource] SET [ManagerId] = @ManagerId, [Name] = @Name, [Description] = @Description, [Image] = @Image, [Enabled] = @Enabled, [SortOrder] = @SortOrder " +
                " WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", reservationresource.Id),
                new SqlParameter("@ManagerId", reservationresource.ManagerId ?? (object) DBNull.Value),
                new SqlParameter("@Name", reservationresource.Name ?? (object) DBNull.Value),
                new SqlParameter("@Description", reservationresource.Description ?? (object) DBNull.Value),
                new SqlParameter("@Image", reservationresource.Image ?? (object) DBNull.Value),
                new SqlParameter("@Enabled", reservationresource.Enabled),
                new SqlParameter("@SortOrder", reservationresource.SortOrder));
        }

        public static void Delete(int id)
        {
            BeforeDelete(id);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[ReservationResource] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        private static void BeforeDelete(int id)
        {
            var empl = Get(id);
            if (empl != null)
                BeforeDelete(empl);
        }

        private static void BeforeDelete(ReservationResource reservationresource)
        {
            //todo Удалить ресурсы
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingReservationResource, reservationresource.Image));

        }

        #endregion

        #region AffiliateReservationResource

        public static void AddUpdateRefAffiliate(int affiliateId, int reservationResourceId, int? bookingIntervalMinutes)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId)
                begin 
                    INSERT INTO [Booking].[AffiliateReservationResource] ([AffiliateId], [ReservationResourceId], [BookingIntervalMinutes]) VALUES (@AffiliateId, @ReservationResourceId, @BookingIntervalMinutes);
                end
                ELSE
                begin
                    UPDATE [Booking].[AffiliateReservationResource] SET [BookingIntervalMinutes] = @BookingIntervalMinutes
                    WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@BookingIntervalMinutes", bookingIntervalMinutes ?? (object)DBNull.Value)
                );
        }

        public static bool ExistsRefAffiliate(int affiliateId, int reservationResourceId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId)
                begin 
                    SELECT 0;
                end
                ELSE
                begin
                    SELECT 1;
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId)) > 0;
        }

        public static int? GetBookingIntervalMinutesForAffiliate(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT [BookingIntervalMinutes] FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId",
                    CommandType.Text,
                    reader => SQLDataHelper.GetNullableInt(reader, "BookingIntervalMinutes"),
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static void DeleteRefAffiliate(int affiliateId, int reservationResourceId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        #endregion

        #region Help

        public static bool CheckAccess(ReservationResource reservationresource, int affiliateId)
        {
            return CheckAccess(reservationresource, AffiliateService.Get(affiliateId), null);
        }

        public static bool CheckAccess(ReservationResource reservationresource, Affiliate affiliate, Manager manager)
        {
            if (ExistsRefAffiliate(affiliate.Id, reservationresource.Id))
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

                        if (CheckAccessManager(reservationresource, affiliate, manager))
                            return true;
                    }
                }
                else if (CheckAccessManager(reservationresource, affiliate, manager))
                    return true;
            }
            return false;
        }

        private static bool CheckAccessManager(ReservationResource reservationresource, Affiliate affiliate, Manager manager)
        {
            if (manager != null && manager.Enabled)
            {
                if (reservationresource.ManagerId == manager.ManagerId || manager.Customer.IsAdmin || affiliate.ManagerIds.Contains(manager.ManagerId))
                    return true;
            }
            return false;
        }

        public static bool CheckAccessToEditing(int reservationresourceId, int affiliateId)
        {
            return CheckAccessToEditing(Get(reservationresourceId), affiliateId);
        }

        public static bool CheckAccessToEditing(ReservationResource reservationresource, int affiliateId)
        {
            return CheckAccessToEditing(reservationresource, AffiliateService.Get(affiliateId), null);
        }

        public static bool CheckAccessToEditing(ReservationResource reservationresource, Affiliate affiliate, Manager manager)
        {
            return AffiliateService.CheckAccessToEditing(affiliate, manager);
        }


        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Список должен быть строго по одному дню</para>
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="reservationResourceId">Идентификатор ресурса бронирования</param>
        /// <param name="listTimes">Список проверяемого времени 
        /// <para>Список должен быть строго по одному дню</para>
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(int affiliateId, int reservationResourceId, List<Tuple<DateTime, DateTime>> listTimes)
        {
            var dateFirst = listTimes[0].Item1.Date;
            if (listTimes.Any(x => x.Item1.Date != dateFirst))
                throw new ArgumentException("Список должен быть строго по одному дню", "listTimes");

            return listTimes.All(x => ExistDateRangeInTimeOfBooking(affiliateId, reservationResourceId, x.Item1, x.Item2));
        }

        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Список должен быть строго по одному дню</para>
        /// </summary>
        /// <param name="listTimes">Список проверяемого времени 
        /// <para>Список должен быть строго по одному дню</para>
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <param name="reservationResourceAdditionalTime">Исключения по времени ресурса бронирования</param>
        /// <param name="reservationResourceTimesOfBookingDayOfWeek">Временная сетка по дню недели ресурса бронирования</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(List<Tuple<DateTime, DateTime>> listTimes,
            List<ReservationResourceAdditionalTime> reservationResourceAdditionalTime, List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBookingDayOfWeek,
            List<AffiliateAdditionalTime> affiliateAdditionalTime, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            var dateFirst = listTimes[0].Item1.Date;
            if (listTimes.Any(x => x.Item1.Date != dateFirst))
                throw new ArgumentException("Список должен быть строго по одному дню", "listTimes");

            return listTimes.All(
                x =>
                    ExistDateRangeInTimeOfBooking(x.Item1, x.Item2, reservationResourceAdditionalTime, reservationResourceTimesOfBookingDayOfWeek,
                        affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek));
        }

        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="reservationResourceId">Идентификатор ресурса бронирования</param>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(int affiliateId, int reservationResourceId, DateTime start, DateTime end)
        {
            return ExistDateRangeInTimeOfBooking(start, end,
                ReservationResourceAdditionalTimeService.GetByDate(affiliateId, reservationResourceId, start),
                ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId, reservationResourceId, start.DayOfWeek),
                AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliateId, start),
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, start.DayOfWeek));
        }

        /// <summary>
        /// Проверка времени на то, что оно входит в ячейки времени бронирования
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="reservationResourceAdditionalTime">Исключения по времени ресурса бронирования</param>
        /// <param name="reservationResourceTimesOfBookingDayOfWeek">Временная сетка по дню недели ресурса бронирования</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBooking(DateTime start, DateTime end,
            List<ReservationResourceAdditionalTime> reservationResourceAdditionalTime, List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBookingDayOfWeek,
            List<AffiliateAdditionalTime> affiliateAdditionalTime, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            if (reservationResourceAdditionalTime.Count > 0)
            {
                return ExistDateRangeInTimeOfBooking(start, end, reservationResourceAdditionalTime, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
            }
            else
            {
                return ExistDateRangeInTimeOfBookingByDayOfWeek(start, end, reservationResourceTimesOfBookingDayOfWeek, affiliateTimesOfBookingDayOfWeek) &&
                    (affiliateAdditionalTime.Count == 0 || AffiliateService.ExistDateRangeInTimeOfBooking(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek));
            }
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBooking(DateTime start, DateTime end, List<ReservationResourceAdditionalTime> reservationResourceAdditionalTime,
            List<AffiliateAdditionalTime> affiliateAdditionalTime, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            if (reservationResourceAdditionalTime.Count == 0)
                return AffiliateService.ExistDateRangeInTimeOfBooking(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);

            /* Если первый элемент рабочий (IsWork), то значит это список
             * с рабочим временем на этот день.
             * Если первый элемент не рабочий, значит это единственный элемент
             * и он покрывает по времени весь день
             */
            return reservationResourceAdditionalTime[0].IsWork &&
                     reservationResourceAdditionalTime.Any(
                         additionalTime =>
                             start == additionalTime.StartTime &&
                             end == additionalTime.EndTime) &&
                             AffiliateService.ExistDateRangeInTimeOfBooking(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="reservationResourceId">Идентификатор ресурса бронирования</param>
        /// <param name="listTimes">Список проверяемого времени
        /// <para>T1 начальное время, T2 время окончания</para></param>
        /// <param name="dayOfWeek">День недели проверяемого времени</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, int reservationResourceId, List<Tuple<TimeSpan, TimeSpan>> listTimes, DayOfWeek dayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(listTimes,
                ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId, reservationResourceId, dayOfWeek),
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, dayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        /// <param name="listTimes">Список проверяемого времени.
        /// T1 начальное время, T2 время окончания.</param>
        /// <param name="reservationResourceTimesOfBookingDayOfWeek">Временная сетка по дню недели ресурса бронирования</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(List<Tuple<TimeSpan, TimeSpan>> listTimes,
            List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBookingDayOfWeek, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            return listTimes.All(x => ExistDateRangeInTimeOfBookingByDayOfWeek(x.Item1, x.Item2, reservationResourceTimesOfBookingDayOfWeek, affiliateTimesOfBookingDayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, int reservationResourceId, DateTime start, DateTime end, DayOfWeek dayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(start, end,
                ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId, reservationResourceId, dayOfWeek),
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, dayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, int reservationResourceId, DateTime start, DateTime end)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(start, end,
                ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId, reservationResourceId, start.DayOfWeek),
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, start.DayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(DateTime start, DateTime end, 
            List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBookingDayOfWeek, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(start.TimeOfDay, end.TimeOfDay, reservationResourceTimesOfBookingDayOfWeek, affiliateTimesOfBookingDayOfWeek);
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(int affiliateId, int reservationResourceId, TimeSpan start, TimeSpan end, DayOfWeek dayOfWeek)
        {
            return ExistDateRangeInTimeOfBookingByDayOfWeek(start, end,
                ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId, reservationResourceId, dayOfWeek),
                AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, dayOfWeek));
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно входит в исключающие ячейки времени бронирования
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool ExistDateRangeInTimeOfBookingByDayOfWeek(TimeSpan start, TimeSpan end,
            List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBookingDayOfWeek, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            //var dayOfWeek = reservationResourceTimesOfBookingDayOfWeek[0].DayOfWeek;
            //if (reservationResourceTimesOfBookingDayOfWeek.Any(x => x.DayOfWeek != dayOfWeek))
            //    throw new ArgumentException("Список расписанию должне быть по одному дню недели", "reservationResourceTimesOfBookingDayOfWeek");

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return reservationResourceTimesOfBookingDayOfWeek.Any(
                timeOfBooking =>
                    start == timeOfBooking.StartTime && end == timeOfBooking.EndTime) &&
                    AffiliateService.ExistDateRangeInTimeOfBookingByDayOfWeek(start, end, affiliateTimesOfBookingDayOfWeek);
        }





        /// <summary>
        /// Проверка времени на то, что оно рабочее
        /// </summary>
        /// <param name="affiliateId">Идентификатор филиала</param>
        /// <param name="reservationResourceId">Идентификатор ресурса бронирования</param>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <returns></returns>
        public static bool CheckDateRangeIsWork(int affiliateId, int reservationResourceId, DateTime start, DateTime end)
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
            var reservationResourceAdditionalTime = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                ReservationResourceAdditionalTimeService.GetByDateFromTo(affiliateId, reservationResourceId, start.Date, end.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            var reservationResourceTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                (oneDay
                    ? ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId, reservationResourceId, start.DayOfWeek)
                    : ReservationResourceTimeOfBookingService.GetBy(affiliateId, reservationResourceId))

                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList())
            );

            return CheckDateRangeIsWork(start, end, reservationResourceAdditionalTime,
                reservationResourceTimesOfBookingDayOfWeek, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
        }

        /// <summary>
        /// Проверка времени на то, что оно рабочее
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="reservationResourceAdditionalTime">Исключения по времени ресурса бронирования</param>
        /// <param name="reservationResourceTimesOfBookingDayOfWeek">Временная сетка по дню недели ресурса бронирования</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        /// <returns></returns>
        public static bool CheckDateRangeIsWork(DateTime start, DateTime end,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> reservationResourceAdditionalTime,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> reservationResourceTimesOfBookingDayOfWeek,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTime,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBookingDayOfWeek)
        {
            if (CheckWeekend(start, end, 
                reservationResourceAdditionalTime, reservationResourceTimesOfBookingDayOfWeek,
                affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek))
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

                if (reservationResourceAdditionalTime.ContainsKey(d) && reservationResourceAdditionalTime[d].Count > 0)
                    listTimes =
                        reservationResourceAdditionalTime[d].Select(
                            x => new Tuple<DateTime, DateTime>(x.StartTime, x.EndTime)).ToList();
                else
                    listTimes = reservationResourceTimesOfBookingDayOfWeek.ContainsKey(d.DayOfWeek)
                        ? reservationResourceTimesOfBookingDayOfWeek[d.DayOfWeek].Select(
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
                            return start >= startWorkInterval && end <= endWorkInterval && 
                                AffiliateService.CheckDateRangeIsWork(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
                        }

                        prev = times.Item2;
                    }
                }
                else
                {
                    var endWorkInterval = prev.Value;
                    return start >= startWorkInterval && end <= endWorkInterval &&
                                AffiliateService.CheckDateRangeIsWork(start, end, affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
                }
            }

            return false;
        }

        /// <summary>
        /// Проверка на пересечение с нерабочим днем
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="end">Время окончания</param>
        /// <param name="reservationResourceAdditionalTime">Исключения по времени ресурса бронирования</param>
        /// <param name="reservationResourceTimesOfBookingDayOfWeek">Временная сетка по дню недели ресурса бронирования</param>
        /// <param name="affiliateAdditionalTime">Исключения по времени филиала</param>
        /// <param name="affiliateTimesOfBookingDayOfWeek">Временная сетка по дню недели филиала</param>
        public static bool CheckWeekend(DateTime start, DateTime end,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> reservationResourceAdditionalTime,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> reservationResourceTimesOfBookingDayOfWeek,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTime,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBookingDayOfWeek)
        {
            DateTime date;

            // перенесено в цикл ниже
            // если в указанном времени есть хоть один нерабочий день
            // date - хранит начало дня (BeginDate)
            //if (reservationResourceAdditionalTime.Where(x => start < (date = x.Key.Date).AddDays(1) && date < end) // start < [EndDate] AND [BeginDate] < end
            //        .Any(x => x.Value.Count > 0 && !x.Value[0].IsWork))
            //    return true;

            // если в указанном времени есть хоть один нерабочий день недели
            //var checkDayOfWeek = new List<DayOfWeek>();
            date = end != end.Date ? end.Date : end.Date.AddDays(-1);//если время оканчания 00:00, тогда день, на который заезжает дата окончания, не нужно проверять
            for (DateTime d = start.Date; d <= date; d = d.AddDays(1))
            {
                if (reservationResourceAdditionalTime.ContainsKey(d) && reservationResourceAdditionalTime[d].Count > 0)
                {
                    if (!reservationResourceAdditionalTime[d][0].IsWork)
                        return true;
                }
                else
                {
                    if (!reservationResourceTimesOfBookingDayOfWeek.ContainsKey(d.DayOfWeek) ||
                        reservationResourceTimesOfBookingDayOfWeek[d.DayOfWeek].Count == 0)
                        return true;
                }

                //checkDayOfWeek.Add(d.DayOfWeek);
                //if (checkDayOfWeek.Count >= 7)// проверили все 7 дней недели
                //    break;
            }

            var affiliateIsWeekend = AffiliateService.CheckWeekend(start, end, affiliateAdditionalTime,
                affiliateTimesOfBookingDayOfWeek);

            return affiliateIsWeekend;
        }

        /// <summary>
        /// Узконаправленная проверка времени на то, что оно рабочее
        /// <para>Не рекомендуется использовать при общей проверке времени</para>
        /// </summary>
        public static bool CheckDateRangeIsWorkByDayOfWeek(TimeSpan start, TimeSpan end,
            List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBookingDayOfWeek, List<AffiliateTimeOfBooking> affiliateTimesOfBookingDayOfWeek)
        {
            //var dayOfWeek = reservationResourceTimesOfBookingDayOfWeek[0].DayOfWeek;
            //if (reservationResourceTimesOfBookingDayOfWeek.Any(x => x.DayOfWeek != dayOfWeek))
            //    throw new ArgumentException("Список расписанию должне быть по одному дню недели", "reservationResourceTimesOfBookingDayOfWeek");

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            // Вариант вхождения в рабочии интервалы
            TimeSpan? startWorkInterval = null;
            TimeSpan? prev = null;

            for (int index = 0; index < reservationResourceTimesOfBookingDayOfWeek.Count; index++)
            {
                var timeOfBooking = reservationResourceTimesOfBookingDayOfWeek[index];

                if (!startWorkInterval.HasValue)
                    startWorkInterval = timeOfBooking.StartTime;
                if (!prev.HasValue)
                    prev = timeOfBooking.StartTime;

                if (prev != timeOfBooking.StartTime || index == reservationResourceTimesOfBookingDayOfWeek.Count - 1)
                {
                    TimeSpan endWorkInterval;
                    bool isWorkReservationResource;

                    if (prev != timeOfBooking.StartTime && index == reservationResourceTimesOfBookingDayOfWeek.Count - 1)
                    {
                        endWorkInterval = prev.Value;
                        isWorkReservationResource = start >= startWorkInterval && end <= endWorkInterval;
                        if (isWorkReservationResource)
                            return AffiliateService.CheckDateRangeIsWorkByDayOfWeek(start, end, affiliateTimesOfBookingDayOfWeek);

                        startWorkInterval = timeOfBooking.StartTime;
                    }

                    endWorkInterval = index == reservationResourceTimesOfBookingDayOfWeek.Count - 1 ? timeOfBooking.EndTime : prev.Value;

                    isWorkReservationResource = start >= startWorkInterval && end <= endWorkInterval;
                    if (isWorkReservationResource)
                        return AffiliateService.CheckDateRangeIsWorkByDayOfWeek(start, end, affiliateTimesOfBookingDayOfWeek);

                    startWorkInterval = timeOfBooking.StartTime;
                }

                prev = timeOfBooking.EndTime;
            }
            return false;
        }

        #endregion
    }
}
