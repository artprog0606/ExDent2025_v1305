//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingCurrencyService
    {
        public static BookingCurrency Get(int bookingId)
        {
            return SQLDataAccess.ExecuteReadOne<BookingCurrency>(
                "SELECT TOP 1 * FROM [Booking].[BookingCurrency] WHERE BookingId = @BookingId", CommandType.Text,
                reader =>
                    new BookingCurrency
                    {
                        BookingId = SQLDataHelper.GetInt(reader, "BookingId"),
                        CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                        CurrencyNumCode = SQLDataHelper.GetInt(reader, "CurrencyNumCode"),
                        CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                        CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                        IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                        EnablePriceRounding = SQLDataHelper.GetBoolean(reader, "EnablePriceRounding"),
                        RoundNumbers = SQLDataHelper.GetFloat(reader, "RoundNumbers"),
                    },
                new SqlParameter("@BookingId", bookingId));
        }

        public static int Add(BookingCurrency bookingcurrency)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                " INSERT INTO [Booking].[BookingCurrency] " +
                " ([BookingId], [CurrencyCode], [CurrencyNumCode], [CurrencyValue], [CurrencySymbol], [IsCodeBefore], [RoundNumbers], [EnablePriceRounding]) " +
                " VALUES (@BookingId, @CurrencyCode, @CurrencyNumCode, @CurrencyValue, @CurrencySymbol, @IsCodeBefore, @RoundNumbers, @EnablePriceRounding)",
                CommandType.Text,
                new SqlParameter("@BookingId", bookingcurrency.BookingId),
                new SqlParameter("@CurrencyCode", bookingcurrency.CurrencyCode),
                new SqlParameter("@CurrencyNumCode", bookingcurrency.CurrencyNumCode),
                new SqlParameter("@CurrencyValue", bookingcurrency.CurrencyValue),
                new SqlParameter("@CurrencySymbol", bookingcurrency.CurrencySymbol),
                new SqlParameter("@IsCodeBefore", bookingcurrency.IsCodeBefore),
                new SqlParameter("@RoundNumbers", bookingcurrency.RoundNumbers),
                new SqlParameter("@EnablePriceRounding", bookingcurrency.EnablePriceRounding)
                );
        }
    }
}
