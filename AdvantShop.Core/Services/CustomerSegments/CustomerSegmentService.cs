using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.CustomerSegments
{
    public static class CustomerSegmentService
    {
        private const string CustomerSegmentCacheKey = "CustomerSegment_";
        private const string CustomerIdsBySegment = CustomerSegmentCacheKey + "{0}_CustomerIdsBySegment";

        public static CustomerSegment Get(int id)
        {
            return CacheManager.Get(CustomerSegmentCacheKey + id,
                () => SQLDataAccess.Query<CustomerSegment>("Select * From [Customers].[CustomerSegment] Where Id=@id",
                    new {id}).FirstOrDefault());
        }

        public static List<CustomerSegment> GetList()
        {
            return CacheManager.Get(CustomerSegmentCacheKey + "list",
                () => SQLDataAccess.Query<CustomerSegment>("Select * From [Customers].[CustomerSegment]").ToList());
        }

        public static List<CustomerSegment> GetListByCustomerId(Guid customerId)
        {
            return SQLDataAccess.Query<CustomerSegment>(
                        "Select s.* From [Customers].[CustomerSegment] as s " +
                        "Left Join [Customers].[CustomerSegment_Customer] as c ON s.Id = c.SegmentId " +
                        "Where c.CustomerId = @customerId", new {customerId}).ToList();
        }

        public static void Add(CustomerSegment segment)
        {
            segment.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into [Customers].[CustomerSegment] (Name, Filter, CreatedDate) Values (@Name, @Filter, getDate()); " +
                "Select scope_identity();",
                CommandType.Text,
                new SqlParameter("@Name", segment.Name),
                new SqlParameter("@Filter", segment.Filter ?? ""));

            CacheManager.RemoveByPattern(CustomerSegmentCacheKey);
        }

        public static void Update(CustomerSegment segment)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[CustomerSegment] Set Name=@Name, Filter=@Filter Where Id=@id",
                CommandType.Text,
                new SqlParameter("@id", segment.Id),
                new SqlParameter("@Name", segment.Name),
                new SqlParameter("@Filter", segment.Filter ?? ""));

            CacheManager.RemoveByPattern(CustomerSegmentCacheKey);
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Customers].[CustomerSegment] Where Id=@id", CommandType.Text,
                new SqlParameter("@id", id));

            CacheManager.RemoveByPattern(CustomerSegmentCacheKey);
        }


        #region CustomerSegment_Customer

        public static void AddCustomer(Guid customerId, int segmentId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[CustomerSegment_Customer] (CustomerId, SegmentId) Values (@customerId, @segmentId)",
                CommandType.Text,
                new SqlParameter("@customerId", customerId),
                new SqlParameter("@segmentId", segmentId));
        }

        public static void DeleteCustomersRelation(int segmentId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Customers].[CustomerSegment_Customer] Where SegmentId=@SegmentId", CommandType.Text,
                new SqlParameter("@SegmentId", segmentId));
        }

        #endregion

        #region GetCustomersBySegment

        public static List<Guid> GetCustomersBySegment(int segmentId)
        {
            return CacheManager.Get(string.Format(CustomerIdsBySegment, segmentId), 20, () =>
            {
                var paging = new SQL2.SqlPaging();

                GetCustomersBySegmentFilter(paging, segmentId);

                return paging.ItemsIds<Guid>("[Customer].CustomerID");
            });
        }


        public static void GetCustomersBySegmentFilter(SQL2.SqlPaging paging, int segmentId, string search = null)
        {
            paging.From("[Customers].[Customer]");
            paging.Where("[Customer].[CustomerRole] = " + (int)Role.User);

            if (!string.IsNullOrEmpty(search))
            {
                paging.Where(
                    "([Customer].Email LIKE '%'+{0}+'%' OR " +
                    "[Customer].Firstname + ' ' + [Customer].Lastname LIKE '%'+{0}+'%' OR " +
                    "[Customer].Lastname + ' ' + [Customer].Firstname LIKE '%'+{0}+'%' OR " +
                    "[Customer].Organization LIKE '%'+{0}+'%' OR " +
                    "[Customer].Phone LIKE '%'+{0}+'%')",
                    search);
            }

            var segment = Get(segmentId);
            if (segment == null || segment.SegmentFilter == null)
                return;

            var filter = segment.SegmentFilter;
            var conditionForOrder = string.Empty;
            if (filter.OrderPeriod.HasValue)
                conditionForOrder = $" AND [Order].[OrderDate] >= CAST('{DateTime.Now.AddDays(-filter.OrderPeriod.Value):yyyy-MM-dd}' AS DATE)";

            if (filter.OrdersSumFrom != null)
            {
                paging.Where(
                    "(Select ISNULL(SUM([Sum]), 0) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + ") >= {0}",
                    filter.OrdersSumFrom.Value);
            }
            if (filter.OrdersSumTo != null)
            {
                paging.Where(
                    "(Select ISNULL(SUM([Sum]), 0) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + ") <= {0}",
                    filter.OrdersSumTo.Value);
            }

            if (filter.OrdersPaidSumFrom != null)
            {
                paging.Where(
                    "(Select SUM([Sum]) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null" + conditionForOrder + ") >= {0}",
                    filter.OrdersPaidSumFrom.Value);
            }
            if (filter.OrdersPaidSumTo != null)
            {
                paging.Where(
                    "(Select SUM([Sum]) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null" + conditionForOrder + ") <= {0}",
                    filter.OrdersPaidSumTo.Value);
            }

            if (filter.OrdersCountFrom != null)
            {
                paging.Where(
                    "(Select Count([Order].[OrderId]) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + ") >= {0}",
                    filter.OrdersCountFrom.Value);
            }
            if (filter.OrdersCountTo != null)
            {
                paging.Where(
                    "(Select Count([Order].[OrderId]) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + ") <= {0}",
                    filter.OrdersCountTo.Value);
            }

            if (filter.LastOrderDateFrom != null)
            {
                paging.Where(
                    "(Select top(1) OrderDate " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + " " +
                    "Order by [OrderDate] Desc) >= {0}",
                    filter.LastOrderDateFrom.Value);
            }
            if (filter.LastOrderDateTo != null)
            {
                paging.Where(
                    "(Select top(1) OrderDate " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + " " +
                    "Order by [OrderDate] Desc) <= {0}",
                    filter.LastOrderDateTo.Value);
            }


            if (filter.AverageCheckFrom != null)
            {
                paging.Where(
                    "(Select SUM([Sum])/Count([Order].OrderId) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null" + conditionForOrder + ") >= {0}",
                    filter.AverageCheckFrom.Value);
            }
            if (filter.AverageCheckTo != null)
            {
                paging.Where(
                    "(Select SUM([Sum])/Count([Order].OrderId) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null" + conditionForOrder + ") <= {0}",
                    filter.AverageCheckTo.Value);
            }

            if (filter.BirthDayFrom != null)
            {
                paging.Where(
                    filter.IgnoreBirthDayYear
                        ? "(Customer.BirthDay is not null and ( MONTH(Customer.BirthDay) > MONTH({0}) or (MONTH(Customer.BirthDay) = MONTH({0}) and DAY(Customer.BirthDay) >= DAY({0})) ))"
                        : "(Customer.BirthDay is not null and Customer.BirthDay >= {0})",
                    filter.BirthDayFrom.Value);
            }
            if (filter.BirthDayTo != null)
            {
                paging.Where(
                    filter.IgnoreBirthDayYear
                        ? "(Customer.BirthDay is not null and ( MONTH(Customer.BirthDay) < MONTH({0}) OR (MONTH(Customer.BirthDay) = MONTH({0}) AND DAY(Customer.BirthDay) <= DAY({0})) ))"
                        : "(Customer.BirthDay is not null and Customer.BirthDay <= {0})",
                    filter.BirthDayTo.Value);
            }

            if (filter.Cities != null && filter.Cities.Count > 0)
            {
                paging.Where(
                    "(Select top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID]) in (" +
                    String.Join(", ", Enumerable.Range(0, filter.Cities.Count).Select(x => "{" + x + "}")) + ")",
                    filter.Cities.ToArray());
            }

            if (filter.Countries != null && filter.Countries.Count > 0)
            {
                paging.Where(
                    "(Select top(1) [Country] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID]) in (" +
                    String.Join(", ", Enumerable.Range(0, filter.Countries.Count).Select(x => "{" + x + "}")) + ")",
                    filter.Countries.ToArray());
            }

            if (filter.Categories != null && filter.Categories.Count > 0)
            {
                var categoryIds = new List<int>();

                foreach (var categoryId in filter.Categories)
                {
                    categoryIds.AddRange(CategoryService.GetAllChildCategoriesIdsByCategoryId(categoryId));
                }

                categoryIds = categoryIds.Distinct().ToList();

                paging.Where(
                    "Exists (Select [ProductCategories].CategoryId " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderItems] on [OrderItems].[OrderId] = [Order].[OrderId] " +
                    "Left Join [Catalog].[ProductCategories] ON [ProductCategories].[ProductId] = [OrderItems].[ProductId] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + " and CategoryId in (" +
                    String.Join(", ", Enumerable.Range(0, categoryIds.Count).Select(x => "{" + x + "}")) + ") )",
                    categoryIds.Select(x => (object)x).ToArray());
            }


            if (filter.CustomerFields != null && filter.CustomerFields.Count > 0)
            {
                foreach (var fieldFilter in filter.CustomerFields.Where(x => x.Value != null && x.Value != "––––"))
                {
                    paging.Where(
                        "Exists(Select 1 " +
                        "From Customers.CustomerFieldValuesMap " +
                        "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                               "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                        fieldFilter.Key, fieldFilter.Value);
                }
            }

            if (filter.IsSocialUser)
            {
                paging.Where(
                    "(Exists(Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = [Customer].[CustomerId]) OR " +
                    "Exists(Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = [Customer].[CustomerId]) OR " +
                    "Exists(Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = [Customer].[CustomerId]) OR " +
                    "Exists(Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = [Customer].[CustomerId]))");
            }
            else
            {
                if (filter.IsVkUser)
                    paging.Where("Exists(Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = [Customer].[CustomerId])");

                if (filter.IsFacebookUser)
                    paging.Where("Exists(Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = [Customer].[CustomerId])");

                if (filter.IsInstagramUser)
                    paging.Where("Exists(Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = [Customer].[CustomerId])");

                if (filter.IsTelegramUser)
                    paging.Where("Exists(Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = [Customer].[CustomerId])");
            }

            if (filter.CustomerType != CustomerType.All)
                paging.Where("[Customer].[CustomerType] = {0}", (int)filter.CustomerType);

            if (filter.OrderType.HasValue)
                paging.Where(
                    @"Exists(Select top(1) 1 From [Order].[Order]
                        Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId]
                        Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder +
                        " AND [Order].[OrderSourceId] IN (Select [OrderSource].[Id] From [Order].[OrderSource] where Type = {0})) ", filter.OrderType.ToString());

            if (filter.IsInstallMobileApp)
                paging.Where("([Customer].[FcmToken] IS NOT NULL OR [Customer].[FcmToken] <> '')");

            if (filter.OrderPeriod.HasValue)
                paging.Where(
                    @"Exists(Select top(1) 1 From [Order].[Order]
                        Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId]
                        Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId]" + conditionForOrder + ") ");
        }

        #endregion
    }
}
