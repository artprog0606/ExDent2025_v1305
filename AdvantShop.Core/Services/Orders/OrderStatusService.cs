//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.Orders
{
    public class OrderStatusService
    {
        private const string OrderStatusCachePrefix = "OrderStatus_";
        
        public static int DefaultOrderStatus
        {
            get
            {
                return CacheManager.Get(OrderStatusCachePrefix + "DefaultOrderStatus",
                    () =>
                        SQLDataAccess.ExecuteScalar<int>(
                            "SELECT OrderStatusID FROM [Order].[OrderStatus] WHERE [IsDefault] = 'True'",
                            CommandType.Text));
            }
        }

        public static int CanceledOrderStatus
        {
            get
            {
                return CacheManager.Get(OrderStatusCachePrefix + "CanceledOrderStatus",
                    () =>
                        SQLDataAccess.ExecuteScalar<int>(
                            "SELECT top(1) OrderStatusID FROM [Order].[OrderStatus] WHERE [IsCanceled] = 'True'",
                            CommandType.Text));
            }
        }

        public static bool StatusCanBeDeleted(int statusId)
        {
            if (statusId == DefaultOrderStatus)
                return false;
            return GetOrderCountByStatusId(statusId) <= 0;
        }

        public static int GetOrderCountByStatusId(object statusId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Order].[Order] WHERE [OrderStatusID] = @StatusID and IsDraft=0",
                CommandType.Text,
                new SqlParameter("@StatusID", statusId));
        }

        public static int GetOrderCountByPaymentStatus(bool isPaid)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Order].[Order] WHERE IsDraft=0 and " + (isPaid ? "PaymentDate is not null" : "PaymentDate is null"),
                CommandType.Text);
        }

        public static int GetCountDraftOrder()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Order].[Order] WHERE IsDraft = 1",
                CommandType.Text);
        }

        public static int GetOrderCountByStatusIdAndManagerId(object statusId, int? managerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Order].[Order] WHERE [Order].[Order].[OrderStatusID] = @StatusID AND [Order].[Order].ManagerId = @managerId and IsDraft=0",
                CommandType.Text,
                new SqlParameter("@StatusID", statusId),
                new SqlParameter("@managerId", managerId));
        }


        public static List<OrderStatus> GetOrderStatuses()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Order].OrderStatus Order By SortOrder", CommandType.Text,
                                                 GetOrderStatusFromReader);
        }

        private static OrderStatus GetOrderStatusFromReader(SqlDataReader reader)
        {
            return new OrderStatus
            {
                StatusID = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                StatusName = SQLDataHelper.GetString(reader, "StatusName"),
                Command = (OrderStatusCommand) SQLDataHelper.GetInt(reader, "CommandID"),
                IsCanceled = SQLDataHelper.GetBoolean(reader, "IsCanceled"),
                IsDefault = SQLDataHelper.GetBoolean(reader, "IsDefault"),
                IsCompleted = SQLDataHelper.GetBoolean(reader, "IsCompleted"),
                Color = SQLDataHelper.GetString(reader, "Color"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                CancelForbidden = SQLDataHelper.GetBoolean(reader, "CancelForbidden"),
                ShowInMenu = SQLDataHelper.GetBoolean(reader, "ShowInMenu"),
                ChangePaymentForbidden = SQLDataHelper.GetBoolean(reader, "ChangePaymentForbidden")
            };
        }

        public static string GetStatusName(int idStatus)
        {
            return SQLDataAccess.ExecuteScalar<string>(
                "SELECT StatusName FROM [Order].[OrderStatus]  WHERE OrderStatusID = @OrderStatusID",
                CommandType.Text, new SqlParameter("OrderStatusID", idStatus));
        }

        public static void ChangeOrderStatusForNewOrder(int orderId, string basis = null)
        {
            var statusId = GetOrderStatusId(orderId);

            if (statusId != 0 && statusId != DefaultOrderStatus) 
                return;

            ChangeOrderStatus(orderId, DefaultOrderStatus,
                !string.IsNullOrEmpty(basis)
                    ? basis
                    : LocalizationService.GetResource("Core.OrderStatus.Created"),
                false);
        }

        public static void ChangeOrderStatus(int orderId, int statusId, string basis, bool updateModules = true, bool isDraftChanged = false)
        {
            var order = OrderService.GetOrder(orderId);

            if (order == null)
                throw new Exception("Order is null");

            var prevStatus = order.OrderStatus;
            var newStatus = GetOrderStatus(statusId);

            if (newStatus == null)
                throw new Exception("Status is null");

            if (prevStatus != null && prevStatus.StatusID == newStatus.StatusID && !newStatus.IsDefault && !isDraftChanged)
                return;

            var user = CustomerContext.CurrentCustomer ?? new Customer();

            var history = new OrderStatusHistory()
            {
                OrderID = orderId,
                CustomerID = user.IsAdmin || user.IsManager || user.IsModerator ? user.Id : (Guid?)null,
                CustomerName = user.IsAdmin || user.IsManager || user.IsModerator ? user.FirstName + " " + user.LastName : string.Empty,
                PreviousStatus = prevStatus != null ? prevStatus.StatusName : string.Empty,
                NewStatus = newStatus.StatusName,
                Basis = basis
            };

            if (prevStatus != null && !prevStatus.Hidden && !order.IsDraft)
            {
                SQLDataAccess.ExecuteNonQuery("update [order].[order] set previousstatus=@prevStatus where orderid=@orderid", CommandType.Text,
                    new SqlParameter("@prevStatus", prevStatus.StatusName), new SqlParameter("@OrderId", orderId));
            }

            var command = ChangeOrderStatusInDb(orderId, statusId);

            if (order.IsDraft)
                return;

            AddOrderStatusHistory(history);

            order.OrderStatusId = statusId;
            order.OrderStatus = null;

            if (SettingsCheckout.DecrementProductsCount && !order.IsDraft && command != null)
            {
                if (command == (int)OrderStatusCommand.Increment)
                {
                    RedistributeStocksService.Redistribute(order);
                    OrderService.IncrementProductsCountAccordingOrder(order, history);
                }
                else if (command == (int)OrderStatusCommand.Decrement)
                {
                    RedistributeStocksService.Redistribute(order);
                    OrderService.DecrementProductsCountAccordingOrder(order, history);
                }
            }

            if (BonusSystem.IsActive)
            {
                var status = GetOrderStatus(statusId);
                if (status != null && status.IsCanceled)
                {
                    BonusSystemService.CancelPurchase(order.BonusCardNumber, order.Number, orderId);
                }
            }

            OrderHistoryService.ChangingStatus(history);

            if (updateModules)
            {
                ModulesExecuter.OrderChangeStatus(orderId);
            }

            GoogleAnalyticsService.SendOrder(order);

            BizProcessExecuter.OrderStatusChanged(order);
            Core.Services.Api.ApiWebhookExecuter.OrderStatusChanged(order);
            TriggerProcessService.ProcessEvent(ETriggerEventType.OrderStatusChanged, order);
        }

        private static int? ChangeOrderStatusInDb(int orderId, int statusId)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[Order].[sp_GetChangeOrderStatus]", CommandType.StoredProcedure,
                                                         new SqlParameter("@OrderID", orderId),
                                                         new SqlParameter("@OrderStatusID", statusId)));
        }

        public static int AddOrderStatus(OrderStatus status)
        {
            status.StatusID =
                SQLDataAccess.ExecuteScalar<int>("[Order].[sp_AddOrderStatus]", CommandType.StoredProcedure,
                    new SqlParameter("@OrderStatusID", status.StatusID),
                    new SqlParameter("@StatusName", status.StatusName),
                    new SqlParameter("@CommandID", (int) status.Command),
                    new SqlParameter("@IsDefault", status.IsDefault),
                    new SqlParameter("@IsCanceled", status.IsCanceled),
                    new SqlParameter("@Hidden", status.Hidden),
                    new SqlParameter("@IsCompleted", status.IsCompleted),
                    new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object) DBNull.Value),
                    new SqlParameter("@SortOrder", status.SortOrder),
                    new SqlParameter("@CancelForbidden", status.CancelForbidden),
                    new SqlParameter("@ShowInMenu", status.ShowInMenu),
                    new SqlParameter("@ChangePaymentForbidden", status.ChangePaymentForbidden)
                );
            
            CacheManager.RemoveByPattern(OrderStatusCachePrefix);

            return status.StatusID;
        }

        public static void UpdateOrderStatus(OrderStatus status)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderStatus]", CommandType.StoredProcedure,
                new SqlParameter("@OrderStatusID", status.StatusID),
                new SqlParameter("@StatusName", status.StatusName),
                new SqlParameter("@CommandID", (int) status.Command),
                new SqlParameter("@IsDefault", status.IsDefault),
                new SqlParameter("@IsCanceled", status.IsCanceled),
                new SqlParameter("@Hidden", status.Hidden),
                new SqlParameter("@IsCompleted", status.IsCompleted),
                new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object) DBNull.Value),
                new SqlParameter("@SortOrder", status.SortOrder),
                new SqlParameter("@CancelForbidden", status.CancelForbidden),
                new SqlParameter("@ShowInMenu", status.ShowInMenu),
                new SqlParameter("@ChangePaymentForbidden", status.ChangePaymentForbidden)
                );
            
            CacheManager.RemoveByPattern(OrderStatusCachePrefix);
        }


        public static bool DeleteOrderStatus(int orderStatusId)
        {
            if (!StatusCanBeDeleted(orderStatusId))
                return false;

            var result = 
                SQLDataAccess.ExecuteScalar<int>(
                "[Order].[sp_DeleteOrderStatus]",
                CommandType.StoredProcedure,
                new SqlParameter("@OrderStatusID", orderStatusId)) == 1;
            
            CacheManager.RemoveByPattern(OrderStatusCachePrefix);

            return result;
        }

        public static OrderStatus GetOrderStatus(int orderStatusId)
        {
            return
                    SQLDataAccess.ExecuteReadOne(
                        "SELECT TOP 1 * FROM [Order].[OrderStatus] WHERE [OrderStatusID] = @OrderStatusID",
                        CommandType.Text,
                        GetOrderStatusFromReader,
                        new SqlParameter("@OrderStatusID", orderStatusId));
        }

        public static int GetOrderStatusId(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT [OrderStatusID] FROM [Order].[Order] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId));
        }


        public static List<Order> GetOrdersByStatusId(int statusId)
        {
            return SQLDataAccess.ExecuteReadList<Order>(
                "SELECT * FROM [Order].[Order] WHERE [OrderStatusID] = @OrderStatusID",
                CommandType.Text,
                OrderService.GetOrderFromReader,
                new SqlParameter("@OrderStatusID", statusId));
        }

        public static List<Order> GetOrdersByStatusId(DateTime from, DateTime to, int? statusId = null)
        {
            var query = "SELECT * FROM [Order].[Order] WHERE ";
            var queryParams = new List<SqlParameter>();

            if (statusId != null && statusId != 0)
            {
                query += "[OrderStatusID] = @OrderStatusID and ";
                queryParams.Add(new SqlParameter("@OrderStatusID", statusId));
            }

            query += "[OrderDate] >= @From and [OrderDate] <= @To";
            queryParams.Add(new SqlParameter("@From", from));
            queryParams.Add(new SqlParameter("@To", to));

            return SQLDataAccess.ExecuteReadList(query, CommandType.Text, OrderService.GetOrderFromReader, queryParams.ToArray());
        }


        public static OrderStatus GetOrderStatusByName(string statusName)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT TOP 1 * FROM [Order].[OrderStatus] WHERE LOWER ([StatusName]) = LOWER (@StatusName)",
                    CommandType.Text,
                    GetOrderStatusFromReader,
                    new SqlParameter("@StatusName", statusName));
        }


        public static void AddOrderStatusHistory(OrderStatusHistory history)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert into [Order].StatusHistory (Date, OrderID, PreviousStatus, NewStatus, CustomerID, CustomerName, Basis) " +
                "values (GetDate(), @OrderID, @PreviousStatus, @NewStatus, @CustomerID, @CustomerName, @Basis)",
                CommandType.Text,
                new SqlParameter("@OrderID", history.OrderID),
                new SqlParameter("@PreviousStatus", history.PreviousStatus),
                new SqlParameter("@NewStatus", history.NewStatus),
                new SqlParameter("@CustomerID", history.CustomerID ?? (object)DBNull.Value),
                new SqlParameter("@CustomerName", history.CustomerName),
                new SqlParameter("@Basis", history.Basis ?? string.Empty)
                );
        }

        public static List<OrderStatusHistory> GetOrderStatusHistory(int orderId)
        {
            return SQLDataAccess.ExecuteReadList<OrderStatusHistory>(
                " Select * from [Order].StatusHistory where Orderid = @OrderID",
                CommandType.Text, GetOrderStatusHistoryFromReader,
                new SqlParameter("@OrderID", orderId)

                );
        }

        private static OrderStatusHistory GetOrderStatusHistoryFromReader(SqlDataReader reader)
        {
            return new OrderStatusHistory
            {
                Date = SQLDataHelper.GetDateTime(reader, "Date"),
                OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                PreviousStatus = SQLDataHelper.GetString(reader, "PreviousStatus"),
                NewStatus = SQLDataHelper.GetString(reader, "NewStatus"),
                CustomerID = SQLDataHelper.GetNullableGuid(reader, "CustomerID"),
                CustomerName = SQLDataHelper.GetString(reader, "CustomerName"),
                Basis = SQLDataHelper.GetString(reader, "Basis")
            };
        }
    }
}