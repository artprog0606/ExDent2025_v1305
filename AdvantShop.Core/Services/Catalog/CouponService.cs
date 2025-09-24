//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using System.Reflection;
using AdvantShop.Core;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Catalog
{
    public class CouponService
    {
        private const string CouponCachePrefix = "Coupon_";
        private const string CustomerCouponCachePrefix = CouponCachePrefix + "CustomerCoupon_";
        

        #region Get, Add, Update, Delete

        public static Coupon GetCoupon(int couponId, bool fromCache = true)
        {
            return fromCache
                ? CacheManager.Get(CouponCachePrefix + couponId, 2, () => GetCouponFromDb(couponId))
                : GetCouponFromDb(couponId);
        }

        private static Coupon GetCouponFromDb(int couponId)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT TOP 1 * FROM [Catalog].[Coupon] WHERE CouponID = @CouponID",
                    CommandType.Text,
                    GetFromReader,
                    new SqlParameter("@CouponID", couponId));
        }

        public static Coupon GetCouponByCode(string code)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM [Catalog].[Coupon] WHERE Code = @Code", CommandType.Text, GetFromReader, new SqlParameter("@Code", code));
        }

        public static List<Coupon> GetAllCoupons()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Catalog].[Coupon]", CommandType.Text, GetFromReader);
        }

        private static Coupon GetFromReader(SqlDataReader reader)
        {
            return new Coupon
            {
                CouponID = SQLDataHelper.GetInt(reader, "CouponID"),
                Code = SQLDataHelper.GetString(reader, "Code"),
                Type = (CouponType) SQLDataHelper.GetInt(reader, "Type"),
                Value = SQLDataHelper.GetFloat(reader, "Value"),
                AddingDate = SQLDataHelper.GetDateTime(reader, "AddingDate"),
                ExpirationDate = SQLDataHelper.GetNullableDateTime(reader, "ExpirationDate"),
                PossibleUses = SQLDataHelper.GetInt(reader, "PossibleUses"),
                ActualUses = SQLDataHelper.GetInt(reader, "ActualUses"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                MinimalOrderPrice = SQLDataHelper.GetFloat(reader, "MinimalOrderPrice"),
                IsMinimalOrderPriceFromAllCart = SQLDataHelper.GetBoolean(reader, "IsMinimalOrderPriceFromAllCart"),
                CurrencyIso3 = SQLDataHelper.GetString(reader, "CurrencyIso3"),
                TriggerActionId = SQLDataHelper.GetNullableInt(reader, "TriggerActionId"),
                TriggerId = SQLDataHelper.GetNullableInt(reader, "TriggerId"),
                Mode = (CouponMode)SQLDataHelper.GetInt(reader, "Mode"),
                Days = SQLDataHelper.GetNullableInt(reader, "Days"),
                CustomerId = SQLDataHelper.GetNullableGuid(reader, "CustomerId"),
                StartDate = SQLDataHelper.GetNullableDateTime(reader, "StartDate"),
                ForFirstOrder = SQLDataHelper.GetBoolean(reader, "ForFirstOrder"),
                OnlyInMobileApp = SQLDataHelper.GetBoolean(reader, "OnlyInMobileApp"),
                ForFirstOrderInMobileApp = SQLDataHelper.GetBoolean(reader, "ForFirstOrderInMobileApp"),
                ModuleId = SQLDataHelper.GetString(reader, "ModuleId"),
                OnlyOnCustomerBirthday = SQLDataHelper.GetBoolean(reader, "OnlyOnCustomerBirthday"),
                DaysBeforeBirthday = SQLDataHelper.GetNullableInt(reader, "DaysBeforeBirthday"),
                DaysAfterBirthday = SQLDataHelper.GetNullableInt(reader, "DaysAfterBirthday"),
                Comment = SQLDataHelper.GetString(reader, "Comment"),
                IsAppliedToPriceWithDiscount = SQLDataHelper.GetBoolean(reader, "IsAppliedToPriceWithDiscount")
            };
        }

        public static void AddCoupon(Coupon coupon)
        {
            coupon.CouponID = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Catalog].[Coupon] " +
                "([Code], [Type], [Value], [AddingDate], [ExpirationDate], [PossibleUses], [ActualUses], [Enabled], " +
                    "[MinimalOrderPrice], IsMinimalOrderPriceFromAllCart, CurrencyIso3, TriggerActionId, Mode, Days, " +
                    "TriggerId, CustomerId, StartDate, ForFirstOrder, EntityId, OnlyInMobileApp, ForFirstOrderInMobileApp, " +
                    "ModuleId, OnlyOnCustomerBirthday, DaysBeforeBirthday, DaysAfterBirthday, Comment, IsAppliedToPriceWithDiscount) " +
                "VALUES (@Code, @Type, @Value, @AddingDate, @ExpirationDate, @PossibleUses, @ActualUses, @Enabled, " +
                    "@MinimalOrderPrice, @IsMinimalOrderPriceFromAllCart, @CurrencyIso3, @TriggerActionId, @Mode, @Days, " +
                    "@TriggerId, @CustomerId, @StartDate, @ForFirstOrder, @EntityId, @OnlyInMobileApp, @ForFirstOrderInMobileApp, " +
                    "@ModuleId, @OnlyOnCustomerBirthday, @DaysBeforeBirthday, @DaysAfterBirthday, @Comment, @IsAppliedToPriceWithDiscount); " +
                "SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Code", coupon.Code),
                new SqlParameter("@Type", coupon.Type),
                new SqlParameter("@Value", coupon.Value),
                new SqlParameter("@AddingDate", coupon.AddingDate),
                new SqlParameter("@ExpirationDate", coupon.ExpirationDate ?? (object) DBNull.Value),
                new SqlParameter("@PossibleUses", coupon.PossibleUses),
                new SqlParameter("@ActualUses", coupon.ActualUses),
                new SqlParameter("@Enabled", coupon.Enabled),
                new SqlParameter("@MinimalOrderPrice", coupon.MinimalOrderPrice),
                new SqlParameter("@IsMinimalOrderPriceFromAllCart", coupon.IsMinimalOrderPriceFromAllCart),
                new SqlParameter("@CurrencyIso3", coupon.CurrencyIso3),
                new SqlParameter("@TriggerActionId", coupon.TriggerActionId ?? (object) DBNull.Value),
                new SqlParameter("@TriggerId", coupon.TriggerId ?? (object) DBNull.Value),
                new SqlParameter("@Mode", coupon.Mode),
                new SqlParameter("@Days", coupon.Days ?? (object) DBNull.Value),
                new SqlParameter("@CustomerId", coupon.CustomerId ?? (object) DBNull.Value),
                new SqlParameter("@StartDate", coupon.StartDate ?? (object) DBNull.Value),
                new SqlParameter("@ForFirstOrder", coupon.ForFirstOrder),
                new SqlParameter("@EntityId", coupon.EntityId ?? (object) DBNull.Value),
                new SqlParameter("@OnlyInMobileApp", coupon.OnlyInMobileApp),
                new SqlParameter("@ForFirstOrderInMobileApp", coupon.ForFirstOrderInMobileApp),
                new SqlParameter("@ModuleId", coupon.ModuleId ?? (object)DBNull.Value),
                new SqlParameter("@OnlyOnCustomerBirthday", coupon.OnlyOnCustomerBirthday),
                new SqlParameter("@DaysBeforeBirthday", coupon.DaysBeforeBirthday ?? (object)DBNull.Value),
                new SqlParameter("@DaysAfterBirthday", coupon.DaysAfterBirthday ?? (object)DBNull.Value),
                new SqlParameter("@Comment", coupon.Comment ?? (object)DBNull.Value),
                new SqlParameter("@IsAppliedToPriceWithDiscount", coupon.IsAppliedToPriceWithDiscount)
                );
        }

        public static void UpdateCoupon(Coupon coupon)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Coupon] " +
                "SET [Code] = @Code, [Type] = @Type, [Value] = @Value, [AddingDate]=@AddingDate, [ExpirationDate] = @ExpirationDate, " +
                "[PossibleUses] = @PossibleUses, [ActualUses] = @ActualUses, [Enabled] = @Enabled, [MinimalOrderPrice] = @MinimalOrderPrice," +
                "IsMinimalOrderPriceFromAllCart = @IsMinimalOrderPriceFromAllCart, CurrencyIso3 = @CurrencyIso3, " +
                "TriggerActionId = @TriggerActionId, Mode=@Mode, Days=@Days, TriggerId=@TriggerId, CustomerId=@CustomerId, " +
                "StartDate = @StartDate, ForFirstOrder = @ForFirstOrder, OnlyInMobileApp=@OnlyInMobileApp, ForFirstOrderInMobileApp=@ForFirstOrderInMobileApp, " +
                "ModuleId = @ModuleId, OnlyOnCustomerBirthday = @OnlyOnCustomerBirthday, DaysBeforeBirthday = @DaysBeforeBirthday, DaysAfterBirthday = @DaysAfterBirthday, " +
                "[Comment] = @Comment, IsAppliedToPriceWithDiscount = @IsAppliedToPriceWithDiscount " +
                "WHERE CouponID = @CouponID", CommandType.Text,
                new SqlParameter("@CouponID", coupon.CouponID),
                new SqlParameter("@Code", coupon.Code),
                new SqlParameter("@Type", coupon.Type),
                new SqlParameter("@Value", coupon.Value),
                new SqlParameter("@AddingDate", coupon.AddingDate),
                new SqlParameter("@ExpirationDate", coupon.ExpirationDate ?? (object) DBNull.Value),
                new SqlParameter("@PossibleUses", coupon.PossibleUses),
                new SqlParameter("@ActualUses", coupon.ActualUses),
                new SqlParameter("@Enabled", coupon.Enabled),
                new SqlParameter("@MinimalOrderPrice", coupon.MinimalOrderPrice),
                new SqlParameter("@IsMinimalOrderPriceFromAllCart", coupon.IsMinimalOrderPriceFromAllCart),
                new SqlParameter("@CurrencyIso3", coupon.CurrencyIso3),
                new SqlParameter("@TriggerActionId", coupon.TriggerActionId ?? (object) DBNull.Value),
                new SqlParameter("@TriggerId", coupon.TriggerId ?? (object)DBNull.Value),
                new SqlParameter("@Mode", coupon.Mode),
                new SqlParameter("@Days", coupon.Days ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", coupon.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@StartDate", coupon.StartDate ?? (object)DBNull.Value),
                new SqlParameter("@ForFirstOrder", coupon.ForFirstOrder),
                new SqlParameter("@OnlyInMobileApp", coupon.OnlyInMobileApp),
                new SqlParameter("@ForFirstOrderInMobileApp", coupon.ForFirstOrderInMobileApp),
                new SqlParameter("@ModuleId", coupon.ModuleId ?? (object)DBNull.Value),
                new SqlParameter("@OnlyOnCustomerBirthday", coupon.OnlyOnCustomerBirthday),
                new SqlParameter("@DaysBeforeBirthday", coupon.DaysBeforeBirthday ?? (object)DBNull.Value),
                new SqlParameter("@DaysAfterBirthday", coupon.DaysAfterBirthday ?? (object)DBNull.Value),
                new SqlParameter("@Comment", coupon.Comment ?? (object)DBNull.Value),
                new SqlParameter("@IsAppliedToPriceWithDiscount", coupon.IsAppliedToPriceWithDiscount)
                );
            
            CacheManager.Remove(CouponCachePrefix + coupon.CouponID);
        }

        public static void DeleteCoupon(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[Coupon] WHERE CouponID = @CouponID", 
                CommandType.Text,
                new SqlParameter("@CouponID", couponId));

            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static void DeleteExpiredGeneratedCoupons()
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[Coupon] WHERE Mode = @Mode and ExpirationDate is not null and dateadd(dd,90,ExpirationDate) < getdate() and ActualUses = 0",
                CommandType.Text,
                new SqlParameter("@Mode", (int) CouponMode.Generated));
            
            CacheManager.RemoveByPattern(CouponCachePrefix);
        }
        
        public static void IncrementActualUses(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Coupon]
                SET [ActualUses] = [ActualUses] + 1
                WHERE CouponID = @CouponID", CommandType.Text,
                new SqlParameter("@CouponID", couponId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        #endregion

        #region Product links

        public static void AddProductToCoupon(int couponId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [Catalog].[CouponProducts] (couponID,  productID) values (@couponID, @productID)",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponId),
                                            new SqlParameter("@productID", productId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static List<int> GetProductsIDsByCoupon(int couponId)
        {
            return SQLDataAccess.ExecuteReadList<int>(
                "Select ProductID from [Catalog].[CouponProducts] where couponID=@couponID", CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "ProductID"),
                new SqlParameter("@CouponID", couponId));
        }

        public static void DeleteProductFromCoupon(int couponId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponProducts] Where couponID=@couponID and productID=@productID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponId),
                                            new SqlParameter("@productID", productId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static void DeleteAllProductsFromCoupon(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponProducts] Where couponID=@couponID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        #endregion

        #region Categories link

        public static void AddCategoryToCoupon(int couponId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [Catalog].[CouponCategories] (couponID,  categoryId) values (@couponID, @categoryId)",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponId),
                                            new SqlParameter("@categoryId", categoryId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static List<int> GetCategoriesIDsByCoupon(int couponId)
        {
            var list = SQLDataAccess.ExecuteReadList(
                "Select CategoryID from [Catalog].[CouponCategories] where couponID=@couponID",
                CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "CategoryID"),
                new SqlParameter("@CouponID", couponId));
            return list;
        }

        public static void DeleteCategoriesFromCoupon(int couponId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponCategories] Where couponID=@couponID and categoryID=@categoryID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponId),
                                            new SqlParameter("@categoryID", categoryId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static void DeleteAllCategoriesFromCoupon(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponCategories] Where couponID=@couponID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        #endregion

        #region CustomerGroup link

        public static void AddCustomerGroupToCoupon(int couponId, int customerGroupId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Catalog].[CouponCustomerGroup] ([CouponId],  [CustomerGroupID]) VALUES (@couponId, @customerGroupId)",
                                            CommandType.Text,
                                            new SqlParameter("@couponId", couponId),
                                            new SqlParameter("@customerGroupId", customerGroupId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static List<int> GetCustomerGroupIdsByCoupon(int couponId)
        {
            return SQLDataAccess.ExecuteReadList<int>("SELECT [CustomerGroupId] FROM [Catalog].[CouponCustomerGroup] WHERE [CouponId]=@couponId",
                                                           CommandType.Text,
                                                           reader => SQLDataHelper.GetInt(reader, "CustomerGroupId"),
                                                           new SqlParameter("@couponId", couponId));
        }

        public static void DeleteCustomerGroupFromCoupon(int couponId, int customerGroupId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[CouponCustomerGroup] WHERE CouponId=@couponId AND CustomerGroupId=@customerGroupId",
                                            CommandType.Text,
                                            new SqlParameter("@couponId", couponId),
                                            new SqlParameter("@customerGroupId", customerGroupId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static void DeleteAllCustomerGroupsFromCoupon(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[CouponCustomerGroup] WHERE CouponId=@couponId",
                                            CommandType.Text,
                                            new SqlParameter("@couponId", couponId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        #endregion

        #region CustomerCoupon

        public static bool CanApplyCustomerCoupon(Coupon coupon)
        {
            return CanApplyCustomerCoupon(coupon, CustomerContext.CustomerId);
        }

        public static bool CanApplyCustomerCoupon(Coupon coupon, Guid customerId)
        {
            return coupon.Enabled &&
                (coupon.StartDate == null || coupon.StartDate < DateTime.Now) &&
                (coupon.ExpirationDate == null || coupon.ExpirationDate > DateTime.Now) &&
                (coupon.PossibleUses == 0 || coupon.PossibleUses > coupon.ActualUses) &&
                !(coupon.ForFirstOrder && OrderService.IsCustomerHasConfirmedOrders(customerId)) &&
                !(coupon.ForFirstOrderInMobileApp && OrderService.IsCustomerHasConfirmedOrdersFromMobileApp(customerId)) &&
                CheckCustomerCouponByBirthday(coupon, CustomerService.GetCustomer(customerId)).IsNullOrEmpty();
        }

        public static string CheckCustomerCouponByBirthday(Coupon coupon, Customer customer)
        {
            if (!coupon.OnlyOnCustomerBirthday)
                return null;

            if (customer == null || !customer.BirthDay.HasValue)
                return LocalizationService.GetResource("Coupon.CouponPost.OnlyOnCustomerBirthday");

            var dateNow = DateTime.Now.Date;
            dateNow = dateNow.AddYears(customer.BirthDay.Value.Year - dateNow.Year);
            var minDate = customer.BirthDay.Value.Date;
            var maxDate = minDate;

            if (coupon.DaysBeforeBirthday.HasValue)
                minDate = minDate.AddDays(-coupon.DaysBeforeBirthday.Value);

            if (coupon.DaysAfterBirthday.HasValue)
                maxDate = maxDate.AddDays(coupon.DaysAfterBirthday.Value);
            if (dateNow < minDate || maxDate < dateNow)
            {
                if (minDate == maxDate)
                    return LocalizationService.GetResource("Coupon.CouponPost.OnlyOnCustomerBirthday");
                else
                    return LocalizationService.GetResourceFormat("Coupon.CouponPost.OnlyOnDates", minDate.ToString("M"), maxDate.ToString("M"));
            }
            return null;
        }

        public static Coupon GetCustomerCoupon()
        {
            return GetCustomerCoupon(CustomerContext.CustomerId);
        }

        public static Coupon GetCustomerCoupon(Guid customerId)
        {
            var customerCouponId = 
                CacheManager.Get(CustomerCouponCachePrefix + customerId, () =>
                    SQLDataAccess.ExecuteScalar<int>(
                        "Select Top(1) CouponID From Customers.CustomerCoupon Where CustomerID = @CustomerID",
                        CommandType.Text,
                        new SqlParameter("@CustomerID", customerId)));

            if (customerCouponId == 0)
                return null;
            
            var coupon = GetCoupon(customerCouponId);
            if (coupon == null)
                return null;

            if (CanApplyCustomerCoupon(coupon, customerId))
                return coupon;

            DeleteCustomerCoupon(coupon.CouponID, customerId);
            
            return null;
        }

        public static void DeleteCustomerCoupon(int couponId, bool executeModules = true)
        {
            DeleteCustomerCoupon(couponId, CustomerContext.CustomerId, executeModules);
        }

        public static void DeleteCustomerCoupon(int couponId, Guid customerId, bool executeModules = true)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Customers.CustomerCoupon Where CouponID = @CouponID and CustomerID = @CustomerID",
                CommandType.Text, 
                new SqlParameter("@CustomerID", customerId), 
                new SqlParameter("@CouponID", couponId));

            if (executeModules)
                ModulesExecuter.DeletedCustomerCoupon(couponId, customerId);
            CacheManager.Remove(CustomerCouponCachePrefix + customerId);
        }

        public static void AddCustomerCoupon(int couponId)
        {
            AddCustomerCoupon(couponId, true, true);
        }

        public static void AddCustomerCoupon(int couponId, bool customerApplied, bool executeModules = true)
        {
            // покупатель может применить только один купон, иначе падало при выборке, также добавлено  Select Top(1) в GetCustomerCoupon
            if (!IsCustomerHaveThisCoupon(couponId, CustomerContext.CustomerId) && !IsCustomerApplyAnyCoupon(CustomerContext.CustomerId))
                AddCustomerCoupon(couponId, CustomerContext.CustomerId, executeModules);

            if (customerApplied)
                PartnerService.SetReferralCookie(couponId);
        }

        public static void AddCustomerCoupon(int couponId, Guid customerId, bool executeModules = true)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (NOT EXISTS(Select 1 from Customers.CustomerCoupon where CustomerID=@CustomerID and CouponID=@CouponID))" +
                "BEGIN" +
                "   INSERT INTO Customers.CustomerCoupon (CustomerID, CouponID) VALUES (@CustomerID, @CouponID)" +
                "END",
                 CommandType.Text,
                 new SqlParameter("@CustomerID", customerId),
                 new SqlParameter("@CouponID", couponId));

            CacheManager.Remove(CustomerCouponCachePrefix + customerId);

            if (executeModules)
                ModulesExecuter.AddedCustomerCoupon(couponId, customerId);
        }

        private static bool IsCustomerApplyAnyCoupon(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>
                ("Select Count(*) from Customers.CustomerCoupon where CustomerID=@CustomerID",
                 CommandType.Text,
                 new SqlParameter("@CustomerID", customerId)                 
                ) > 0;
        }

        private static bool IsCustomerHaveThisCoupon(int couponId, Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>
                ("Select Count(*) from Customers.CustomerCoupon where CustomerID=@CustomerID and CouponID=@CouponID",
                 CommandType.Text,
                 new SqlParameter("@CustomerID", customerId),
                 new SqlParameter("@CouponID", couponId)
                ) > 0;
        }

        #endregion

        public static string GenerateCouponCode()
        {
            var code = "";
            while (string.IsNullOrEmpty(code) || IsExistCouponCode(code) || GiftCertificateService.IsExistCertificateCode(code))
            {
                code = Strings.GetRandomString(8);
            }
            return code;
        }

        public static bool IsExistCouponCode(string code)
        {
            return
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar("Select COUNT(CouponID) From Catalog.Coupon Where Code = @Code",
                        CommandType.Text,
                        new SqlParameter("@Code", code))) > 0;
        }

        public static bool IsCouponCanBeAppliedToProduct(int couponId, int productId)
        {
            return
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar("Catalog.sp_IsCouponAppliedToProduct", 
                        CommandType.StoredProcedure,
                        new SqlParameter("@CouponID", couponId),
                        new SqlParameter("@productId", productId))) > 0;
        }

        public static void SetCouponActivity(int couponId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Coupon Set Enabled = @Enabled Where CouponID = @CouponID",
                                         CommandType.Text,
                                          new SqlParameter("@CouponID", couponId),
                                          new SqlParameter("@Enabled", active));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static List<Coupon> GetCouponsByTriggerAction(int triggerActionId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerActionId = @TriggerActionId and Mode=@Mode",
                CommandType.Text, GetFromReader, 
                new SqlParameter("@TriggerActionId", triggerActionId),
                new SqlParameter("@Mode", (int)CouponMode.Template));
        }

        public static List<Coupon> GetAllCouponsByTriggerAction(int triggerActionId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerActionId = @TriggerActionId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerActionId", triggerActionId));
        }

        public static Coupon GetCouponByTrigger(int triggerId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[Coupon] WHERE TriggerId = @TriggerId and Mode=@Mode",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerId", triggerId),
                new SqlParameter("@Mode", (int)CouponMode.TriggerTemplate));
        }

        public static List<Coupon> GetAllCouponsByTrigger(int triggerId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerId = @TriggerId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerId", triggerId));
        }

        public static Coupon GenerateCoupon(Coupon couponTemplate, Guid? customerId = null, int? entityId = null)
        {
            var coupon = couponTemplate.DeepClone();
            coupon.Code = GenerateCouponCode();
            coupon.Mode = CouponMode.Generated;
            coupon.CustomerId = customerId;
            coupon.AddingDate = DateTime.Now;
            coupon.EntityId = entityId;

            if (coupon.Days != null)
                coupon.ExpirationDate = DateTime.Now.AddDays(coupon.Days.Value);

            AddCoupon(coupon);

            foreach (var categoryId in couponTemplate.CategoryIds)
                AddCategoryToCoupon(coupon.CouponID, categoryId);

            foreach (var productId in couponTemplate.ProductsIds)
                AddProductToCoupon(coupon.CouponID, productId);

            foreach (var customerGroupId in couponTemplate.CustomerGroupIds)
                AddCustomerGroupToCoupon(coupon.CouponID, customerGroupId);

            return coupon;
        }

        public static Coupon GeneratePartnerCoupon(Coupon couponTemplate, string code)
        {
            var coupon = couponTemplate.DeepClone();
            coupon.Code = code;
            coupon.Mode = CouponMode.Partner;
            coupon.AddingDate = DateTime.Now;

            if (coupon.Days != null)
                coupon.ExpirationDate = DateTime.Now.AddDays(coupon.Days.Value);

            AddCoupon(coupon);

            foreach (var categoryId in couponTemplate.CategoryIds)
                AddCategoryToCoupon(coupon.CouponID, categoryId);

            foreach (var productId in couponTemplate.ProductsIds)
                AddProductToCoupon(coupon.CouponID, productId);

            foreach (var customerGroupId in couponTemplate.CustomerGroupIds)
                AddCustomerGroupToCoupon(coupon.CouponID, customerGroupId);

            return coupon;
        }

        public static Coupon GetGeneratedTriggerCouponByCustomerId(int triggerId, Guid customerId, int entityId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[Coupon] WHERE TriggerId=@TriggerId and CustomerId=@CustomerId and Mode=@Mode and EntityId=@EntityId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerId", triggerId),
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@EntityId", entityId),
                new SqlParameter("@Mode", (int)CouponMode.Generated));
        }
        
        public static void RefreshCouponProducts(ShoppingCart cart)
        {
            var coupon = cart.Coupon;
            var isAppliedToCard = coupon != null && coupon.IsAppliedToCard(cart);
            
            if (coupon == null || 
                !isAppliedToCard || 
                (coupon.Type == CouponType.FixedOnGiftOffer && coupon.GiftOfferId.HasValue && 
                 cart.Any(x => x.IsByCoupon && (x.OfferId != coupon.GiftOfferId.Value || x.CustomPrice != coupon.GetRate())))
               )
            {
                var giftItem = cart.FirstOrDefault(x => x.IsByCoupon);
                if (giftItem != null)
                {
                    ShoppingCartService.DeleteShoppingCartItem(giftItem);
                    cart.Remove(giftItem);
                    
                    if (coupon != null)
                        DeleteCustomerCoupon(coupon.CouponID);
                    cart.Coupon = null;
                }
            }
            else if (isAppliedToCard && coupon.Type == CouponType.FixedOnGiftOffer && 
                     coupon.GiftOfferId.HasValue && coupon.GiftOfferId != 0)
            {
                if (!cart.Any(x => x.IsByCoupon && x.OfferId == coupon.GiftOfferId.Value))
                {
                    var product = ProductService.GetProductByOfferId(coupon.GiftOfferId.Value);
                    if (product != null)
                    {
                        var cartItem = new ShoppingCartItem
                        {
                            OfferId = coupon.GiftOfferId.Value,
                            Amount = product.GetMinAmount(),
                            CustomPrice = coupon.GetRate(),
                            IsForbiddenChangeAmount = true,
                            IsByCoupon = true
                        };
                        ShoppingCartService.AddShoppingCartItem(cartItem);
                        cart.Add(cartItem);
                    }
                }
            }
        }

        #region Partner Coupon

        public static Coupon GetPartnersCouponTemplate()
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[Coupon] WHERE Mode=@Mode",
                CommandType.Text, GetFromReader,
                new SqlParameter("@Mode", (int)CouponMode.PartnersTemplate));
        }

        #endregion

        #region Offer links

        public static void AddUpdateOfferToCoupon(int couponId, int offerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT TOP 1 * FROM [Catalog].[CouponOffers] WHERE CouponID = @couponId)
                BEGIN   
                    INSERT INTO [Catalog].[CouponOffers] (CouponID,  OfferID) VALUES (@couponId, @offerId)
                END
                ELSE
                BEGIN
                    UPDATE [Catalog].[CouponOffers] SET OfferID = @offerId WHERE CouponID = @couponId
                END",
                CommandType.Text,
                new SqlParameter("@couponId", couponId),
                new SqlParameter("@offerId", offerId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        public static int GetOfferIdByCoupon(int couponId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 OfferID FROM [Catalog].[CouponOffers] WHERE CouponID=@couponId", 
                CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "OfferID"),
                new SqlParameter("@couponId", couponId));
        }

        public static void DeleteOfferFromCoupon(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponOffers] Where couponID=@couponId",
                                            CommandType.Text,
                                            new SqlParameter("@couponId", couponId));
            
            CacheManager.Remove(CouponCachePrefix + couponId);
        }

        #endregion

    }
}