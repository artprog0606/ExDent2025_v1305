using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using System;
using System.Linq;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerSendToShippingService
    {
        public static bool SendToShippingService(TriggerAction action, ITriggerObject triggerObject)
        {
            if (!(triggerObject is Order order) || order.ShippingMethod == null)
                return false;

            if (!ShippingMethodService.ShippingMethodTypesUseUnloadOrder.Contains(order.ShippingMethod.ShippingType))
            {
                SendMail(action, order, LocalizationService.GetResource("Core.Triggers.Action.SendToShippingService.UnloadOrderNotSupport"));
                return false;
            }

            try
            {
                var shippingCalculationParameters = ShippingCalculationConfigurator.Configure()
                    .ByOrder(order)
                    .Build();
                var type = ReflectionExt
                    .GetTypeByAttributeValue<ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, order.ShippingMethod.ShippingType);

                var shipping = (BaseShipping)Activator.CreateInstance(type, order.ShippingMethod, shippingCalculationParameters);
                var unloadOrderHandler = shipping as IUnloadOrder;
                if (unloadOrderHandler is null)
                {
                    SendMail(action, order, LocalizationService.GetResource("Core.Triggers.Action.SendToShippingService.UnloadOrderNotSupport"));
                    return false;
                }

                var result = unloadOrderHandler.UnloadOrder(order);
                if (result == null || !result.Success)
                    SendMail(action, order, result?.Message ?? LocalizationService.GetResource("Core.Triggers.Action.SendToShippingService.UnknownResult"));

                return result?.Success ?? false;
            }
            catch(Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
        }

        private static void SendMail(TriggerAction action, Order order, string errorMessage)
        {
            if (!action.SendToShippingServiceData.SendEmailOnError
                || action.SendToShippingServiceData.EmailForError.IsNullOrEmpty()
                || action.SendToShippingServiceData.EmailBody.IsNullOrEmpty()
                || action.SendToShippingServiceData.EmailSubject.IsNullOrEmpty())
                return;

            var subject = ReplaceVariablesForOrder(action.SendToShippingServiceData.EmailSubject, order, null);
            var body = ReplaceVariablesForOrder(action.SendToShippingServiceData.EmailBody, order, errorMessage);
            
            MailService.SendMailNow(Guid.Empty, action.SendToShippingServiceData.EmailForError, subject, body, true, logging: false);
        }

        private static string ReplaceVariablesForOrder(string value, Order order, string errorMessage)
        {
            value = value.Replace("#OrderId#", order.OrderID.ToString())
                    .Replace("#Number#", order.Number)
                    .Replace("#IsPaid#", order.Payed.ToLowerString())
                    .Replace("#Sum#", order.Sum.ToString("#.##"))
                    .Replace("#Status#", order.OrderStatus.StatusName)
                    .Replace("#ShippingMethod#",
                        (order.ArchivedShippingName +
                        (order.OrderPickPoint != null ? " " + order.OrderPickPoint.PickPointAddress : "")))
                    .Replace("#PaymentMethod#", order.ArchivedPaymentName)
                    .Replace("#ErrorMessage#", errorMessage);
            return value;
        }
    }
}
