//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using Newtonsoft.Json;
using RestSharp;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Statistic
{
    public enum EGroupDateBy
    {
        Day,
        Week,
        Month
    }

    public class StatisticService
    {
        private const int SearchTermColLength = 250;

        #region Search statistic

        public static void AddSearchStatistic(string request, string searchTerm, string description, int resultCount, Guid customerId)
        {
            if (DebugMode.IsDebugMode(eDebugMode.CriticalCss))
            {
                return;   
            }

            searchTerm = searchTerm.Reduce(SearchTermColLength);
            SQLDataAccess.ExecuteNonQuery(@"IF (SELECT COUNT(ID) FROM [Statistic].[SearchStatistic] WHERE SearchTerm = @SearchTerm AND Description = @Description AND CustomerID = @CustomerID) = 0 Begin
                INSERT INTO [Statistic].[SearchStatistic] ([Request],[ResultCount],[Date],[SearchTerm],[Description],[CustomerID]) VALUES (@Request, @Resultcount, GETDATE(), @SearchTerm, @Description,@CustomerID) end ",
                CommandType.Text,
                new SqlParameter("@Request", request),
                new SqlParameter("@Resultcount", resultCount),
                new SqlParameter("@SearchTerm", searchTerm),
                new SqlParameter("@Description", description),
                new SqlParameter("@CustomerID", customerId));
        }

        public static void DeleteExpiredSearchStatistic(DateTime date)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From  [Statistic].[SearchStatistic] Where Date < @Date",
                CommandType.Text, 60*2, new SqlParameter("@Date", date));
        }

        #endregion

        #region Common statistic

        public static int GetOrdersCountByDateRange(DateTime? fromDate, DateTime? toDate, bool onlyPaied = false, bool notCanceled = true)
        {
            return SQLDataAccess2.ExecuteScalar<int>(
                "SELECT COUNT(OrderID) " +
                "FROM [Order].[Order] INNER JOIN [Order].OrderStatus ON OrderStatus.OrderStatusID = [Order].OrderStatusID " +
                "WHERE IsDraft = 0 " + 
                (notCanceled ? " AND OrderStatus.IsCanceled = 0" : "") +
                (onlyPaied ? " AND [PaymentDate] IS NOT NULL" : "") +
                (fromDate.HasValue ? " AND OrderDate >= @fromDate" : "") +
                (toDate.HasValue ? " AND OrderDate < @toDate" : ""),
                new 
                { 
                    fromDate = fromDate.HasValue ? fromDate.Value.Date : (DateTime?)null,
                    toDate = toDate.HasValue ? toDate.Value.Date.AddDays(1) : (DateTime?)null,
                });
        }

        public static float GetOrdersSumByDateRange(DateTime? fromDate, DateTime? toDate, bool onlyPaied = false, bool notCanceled = true)
        {
            return SQLDataAccess2.ExecuteScalar<float>(
                "SELECT isnull(Sum([Order].[Sum] * [OrderCurrency].[CurrencyValue]), 0) " +
                "FROM [Order].[Order] INNER JOIN [Order].OrderStatus ON OrderStatus.OrderStatusID = [Order].OrderStatusID " +
                "Left Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0" +
                (onlyPaied ? " AND [PaymentDate] IS NOT NULL" : "") +
                (notCanceled ? " AND OrderStatus.IsCanceled = 0" : "") +
                (fromDate.HasValue ? " AND OrderDate >= @fromDate" : "") +
                (toDate.HasValue ? " AND OrderDate < @toDate" : ""),
                new
                {
                    fromDate = fromDate.HasValue ? fromDate.Value.Date : (DateTime?)null,
                    toDate = toDate.HasValue ? toDate.Value.Date.AddDays(1) : (DateTime?)null,
                });
        }

        public static int GetOrdersCountByDate(DateTime date, bool notCanceled = true)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(OrderID) FROM [Order].[Order] INNER JOIN[Order].OrderStatus ON OrderStatus.OrderStatusID = [Order].OrderStatusID WHERE IsDraft = 0 " +
                (notCanceled ? " AND OrderStatus.IsCanceled = 0" : "") +
                "and Convert(date, [OrderDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date));
        }


        public static float GetOrdersSumByDate(DateTime date, bool onlyPaied = false, bool notCanceled = true)
        {
            var cmd = 
                "SELECT isnull(Sum([Order].[Sum] * [OrderCurrency].[CurrencyValue]), 0) FROM [Order].[Order] " +
                "INNER JOIN[Order].OrderStatus ON OrderStatus.OrderStatusID = [Order].OrderStatusID " +
                "Left Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 " +
                (onlyPaied ? " AND [PaymentDate] IS NOT NULL" : "") +
                (notCanceled ? " AND OrderStatus.IsCanceled = 0" : "") +
                " AND Convert(date, [OrderDate]) = Convert(date, @Date)";

            return SQLDataAccess.ExecuteScalar<float>(cmd, CommandType.Text, new SqlParameter("@Date", date));
        }


        public static int GetOrdersCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(OrderID) FROM [Order].[Order] Where IsDraft = 0",
                CommandType.Text);
        }


        public static int GetProductsCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(ProductID) FROM [Catalog].[Product]",
                CommandType.Text);
        }

        /// <summary>
        /// get orders with order status @default order status@
        /// </summary>
        /// <returns>orders count</returns>
        public static int GetLastOrdersCount(int? managerId = null, int? orderSourceId = null)
        {
            var sql = "SELECT COUNT(OrderID) FROM [Order].[Order] WHERE IsDraft = 0 and OrderStatusID = (SELECT OrderStatusID FROM [Order].OrderStatus WHERE IsDefault = 1)" +
                    (orderSourceId.HasValue ? " AND OrderSourceId = @OrderSourceId" : null);
            if (managerId == null)
                return SQLDataAccess.ExecuteScalar<int>(sql, CommandType.Text, new SqlParameter("@OrderSourceId", orderSourceId ?? (object)DBNull.Value));

            switch (SettingsManager.ManagersOrderConstraint)
            {
                case ManagersOrderConstraint.Assigned:
                    sql += " and (ManagerId = @ManagerId)";
                    break;

                case ManagersOrderConstraint.AssignedAndFree:
                    sql += " and (ManagerId = @ManagerId or ManagerId is null)";
                    break;
            }
            return SQLDataAccess.ExecuteScalar<int>(sql, CommandType.Text, 
                new SqlParameter("@ManagerId", managerId), 
                new SqlParameter("@OrderSourceId", orderSourceId ?? (object)DBNull.Value));
        }


        /// <summary>
        /// get reviews count
        /// </summary>
        /// <returns></returns>
        public static int GetReviewsCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(ReviewID) FROM [CMS].[Review]",
                CommandType.Text);
        }

        /// <summary>
        /// get last reviews count
        /// </summary>
        /// <returns></returns>
        public static int GetLastReviewsCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(ReviewID) FROM [CMS].[Review] WHERE [Checked] = 0",
                CommandType.Text);
        }

        #endregion

        #region Leads

        public static int GetLeadsCountByDateRange(DateTime fromDate, DateTime toDate)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Order].[Lead] WHERE Convert(date, @FromDate) <= Convert(date, [CreatedDate]) AND Convert(date, [CreatedDate]) <=  Convert(date, @ToDate)",
                CommandType.Text,
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate));
        }

        public static float GetLeadsSumByDateRange(DateTime fromDate, DateTime toDate)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "SELECT isnull(Sum(Price * Amount * [LeadCurrency].[CurrencyValue]), 0) " +
                "FROM [Order].[Lead] " +
                "Left Join [Order].[LeadItem] On [LeadItem].[LeadId] = [Lead].[Id] " +
                "Left Join [Order].[LeadCurrency] On [LeadCurrency].[LeadId] = [Lead].[Id] " +
                "WHERE Convert(date, @FromDate) <= Convert(date, [CreatedDate]) AND Convert(date, [CreatedDate]) <=  Convert(date, @ToDate)",
                CommandType.Text,
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate));
        }

        public static int GetLeadsCountByDate(DateTime date)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Order].[Lead] WHERE Convert(date, [CreatedDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date));
        }


        public static float GetLeadsSumByDate(DateTime date)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "SELECT isnull(Sum(Price * Amount * [LeadCurrency].[CurrencyValue]), 0) " +
                "FROM [Order].[Lead] " +
                "Left Join [Order].[LeadItem] On [LeadItem].[LeadId] = [Lead].[Id] " +
                "Left Join [Order].[LeadCurrency] On [LeadCurrency].[LeadId] = [Lead].[Id] " +
                "WHERE Convert(date, [CreatedDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date));
        }

        #endregion


        #region Reviews

        public static int GetReviewsCountByDate(DateTime date)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(ReviewId) FROM [CMS].[Review] WHERE Convert(date, [AddDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date));
        }

        public static int GetReviewsCountByDateRange(DateTime fromDate, DateTime toDate)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(ReviewId) FROM [CMS].[Review] WHERE Convert(date, @FromDate) <= Convert(date, [AddDate]) AND Convert(date, [AddDate]) <=  Convert(date, @ToDate)",
                CommandType.Text,
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate));
        }

        #endregion

        #region Calls

        public static int GetCallsCountByDate(DateTime date)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Customers].[Call] WHERE Convert(date, [CallDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date));
        }

        public static int GetCallsCountByDateRange(DateTime fromDate, DateTime toDate)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Customers].[Call] WHERE Convert(date, @FromDate) <= Convert(date, [CallDate]) AND Convert(date, [CallDate]) <=  Convert(date, @ToDate)",
                CommandType.Text,
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate));
        }

        #endregion
        public static float GetCustomerOrdersSum(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "SELECT isnull(Sum([Order].[Sum] * [OrderCurrency].[CurrencyValue]), 0) " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCustomer] On [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                "Left Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE OrderCustomer.CustomerId = @CustomerId And IsDraft = 0 AND [PaymentDate] IS NOT NULL",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }

        public static int GetCustomerOrdersCount(Guid customerId, bool onlyPaied = true)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([Order].[OrderId]) " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCustomer] On [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                "Left Join [Order].[OrderCurrency] On [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "WHERE OrderCustomer.CustomerId = @CustomerId And IsDraft = 0" + 
                (onlyPaied ? " AND [PaymentDate] IS NOT NULL" : ""),
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }
        
        public static int GetCustomerOrdersCountNotCompleteNotCancelled(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count([Order].[OrderID])  " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCustomer] On [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                "Left Join [Order].[OrderStatus] os On [Order].[OrderStatusID] = os.[OrderStatusID] " +
                "WHERE OrderCustomer.CustomerId = @CustomerId and IsDraft = 0 and os.[IsCompleted] = 0 and os.[IsCanceled] = 0 ",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }

        public static List<string> GetCustomerInterestingCategories(Guid customerId)
        {
            var categories = SQLDataAccess.Query<Category>(
                "Select distinct Category.CategoryId, Category.Name, Category.ParentCategory as ParentCategoryId " +
                "From [Order].[Order] " +
                "Left Join [Order].[OrderCustomer] on [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                "Left Join [Order].[OrderItems] on [OrderItems].[OrderId] = [Order].[OrderId] " +
                "Left Join [Catalog].[ProductCategories] ON [ProductCategories].[ProductId] = [OrderItems].[ProductId] and Main=1 " +
                "Left Join [Catalog].[Category] On [Category].[CategoryId] = [ProductCategories].[CategoryId] " +
                "Where IsDraft = 0 and [OrderCustomer].[CustomerId] = @customerId",
                new {customerId}).ToList();

            if (categories.Count == 0)
                return new List<string>();

            var result = new List<string>();

            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.Name))
                    continue;

                if (category.ParentCategoryId == 0)
                {
                    result.Add(category.Name);
                    continue;
                }

                var parent = CategoryService.GetParentCategories(category.CategoryId).LastOrDefault(x => x.CategoryId != 0);
                if (parent != null)
                    result.Add(parent.Name + " (" + category.Name + ")");
            }

            return result;
        }
    }
}