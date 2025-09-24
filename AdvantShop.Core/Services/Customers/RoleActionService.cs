using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;

namespace AdvantShop.Customers
{
    public class RoleActionService
    {
        #region Customer Role Action

        public static List<RoleAction> GetRoleActionsByCustomerId(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList<RoleAction>("Select RoleActionKey From Customers.CustomerRoleAction Where CustomerRoleAction.CustomerID=@CustomerID And Enabled='True'", CommandType.Text,
                                                                    reader => SQLDataHelper.GetString(reader, "RoleActionKey").TryParseEnum<RoleAction>(),
                                                                    new SqlParameter("@CustomerID", customerId));
        }

        public static void UpdateOrInsertCustomerRoleAction(Guid customerId, string roleActionKey, bool enabled)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_UpdateInserCustomerRoleAction]", CommandType.StoredProcedure,
                                                new SqlParameter("@CustomerID", customerId),
                                                new SqlParameter("@RoleActionKey", roleActionKey),
                                                new SqlParameter("@Enabled", enabled));

            var cacheName = CacheNames.GetRoleActionsCacheObjectName(customerId.ToString());
            if (CacheManager.Contains(cacheName))
                CacheManager.RemoveByPattern(cacheName);
        }

        public static void DeleteCustomerRoleActions(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From Customers.CustomerRoleAction Where CustomerID = @CustomerID", CommandType.Text,
                                                new SqlParameter("@CustomerID", customerId));

            var cacheName = CacheNames.GetRoleActionsCacheObjectName(customerId.ToString());
            if (CacheManager.Contains(cacheName))
            {
                CacheManager.RemoveByPattern(cacheName);
            }
        }


        /// <summary>
        /// Действия доступные данному пользователю
        /// </summary>
        /// <returns></returns>
        public static List<CustomerRoleAction> GetCustomerRoleActionsByCustomerId(Guid customerId)
        {
            List<CustomerRoleAction> actions;

            var cacheName = CacheNames.GetRoleActionsCacheObjectName(customerId.ToString());

            if (CacheManager.TryGetValue(cacheName, out actions))
                return actions;

            actions = GetCustomerRoleActionsByCustomerIdFromDb(customerId);

            CacheManager.Insert(cacheName, actions ?? new List<CustomerRoleAction>());

            return actions;
        }

        private static List<CustomerRoleAction> GetCustomerRoleActionsByCustomerIdFromDb(Guid customerId)
        {
            return
                SQLDataAccess.ExecuteReadList<CustomerRoleAction>(
                    "Select * From Customers.CustomerRoleAction Where CustomerID = @CustomerID",
                    CommandType.Text,
                    GetCustomerRoleActionFromReader,
                    new SqlParameter("@CustomerID", customerId));
        }

        public static bool HasCustomerRoleAction(Guid customerId, RoleAction key)
        {
            return GetCustomerRoleActionsByCustomerId(customerId).Find(x => x.Role == key) != null;
        }

        public static bool HasCurrentCustomerRoleAction(RoleAction key)
        {
            if (CustomerContext.CurrentCustomer.IsAdmin)
                return true;

            if (CustomerContext.CurrentCustomer.IsModerator)
            {
                return GetCustomerRoleActionsByCustomerId(CustomerContext.CurrentCustomer.Id).Find(x => x.Role == key) != null;
            }
            else
                return false;
        }


        private static CustomerRoleAction GetCustomerRoleActionFromReader(SqlDataReader reader)
        {
            return new CustomerRoleAction
            {
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerID"),
                Role = SQLDataHelper.GetString(reader, "RoleActionKey").TryParseEnum<RoleAction>()
            };
        }

        #endregion

        public static List<RoleAction> GetBizProcessRoleActions(EBizProcessEventType bizProcessEventType)
        {
            var roleActions = new List<RoleAction> { RoleAction.Tasks };
            switch (bizProcessEventType)
            {
                case EBizProcessEventType.OrderCreated:
                case EBizProcessEventType.OrderStatusChanged:
                case EBizProcessEventType.OrderManagerAssigned:
                    roleActions.Add(RoleAction.Orders);
                    break;
                case EBizProcessEventType.LeadCreated:
                case EBizProcessEventType.LeadStatusChanged:
                case EBizProcessEventType.CallMissed:
                    roleActions.Add(RoleAction.Crm);
                    break;
                case EBizProcessEventType.ReviewAdded:
                    roleActions.Add(RoleAction.Catalog);
                    break;
                case EBizProcessEventType.MessageReply:
                    roleActions.Add(RoleAction.Customers);
                    break;
                case EBizProcessEventType.TaskCreated:
                case EBizProcessEventType.TaskStatusChanged:
                    break;
                case EBizProcessEventType.BookingCreated:
                case EBizProcessEventType.BookingChanged:
                case EBizProcessEventType.BookingDeleted:
                    roleActions.Add(RoleAction.Booking);
                    break;
                default:
                    throw new NotImplementedException("No implementation for event type " + bizProcessEventType);
            }
            return roleActions;
        }
    }
}