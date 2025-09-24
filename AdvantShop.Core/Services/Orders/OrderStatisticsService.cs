//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Orders
{
    public class OrderStatisticsService
    {
        private static float? _salesPlan;
        private static float? _profitPlan;

        public static float SalesPlan
        {
            get
            {
                if (_salesPlan != null)
                    return _salesPlan.Value;

                GetProfitPlan();

                return _salesPlan != null ? _salesPlan.Value : 0;
            }
            set { _salesPlan = value; }
        }

        public static float ProfitPlan
        {
            get
            {
                if (_profitPlan != null)
                    return _profitPlan.Value;
                GetProfitPlan();
                return _profitPlan != null ? _profitPlan.Value : 0;
            }
            set { _profitPlan = value; }
        }

        public static Dictionary<DateTime, float> GetOrdersSumByDays(DateTime minDate, DateTime maxDate)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate])) as 'Date', SUM([Sum]*CurrencyValue) as 'Sum'  " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @MinDate and [OrderDate] <= @MaxDate and [PaymentDate] is not null " +
                "GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate]))",
                CommandType.Text,
                "Date", "Sum",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }

        public static Dictionary<DateTime, float> GetOrdersProfitByDays(DateTime minDate, DateTime maxDate)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate])) as 'Date', " +
                "SUM(([Sum] - [ShippingCost] - [TaxCost]) * CurrencyValue) - SUM([SupplyTotal]) as 'Profit' " +
                "FROM[Order].[Order] Inner Join[Order].[OrderCurrency] On[OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @MinDate and [OrderDate] < @MaxDate and [PaymentDate] is not null " +
                "GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate]))",
                CommandType.Text,
                "Date", "Profit",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }


        public static Dictionary<DateTime, float> GetOrdersProfit(
                                                        string group, DateTime fromDate, DateTime toDate,
                                                        bool? onlyPayed = null, int? orderStatusId = null, 
                                                        List<int> orderSourceIds = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', " +
                "SUM(([Sum] - [ShippingCost] - [TaxCost]) * CurrencyValue) - SUM([SupplyTotal] * CurrencyValue) as 'Profit' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +
                (onlyPayed != null
                    ? " and [PaymentDate] is " + ((bool)onlyPayed
                        ? "not"
                        : "") + " null "
                    : "") +
                (orderStatusId != null
                    ? $" and [OrderStatusID] = {orderStatusId} "
                    : "") +
                (orderSourceIds != null && orderSourceIds.Count > 0
                    ? " and [OrderSourceId] in (" + string.Join(",", orderSourceIds) + ") "
                    : "") +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "Profit",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static Dictionary<DateTime, int> GetOrdersCountByPeriod(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, int>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetCountByMonths]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(
                            new DateTime(SQLDataHelper.GetInt(reader, "Year"), SQLDataHelper.GetInt(reader, "Month"), 1),
                            SQLDataHelper.GetInt(reader, "Count"));
                    }
                db.cnClose();
                return sums;
            }
        }


        public static List<KeyValuePair<string, int>> GetTopPayments()
        {
            return SQLDataAccess.ExecuteReadList("[Order].[sp_GetPaymentRating]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "PaymentMethod"),
                                                  SQLDataHelper.GetInt(reader, "Rating")));
        }

        public static List<KeyValuePair<string, int>> GetTopShippings()
        {
            return SQLDataAccess.ExecuteReadList("[Order].[sp_GetShippingRating]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "ShippingMethod"),
                                                  SQLDataHelper.GetInt(reader, "Rating")));
        }

        public static List<KeyValuePair<string, int>> GetTopCities()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT Top(10) [City], Count(*) as Rating " +
                "FROM [Order].[OrderContact] " +
                "Group BY [City] " +
                "Order By Rating Desc",
                CommandType.Text,
                reader =>
                    new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "City"),
                                                  SQLDataHelper.GetInt(reader, "Rating")));
        }

        public static DataTable GetTopCustomersBySumPrice()
        {
            return SQLDataAccess.ExecuteTable(
                "Select Top(10) [CustomerID], [Email], " +
                "(Select top 1 [FirstName]+' '+[LastName] From [Order].[OrderCustomer] as c Where c.[CustomerID] = [OrderCustomer].[CustomerID]) as fio, " +
                "Sum([Order].[Sum]) as Summary " +
                "From [Order].[OrderCustomer] " +
                "Join [Order].[Order] On [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                "Group By [CustomerID], Email " +
                "Order By Summary Desc",
                CommandType.Text);
        }

        public static DataTable GetTopProductsByCount()
        {
            return SQLDataAccess.ExecuteTable(
                "Select Top(10) ProductID, Name, ArtNo, " +
                                "(Select [UrlPath] From [Catalog].[Product] Where [ProductId] = [OrderItems].[ProductID]) as UrlPath, " +
                                "Sum([Amount]) as Summary " +
                "From [Order].[OrderItems] " +
                "Group By [ProductID], [Name], [ArtNo] " +
                "Order By Summary Desc",
                CommandType.Text);
        }

        public static DataTable GetTopProductsBySum()
        {
            return SQLDataAccess.ExecuteTable(
                "Select Top(10) ProductID, Name, ArtNo, " +
                                "(Select [UrlPath] From [Catalog].[Product] Where [ProductId] = [OrderItems].[ProductID]) as UrlPath, " +
                                " Sum([Amount]*[Price]) as Summary " +
                "From [Order].[OrderItems] " +
                "Group By [ProductID], [Name], [ArtNo] " +
                "Order By Summary Desc",
                CommandType.Text);
        }

        public static KeyValuePair<float, float> GetMonthProgress()
        {
            return SQLDataAccess.ExecuteReadOne("[Order].[sp_GetOrdersMonthProgress]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<float, float>(SQLDataHelper.GetFloat(reader, "Sum"),
                                                   SQLDataHelper.GetFloat(reader, "Profit")));
        }

        public static void GetProfitPlan()
        {
            var data = SQLDataAccess.ExecuteReadOne("[Settings].[sp_GetLastProfitPlan]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<float?, float?>(SQLDataHelper.GetNullableFloat(reader, "SalesPlan"),
                                                     SQLDataHelper.GetNullableFloat(reader, "ProfitPlan")));

            SalesPlan = data.Key ?? 0;
            ProfitPlan = data.Value ?? 0;
        }

        public static void SetProfitPlan(float sales, float profit)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_SetPlan]", CommandType.StoredProcedure,
                                            new SqlParameter("@SalesPlan", sales),
                                            new SqlParameter("@ProfitPlan", profit));
            GetProfitPlan();
        }


        public static Dictionary<DateTime, float> GetOrdersSum(string group, DateTime fromDate, DateTime toDate,
                                                                bool? onlyPayed = null, int? orderStatusId = null,
                                                                bool useShippingCost = true, List<int> orderSourceIds = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', SUM(([Sum]" + (!useShippingCost ? " - [ShippingCost]" : "") + ")*CurrencyValue) as 'Sum' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +
                (orderSourceIds != null && orderSourceIds.Count > 0 ? " and [OrderSourceId] in (" + string.Join(",", orderSourceIds) + ")" : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "Sum",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static Dictionary<DateTime, float> GetOrdersCount(string group, DateTime fromDate, DateTime toDate,
                                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Count([OrderID]) as Count " +
                "FROM [Order].[Order] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "Count",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static int GetOrdersCount(DateTime fromDate, DateTime toDate, bool? onlyPayed = null, bool isCompleteStatus = false)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "Select Count([OrderID]) as Count " +
                "FROM [Order].[Order] " +
                (isCompleteStatus ? " Left Join [Order].[OrderStatus] On [Order].[OrderStatusID] = [OrderStatus].[OrderStatusID] " : "") +

                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (isCompleteStatus ? " and [IsCompleted] = 1" : ""),

                CommandType.Text,
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate)));
        }

        public static Dictionary<DateTime, float> GetOrdersReg(string group, DateTime fromDate, DateTime toDate, bool isRegistered,
                                                                bool? onlyPayed = null, int? orderStatusId = null,
                                                                int? orderSourceId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Count([Order].[OrderID]) as Count " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCustomer] On [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                "Left Join [Customers].[Customer] On [Customer].[CustomerID] = [OrderCustomer].[CustomerID] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (isRegistered ? "and [Customer].[Email] is not null " : "and [Customer].[Email] is null ") +
                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +
                (orderSourceId != null ? " and [OrderSourceId] = " + orderSourceId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "Count",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static Dictionary<DateTime, float> GetOrdersAvgCheck(string group, DateTime fromDate, DateTime toDate,
                                                                    bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Round(SUM([Sum]*CurrencyValue)/Count([Order].[OrderID]), 2) as 'AvgSum' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "AvgSum",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static float GetOrdersAvgCheckValue(string group, DateTime fromDate, DateTime toDate,
                                                    bool? onlyPayed = null, int? orderStatusId = null)
        {
            return
                SQLDataHelper.GetFloat(SQLDataAccess.ExecuteScalar(
                    "Select Avg([Sum]*CurrencyValue) " +
                    "FROM [Order].[Order] " +
                    "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                    "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                    (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                    (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : ""),

                    CommandType.Text,
                    new SqlParameter("@fromDate", fromDate),
                    new SqlParameter("@toDate", toDate)));
        }

        public static List<StatisticsDataItem> GetOrdersAvgCheckByCity(string group, DateTime fromDate, DateTime toDate,
                                                                    bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess
                .Query<StatisticsDataItem>(
                    "Select City as Name, SUM([Sum]*CurrencyValue)/Count([Order].[OrderID]) as Sum, Count([Order].[OrderID]) as Count " +
                    "FROM [Order].[Order] " +
                    "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                    "LEFT JOIN [Order].[OrderCustomer] On [Order].[OrderID] = [OrderCustomer].[OrderId]" +
                    "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                    (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                    (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                    "GROUP BY City",
                    new { fromDate, toDate })
                .ToList();
        }

        public static List<StatisticsDataItem> GetPayments(DateTime fromDate, DateTime toDate,
                                                            bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.Query<StatisticsDataItem>(
                "Select [PaymentMethod].[Name] as Name, Count([Order].[OrderID]) as Count, Sum([Sum]*CurrencyValue) as Sum " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[PaymentMethod] on [PaymentMethod].[PaymentMethodID] = [Order].[PaymentMethodID] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY [PaymentMethod].[Name]",
                new { fromDate, toDate }).ToList();
        }

        public static List<StatisticsDataItem> GetShippings(
                                                    DateTime fromDate, DateTime toDate,
                                                    bool? onlyPayed = null, int? orderStatusId = null,
                                                    DateTime? minDeliveryDate = null, DateTime? maxDeliveryDate = null)
        {
            return SQLDataAccess
                .Query<StatisticsDataItem>(
                    "Select isnull((select [Name] FROM [Order].[ShippingMethod] WHERE [ShippingMethodID] = [Order].[ShippingMethodID]), '') as Name, Count([Order].[OrderId]) as Count, Sum([Sum]*CurrencyValue) as Sum " +
                    "FROM [Order].[Order]  " +
                    "Left join [Order].[ShippingMethod] On [ShippingMethod].[ShippingMethodID] = [Order].[ShippingMethodID] " +
                    "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                    "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                    (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                    (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +
                    (minDeliveryDate.HasValue ? " and [DeliveryDate] >= @minDeliveryDate " : "") +
                    (maxDeliveryDate.HasValue ? " and [DeliveryDate] <= @maxDeliveryDate " : "") +
                    " GROUP BY [Order].[ShippingMethodID]  ",
                    new { fromDate, toDate, minDeliveryDate, maxDeliveryDate })
                .ToList();
        }

        public static List<StatisticsDataItem> GetShippingsGroupedByName(
                                                    DateTime fromDate, DateTime toDate,
                                                    bool? onlyPayed = null,
                                                    int? orderStatusId = null,
                                                    DateTime? minDeliveryDate = null, DateTime? maxDeliveryDate = null)
        {
            return SQLDataAccess
                .Query<StatisticsDataItem>(
                    "SELECT ISNULL([ShippingMethodName], '') AS Name, COUNT([Order].[OrderId]) AS Count, SUM([Sum]*CurrencyValue) AS Sum " +
                    "FROM [Order].[Order]  " +
                    "INNER JOIN [Order].[OrderCurrency] ON [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                    "WHERE IsDraft = 0 AND [OrderDate] >= @fromDate AND [OrderDate] <= @toDate " +
                    (onlyPayed != null ? (" AND [PaymentDate] IS " + ((bool)onlyPayed ? "NOT" : "") + " NULL ") : "") +
                    (orderStatusId != null ? " AND [OrderStatusID] = " + orderStatusId : "") +
                    (minDeliveryDate.HasValue ? " AND [DeliveryDate] >= @minDeliveryDate " : "") +
                    (maxDeliveryDate.HasValue ? " AND [DeliveryDate] <= @maxDeliveryDate " : "") +
                    " GROUP BY [Order].[ShippingMethodName]  ",
                    new { fromDate, toDate, minDeliveryDate, maxDeliveryDate })
                .ToList();
        }

        public static Dictionary<string, float> GetOrdersByOrderType(DateTime fromDate, DateTime toDate,
                                                            bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<string, float>(
                "Select isnull([OrderSource].[Name], '') as OrderType, Count([OrderId]) as Count " +
                "FROM [Order].[Order]  " +
                "Left Join [Order].[OrderSource] on [OrderSource].[Id] = [Order].[OrderSourceId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY [OrderSource].[Name] ",
                CommandType.Text,
                "OrderType", "Count",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static Dictionary<string, float> GetOrdersByStatus(DateTime fromDate, DateTime toDate,
                                                                  bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<string, float>(
                "Select (Select StatusName From [Order].[OrderStatus] Where [OrderStatus].OrderStatusID = [Order].[OrderStatusID]) as StatusName, Count([Order].[OrderId]) as Count " +
                "FROM [Order].[Order]  " +
                "Left Join [Order].[OrderStatus] On [Order].[OrderStatusID] = [OrderStatus].[OrderStatusID] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [Order].[OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY [Order].[OrderStatusID]  ",
                CommandType.Text,
                "StatusName", "Count",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static Dictionary<DateTime, float> GetRepeatOrders(string group, DateTime fromDate, DateTime toDate,
                                                                  bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Count([Order].[OrderID]) as 'RepeatCount' " +
                "FROM [Order].[Order] " +
                "Left Join [Order].[OrderCustomer] On [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +
                "and (Select Count(oc.CustomerId) From [Order].[OrderCustomer] as oc Where oc.CustomerID = [OrderCustomer].[CustomerId]) > 1 " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "RepeatCount",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static List<AbcXyzAnalysisData> GetAbcXyzOrderItems(DateTime minDate, DateTime maxDate)
        {
            return
                SQLDataAccess.Query<AbcXyzAnalysisData>(
                    "Select ArtNo, Sum(Price*Amount*CurrencyValue) as PriceSum " +
                    "FROM [Order].[Order] " +
                    "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                    "Inner Join [Order].[OrderItems] On [OrderItems].[OrderId] = [Order].[OrderId] " +
                    "WHERE IsDraft = 0 and [OrderDate] >= @MinDate and [OrderDate] <= @MaxDate and [PaymentDate] is not null " +
                    "GROUP BY ArtNo",
                    new { minDate, maxDate }).ToList();
        }

        public static List<RfmAnalysisDataItem> GetRfmAnalysisDataItems()
        {
            return
                SQLDataAccess.Query<RfmAnalysisDataItem>(
                    ";with Customers as (Select Distinct CustomerId From [Order].[OrderCustomer] Left Join [Order].[Order] On [Order].[OrderId] = [OrderCustomer].[OrderId] Where [PaymentDate] is not null) " +
                    "Select CustomerId, " +
                    "  (Select top(1) OrderDate From [Order].[OrderCustomer] as oc Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null Order By OrderDate Desc) as LastOrderDate, " +
                    "  (Select Count([Order].OrderId) From [Order].[OrderCustomer] as oc Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null) as OrdersCount, " +
                    "  (Select Sum([Sum]*CurrencyValue) From [Order].[OrderCustomer] as oc Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = oc.[OrderId] Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null) as OrdersSum " +
                    "From Customers").ToList();
        }

        public static List<RfmAnalysisDataItem> GetRfmAnalysisDataItems(DateTime from, DateTime to)
        {
            return
                SQLDataAccess.Query<RfmAnalysisDataItem>(
                    ";with Customers as (Select Distinct CustomerId From [Order].[OrderCustomer] " +
                    "Left Join [Order].[Order] On [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                    "Where [PaymentDate] is not null and OrderDate >= @from and OrderDate <= @to) " +
                    "Select CustomerId, " +
                    "  (Select top(1) Number From [Order].[OrderCustomer] as oc Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null Order By OrderDate Desc) as LastOrderNumber, " +
                    "  (Select top(1) OrderDate From [Order].[OrderCustomer] as oc Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null Order By OrderDate Desc) as LastOrderDate, " +
                    "  (Select Count([Order].OrderId) From [Order].[OrderCustomer] as oc Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null) as OrdersCount, " +
                    "  (Select Sum([Sum]*CurrencyValue) From [Order].[OrderCustomer] as oc Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = oc.[OrderId] Left Join [Order].[Order] On [Order].[OrderId] = oc.[OrderId] Where oc.CustomerId = Customers.CustomerId and [PaymentDate] is not null) as OrdersSum " +
                    "From Customers",
                    new { from, to }).ToList();
        }

        public static Dictionary<DateTime, float> GetCallsCount(string group, ECallType type, DateTime fromDate, DateTime toDate)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [CallDate]), 0) as 'Date', Count(Id) as 'CallCount' " +
                "FROM [Customers].[Call] " +
                "WHERE [CallDate] >= @fromDate and [CallDate] <= @toDate and [Type] = @Type " +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [CallDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "CallCount",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate),
                new SqlParameter("@Type", type.ToString()));
        }

        public static Dictionary<DateTime, float> GetAvgDurationOfCalls(string group, DateTime fromDate, DateTime toDate)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [CallDate]), 0) as 'Date', Avg(Duration) as 'Duration' " +
                "FROM [Customers].[Call] " +
                "WHERE [CallDate] >= @fromDate and [CallDate] <= @toDate " +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [CallDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "Duration",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }

        public static Dictionary<DateTime, float> GetProductSumStatistics(
                                                        string group, DateTime fromDate, DateTime toDate, int productId, 
                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Sum(Price*Amount*CurrencyValue) as 'OrdersSum' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Order].[OrderItems] On [OrderItems].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate and [OrderItems].[ProductId]=@ProductId " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "OrdersSum",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate),
                new SqlParameter("@ProductId", productId));
        }

        public static Dictionary<DateTime, float> GetProductCountStatistics(
                                                        string group, DateTime fromDate, DateTime toDate, int productId, 
                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Sum(Amount) as 'OrdersProductAmount' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Order].[OrderItems] On [OrderItems].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate and [OrderItems].[ProductId]=@ProductId " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "OrdersProductAmount",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate),
                new SqlParameter("@ProductId", productId));
        }

        public static Dictionary<DateTime, float> GetOfferSumStatistics(
                                                        string group, DateTime fromDate, DateTime toDate, int offerId, 
                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Sum([OrderItems].Price*[OrderItems].Amount*CurrencyValue) as 'OrdersSum' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Order].[OrderItems] On [OrderItems].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Catalog].[Offer] ON [Offer].[ArtNo] = [OrderItems].[ArtNo] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate and [Offer].[OfferId]=@OfferId " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "OrdersSum",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate),
                new SqlParameter("@OfferId", offerId));
        }

        public static Dictionary<DateTime, float> GetOfferCountStatistics(
                                                        string group, DateTime fromDate, DateTime toDate, int offerId, 
                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Sum([OrderItems].Amount) as 'OrdersProductAmount' " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Order].[OrderItems] On [OrderItems].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Catalog].[Offer] ON [Offer].[ArtNo] = [OrderItems].[ArtNo] " +
                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate and [Offer].[OfferId]=@OfferId " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "OrdersProductAmount",
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate),
                new SqlParameter("@OfferId", offerId));
        }

        public static float GetOrdersSum(Guid customerId, bool paid = false)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "Select IsNull(SUM([Sum]*CurrencyValue), 0) FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Order].[OrderCustomer] On [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderCustomer].[CustomerId] = @CustomerId" +
                (paid ? " and [PaymentDate] is not null" : ""),
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }

        public static float GetOrdersCount(Guid customerId, bool paid = false)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "Select Count([Order].[OrderId]) FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "Inner Join [Order].[OrderCustomer] On [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 and [OrderCustomer].[CustomerId] = @CustomerId" +
                (paid ? " and [PaymentDate] is not null" : ""),
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }

        public static float GetCustomersCountWithMoreOneOrder(DateTime orderDateFrom, DateTime orderDateTo)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "SELECT COUNT(*) FROM [Customers].[Customer] " +
                "WHERE EXISTS ( " +
                    "SELECT 1 FROM [Order].[Order] " +
                    "INNER JOIN [Order].[OrderCustomer] ON [OrderCustomer].[OrderId] = [Order].[OrderID] " +
                    "WHERE [OrderCustomer].[CustomerID] = [Customers].[Customer].[CustomerID] " +
                    "AND  [Order].[OrderDate] >= @from AND [Order].[OrderDate] <= @to " +
                    "HAVING COUNT(*) > 1 " +
                ")",
                CommandType.Text,
                new SqlParameter("@from", SqlDbType.DateTime) { Value = orderDateFrom },
                new SqlParameter("@to", SqlDbType.DateTime) { Value = orderDateTo });
        }

        public static float GetLifetimeValue(DateTime from, DateTime to)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "Select ISNULL(Sum([Order].Sum)/Count([Customers].[Customer].CustomerID),0) FROM [Customers].[Customer] " +
                "Inner Join [Order].[OrderCustomer] On [OrderCustomer].[CustomerID] = [Customer].[CustomerID] and  Customer.RegistrationDateTime >= @from and Customer.RegistrationDateTime <= @to " +
                "Inner Join [Order].[Order] On [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                "Where [Order].PaymentDate is not null and [Order].PaymentDate >= @from and [Order].PaymentDate <= @to ",
                CommandType.Text,
                new SqlParameter("@from", from),
                new SqlParameter("@to", to));
        }

        public static OrdersStatisticsDto GetOrdersStatistics(
                                                DateTime fromDate, DateTime toDate,
                                                bool? onlyPayed = null,
                                                bool isCompleteStatus = false, List<int> orderSourceIds = null)
        {
            return SQLDataAccess
                .Query<OrdersStatisticsDto>(
                    "Select Count(o.[OrderID]) as OrdersCount, " +
                    "Sum([Sum]*CurrencyValue) as OrdersSum, " +
                    "Round(SUM([Sum]*CurrencyValue)/Count(o.[OrderID]), 2) as AvgCheck, " +
                    "SUM(([Sum] - [ShippingCost] - [TaxCost]) * CurrencyValue) - SUM([SupplyTotal] * CurrencyValue) AS Profit " +
                    "FROM [Order].[Order] as o " +
                    "Inner Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = o.[OrderId] " +
                    (
                        isCompleteStatus
                            ? " Left Join [Order].[OrderStatus] On [Order].[OrderStatusID] = [OrderStatus].[OrderStatusID] "
                            : ""
                    ) +
                    "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                    (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                    (isCompleteStatus ? " and [IsCompleted] = 1" : "") +
                    (orderSourceIds != null && orderSourceIds.Count > 0 ? " and [OrderSourceId] in (" + string.Join(",", orderSourceIds) + ")" : ""),

                    new { fromDate, toDate })
                .FirstOrDefault();
        }

        public static float GetOrdersStatisticsProductsCount(
                                    DateTime fromDate, DateTime toDate, bool? onlyPayed = null, 
                                    bool isCompleteStatus = false, List<int> orderSourceIds = null)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "Select IsNull(Sum(oi.Amount), 0) as ProductsCount " +
                "FROM [Order].[Order] as o " +
                "Inner Join [Order].[OrderItems] as oi On oi.[OrderId] = o.[OrderID] " +
                (isCompleteStatus
                    ? " Left Join [Order].[OrderStatus] On [Order].[OrderStatusID] = [OrderStatus].[OrderStatusID] "
                    : "") +

                "WHERE IsDraft = 0 and [OrderDate] >= @fromDate and [OrderDate] <= @toDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool) onlyPayed ? "not" : "") + " null ") : "") +
                (isCompleteStatus ? " and [IsCompleted] = 1" : "") +
                (orderSourceIds != null && orderSourceIds.Count > 0
                    ? " and [OrderSourceId] in (" + string.Join(",", orderSourceIds) + ")"
                    : ""),

                CommandType.Text,
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate));
        }
    }
}