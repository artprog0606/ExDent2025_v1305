//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class CustomerGroupService
    {
        private const string CustomerGroupCacheKey = "CustomerGroup_";
        private const string CustomerGroupCategoriesCacheKey = "CustomerGroupCategories_";
        private const int DefaultCustomerGroupId = 1;

        private static int _defaultCustomerGroup;
        public static int DefaultCustomerGroup
        {
            get
            {
                if (_defaultCustomerGroup != 0)
                    return _defaultCustomerGroup;

                var list = GetCustomerGroupListIds();

                _defaultCustomerGroup = list.Contains(DefaultCustomerGroupId) || list.Count == 0
                    ? DefaultCustomerGroupId
                    : list[0];

                return _defaultCustomerGroup;
            }
        }

        public static CustomerGroup GetDefaultCustomerGroup()
        {
            return GetCustomerGroup(DefaultCustomerGroup);
        }

        public static CustomerGroup GetCustomerGroup(int customerGroupId = 1)
        {
            return CacheManager.Get(CustomerGroupCacheKey + customerGroupId, 20,
                () =>
                    SQLDataAccess.ExecuteReadOne(
                        "SELECT TOP 1 * FROM [Customers].[CustomerGroup] WHERE CustomerGroupId = @CustomerGroupId",
                        CommandType.Text, GetCustomerGroupFromReader,
                        new SqlParameter("@CustomerGroupId", customerGroupId)));
        }

        public static CustomerGroup GetCustomerGroup(string groupName)
        {
            return SQLDataAccess.ExecuteReadOne(
                        "SELECT TOP 1 * FROM [Customers].[CustomerGroup] WHERE GroupName = @GroupName",
                        CommandType.Text, GetCustomerGroupFromReader,
                        new SqlParameter("@GroupName", groupName.Trim()));
        }

        public static List<CustomerGroup> GetCustomerGroupList()
        {
            return CacheManager.Get(CustomerGroupCacheKey + "List", 20,
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "SELECT * FROM [Customers].[CustomerGroup] order by GroupDiscount",
                        CommandType.Text, GetCustomerGroupFromReader));
        }

        public static List<int> GetCustomerGroupListIds()
        {
            return CacheManager.Get(CustomerGroupCacheKey + "ListIds", 20,
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "SELECT [CustomerGroupId] FROM [Customers].[CustomerGroup] order by GroupDiscount",
                        CommandType.Text, reader => SQLDataHelper.GetInt(reader, "CustomerGroupId")));
        }


        public static float GetMinimumOrderPrice(int? customerGroupId = null, CustomerType? customerType =  null)
        {
            float minimumPriceForGroup = GetMinimumOrderPriceByGroup(customerGroupId);
            float? minimumPriceForCustomerType = null;

            customerType = customerType ?? CustomerContext.CurrentCustomer?.CustomerType;
            if (customerType == CustomerType.PhysicalEntity)
                minimumPriceForCustomerType = SettingsCustomers.MinimalOrderPriceForPhysicalEntity;
            if (customerType == CustomerType.LegalEntity)
                minimumPriceForCustomerType = SettingsCustomers.MinimalOrderPriceForLegalEntity;

            if (minimumPriceForCustomerType.HasValue)
                return minimumPriceForCustomerType.Value > minimumPriceForGroup ? minimumPriceForCustomerType.Value : minimumPriceForGroup;
            return minimumPriceForGroup;
        }

        public static float GetMinimumOrderPriceByGroup(int? customerGroupId = null)
        {
            var groupId = customerGroupId ?? (CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.CustomerGroupId : DefaultCustomerGroup);
            var group = GetCustomerGroup(groupId);

            return group != null ? group.MinimumOrderPrice : 0;
        }


        private static CustomerGroup GetCustomerGroupFromReader(SqlDataReader reader)
        {
            return new CustomerGroup
            {
                CustomerGroupId = SQLDataHelper.GetInt(reader, "CustomerGroupId"),
                GroupName = SQLDataHelper.GetString(reader, "GroupName"),
                GroupDiscount = SQLDataHelper.GetFloat(reader, "GroupDiscount"),
                MinimumOrderPrice = SQLDataHelper.GetFloat(reader, "MinimumOrderPrice")
            };
        }

        public static void AddCustomerGroup(CustomerGroup customerGroup)
        {
            customerGroup.CustomerGroupId = SQLDataAccess.ExecuteScalar<int>("INSERT INTO [Customers].[CustomerGroup] ([GroupName], [GroupDiscount],MinimumOrderPrice) VALUES (@GroupName, @GroupDiscount, @MinimumOrderPrice); SELECT SCOPE_IdENTITY();",
                                                                                CommandType.Text,
                                                                                new SqlParameter("@GroupName", customerGroup.GroupName),
                                                                                new SqlParameter("@GroupDiscount", customerGroup.GroupDiscount),
                                                                                new SqlParameter("@MinimumOrderPrice", customerGroup.MinimumOrderPrice)
                                                                                );
            CacheManager.RemoveByPattern(CustomerGroupCacheKey);
        }

        public static void UpdateCustomerGroup(CustomerGroup customerGroup)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Customers].[CustomerGroup] SET [GroupName] = @GroupName, [GroupDiscount] = @GroupDiscount, MinimumOrderPrice=@MinimumOrderPrice " +
                " WHERE CustomerGroupId = @CustomerGroupId", CommandType.Text,
                new SqlParameter("@CustomerGroupId", customerGroup.CustomerGroupId),
                new SqlParameter("@GroupName", customerGroup.GroupName),
                new SqlParameter("@GroupDiscount", customerGroup.GroupDiscount),
                new SqlParameter("@MinimumOrderPrice", customerGroup.MinimumOrderPrice));
            CacheManager.RemoveByPattern(CustomerGroupCacheKey);
        }

        public static void DeleteCustomerGroup(int customerGroupId)
        {
            if (customerGroupId == DefaultCustomerGroup)
                return;
            SQLDataAccess.ExecuteNonQuery("UPDATE [Customers].[Customer] set CustomerGroupId = @NewCustomerGroupId Where CustomerGroupId=@OldCustomerGroupId",
                                            CommandType.Text,
                                            new SqlParameter("@OldCustomerGroupId", customerGroupId),
                                            new SqlParameter("@NewCustomerGroupId", DefaultCustomerGroup));

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[CustomerGroup] WHERE CustomerGroupId = @CustomerGroupId",
                                            CommandType.Text, new SqlParameter("@CustomerGroupId", customerGroupId));

            CacheManager.RemoveByPattern(CustomerGroupCacheKey);
        }
        
        #region CustomerGroup category discount

        private static List<CustomerGroupCategoryDiscount> GetCustomerGroupCategoryDiscounts()
        {
            return
                CacheManager.Get(CustomerGroupCategoriesCacheKey + "list", 60, () =>
                    SQLDataAccess
                        .Query<CustomerGroupCategoryDiscount>("Select * From [Customers].[CustomerGroup_Category]")
                        .ToList());
        }

        public static float? GetCustomerGroupDiscountByCategory(int customerGroupId, int categoryId)
        {
            var result = CacheManager.Get(CustomerGroupCategoriesCacheKey + customerGroupId + "_" + categoryId, 60, () =>
            {
                var categoryDiscounts = GetCustomerGroupCategoryDiscounts();
                if (categoryDiscounts.Count == 0)
                    return new WrapResult<float?>(null);

                var discount =
                    categoryDiscounts.Find(x => x.CustomerGroupId == customerGroupId && x.CategoryId == categoryId);

                return new WrapResult<float?>(discount?.Discount);
            });

            return result.Value;
        }
        
        public static void AddOrUpdateCustomerGroupCategoryDiscount(CustomerGroupCategoryDiscount customerGroupCategoryDiscount)
        {
            SQLDataAccess.ExecuteNonQuery(
@"If Exists (Select 1 From [Customers].[CustomerGroup_Category] Where CustomerGroupId=@CustomerGroupId and CategoryId=@CategoryId) 
    Update [Customers].[CustomerGroup_Category] Set Discount=@Discount Where CustomerGroupId=@CustomerGroupId and CategoryId=@CategoryId 
Else 
    INSERT INTO [Customers].[CustomerGroup_Category] (CustomerGroupId, CategoryId, Discount) VALUES (@CustomerGroupId, @CategoryId, @Discount)",
                CommandType.Text,
                new SqlParameter("@CustomerGroupId", customerGroupCategoryDiscount.CustomerGroupId),
                new SqlParameter("@CategoryId", customerGroupCategoryDiscount.CategoryId),
                new SqlParameter("@Discount", customerGroupCategoryDiscount.Discount)
            );
            CacheManager.RemoveByPattern(CustomerGroupCategoriesCacheKey);
        }
        
        public static void DeleteCustomerGroupCategoryDiscount(int customerGroupId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete [Customers].[CustomerGroup_Category] Where CustomerGroupId=@CustomerGroupId and CategoryId=@CategoryId",
                CommandType.Text,
                new SqlParameter("@CustomerGroupId", customerGroupId),
                new SqlParameter("@CategoryId", categoryId)
            );
            CacheManager.RemoveByPattern(CustomerGroupCategoriesCacheKey);
        }
        
        #endregion
    }
}