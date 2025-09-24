using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking.Sms
{
    public class SmsTemplateService
    {
        public static SmsTemplate Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne<SmsTemplate>(
                "SELECT TOP 1 * FROM [Booking].[AffiliateSmsTemplate] WHERE Id = @Id", CommandType.Text,
                GetFromReader, new SqlParameter("@Id", id));
        }

        public static List<SmsTemplate> Get(int affiliateId, BookingStatus status)
        {
            return SQLDataAccess.ExecuteReadList<SmsTemplate>(
                "SELECT * FROM [Booking].[AffiliateSmsTemplate] WHERE AffiliateId = @AffiliateId and Status = @Status",
                CommandType.Text,
                GetFromReader,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@Status", (int) status));
        }

        public static List<SmsTemplate> GetList(int affiliateId)
        {
            return
                SQLDataAccess.ExecuteReadList<SmsTemplate>(
                    "SELECT * FROM [Booking].[AffiliateSmsTemplate] WHERE AffiliateId = @AffiliateId",
                    CommandType.Text, GetFromReader, new SqlParameter("@AffiliateId", affiliateId));
        }

        public static List<int> GetIds(int affiliateId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    "SELECT [Id] FROM [Booking].[AffiliateSmsTemplate] WHERE AffiliateId = @AffiliateId",
                    CommandType.Text, "Id", new SqlParameter("@AffiliateId", affiliateId));
        }

        private static SmsTemplate GetFromReader(SqlDataReader reader)
        {
            return new SmsTemplate
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                AffiliateId = SQLDataHelper.GetInt(reader, "AffiliateId"),
                Status = (BookingStatus) SQLDataHelper.GetInt(reader, "Status"),
                Text = SQLDataHelper.GetString(reader, "Text"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled")
            };
        }

        public static int Add(SmsTemplate template)
        {
            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[AffiliateSmsTemplate] " +
                                                    " ([AffiliateId], [Status], [Text], [Enabled]) " +
                                                    " VALUES (@AffiliateId, @Status, @Text, @Enabled); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@AffiliateId", template.AffiliateId),
                new SqlParameter("@Status", (int) template.Status),
                new SqlParameter("@Text", template.Text ?? (object) DBNull.Value),
                new SqlParameter("@Enabled", template.Enabled)
                );
        }

        public static void Update(SmsTemplate template)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[AffiliateSmsTemplate] SET [AffiliateId] = @AffiliateId, [Status] = @Status, [Text] = @Text, [Enabled] = @Enabled " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", template.Id),
                new SqlParameter("@AffiliateId", template.AffiliateId),
                new SqlParameter("@Status", (int) template.Status),
                new SqlParameter("@Text", template.Text ?? (object) DBNull.Value),
                new SqlParameter("@Enabled", template.Enabled)
                );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateSmsTemplate] WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        #region Help

        public static readonly Dictionary<string,string> Variables = new Dictionary<string, string>()
        {
            { "#BOOKING_ID#", "Номер брони"},
            { "#FIRSTNAME#", "Имя"},
            { "#LASTNAME#", "Фамилия"},
            { "#PATRONYMIC#", "Отчество "},
            { "#FULLNAME#", "ФИО"},
            { "#PHONE#", "Телефон"},
            { "#EMAIL#", "Email"},
            { "#STARTTIME#", "Время начала"},
            { "#ENDTIME#", "Время окончания"},
            { "#STARTDATE#", "Дата начала"},
            { "#ENDDATE#", "Дата окончания"},
            { "#RESERVATIONRESOURCE#", "Ресурс"},
            { "#SUM#", "Сумма"},
            { "#STORE_NAME#", "Название магазина"},
        };

        public static string BuildTemplate(string template, Booking booking)
        {
            if (string.IsNullOrWhiteSpace(template))
                return template;

            var str = template
                    .Replace("#BOOKING_ID#", booking.Id.ToString())
                    .Replace("#FIRSTNAME#", booking.FirstName.Default(string.Empty))
                    .Replace("#LASTNAME#", booking.LastName.Default(string.Empty))
                    .Replace("#PATRONYMIC#", booking.Patronymic.Default(string.Empty))
                    .Replace("#FULLNAME#",
                        string.Format("{0} {1} {2}", booking.LastName, booking.FirstName, booking.Patronymic))
                    .Replace("#PHONE#", booking.Phone)
                    .Replace("#EMAIL#", booking.Email)
                    .Replace("#STARTTIME#", booking.BeginDate.ToShortTimeString())
                    .Replace("#ENDTIME#", booking.EndDate.ToShortTimeString())
                    .Replace("#STARTDATE#", booking.BeginDate.ToShortDateString())
                    .Replace("#ENDDATE#", booking.EndDate.ToShortDateString())
                    .Replace("#SUM#", PriceFormatService.FormatPrice(booking.Sum, booking.BookingCurrency))
                    .Replace("#STORE_NAME#", SettingsMain.ShopName);

            // lazyload fields
            if (str.Contains("#RESERVATIONRESOURCE#"))
                str = str.Replace("#RESERVATIONRESOURCE#",
                    booking.ReservationResource != null ? booking.ReservationResource.Name : string.Empty);

            return str;
        }

        #endregion 
    }
}
