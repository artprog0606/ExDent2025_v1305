using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Letters;
using AdvantShop.Mails;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Triggers.Orders
{
    public abstract class OrderTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType => ETriggerObjectType.Order;
    }

    public class OrderCreatedTriggerRule : OrderTriggerRule
    {
        public override ETriggerEventType EventType => ETriggerEventType.OrderCreated;

        public override List<LetterFormatKey> AvailableVariables =>
            LetterBuilderHelper.GetLetterFormatKeys(
                    new List<OrderLetterTemplateKey>()
                    {
                        OrderLetterTemplateKey.Number,
                        OrderLetterTemplateKey.FirstName,
                        OrderLetterTemplateKey.LastName,
                        OrderLetterTemplateKey.Email,
                        OrderLetterTemplateKey.CustomerContacts,
                        OrderLetterTemplateKey.City,
                        OrderLetterTemplateKey.Address,
                        OrderLetterTemplateKey.AdditionalCustomerFields,
                        OrderLetterTemplateKey.Inn,
                        OrderLetterTemplateKey.CompanyName,
                        OrderLetterTemplateKey.CustomerComment,
                        OrderLetterTemplateKey.BillingLink,
                        OrderLetterTemplateKey.BillingShortLink,
                        OrderLetterTemplateKey.OrderItemsHtml,
                        OrderLetterTemplateKey.OrderItemsPlain,
                        OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks,
                        OrderLetterTemplateKey.OrderItemsPlainDownloadLinks,
                        OrderLetterTemplateKey.Sum,
                        OrderLetterTemplateKey.SumWithoutCurrency,
                        OrderLetterTemplateKey.TrackNumber,
                        OrderLetterTemplateKey.PaymentName,
                        OrderLetterTemplateKey.ShippingNameWithPickpointAddressHtml,
                        OrderLetterTemplateKey.ShippingName,
                        OrderLetterTemplateKey.PickpointAddress,
                        OrderLetterTemplateKey.DeliveryDate,
                        OrderLetterTemplateKey.DeliveryDateWithPrefix,
                        OrderLetterTemplateKey.ManagerName,
                        OrderLetterTemplateKey.ManagerSign,
                        OrderLetterTemplateKey.CurrencyCode,
                        OrderLetterTemplateKey.PaymentStatus,
                        OrderLetterTemplateKey.OrderPaidOrNotPaid,
                        OrderLetterTemplateKey.PostalCode,
                        OrderLetterTemplateKey.ReceivingMethod,
                        OrderLetterTemplateKey.CountDevices,
                        OrderLetterTemplateKey.RecipientLastName,
                        OrderLetterTemplateKey.RecipientFirstName,
                        OrderLetterTemplateKey.RecipientPhone,
                    })
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<CommonLetterTemplateKey>())
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                .ToList();

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order) triggerObject;

            _mail = NewOrderMailTemplate.Create(order);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order) triggerObject;

            _mail = NewOrderMailTemplate.Create(order);

            return _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnNewOrder.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }

    public class OrderStatusChangedTriggerRule : OrderTriggerRule
    {
        public override ETriggerEventType EventType => ETriggerEventType.OrderStatusChanged;

        public override List<LetterFormatKey> AvailableVariables =>
            LetterBuilderHelper.GetLetterFormatKeys(
                    new List<OrderLetterTemplateKey>()
                    {
                        OrderLetterTemplateKey.Number,
                        OrderLetterTemplateKey.Status,
                        OrderLetterTemplateKey.StatusCommentHtml,
                        OrderLetterTemplateKey.FirstName,
                        OrderLetterTemplateKey.LastName,
                        OrderLetterTemplateKey.Email,
                        OrderLetterTemplateKey.CustomerContacts,
                        OrderLetterTemplateKey.City,
                        OrderLetterTemplateKey.Address,
                        OrderLetterTemplateKey.AdditionalCustomerFields,
                        OrderLetterTemplateKey.Inn,
                        OrderLetterTemplateKey.CompanyName,
                        OrderLetterTemplateKey.CustomerComment,
                        OrderLetterTemplateKey.BillingLink,
                        OrderLetterTemplateKey.BillingShortLink,
                        OrderLetterTemplateKey.OrderItemsHtml,
                        OrderLetterTemplateKey.OrderItemsPlain,
                        OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks,
                        OrderLetterTemplateKey.OrderItemsPlainDownloadLinks,
                        OrderLetterTemplateKey.Sum,
                        OrderLetterTemplateKey.SumWithoutCurrency,
                        OrderLetterTemplateKey.TrackNumber,
                        OrderLetterTemplateKey.PaymentName,
                        OrderLetterTemplateKey.ShippingNameWithPickpointAddressHtml,
                        OrderLetterTemplateKey.ShippingName,
                        OrderLetterTemplateKey.PickpointAddress,
                        OrderLetterTemplateKey.DeliveryDate,
                        OrderLetterTemplateKey.DeliveryDateWithPrefix,
                        OrderLetterTemplateKey.ManagerName,
                        OrderLetterTemplateKey.ManagerSign,
                        OrderLetterTemplateKey.CurrencyCode,
                        OrderLetterTemplateKey.PaymentStatus,
                        OrderLetterTemplateKey.OrderPaidOrNotPaid,
                        OrderLetterTemplateKey.PostalCode,
                        OrderLetterTemplateKey.ReceivingMethod,
                        OrderLetterTemplateKey.CountDevices,
                        OrderLetterTemplateKey.RecipientLastName,
                        OrderLetterTemplateKey.RecipientFirstName,
                        OrderLetterTemplateKey.RecipientPhone,
                    })
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<CommonLetterTemplateKey>())
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                .ToList();


        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon,
                                                string triggerCouponCode)
        {
            var order = (Order) triggerObject;

            _mail = new OrderStatusMailTemplate(order);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }
        
        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, 
                                                                            Coupon coupon, string triggerCouponCode)
        {
            var order = (Order) triggerObject;

            _mail = new OrderStatusMailTemplate(order);

            return _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }


        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnChangeOrderStatus.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }

    public class OrderPayTriggerRule : OrderTriggerRule
    {
        public override ETriggerEventType EventType => ETriggerEventType.OrderPaied;

        public override List<LetterFormatKey> AvailableVariables =>
            LetterBuilderHelper.GetLetterFormatKeys(
                    new List<OrderLetterTemplateKey>()
                    {
                        OrderLetterTemplateKey.Number,
                        OrderLetterTemplateKey.FirstName,
                        OrderLetterTemplateKey.LastName,
                        OrderLetterTemplateKey.Email,
                        OrderLetterTemplateKey.CustomerContacts,
                        OrderLetterTemplateKey.City,
                        OrderLetterTemplateKey.Address,
                        OrderLetterTemplateKey.AdditionalCustomerFields,
                        OrderLetterTemplateKey.Inn,
                        OrderLetterTemplateKey.CompanyName,
                        OrderLetterTemplateKey.CustomerComment,
                        OrderLetterTemplateKey.BillingLink,
                        OrderLetterTemplateKey.BillingShortLink,
                        OrderLetterTemplateKey.OrderItemsHtml,
                        OrderLetterTemplateKey.OrderItemsPlain,
                        OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks,
                        OrderLetterTemplateKey.OrderItemsPlainDownloadLinks,
                        OrderLetterTemplateKey.Sum,
                        OrderLetterTemplateKey.SumWithoutCurrency,
                        OrderLetterTemplateKey.TrackNumber,
                        OrderLetterTemplateKey.PaymentName,
                        OrderLetterTemplateKey.ShippingNameWithPickpointAddressHtml,
                        OrderLetterTemplateKey.ShippingName,
                        OrderLetterTemplateKey.PickpointAddress,
                        OrderLetterTemplateKey.DeliveryDate,
                        OrderLetterTemplateKey.DeliveryDateWithPrefix,
                        OrderLetterTemplateKey.ManagerName,
                        OrderLetterTemplateKey.ManagerSign,
                        OrderLetterTemplateKey.CurrencyCode,
                        OrderLetterTemplateKey.PaymentStatus,
                        OrderLetterTemplateKey.OrderPaidOrNotPaid,
                        OrderLetterTemplateKey.PostalCode,
                        OrderLetterTemplateKey.ReceivingMethod,
                        OrderLetterTemplateKey.CountDevices,
                        OrderLetterTemplateKey.LastName,
                        OrderLetterTemplateKey.RecipientFirstName,
                        OrderLetterTemplateKey.RecipientPhone,
                    })
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<CommonLetterTemplateKey>())
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                .ToList();


        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order) triggerObject;

            _mail = new PayOrderTemplate(order);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order) triggerObject;

            _mail = new PayOrderTemplate(order);

            return _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnPayOrder.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }
}
