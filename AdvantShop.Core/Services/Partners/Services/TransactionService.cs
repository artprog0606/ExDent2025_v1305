using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Partners
{
    public class TransactionService
    {
        /// <summary>
        /// Транзакции за период, включая даты диапазона
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static List<Transaction> GetTransactions(int partnerId, DateTime dateFrom, DateTime dateTo)
        {
            return SQLDataAccess.ExecuteReadList<Transaction>(
                "SELECT t.*, tc.* FROM [Partners].[Transaction] t INNER JOIN [Partners].[TransactionCurrency] tc ON t.Id = tc.TransactionId " +
                "WHERE t.PartnerId = @PartnerId AND t.DateCreated >= @DateFrom AND DateCreated < @DateTo",
                CommandType.Text, GetFromReader, 
                new SqlParameter("@PartnerId", partnerId),
                new SqlParameter("@DateFrom", dateFrom.Date),
                new SqlParameter("@DateTo", dateTo.AddDays(1).Date));
        }

        public static Transaction GetTransaction(int id)
        {
            return SQLDataAccess.ExecuteReadOne<Transaction>(
                "SELECT t.*, tc.* FROM [Partners].[Transaction] t INNER JOIN [Partners].[TransactionCurrency] tc ON t.Id = tc.TransactionId WHERE t.Id = @Id",
                CommandType.Text, GetFromReader,
                new SqlParameter("@Id", id));
        }

        private static Transaction GetFromReader(SqlDataReader reader)
        {
            return new Transaction
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                PartnerId = SQLDataHelper.GetInt(reader, "PartnerId"),
                Balance = SQLDataHelper.GetDecimal(reader, "Balance"),
                Amount = SQLDataHelper.GetDecimal(reader, "Amount"),
                Basis = SQLDataHelper.GetString(reader, "Basis"),
                CustomerId = SQLDataHelper.GetNullableGuid(reader, "CustomerId"),
                OrderId = SQLDataHelper.GetNullableInt(reader, "OrderId"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                IsRewardPayout = SQLDataHelper.GetBoolean(reader, "IsRewardPayout"),
                RewardPeriodTo = SQLDataHelper.GetNullableDateTime(reader, "RewardPeriodTo"),
                DetailsJson = SQLDataHelper.GetString(reader, "DetailsJson"),
                Currency = new TransactionCurrency
                {
                    CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                    CurrencyNumCode = SQLDataHelper.GetInt(reader, "CurrencyNumCode"),
                    CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                    CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                    IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                    RoundNumbers = SQLDataHelper.GetFloat(reader, "RoundNumbers"),
                    EnablePriceRounding = SQLDataHelper.GetBoolean(reader, "EnablePriceRounding"),
                }
            };
        }

        public static void AddTransaction(Transaction transaction)
        {
            if (transaction.Currency == null)
                throw new ArgumentNullException("transaction.Currency");

            transaction.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Partners].[Transaction] (PartnerId, Balance, Amount, Basis, CustomerId, OrderId, DateCreated, IsRewardPayout, RewardPeriodTo, DetailsJson) " +
                "VALUES (@PartnerId, @Balance, @Amount, @Basis, @CustomerId, @OrderId, @DateCreated, @IsRewardPayout, @RewardPeriodTo, @DetailsJson); SELECT SCOPE_IDENTITY();", CommandType.Text,
                new SqlParameter("@PartnerId", transaction.PartnerId),
                new SqlParameter("@Balance", transaction.Balance),
                new SqlParameter("@Amount", transaction.Amount),
                new SqlParameter("@Basis", transaction.Basis ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", transaction.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@OrderId", transaction.OrderId ?? (object)DBNull.Value),
                new SqlParameter("@DateCreated", DateTime.Now),
                new SqlParameter("@IsRewardPayout", transaction.IsRewardPayout),
                new SqlParameter("@RewardPeriodTo", transaction.RewardPeriodTo ?? (object)DBNull.Value),
                new SqlParameter("@DetailsJson", transaction.DetailsJson ?? (object)DBNull.Value));

            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Partners].[TransactionCurrency] (TransactionId, CurrencyCode, CurrencyNumCode, CurrencyValue, CurrencySymbol, IsCodeBefore, RoundNumbers, EnablePriceRounding) " +
                "VALUES (@TransactionId, @CurrencyCode, @CurrencyNumCode, @CurrencyValue, @CurrencySymbol, @IsCodeBefore, @RoundNumbers, @EnablePriceRounding)", CommandType.Text,
                new SqlParameter("@TransactionId", transaction.Id),
                new SqlParameter("@CurrencyCode", transaction.Currency.CurrencyCode),
                new SqlParameter("@CurrencyNumCode", transaction.Currency.CurrencyNumCode),
                new SqlParameter("@CurrencyValue", transaction.Currency.CurrencyValue),
                new SqlParameter("@CurrencySymbol", transaction.Currency.CurrencySymbol),
                new SqlParameter("@IsCodeBefore", transaction.Currency.IsCodeBefore),
                new SqlParameter("@RoundNumbers", transaction.Currency.RoundNumbers),
                new SqlParameter("@EnablePriceRounding", transaction.Currency.EnablePriceRounding));
        }


        /// <summary>
        /// Последняя выплата вознаграждения
        /// </summary>
        public static Transaction GetLastRewardPayout(int partnerId)
        {
            return SQLDataAccess.Query<Transaction>(
                "SELECT TOP(1) * FROM [Partners].[Transaction] WHERE PartnerId = @PartnerId AND IsRewardPayout = 1 ORDER BY DateCreated DESC",
                new { partnerId }).FirstOrDefault();
        }

        /// <summary>
        /// Накопленное вознаграждение за период 
        /// </summary>
        /// <param name="partnerId">ID партнера</param>
        /// <param name="dateFrom">с даты (входит в диапазон)</param>
        /// <param name="dateTo">до даты (входит в диапазон)</param>
        public static decimal GetRewardAmount(int partnerId, DateTime? dateFrom = null, DateTime? dateTo = null, Guid? customerId = null)
        {
            var sqlParams = new List<SqlParameter> { new SqlParameter("@PartnerId", partnerId) };
            if (dateFrom.HasValue)
                sqlParams.Add(new SqlParameter("@DateFrom", dateFrom.Value.Date));
            if (dateTo.HasValue)
                sqlParams.Add(new SqlParameter("@DateTo", dateTo.Value.Date.AddDays(1)));
            if (customerId.HasValue)
                sqlParams.Add(new SqlParameter("@CustomerId", customerId.Value));

            var transactions = SQLDataAccess.ExecuteReadList<Transaction>(
                "SELECT t.*, tc.* FROM [Partners].[Transaction] t INNER JOIN [Partners].[TransactionCurrency] tc ON t.Id = tc.TransactionId " +
                "WHERE t.PartnerId = @PartnerId AND t.IsRewardPayout = 0 " +
                (dateFrom.HasValue ? " AND t.DateCreated >= @DateFrom" : string.Empty) +
                (dateTo.HasValue ? " AND t.DateCreated < @DateTo" : string.Empty) +
                (customerId.HasValue ? " AND t.CustomerId = @CustomerId" : string.Empty),
                CommandType.Text, GetFromReader, sqlParams.ToArray());
            return transactions.Sum(x => x.RoundedBaseAmount);
        }

        /// <summary>
        /// Сумма выплат вознаграждений
        /// </summary>
        public static decimal GetRewardPayoutSum(int partnerId)
        {
            var transactions = SQLDataAccess.ExecuteReadList<Transaction>(
                "SELECT t.*, tc.* FROM [Partners].[Transaction] t INNER JOIN [Partners].[TransactionCurrency] tc ON t.Id = tc.TransactionId " +
                "WHERE t.PartnerId = @PartnerId AND t.IsRewardPayout = 1",
                CommandType.Text, GetFromReader, new SqlParameter("@PartnerId", partnerId));
            return -transactions.Sum(x => x.RoundedBaseAmount); // выплаты с отрицательной суммой
        }


        /// <summary>
        /// сумма товаров в оплаченных покупателями партнера заказах, с которых начислено вознаграждение
        /// </summary>
        public static decimal GetPaidOrderItemsSum(int partnerId, DateTime? dateFrom = null, DateTime? dateTo = null, Guid? customerId = null)
        {
            var sqlParams = new List<SqlParameter> { new SqlParameter("@PartnerId", partnerId) };
            if (dateFrom.HasValue)
                sqlParams.Add(new SqlParameter("@DateFrom", dateFrom.Value.Date));
            if (dateTo.HasValue)
                sqlParams.Add(new SqlParameter("@DateTo", dateTo.Value.Date.AddDays(1)));
            if (customerId.HasValue)
                sqlParams.Add(new SqlParameter("@CustomerId", customerId.Value));
            return SQLDataAccess.ExecuteScalar<decimal>(
                "SELECT ISNULL(SUM((o.[Sum] - o.ShippingCost - o.PaymentCost) * oCurr.CurrencyValue), 0) " +
                "FROM [Order].[Order] o " +
                    "INNER JOIN Partners.[Transaction] t ON t.OrderId = o.OrderId " +  // есть начисления с заказа
                    "INNER JOIN [Order].OrderCustomer oCust ON o.OrderId = oCust.OrderId " +
                    "INNER JOIN [Order].OrderCurrency oCurr on oCurr.OrderID = o.OrderID " +
                    "INNER JOIN Partners.BindedCustomer bc ON bc.CustomerId = oCust.CustomerId " +
                "WHERE bc.PartnerId = @PartnerId and o.PaymentDate is not null" +
                (dateFrom.HasValue ? " AND t.DateCreated >= @DateFrom" : string.Empty) +
                (dateTo.HasValue ? " AND t.DateCreated < @DateTo" : string.Empty) +
                (customerId.HasValue ? " AND t.CustomerId = @CustomerId" : string.Empty),
                CommandType.Text,
                sqlParams.ToArray());
        }


        public static bool PartnerHasTransactions(int partnerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Partners].[Transaction] WHERE [PartnerId] = @PartnerId", CommandType.Text, 
                new SqlParameter("@PartnerId", partnerId)) > 0;
        }

        public static bool CustomerHasTransactions(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Partners].[Transaction] WHERE [CustomerId] = @CustomerId", CommandType.Text,
                new SqlParameter("@CustomerId", customerId)) > 0;
        }

        public static bool OrderHasTransaction(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Partners].[Transaction] WHERE [OrderId] = @OrderId", CommandType.Text,
                new SqlParameter("@OrderId", orderId)) > 0;
        }
    }
}
