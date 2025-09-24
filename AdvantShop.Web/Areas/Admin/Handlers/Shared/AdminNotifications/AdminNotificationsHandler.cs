using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Hubs;
using Microsoft.AspNet.SignalR;

namespace AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications
{
    public class AdminNotificationsHandler
    {
        private Lazy<IHubContext> hub = new Lazy<IHubContext>(
          () => GlobalHost.ConnectionManager.GetHubContext<NotifyHub>()
        );

        protected IHubContext Hub
        {
            get { return hub.Value; }
        }

        private static readonly Object SyncObject = new Object();

        private dynamic AllClientsWithoutCurrent
        {
            get
            {
                if (CustomerContext.CurrentCustomer != null)
                {
                    var connectionId = NotifyHub.GetConnectionId(CustomerContext.CurrentCustomer.Id);
                    if (connectionId.IsNotEmpty())
                        return Hub.Clients.AllExcept(connectionId);
                }
                return Hub.Clients.All;
            }
        }

        public dynamic GetClients(params RoleAction[] rolesAction)
        {
            return GetClients(rolesAction,
                CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : (Guid?) null);
        }

        public dynamic GetClients(RoleAction[] rolesAction, Guid? currentCustomerId)
        {
            var connectionIds = new List<string>();
            var customerIds = NotifyHub.GetOnlineCustomerIds();
            foreach (var customerId in customerIds.Where(x => !currentCustomerId.HasValue || x != currentCustomerId.Value))
            {
                Customer customer = CustomerService.GetCustomer(customerId);
                if (customer == null || (!customer.IsAdmin && !customer.IsVirtual && (!customer.IsModerator || !HasRole(customer, rolesAction))))
                    continue;
                var connectionId = NotifyHub.GetConnectionId(customerId);
                if (connectionId.IsNotEmpty())
                    connectionIds.Add(connectionId);
            }

            return Hub.Clients.Clients(connectionIds);
        }

        private bool HasRole(Customer customer, RoleAction[] rolesAction)
        {
            var customerRolesActions = RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id);

