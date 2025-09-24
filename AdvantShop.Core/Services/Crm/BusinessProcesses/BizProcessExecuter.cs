using System;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.Webhook;
using AdvantShop.Core.Services.Webhook.Models;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public class BizProcessExecuter : WebhookExecuter
    {
        public static void OrderAdded(Order order)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookOrderModel)order;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/orderAdded", data, !order.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.OrderCreated, data);
        }

        public static void OrderStatusChanged(Order order)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookOrderModel)order;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/orderStatusChanged", data, !order.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.OrderStatusChanged, data);
        }

        public static void OrderManagerAssigned(Order order)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookOrderModel)order;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/orderManagerAssigned", data, !order.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.OrderManagerAssigned, data);
        }

        public static void LeadAdded(Lead lead)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookLeadModel)lead;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/leadAdded", data, !lead.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.LeadCreated, data);
        }

        public static void LeadChanged(Lead lead)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookLeadModel)lead;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/leadChanged", data, !lead.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.LeadChanged, data);
        }

        public static void LeadStatusChanged(Lead lead)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookLeadModel)lead;
            MakeSystemRequest("advantshopevents/leadStatusChanged", data, !lead.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.LeadStatusChanged, data);
        }

        public static void CallMissed(Call call)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookCallModel)call;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/callMissed", data);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.CallMissed, data);
        }

        public static void ReviewAdded(Review review)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookReviewModel)review;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/reviewAdded", data);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.ReviewAdded, data);
        }

        public static void MessageReply(Customer customer, EMessageReplyFieldType type)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookCustomerModel)customer;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            data.Type = type;

            MakeSystemRequest("advantshopevents/messageReply", data);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.MessageReply, data);
        }

        public static void TaskAdded(Task task)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookTaskModel)task;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/taskAdded", data);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.TaskCreated, data);
        }

        public static void TaskStatusChanged(Task task)
        {
            //if (!SaasDataService.IsEnabledFeature(ESaasProperty.HaveCrm))
            //    return;
            var data = (WebhookTaskModel)task;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/taskStatusChanged", data);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.TaskStatusChanged, data);
        }

        public static void LeadEvent(int objId, LeadEventType eventType)
        {
            var data = new WebhookLeadEventModel(objId, eventType);
            MakeSystemRequest("advantshopevents/leadEvent", data);
        }

        public static void DeferredTaskAdded(Task task)
        {
            var data = (WebhookTaskModel)task;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/deferredTaskAdded", data);
        }

        public static void BookingAdded(Booking.Booking booking)
        {
            var data = (WebhookBookingModel)booking;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/bookingAdded", data, !booking.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.BookingCreated, data);
        }

        public static void BookingChanged(Booking.Booking booking)
        {
            var data = (WebhookBookingModel)booking;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/bookingChanged", data, !booking.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.BookingChanged, data);
        }

        public static void BookingDeleted(Booking.Booking booking)
        {
            var data = (WebhookBookingModel)booking;
            data.CurrentCustomerId = CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : Guid.Empty;
            MakeSystemRequest("advantshopevents/bookingDeleted", data, !booking.IsFromAdminArea);

            MakeRequestAsync<BizProcessWebhookUrlList, BizProcessWebhookUrl>(x => x.EventType == EBizProcessEventType.BookingDeleted, data);
        }

    }
}