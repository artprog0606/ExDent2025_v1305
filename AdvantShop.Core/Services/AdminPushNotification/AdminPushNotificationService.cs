using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.AdminPushNotification
{
    public static class AdminPushNotificationService
    {
        private const string ActionLinkDataKey = "action_link";

        public static void NewOrder(Order order)
        {
            if (order.IsFromAdminArea || order.IsDraft)
                return;

            SendPush("Новый заказ", "Поступил новый заказ №" + order.Number,
                $"{Link()}/adminv3/orders/edit/{order.OrderID}");
        }

        public static void NewOrderReview(Order order) =>
            SendPush("Новый комментарий к заказу", "Комментарий к заказу №" + order.Number,
                $"{Link()}/adminv3/orders/edit/{order.OrderID}");

        public static void NewLead(Lead lead)
        {
            if (lead.IsFromAdminArea)
                return;

            SendPush("Новый лид", "№" + lead.Id,
                $"{Link()}/adminv3/leads?salesFunnelId={lead.SalesFunnelId}&leadIdInfo={lead.Id}");
        }

        public static void SendPushFromAdminInformer(AdminInformer adminInformer) => SendPush(adminInformer.Title,
            adminInformer.Body, customerId: adminInformer.PrivateCustomerId);


        public static void SendPush(string title, string body, string actionLink = null, Guid? customerId = null)
        {
            var tokensToNotify = CustomerAdminPushNotificationService.GetFcmTokensToNotify();

            var data = new Dictionary<string, string>();
            if (actionLink != null) data.Add(ActionLinkDataKey, actionLink);

            if (customerId.HasValue)
            {
                var customerFcmToken = tokensToNotify.Where(tuple => tuple.CustomerId == customerId.Value)
                    .Select(x => x.FcmToken).FirstOrDefault();

                if (customerFcmToken != null)
                {
                    AdminPushNotificationServiceClient.SendNotification(
                        customerFcmToken,
                        title,
                        body,
                        data);
                }

                return;
            }

            foreach (var tuple in tokensToNotify)
            {
                AdminPushNotificationServiceClient.SendNotification(
                    tuple.FcmToken,
                    title,
                    body,
                    data);
            }
        }

        private static string Link() => UrlService.GenerateBaseUrl().TrimEnd('/');
    }
}