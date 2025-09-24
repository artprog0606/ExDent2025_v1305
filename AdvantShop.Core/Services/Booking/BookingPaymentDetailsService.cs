//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Payment;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingPaymentDetailsService
    {
        public static PaymentDetails GetPaymentDetails(int bookingId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Booking].[PaymentDetails] WHERE BookingId = @BookingId", CommandType.Text,
                reader => new PaymentDetails
                {
                    CompanyName = SQLDataHelper.GetString(reader, "CompanyName"),
                    INN = SQLDataHelper.GetString(reader, "INN"),
                    Phone = SQLDataHelper.GetString(reader, "Phone"),
                    Contract = SQLDataHelper.GetString(reader, "Contract"),
                    IsCashOnDeliveryPayment = SQLDataHelper.GetBoolean(reader, "IsCashOnDeliveryPayment"),
                    IsPickPointPayment = SQLDataHelper.GetBoolean(reader, "IsPickPointPayment")
                },
                new SqlParameter("@BookingId", bookingId));
        }

        public static void AddPaymentDetails(int bookingId, PaymentDetails details)
        {
            if (details != null)
                SQLDataAccess.ExecuteNonQuery(
                    @"INSERT INTO [Booking].[PaymentDetails] ([BookingId],[CompanyName],[INN],[Phone],[Contract],[IsCashOnDeliveryPayment],[IsPickPointPayment])
                      VALUES (@BookingId,@CompanyName,@INN,@Phone,@Contract,@IsCashOnDeliveryPayment,@IsPickPointPayment)",
                    CommandType.Text,
                    new SqlParameter("@BookingId", bookingId),
                    new SqlParameter("@CompanyName", details.CompanyName ?? string.Empty),
                    new SqlParameter("@INN", details.INN ?? string.Empty),
                    new SqlParameter("@Phone", details.Phone ?? string.Empty),
                    new SqlParameter("@Contract", details.Contract ?? string.Empty),
                    new SqlParameter("@IsCashOnDeliveryPayment", details.IsCashOnDeliveryPayment),
                    new SqlParameter("@IsPickPointPayment", details.IsPickPointPayment));
        }

        public static void UpdatePaymentDetails(int bookingId, PaymentDetails details, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (details == null)
                return;

            if (trackChanges)
                BookingHistoryService.TrackChangingPaymentDetails(bookingId, details, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                @"IF(EXISTS(SELECT [BookingId] FROM [Booking].[PaymentDetails] WHERE [BookingId]=@BookingId)) 
                    UPDATE [Booking].[PaymentDetails] SET CompanyName=@CompanyName, INN=@INN, Phone=@phone, Contract=@Contract, IsCashOnDeliveryPayment=@IsCashOnDeliveryPayment, IsPickPointPayment=@IsPickPointPayment WHERE [BookingId]=@BookingId
                ELSE 
                    INSERT INTO [Booking].[PaymentDetails] ([BookingId], CompanyName, INN, Phone, Contract,[IsCashOnDeliveryPayment],[IsPickPointPayment]) VALUES (@BookingId, @CompanyName, @INN, @phone, @Contract,@IsCashOnDeliveryPayment,@IsPickPointPayment) ",
                CommandType.Text,
                new SqlParameter("@BookingId", bookingId),
                new SqlParameter("@CompanyName", details.CompanyName ?? string.Empty),
                new SqlParameter("@INN", details.INN ?? string.Empty),
                new SqlParameter("@Phone", details.Phone ?? string.Empty),
                new SqlParameter("@Contract", details.Contract ?? string.Empty),
                new SqlParameter("@IsCashOnDeliveryPayment", details.IsCashOnDeliveryPayment),
                new SqlParameter("@IsPickPointPayment", details.IsPickPointPayment));
        }

    }
}