            foreach (var actionKey in rolesAction)
            {
                if (actionKey != RoleAction.None && customerRolesActions.Any(item => item.Role == actionKey))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// notify all customers in all tabs by toaster
        /// </summary>
        /// <param name="notification"></param>
        public void NotifyByToaster(AdminNotification notification)
        {
            lock (SyncObject)
            {
                Hub.Clients.All.popNotification(notification);
            }
        }

        /// <summary>
        /// close toaster notification
        /// </summary>
        /// <param name="toastId"></param>
        public void CloseToaster(string toastId)
        {
            lock (SyncObject)
            {
                Hub.Clients.All.closeToaster(toastId);
            }
        }

        /// <summary>
        /// notify all online customers by webNotification
        /// </summary>
        /// <param name="notification"></param>
        public void NotifyAllCustomers(AdminNotification notification)
        {
            lock (SyncObject)
            {
                var connectionIds = NotifyHub.GetOnlineConnectionIds();
                foreach (var connectionId in connectionIds)
                    Hub.Clients.Client(connectionId).showNotification(notification);
            }
        }

        /// <summary>
        /// notify all online customers by webNotification considering access rights
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="roleAction"></param>
        public void NotifyAllCustomers(AdminNotification notification, RoleAction roleAction, Guid? currentCustomerId = null)
        {
            lock (SyncObject)
            {
                var customerIds = NotifyHub.GetOnlineCustomerIds();
                foreach (var customerId in customerIds.Where(x => !currentCustomerId.HasValue || x != currentCustomerId.Value))
                {
                    Customer customer = null;
                    if (roleAction != RoleAction.None && (customer = CustomerService.GetCustomer(customerId)) != null && !customer.HasRoleAction(roleAction))
                        continue;
                    var connectionId = NotifyHub.GetConnectionId(customerId);
                    if (connectionId.IsNotEmpty())
                        Hub.Clients.Client(connectionId).showNotification(notification);
                }
            }
        }

        /// <summary>
        /// notify one customer by webNotification
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="notifications"></param>
        public void NotifyCustomer(Guid customerId, params AdminNotification[] notifications)
        {
            var connectionId = NotifyHub.GetConnectionId(customerId);
            if (connectionId.IsNotEmpty())
                Hub.Clients.Client(connectionId).showNotifications(notifications);
        }

        /// <summary>
        /// notify customers by webNotification
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="customerIds"></param>
        public void NotifyCustomers(AdminNotification notification, params Guid[] customerIds)
        {
            lock (SyncObject)
            {
                foreach (var customerId in customerIds.Where(x => x != Guid.Empty).Distinct())
                {
                    var connectionId = NotifyHub.GetConnectionId(customerId);
                    if (connectionId.IsNotEmpty())
                        Hub.Clients.Client(connectionId).showNotification(notification);
                    else
                        AdminNotificationService.AddAdminNotification(notification, customerId); 
                }
            }
        }

        public void NotifyCustomers(AdminNotification[] notifications, params Guid[] customerIds)
        {
            if (notifications == null || !notifications.Any())
                return;
            lock (SyncObject)
            {
                for (int i = 0; i < notifications.Length; i++)
                {
                    if (notifications[i].Tag.IsNullOrEmpty())
                        notifications[i].Tag = i.ToString();
                }
                foreach (var customerId in customerIds.Where(x => x != Guid.Empty).Distinct())
                {
                    var connectionId = NotifyHub.GetConnectionId(customerId);
                    if (connectionId.IsNotEmpty())
                        Hub.Clients.Client(connectionId).showNotifications(notifications);
                    else
                    {
                        foreach (var notification in notifications)
                            AdminNotificationService.AddAdminNotification(notification, customerId);
                    }
                }
            }
        }

        public void UpdateOrders(Guid? currentCustomerId = null)
        {
            lock (SyncObject)
            {
                (currentCustomerId.HasValue
                        ? GetClients(new[] {RoleAction.Orders}, currentCustomerId)
                        : GetClients(RoleAction.Orders))
                    .updateOrders(); //Hub.Clients.All
            }
        }

        public void UpdateLeads(Guid? currentCustomerId = null)
        {
            lock (SyncObject)
            {
                (currentCustomerId.HasValue
                        ? GetClients(new[] {RoleAction.Crm}, currentCustomerId)
                        : GetClients(RoleAction.Crm))
                    .updateLeads(); //Hub.Clients.All
            }
        }

        public void UpdateTasks()
        {
            lock (SyncObject)
            {
                GetClients(RoleAction.Tasks).updateTasks();//Hub.Clients.All
            }
        }

        public void UpdateBookings(int? affiliateId, int? reservationResourceId, int? bookingId, string typeChange, Guid? currentCustomerId = null)
        {
            lock (SyncObject)
            {
                (currentCustomerId.HasValue
                        ? GetClients(new[] {RoleAction.Booking}, currentCustomerId)
                        : GetClients(RoleAction.Booking))
                    .updateBookings(affiliateId, reservationResourceId, bookingId, typeChange); //Hub.Clients.All
            }
        }

        public void UpdateAdminComment(AdminComment comment, string typeChange)
        {
            lock (SyncObject)
            {
                Guid? relatedCustomerId = null;
                dynamic clients = null;
                switch (comment.Type)
                {
                    case AdminCommentType.Booking:
                        clients = GetClients(RoleAction.Booking);
                        break;
                    case AdminCommentType.Lead:
                        clients = GetClients(RoleAction.Crm);
                        break;
                    case AdminCommentType.Order:
                        clients = GetClients(RoleAction.Orders);
                        break;
                    case AdminCommentType.Task:
                    case AdminCommentType.TaskHidden:
                        clients = GetClients(RoleAction.Tasks);
                        break;
                    case AdminCommentType.Call:
                    case AdminCommentType.Customer:
                        // т.к. комментарии этих объектов отображаются через <lead-events>,
                        // который работает через контроллер CrmEventsController,
                        // то и уведомления нужно отсылать пользователям с ролями имеющими доступ к этому контроллеру
                        var crmEventsControllerAuthAttribute = ((Attributes.AuthAttribute)
                            typeof(Controllers.Crm.CrmEventsController)
                                .GetCustomAttributes(typeof(Attributes.AuthAttribute), false).FirstOrDefault());

                        clients = crmEventsControllerAuthAttribute != null
                            ? GetClients(crmEventsControllerAuthAttribute.RolesAction.ToArray())
                            : AllClientsWithoutCurrent;

                        if (comment.Type == AdminCommentType.Call)
                        {
                            var call = CallService.GetCall(comment.ObjId);
                            if (call != null && call.Customers.Count > 0)
                                relatedCustomerId = call.Customers.First().Id;
                        }
                        if (comment.Type == AdminCommentType.Customer)
                        {
                            var customer = CustomerService.GetCustomer(comment.ObjId);
                            if (customer != null)
                                relatedCustomerId = customer.Id;
                        }

                        break;

                    default:
                        clients = AllClientsWithoutCurrent;
                        break;
                }
                
                if (clients != null)
                    clients.updateAdminComment(comment.Id, comment.ParentId, comment.ObjId, comment.Type.StrName(), relatedCustomerId, typeChange);
            }
        }
    }
}
