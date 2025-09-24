using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Crm.SalesFunnels
{
    /// <summary>
    /// Воронки продаж в CRM
    /// </summary>
    public class SalesFunnelService
    {
        private const string SalesFunnelCacheKey = "SalesFunnel_";
        private const string LastAdminFunnelCookieName = "leads_lastFunnel";

        #region Sales Funnel

        public static SalesFunnel Get(int id)
        {
            if (id <= 0)
                return null;

            return CacheManager.Get(SalesFunnelCacheKey + id,
                () =>
                    SQLDataAccess.Query<SalesFunnel>("Select * From [CRM].[SalesFunnel] Where Id=@id", new { id })
                        .FirstOrDefault());
        }

        public static SalesFunnel Get(string name)
        {
            return SQLDataAccess.Query<SalesFunnel>("Select * From [CRM].[SalesFunnel] Where Name=@Name", new { name }).FirstOrDefault();
        }


        public static List<SalesFunnel> GetList()
        {
            return CacheManager.Get(SalesFunnelCacheKey + "list",
                () => SQLDataAccess.Query<SalesFunnel>("Select * From [CRM].[SalesFunnel] Order by Enable Desc, SortOrder").ToList());
        }

        public static int GetCount(string condition = null, params SqlParameter[] parameters)
        {
            if (condition == null)
                return SQLDataAccess.ExecuteScalar<int>("Select Count(Id) From [CRM].[SalesFunnel]", CommandType.Text);
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(Id) From [CRM].[SalesFunnel] WHERE " + condition,
                CommandType.Text, parameters);
        }

        public static void Add(SalesFunnel funnel)
        {
            if (SaasDataService.IsSaasEnabled && GetCount("[Enable] = 1") >= SaasDataService.CurrentSaasData.LeadsListsCount)
                throw new BlException("Количество воронок по тарифному плану не может превышать " + SaasDataService.CurrentSaasData.LeadsListsCount);

            funnel.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into [CRM].[SalesFunnel] (Name, SortOrder, FinalSuccessAction, NotSendNotificationsOnLeadCreation, NotSendNotificationsOnLeadChanged, Enable, LeadAutoCompleteActionType) " +
                    "Values(@Name, @SortOrder, @FinalSuccessAction, @NotSendNotificationsOnLeadCreation, @NotSendNotificationsOnLeadChanged, @Enable, @LeadAutoCompleteActionType); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Name", funnel.Name),
                    new SqlParameter("@SortOrder", funnel.SortOrder),
                    new SqlParameter("@FinalSuccessAction", (int)funnel.FinalSuccessAction),
                    new SqlParameter("@NotSendNotificationsOnLeadCreation", funnel.NotSendNotificationsOnLeadCreation),
                    new SqlParameter("@NotSendNotificationsOnLeadChanged", funnel.NotSendNotificationsOnLeadChanged),
                    new SqlParameter("@Enable", funnel.Enable),
                    new SqlParameter("@LeadAutoCompleteActionType", funnel.LeadAutoCompleteActionType));

            CacheManager.RemoveByPattern(SalesFunnelCacheKey);
        }

        public static void Update(SalesFunnel funnel)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CRM].[SalesFunnel] " +
                "Set Name=@Name, SortOrder=@SortOrder, FinalSuccessAction=@FinalSuccessAction, NotSendNotificationsOnLeadCreation=@NotSendNotificationsOnLeadCreation, " +
                "NotSendNotificationsOnLeadChanged=@NotSendNotificationsOnLeadChanged, Enable=@Enable, LeadAutoCompleteActionType=@LeadAutoCompleteActionType " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", funnel.Id),
                new SqlParameter("@Name", funnel.Name),
                new SqlParameter("@SortOrder", funnel.SortOrder),
                new SqlParameter("@FinalSuccessAction", (int)funnel.FinalSuccessAction),
                new SqlParameter("@NotSendNotificationsOnLeadCreation", funnel.NotSendNotificationsOnLeadCreation),
                new SqlParameter("@NotSendNotificationsOnLeadChanged", funnel.NotSendNotificationsOnLeadChanged),
                new SqlParameter("@Enable", funnel.Enable),
                new SqlParameter("@LeadAutoCompleteActionType", funnel.LeadAutoCompleteActionType));

            CacheManager.RemoveByPattern(SalesFunnelCacheKey);
        }

        public static void Delete(int id)
        {
            var funnel = Get(id);
            if (funnel == null)
                return;

            if (id == SettingsCrm.DefaultSalesFunnelId)
                throw new BlException(LocalizationService.GetResourceFormat("Core.Crm.SalesFunnels.Errors.CantDeleteDefaultSalesFunnel", funnel.Name));

            if (GetCount() <= 1)
                throw new BlException(LocalizationService.GetResource("Core.Crm.SalesFunnels.Errors.CantDeleteLastSalesFunnel"));

            var leadsCount = SQLDataAccess.ExecuteScalar<int>("Select count(*) id From [Order].[Lead] Where SalesFunnelId=@SalesFunnelId", CommandType.Text, new SqlParameter("@SalesFunnelId", id));
            if (leadsCount > 0)
                throw new BlException(LocalizationService.GetResourceFormat("Core.Crm.SalesFunnels.Errors.CantDeleteNotEmptySalesFunnel", funnel.Name));

            foreach (var dealStatus in DealStatusService.GetList(id))
                DealStatusService.Delete(dealStatus.Id);

            SQLDataAccess.ExecuteNonQuery("Delete From [CRM].[SalesFunnel] Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(SalesFunnelCacheKey);
        }

        public static bool CheckAccess(SalesFunnel funnel)
        {
            return CheckAccess(funnel.Id, CustomerContext.CurrentCustomer);
        }

        public static bool CheckAccess(int salesFunnelId, Customer customer)
        {
            if (customer.IsAdmin || customer.IsVirtual)
                return true;

            var managers = GetSalesFunnelManagers(salesFunnelId);
            if (managers == null || managers.Count == 0)
                return true;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled && managers.Any(x => x.ManagerId == manager.ManagerId))
                    return true;
            }
            return false;
        }

        public static void DeactivateSalesFunnelMoreThan(int activeLeadsListsCount)
        {
            if (activeLeadsListsCount <= 0)
                return;

            SQLDataAccess.ExecuteNonQuery(
                @"if(Select (Count(Id)) From [CRM].[SalesFunnel] Where [Enable] = 1 ) > @activeLeadsListsCount
                  BEGIN
                    ;WITH salesFunnelToDeactivate AS 
                    ( 
	                    Select 
	                    Top(Select (Count(Id) - @activeLeadsListsCount) From [CRM].[SalesFunnel] Where [SalesFunnel].Enable = 1) [SalesFunnel].Id 
	                    From [CRM].[SalesFunnel]
	                    Where [SalesFunnel].Enable = 1 
	                    Order by [SalesFunnel].[Id] Desc
                    ) 
                    UPDATE [CRM].[SalesFunnel] SET [Enable] = 0 Where Id in (Select Id from salesFunnelToDeactivate)
                  END",
                CommandType.Text,
                new SqlParameter("@activeLeadsListsCount", activeLeadsListsCount));
        }

        #endregion

        #region SalesFunnel DealStatus

        public static void AddDealStatus(int salesFunnelId, int dealStatusId)
        {
            SQLDataAccess.ExecuteNonQuery(
                    "Insert Into [CRM].[SalesFunnel_DealStatus] (SalesFunnelId, DealStatusId) Values (@SalesFunnelId, @DealStatusId)",
                    CommandType.Text,
                    new SqlParameter("@SalesFunnelId", salesFunnelId),
                    new SqlParameter("@DealStatusId", dealStatusId));
        }

        public static SalesFunnel GetByDealStatus(int dealStatusId)
        {
            return SQLDataAccess.Query<SalesFunnel>(
                "Select sf.* From [CRM].[SalesFunnel] sf INNER JOIN [CRM].[SalesFunnel_DealStatus] sfds ON sfds.SalesFunnelId = sf.Id Where DealStatusId = @dealStatusId",
                new { dealStatusId }).FirstOrDefault();
        }

        #endregion

        #region SalesFunnel Managers

        public static List<Manager> GetSalesFunnelManagers(int salesFunnelId)
        {
            return SQLDataAccess.Query<Manager>(
                "SELECT Managers.* FROM Customers.Managers " +
                "INNER JOIN [CRM].SalesFunnel_Manager ON SalesFunnel_Manager.ManagerId = Managers.ManagerId " +
                "WHERE SalesFunnel_Manager.SalesFunnelId = @id",
                new { id = salesFunnelId }).ToList();
        }

        public static void ClearSalesFunnelManagers(int salesFunnelId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [CRM].SalesFunnel_Manager WHERE SalesFunnelId = @salesFunnelId", CommandType.Text,
                new SqlParameter("@salesFunnelId", salesFunnelId));
        }

        public static void AddSalesFunnelManager(int salesFunnelId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [CRM].SalesFunnel_Manager (SalesFunnelId, ManagerId) VALUES (@SalesFunnelId, @ManagerId)",
                CommandType.Text,
                new SqlParameter("@SalesFunnelId", salesFunnelId),
                new SqlParameter("@ManagerId", managerId));
        }

        public Dictionary<int, int> GetSalesFunnelsManagerIds()
        {
            return SQLDataAccess.Query<KeyValuePair<int, int>>(
                "SELECT ManagerId as [Key], SalesFunnelId as [Value] FROM [CRM].SalesFunnel_Manager").ToDictionary(x => x.Key, x => x.Value);
        }

        #endregion

        #region Lead Auto Complete

        public static List<SalesFunnel> GetByLeadAutoCompleteAction(ELeadAutoCompleteActionType actionType)
        {
            return SQLDataAccess.Query<SalesFunnel>(
                "SELECT * FROM [CRM].[SalesFunnel] WHERE LeadAutoCompleteActionType = @ActionType",
                new { actionType }).ToList();
        }

        public static List<int> GetLeadAutoCompleteCategoryIds(int salesFunnelId)
        {
            return SQLDataAccess.Query<int>(
                "SELECT CategoryId FROM CRM.SalesFunnel_LeadAutoCompleteCategory WHERE SalesFunnelId = @SalesFunnelId", new { salesFunnelId }).ToList();
        }

        public static void ClearLeadAutoCompleteCategories(int salesFunnelId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM CRM.SalesFunnel_LeadAutoCompleteCategory WHERE SalesFunnelId = @SalesFunnelId", CommandType.Text,
                new SqlParameter("@SalesFunnelId", salesFunnelId));
        }

        public static void AddLeadAutoCompleteCategory(int salesFunnelId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO CRM.SalesFunnel_LeadAutoCompleteCategory (SalesFunnelId, CategoryId) VALUES (@SalesFunnelId, @CategoryId)",
                CommandType.Text,
                new SqlParameter("@SalesFunnelId", salesFunnelId),
                new SqlParameter("@CategoryId", categoryId));
        }

        public static Dictionary<int, string> GetLeadAutoCompleteCategoriesInfo(List<int> categoryIds)
        {
            var result = new Dictionary<int, string>();
            foreach (var categoryId in categoryIds)
            {
                var category = CategoryService.GetCategory(categoryId);
                if (category != null && !result.ContainsKey(categoryId))
                    result.Add(category.CategoryId, category.Name);
            }
            return result;
        }


        public static List<int> GetLeadAutoCompleteProductIds(int salesFunnelId)
        {
            return SQLDataAccess.Query<int>(
                "SELECT ProductId FROM CRM.SalesFunnel_LeadAutoCompleteProduct WHERE SalesFunnelId = @SalesFunnelId", new { salesFunnelId }).ToList();
        }

        public static void ClearLeadAutoCompleteProducts(int salesFunnelId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM CRM.SalesFunnel_LeadAutoCompleteProduct WHERE SalesFunnelId = @SalesFunnelId", CommandType.Text,
                new SqlParameter("@SalesFunnelId", salesFunnelId));
        }

        public static void AddLeadAutoCompleteProduct(int salesFunnelId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO CRM.SalesFunnel_LeadAutoCompleteProduct (SalesFunnelId, ProductId) VALUES (@SalesFunnelId, @ProductId)",
                CommandType.Text,
                new SqlParameter("@SalesFunnelId", salesFunnelId),
                new SqlParameter("@ProductId", productId));
        }

        public static Dictionary<int, string> GetLeadAutoCompleteProductsInfo(List<int> productIds)
        {
            var result = new Dictionary<int, string>();
            foreach (var productId in productIds)
            {
                var product = ProductService.GetProduct(productId);
                if (product != null && !result.ContainsKey(productId))
                    result.Add(product.ProductId, string.Format("[{0}] {1}", product.ArtNo, product.Name));
            }
            return result;
        }

        #endregion

        #region Last Admin Funnel

        public static int? GetLastAdminFunnel()
        {
            return CommonHelper.GetCookieString(LastAdminFunnelCookieName).TryParseInt(true);
        }

        public static void SetLastAdminFunnel(int? salesFunnelId)
        {
            if (!salesFunnelId.HasValue)
                CommonHelper.DeleteCookie(LastAdminFunnelCookieName);
            else
                CommonHelper.SetCookie(LastAdminFunnelCookieName, salesFunnelId.Value.ToString(), new TimeSpan(365, 0, 0, 0), false);
        }

        #endregion
    }
}
