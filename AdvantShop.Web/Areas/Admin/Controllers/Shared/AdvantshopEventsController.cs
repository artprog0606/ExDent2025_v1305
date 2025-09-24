using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Webhook;
using AdvantShop.Core.Services.Webhook.Models;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Handlers.BizProcesses;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.Web.Admin.Handlers.Tasks;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    [WebhookAuth(EWebhookType.BizProcess)]
    public class AdvantshopEventsController : BaseController
    {
        [HttpPost]
        public JsonResult OrderAdded(WebhookOrderModel model)
        {
            var order = OrderService.GetOrder(model.Id);
            if (order == null)
                return JsonError("order not found");

            var notificationsHandler = new AdminNotificationsHandler();
            notificationsHandler.NotifyAllCustomers(new OrderAddedNotification(order), RoleAction.Orders, model.CurrentCustomerId);
            notificationsHandler.UpdateOrders(model.CurrentCustomerId);

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessOrderCreatedRule>();
                var handler = new BizProcessOrderHandler<BizProcessOrderCreatedRule>(rules, order);
                handler.ProcessBizObject();
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult OrderStatusChanged(WebhookOrderModel model)
        {
            var order = OrderService.GetOrder(model.Id);
            if (order == null)
                return JsonError("order not found");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessOrderStatusChangedRule>(order.OrderStatusId);
                var handler = new BizProcessOrderHandler<BizProcessOrderStatusChangedRule>(rules, order);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult OrderManagerAssigned(WebhookOrderModel model)
        {
            var order = OrderService.GetOrder(model.Id);
            if (order == null)
                return JsonError("order not found");

            var notificationsHandler = new AdminNotificationsHandler();
            if (order.Manager != null)
                notificationsHandler.NotifyCustomers(new OrderManagerAssignedNotification(order), order.Manager.CustomerId);
            notificationsHandler.UpdateOrders(model.CurrentCustomerId);

            return JsonOk();
        }

        [HttpPost]
        public JsonResult LeadAdded(WebhookLeadModel model)
        {
            var lead = LeadService.GetLead(model.Id);
            if (lead == null)
                return JsonError("lead not found");

            var notificationsHandler = new AdminNotificationsHandler();

            if (lead.ManagerId.HasValue)
            {
                var leadManager = ManagerService.GetManager(lead.ManagerId.Value);
                if (leadManager != null && leadManager.CustomerId != model.CurrentCustomerId)
                    notificationsHandler.NotifyCustomers(new LeadAddedNotification(lead), leadManager.CustomerId);
            }

            var managers = SalesFunnelService.GetSalesFunnelManagers(lead.SalesFunnelId);

            var salesFunnel = SalesFunnelService.Get(lead.SalesFunnelId);
            var showNotifications = salesFunnel != null && !salesFunnel.NotSendNotificationsOnLeadCreation;

            if (showNotifications)
            {
                if (managers.Count != 0)
                    notificationsHandler.NotifyCustomers(new LeadAddedNotification(lead), managers.Where(x => x.CustomerId != model.CurrentCustomerId).Select(x => x.CustomerId).ToArray());
                else
                    notificationsHandler.NotifyAllCustomers(new LeadAddedNotification(lead), RoleAction.Crm, model.CurrentCustomerId);
            }

            notificationsHandler.UpdateLeads(model.CurrentCustomerId);

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessLeadCreatedRule>();
                var handler = new BizProcessLeadHandler<BizProcessLeadCreatedRule>(rules, lead);
                handler.ProcessBizObject();
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult LeadChanged(WebhookLeadModel model)
        {
            var lead = LeadService.GetLead(model.Id);
            if (lead == null)
                return JsonError("lead not found");

            var notificationsHandler = new AdminNotificationsHandler();

            if (lead.ManagerId.HasValue)
            {
                var leadManager = ManagerService.GetManager(lead.ManagerId.Value);
                if (leadManager != null && leadManager.CustomerId != model.CurrentCustomerId)
                    notificationsHandler.NotifyCustomers(new LeadChangedNotification(lead), leadManager.CustomerId);
            }

            var salesFunnel = SalesFunnelService.Get(lead.SalesFunnelId);
            if (salesFunnel != null && !salesFunnel.NotSendNotificationsOnLeadChanged)
            {
                var managers = SalesFunnelService.GetSalesFunnelManagers(lead.SalesFunnelId);

                if (managers.Count != 0)
                    notificationsHandler.NotifyCustomers(new LeadChangedNotification(lead),
                        managers.Where(x => x.CustomerId != model.CurrentCustomerId).Select(x => x.CustomerId)
                            .ToArray());
                else
                    notificationsHandler.NotifyAllCustomers(new LeadChangedNotification(lead), RoleAction.Crm,
                        model.CurrentCustomerId);
            }

            notificationsHandler.UpdateLeads(model.CurrentCustomerId);

            return JsonOk();
        }

        [HttpPost]
        public JsonResult LeadStatusChanged(WebhookLeadModel model)
        {
            var lead = LeadService.GetLead(model.Id);
            if (lead == null)
                return JsonError("lead not found");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessLeadStatusChangedRule>(lead.DealStatusId);
                var handler = new BizProcessLeadHandler<BizProcessLeadStatusChangedRule>(rules, lead);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult DeferredTaskAdded(WebhookTaskModel model)
        {
            var task = TaskService.GetTask(model.Id);
            if (task == null)
                return JsonError("task not found");

            new TaskAddedNotificationsHandler(task).Execute();

            return JsonOk();
        }

        [HttpPost]
        public JsonResult CallMissed(WebhookCallModel model)
        {
            var call = CallService.GetCall(model.Id);
            if (call == null)
                return JsonError("call not found");
            if (call.Type != ECallType.Missed)
                return JsonError("call not missed");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessCallMissedRule>();
                var handler = new BizProcessHandler<BizProcessCallMissedRule>(rules, call);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult ReviewAdded(WebhookReviewModel model)
        {
            var review = ReviewService.GetReview(model.Id);
            if (review == null)
                return JsonError("review not found");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessReviewAddedRule>();
                var handler = new BizProcessReviewHandler(rules, review);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult MessageReply(WebhookCustomerModel model)
        {
            var customer = CustomerService.GetCustomer(model.Id);
            if (customer == null)
                return JsonError("customer not found");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessMessageReplyRule>();
                var handler = new BizProcessCustomerHandler<BizProcessMessageReplyRule>(rules, customer, model.Type);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult TaskAdded(WebhookTaskModel model)
        {
            var task = TaskService.GetTask(model.Id);
            if (task == null)
                return JsonError("task not found");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessTaskCreatedRule>();
                var handler = new BizProcessTaskHandler<BizProcessTaskCreatedRule>(rules, task);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult TaskStatusChanged(WebhookTaskModel model)
        {
            var task = TaskService.GetTask(model.Id);
            if (task == null)
                return JsonError("task not found");

            if (SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess))
            {
                var status = TaskService.GetBPTaskStatus(task);
                var rules = BizProcessRuleService.GetBizProcessRules<BizProcessTaskStatusChangedRule>((int)status);
                var handler = new BizProcessTaskHandler<BizProcessTaskStatusChangedRule>(rules, task);
                handler.GenerateTask();
            }

            return JsonOk();
        }

        [HttpPost]
        public JsonResult LeadEvent(WebhookLeadEventModel model)
        {
            LeadEventNotification notification = null;
            switch (model.EventType)
            {
                case LeadEventType.Vk:
                {
                    var msg = VkService.GetCustomerMessage(model.ObjId);
                    if (msg != null)
                        notification = new VkMessageNotification(msg);
                    break;
                }

                case LeadEventType.Facebook:
                {
                    var msg = FacebookService.GetCustomerMessage(model.ObjId);
                    if (msg != null)
                        notification = new FacebookMessageNotification(msg);
                    break;
                }

                case LeadEventType.Instagram:
                {
                    var msg = InstagramService.GetCustomerMessage(model.ObjId);
                    if (msg != null)
                        notification = new InstagramMessageNotification(msg);
                    break;
                }
                
                case LeadEventType.Ok:
                {
                    var msg = OkService.GetUserMessage(model.ObjId);
                    if (msg != null)
                        notification = new OkMessageNotification(msg);
                    break;
                }
                
                case LeadEventType.Telegram:
                {
                    var msg = new TelegramService().GetCustomerMessage(model.ObjId);
                    if (msg != null)
                        notification = new TelegramMessageNotification(msg);
                    break;
                }
            }

            if (notification == null)
                return JsonError("no data");

            var notificationsHandler = new AdminNotificationsHandler();
            notificationsHandler.NotifyAllCustomers(notification, RoleAction.Customers, model.CurrentCustomerId);

            return JsonOk();
        }

        [HttpPost]
        public JsonResult BookingAdded(WebhookBookingModel model)
        {
            var booking = BookingService.Get(model.Id);
            if (booking == null)
                return JsonError("booking not found");

            var notificationsHandler = new AdminNotificationsHandler();
            var notification = new BookingAddedNotification(booking);

            if (booking.Affiliate.AccessForAll)
                notificationsHandler.NotifyAllCustomers(notification, RoleAction.Booking, model.CurrentCustomerId);
            else
            {
                var customerIds = booking.Affiliate.Managers.Select(x => x.CustomerId).ToList();

                if (booking.ReservationResource != null
                    && booking.ReservationResource.Manager != null
                    && !customerIds.Contains(booking.ReservationResource.Manager.CustomerId))
                    customerIds.Add(booking.ReservationResource.Manager.CustomerId);

                if (booking.Manager != null
                    && !customerIds.Contains(booking.Manager.CustomerId))
                    customerIds.Add(booking.Manager.CustomerId);

                if (customerIds.Count > 0)
                    notificationsHandler.NotifyCustomers(notification, customerIds.ToArray());
            }

            notificationsHandler.UpdateBookings(booking.AffiliateId, model.ReservationResourceId, booking.Id, "added", model.CurrentCustomerId);

            return JsonOk();
        }

        [HttpPost]
        public JsonResult BookingChanged(WebhookBookingModel model)
        {
            var booking = BookingService.Get(model.Id);
            if (booking == null)
                return JsonError("booking not found");

            var notificationsHandler = new AdminNotificationsHandler();
            notificationsHandler.UpdateBookings(booking.AffiliateId, model.ReservationResourceId, booking.Id, "changed", model.CurrentCustomerId);

            return JsonOk();
        }

        [HttpPost]
        public JsonResult BookingDeleted(WebhookBookingModel model)
        {
            var notificationsHandler = new AdminNotificationsHandler();
            notificationsHandler.UpdateBookings(model.AffiliateId, model.ReservationResourceId, model.Id, "deleted", model.CurrentCustomerId);

            return JsonOk();
        }

        [HttpPost]
        public JsonResult CheckSendingMails()
        {
            if (!SaasDataService.IsSaasEnabled ||
                string.IsNullOrEmpty(SaasDataService.CurrentSaasData.CheckMailingSendTo))
                return JsonOk();

            var sendTo = SaasDataService.CurrentSaasData.CheckMailingSendTo;

            var senderInfo = string.Format(
                "ImapHost:{0} ImapPort:{1} login:{2} password:{3} email:{4} ssl:{5} sednerName:{6}",
                SettingsMail.ImapHost,
                SettingsMail.ImapPort,
                SettingsMail.Login,
                SettingsMail.Password.FirstOrDefault() + "***" + SettingsMail.Password.LastOrDefault(),
                SettingsMail.From,
                SettingsMail.SSL,
                SettingsMail.SenderName);

            var typeSpecificInfo = SettingsMail.UseAdvantshopMail
                ? "advEmail:" + Core.Services.Configuration.Settings.CapShopSettings.FromEmail
                : $"SMTP:{SettingsMail.SMTP} Port:{SettingsMail.Port}";

            var body = $"{UrlService.GenerateBaseUrl()} - ({senderInfo} {typeSpecificInfo})";

            var sended = false;
            try
            {
                sended = MailService.SendMail(Guid.Empty, sendTo, SettingsLic.AdvId + " проверка отправки писем", body,
                    false, needretry: false, logging: false);
            }
            catch
            {
                //ignore
            }

            if (!sended)
            {
                AdminInformerService.Add(new AdminInformer
                    {Body = "Ошибка при проверке почтового сервиса, проверьте настройки и попробуйте отправить пробное письмо.", Type = AdminInformerType.Error});
            }

            return JsonOk();
        }
    }
}
