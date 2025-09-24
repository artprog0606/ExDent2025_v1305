using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Crm.DealStatuses
{
    public static class DealStatusService
    {
        private const string DealStatusCacheKey = "DealStatus_";

        public static List<DealStatus> GetDefaultDealStatuses()
        {
            return new List<DealStatus>()
            {
                new DealStatus() { Name = "Новый", Color ="8bc34a",  SortOrder = 10},
                //new DealStatus() { Name = "Созвон с клиентом", Color ="ffc73e",  SortOrder = 20},
                //new DealStatus() { Name = "Выставление КП", Color ="1ec5b8",  SortOrder = 30},
                //new DealStatus() { Name = "Ожидание решения клиента", Color ="78909c",  SortOrder = 40},
                new DealStatus() { Name = "Сделка заключена", Color ="000000",  SortOrder = 50, Status = SalesFunnelStatusType.FinalSuccess},
                new DealStatus() { Name = "Сделка отклонена", Color ="b0bec5",  SortOrder = 60, Status = SalesFunnelStatusType.Canceled},
            };
        }

        public static List<DealStatus> GetList()
        {
            return CacheManager.Get(DealStatusCacheKey + "alllist",
                () => SQLDataAccess.Query<DealStatus>("Select * From CRM.DealStatus Order by Status, SortOrder").ToList());
        }

        public static List<DealStatus> GetList(int salesFunnelId)
        {
            return CacheManager.Get(DealStatusCacheKey + "list_" + salesFunnelId,
                () => SQLDataAccess.Query<DealStatus>(
                    "Select * From CRM.DealStatus s " +
                    "Inner Join CRM.SalesFunnel_DealStatus fs On fs.DealStatusId=s.Id " +
                    "Where fs.SalesFunnelId=@salesFunnelId " +
                    "Order by s.Status, SortOrder", 
                    new {salesFunnelId}).ToList());
        }

        public static List<DealStatusWithCount> GetListWithCount(int salesFunnelId)
        {
            var sql = "Select s.*, (Select Count(Id) From [Order].[Lead] Where DealStatusId=s.Id {0}) as leadsCount " +
                      "From CRM.DealStatus s " +
                      "Inner Join CRM.SalesFunnel_DealStatus fs On fs.DealStatusId=s.Id " +
                      "Where fs.SalesFunnelId=@salesFunnelId " +
                      "Order by s.Status, SortOrder";

            var subsql = "";

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.Assigned)
                    {
                        subsql += " and Lead.ManagerId = " + manager.ManagerId;
                    }
                    else if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.AssignedAndFree)
                    {
                        subsql += " and (Lead.ManagerId = " + manager.ManagerId + " or Lead.ManagerId is null)";
                    }

                    subsql +=
                        " and (Exists ( " +
                        "Select 1 From [CRM].[SalesFunnel_Manager] " +
                        "Where (SalesFunnel_Manager.SalesFunnelId = Lead.SalesFunnelId and SalesFunnel_Manager.ManagerId = "+ manager.ManagerId + ") " +
                        ") OR " +
                        "(Select Count(*) From [CRM].[SalesFunnel_Manager] Where SalesFunnel_Manager.SalesFunnelId = Lead.SalesFunnelId) = 0)";
                }
            }

            return CacheManager.Get(DealStatusCacheKey + "list_withcount_" + salesFunnelId + "_" + customer.Id,
                () => SQLDataAccess.Query<DealStatusWithCount>(string.Format(sql, subsql), new { salesFunnelId }).ToList());
        }

        public static DealStatus Get(int id)
        {
            return CacheManager.Get(DealStatusCacheKey + id,
                () =>
                    SQLDataAccess.Query<DealStatus>("Select * From CRM.DealStatus Where Id = @id", new {id})
                        .FirstOrDefault());
        }

        public static int Add(DealStatus status)
        {
            CacheManager.RemoveByPattern(DealStatusCacheKey);

            status.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO CRM.DealStatus (Name, SortOrder, Color, Status) VALUES (@Name, @SortOrder, @Color, @Status); SELECT SCOPE_IDENTITY()",
                CommandType.Text,
                new SqlParameter("@Name", status.Name),
                new SqlParameter("@SortOrder", status.SortOrder),
                new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object)DBNull.Value),
                new SqlParameter("@Status", (int)status.Status));

            return status.Id;
        }

        public static int Update(DealStatus status)
        {
            CacheManager.RemoveByPattern(DealStatusCacheKey);

            return SQLDataAccess.ExecuteScalar<int>(
                "UPDATE CRM.DealStatus SET Name = @Name, SortOrder = @SortOrder, Color = @Color WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", status.Id),
                new SqlParameter("@Name", status.Name),
                new SqlParameter("@SortOrder", status.SortOrder),
                new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object)DBNull.Value));
        }

        public static void Delete(int id)
        {
            var funnel = SalesFunnelService.GetByDealStatus(id);
            if (funnel != null)
            {
                var statuses = GetList(funnel.Id).Where(x => x.Status != SalesFunnelStatusType.Canceled && x.Status != SalesFunnelStatusType.FinalSuccess).ToList();

                DealStatus newStatus = null;

                for (int i = 0; i < statuses.Count; i++)
                {
                    if (statuses[i].Id == id)
                        break;
                    newStatus = statuses[i];
                }

                if (newStatus == null)
                    newStatus = statuses.FirstOrDefault(x => x.Id != id);

                if (newStatus != null)
                    SQLDataAccess.ExecuteNonQuery(
                        "Update [Order].[Lead] Set DealStatusId=@NewDealStatusId WHERE DealStatusId = @OldDealStatusId",
                        CommandType.Text,
                        new SqlParameter("@NewDealStatusId", newStatus.Id),
                        new SqlParameter("@OldDealStatusId", id));
            }

            SQLDataAccess.ExecuteNonQuery("DELETE FROM CRM.DealStatus WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
            CacheManager.RemoveByPattern(DealStatusCacheKey);
        }

        public static void ClearCache()
        {
            CacheManager.RemoveByPattern(DealStatusCacheKey);
        }
    }
}
