using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Letters
{
    public class OrderLetterBuilder : BaseLetterTemplateBuilder<Order, OrderLetterTemplateKey>
    {
        private readonly float? _totalDiscount;
        private readonly float? _bonusPlus;
        private readonly string _shippingName;
        private readonly bool? _isForCertificate;
        
        public OrderLetterBuilder(Order order) : base(order, null) {  }

        public OrderLetterBuilder(Order order, OrderLetterTemplateKey[] availableKeys) : base(order, availableKeys)
        {
        }
        
        public OrderLetterBuilder(Order order, bool? isForCertificate, OrderLetterTemplateKey[] availableKeys) : this(order, availableKeys)
        {
            _isForCertificate = isForCertificate;
        }

        public OrderLetterBuilder(Order order, float totalDiscount, float? bonusPlus, string shippingName, OrderLetterTemplateKey[] availableKeys) : this(order, availableKeys)
        {
            _totalDiscount = totalDiscount;
            _bonusPlus = bonusPlus;
            _shippingName = shippingName;
        }
        
        protected override string GetValue(OrderLetterTemplateKey key)
        {
            var order = _entity;
            
            switch (key)
            {
                case OrderLetterTemplateKey.OrderId: return order.OrderID.ToString();
                case OrderLetterTemplateKey.Number: return order.Number;
                
                case OrderLetterTemplateKey.Email: return order.OrderCustomer?.Email;
                case OrderLetterTemplateKey.FirstName: return order.OrderCustomer?.FirstName;
                case OrderLetterTemplateKey.LastName: return order.OrderCustomer?.LastName;
                case OrderLetterTemplateKey.FullName: return order.OrderCustomer?.GetFullName();
                case OrderLetterTemplateKey.Phone: return order.OrderCustomer?.Phone;
                case OrderLetterTemplateKey.City: return order.OrderCustomer?.City;
                case OrderLetterTemplateKey.Address: return order.OrderCustomer?.GetCustomerAddress();
                case OrderLetterTemplateKey.CustomerContacts:
                {
                    if (_isForCertificate.HasValue && _isForCertificate.Value)
                    {
                        var customerContacts =
                            string.Format(
                                "<div class='l-row'><div class='l-name vi cs-light' style='color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 65px;'>{0}:</div><div class='l-value vi' style='display: inline-block; margin: 5px 0;'>{1}</div></div>",
                                "Email", order.OrderCustomer?.Email);

                        return customerContacts;
                    }
                    return OrderMailService.GetCustomerContactsLetterHtml(order.OrderCustomer);
                }
                case OrderLetterTemplateKey.AdditionalCustomerFields:
                    return OrderMailService.GetAdditionalCustomerFieldsLetterHtml(order.OrderCustomer);
                case OrderLetterTemplateKey.Inn: 
                    return OrderMailService.GetInnAndCompanyNameLetterHtml(order).Item1;
                case OrderLetterTemplateKey.CompanyName:
                    return OrderMailService.GetInnAndCompanyNameLetterHtml(order).Item2;
                case OrderLetterTemplateKey.Kpp:
                    return OrderMailService.GetInnAndCompanyNameLetterHtml(order).Item3;
                case OrderLetterTemplateKey.ManagerName: return order.Manager?.FullName;
                case OrderLetterTemplateKey.ManagerSign: return order.Manager?.Sign;
                case OrderLetterTemplateKey.CustomerComment: return order.CustomerComment;
                
                case OrderLetterTemplateKey.Sum: return PriceFormatService.FormatPrice(order.Sum, order.OrderCurrency);
                case OrderLetterTemplateKey.SumWithoutCurrency: return order.Sum.FormatPriceInvariant();
                case OrderLetterTemplateKey.Status: return order.OrderStatus.StatusName;
                case OrderLetterTemplateKey.StatusComment: return order.StatusComment;
                case OrderLetterTemplateKey.StatusCommentHtml: return order.StatusComment.Replace("\r\n", "<br />");
                case OrderLetterTemplateKey.TrackNumber: return order.TrackNumber;
                case OrderLetterTemplateKey.ShippingName: return _shippingName ?? order.ArchivedShippingName;
                case OrderLetterTemplateKey.PickpointAddress: return order.OrderPickPoint?.PickPointAddress;
                case OrderLetterTemplateKey.ShippingNameWithPickpointAddressHtml:
                    return (_shippingName ?? order.ArchivedShippingName) +
                           (order.OrderPickPoint != null ? "<br />" + order.OrderPickPoint.PickPointAddress : "");
                case OrderLetterTemplateKey.PaymentName: return order.ArchivedPaymentName;
                case OrderLetterTemplateKey.PaymentStatus:
                    return LocalizationService.GetResource(
                        order.Payed ? "Core.Orders.Order.PaySpend" : "Core.Orders.Order.PayCancel").ToLower();

                case OrderLetterTemplateKey.OrderPaidOrNotPaid:
                    return LocalizationService.GetResource(
                        order.Payed ? "Core.Orders.Order.OrderPaid" : "Core.Orders.Order.OrderNotPaid");
                
                case OrderLetterTemplateKey.BillingShortLink:
                {
                    if (string.IsNullOrEmpty(order.PayCode))
                        order.PayCode = OrderService.GeneratePayCode(order.OrderID);

                    return SettingsMain.SiteUrl + "/pay/" + order.PayCode;
                }
                case OrderLetterTemplateKey.BillingLink:
                    return SettingsMain.SiteUrl +
                           "/checkout/billing?code=" + order.Code + "&hash=" + OrderService.GetBillingLinkHash(order);

                case OrderLetterTemplateKey templateKey when (templateKey == OrderLetterTemplateKey.OrderItemsHtml 
                                                              || templateKey == OrderLetterTemplateKey.OrderItemsPlain
                                                              || templateKey == OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks
                                                              || templateKey == OrderLetterTemplateKey.OrderItemsPlainDownloadLinks):
                {
                    if (_isForCertificate.HasValue && _isForCertificate.Value)
                    {
                        return OrderMailService.GetCertificatesLetterHtml(
                            order.OrderCertificates,
                            order.OrderCurrency,
                            order.PaymentCost,
                            order.TaxCost);
                    }

                    var bonusPlus = _bonusPlus ?? 0f;
                    if (_bonusPlus == null && order.BonusCardNumber != null)
                    {
                        var purchase = PurchaseService.GetByOrderId(order.OrderID);
                        if (purchase != null)
                            bonusPlus = (float) purchase.NewBonusAmount;
                    }

                    return OrderMailService.GetOrderItemsLetterHtml(
                        order.OrderItems,
                        order.OrderCurrency,
                        order.OrderItems.Sum(x => x.Price * x.Amount),
                        order.OrderDiscount,
                        order.OrderDiscountValue,
                        order.Coupon,
                        order.Certificate,
                        _totalDiscount ?? order.TotalDiscount,
                        order.ShippingCost,
                        order.PaymentCost,
                        order.TaxCost,
                        order.BonusCost,
                        bonusPlus,
                        templateKey);
                }
                case OrderLetterTemplateKey.CurrencyCode: return order.OrderCurrency.CurrencyCode;
                case OrderLetterTemplateKey.DeliveryDateWithPrefix:
                    return OrderMailService.GetDeliveryDateAndTimeLetterHtml(order.DeliveryDate, order.DeliveryInterval.ReadableString);
                case OrderLetterTemplateKey.DeliveryDate:
                    return OrderMailService.GetDeliveryDateAndTimeLetterHtml(order.DeliveryDate, order.DeliveryInterval.ReadableString, false);
                case OrderLetterTemplateKey.PostalCode:
                    return order.OrderCustomer?.Zip;
                case OrderLetterTemplateKey.ReceivingMethod:
                    return order.ReceivingMethod?.Localize();
                case OrderLetterTemplateKey.CountDevices:
                    return order.CountDevices.ToString();
                case OrderLetterTemplateKey.RecipientLastName:
                    return order.OrderRecipient?.LastName;
                case OrderLetterTemplateKey.RecipientFirstName:
                    return order.OrderRecipient?.FirstName;
                case OrderLetterTemplateKey.RecipientPhone:
                    return order.OrderRecipient?.Phone;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}