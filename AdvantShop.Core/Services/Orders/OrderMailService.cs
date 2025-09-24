using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Letters;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Core.Services.Orders
{
    public class OrderMailService
    {
        private const string CustomerLetterFormatLine =
            "<div class='l-row'><div class='l-name vi cs-light' style='display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px;'>{0}:</div><div class='l-value vi' style='display: inline-block; margin: 5px 0;'>{1}</div></div>";

        public static void SendMail(Order order, float? bonusPlus = null)
        {
            SendMail(order, order.TotalDiscount, bonusPlus, order.ArchivedShippingName);
        }

        public static void SendMail(Order order, float totalDiscount, float? bonusPlus, string shippingName)
        {
            var customer = order.OrderCustomer;
            var email = customer.Email;

            var mail = NewOrderMailTemplate.Create(order, totalDiscount, bonusPlus, shippingName);

            if (!string.IsNullOrWhiteSpace(email))
            {
                MailService.SendMailNow(customer.CustomerID, email, mail);
            }
            MailService.SendMailNow(SettingsMail.EmailForOrders, mail, replyTo: email);
        }

        public static string GetOrderItemsLetterHtml(Order order)
        {
            return GetOrderItemsLetterHtml(order.OrderItems, order.OrderCurrency,
                order.OrderItems.Sum(oi => oi.Price * oi.Amount),
                order.OrderDiscount, order.OrderDiscountValue,
                order.Coupon, order.Certificate,
                order.TotalDiscount,
                order.ShippingCost, order.PaymentCost,
                order.TaxCost,
                order.BonusCost,
                0,
                OrderLetterTemplateKey.OrderItemsHtml);
        }
        
        public static string GetOrderItemsLetterHtml(List<OrderItem> orderItems, Currency currency, float productsPrice,
                                                    float orderDiscountPercent, float orderDiscountValue, OrderCoupon coupon, OrderCertificate certificate,
                                                    float totalDiscount, float shippingPrice, float paymentPrice, float taxesTotal, float bonusPrice,
                                                    float newBonus, OrderLetterTemplateKey templateKey)
        {
            var orderItemsHtml = new StringBuilder();
            var isHaveAnyDownloadLink = orderItems.Any(x => !string.IsNullOrEmpty(x.DownloadLink));

            orderItemsHtml.Append("<table class='orders-table' style='border-collapse: collapse; width: 100%;'>");
            orderItemsHtml.Append("<tr class='orders-table-header'>");
            orderItemsHtml.AppendFormat("<th class='photo' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: left;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Goods"));
            orderItemsHtml.Append("<th class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left; width: 50%;'></th>");
            orderItemsHtml.AppendFormat("<th class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Price"));
            orderItemsHtml.AppendFormat("<th class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center; white-space:nowrap;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Count"));
            orderItemsHtml.AppendFormat("<th class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Cost"));
            if(isHaveAnyDownloadLink && (templateKey == OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks || templateKey == OrderLetterTemplateKey.OrderItemsPlainDownloadLinks))
                orderItemsHtml.AppendFormat("<th class='download' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;'></th>");
            orderItemsHtml.Append("</tr>");

            // Добавление заказанных товаров
            foreach (var item in orderItems)
            {
                orderItemsHtml.Append("<tr>");

                Photo photo;
                if (item.PhotoID.HasValue && item.PhotoID != 0 && (photo = PhotoService.GetPhoto((int)item.PhotoID)) != null)
                {
                    orderItemsHtml.AppendFormat(
                        "<td class='photo' style='border-bottom: 1px solid #e3e3e3; margin-right: 15px; padding: 20px 5px 20px 0; padding-left: 20px; text-align: left; width:{1}px;'><img style='border:none;display:block;outline:none;text-decoration:none;max-width:100%;height:auto;' src='{0}' /></td>",
                        FoldersHelper.GetImageProductPath(ProductImageType.XSmall, photo.PhotoName, false), SettingsPictureSize.XSmallProductImageWidth);
                }
                else
                {
                    orderItemsHtml.AppendFormat("<td>&nbsp;</td>");
                }

                var productUrl = item.ProductID.HasValue ? ProductService.GetProductUrlByProductId(item.ProductID.Value) : null;

                orderItemsHtml.AppendFormat("<td class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: left; min-width:150px; width: 50%;'>" +
                                                    "<div class='description' style='display: inline-block;'>" +
                                                        "<div class='prod-name' style='font-size: 14px; margin-bottom: 5px;'>" +
                                                            "{0}" +
                                                        "</div> " +
                                                        "{1} {2} {3}" +
                                                    "</div>" +
                                            "</td>",
                                            templateKey == OrderLetterTemplateKey.OrderItemsHtml || templateKey == OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks 
                                                ? string.Format("<a href='{0}' class='cs-link' style='color: #0764c3; text-decoration: none;'>" + 
                                                                    "{1}" +
                                                                "</a>", 
                                                                !string.IsNullOrEmpty(productUrl) 
                                                                    ? SettingsMain.SiteUrl.Trim('/') + "/" + UrlService.GetLink(ParamType.Product, productUrl, item.ProductID.Value) 
                                                                    : string.Empty,
                                                                item.ArtNo + (!string.IsNullOrEmpty(item.ArtNo) ? ", " : "") + item.Name)
                                                : $"{item.ArtNo + (!string.IsNullOrEmpty(item.ArtNo) ? ", " : "") + item.Name}",
                                            item.Color.IsNotEmpty() ? "<div class='prod-option' style='margin-bottom: 5px;'><span class='cs-light' style='color: #acacac;'>" + SettingsCatalog.ColorsHeader + ":</span><span class='value cs-link' style='padding-left: 10px;'>" + item.Color + "</span></div>" : "",
                                            item.Size.IsNotEmpty() ? "<div class='prod-option' style='margin-bottom: 5px;'><span class='cs-light' style='color: #acacac;'>" + SettingsCatalog.SizesHeader + ":</span><span class='value cs-link' style='padding-left: 10px;'>" + item.Size + "</span></div>" : "",
                                            OrderService.RenderSelectedOptions(item.SelectedOptions, currency));
                orderItemsHtml.AppendFormat("<td class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center; white-space: nowrap;'>{0}</td>", item.Price.FormatPrice(currency));
                orderItemsHtml.AppendFormat("<td class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center;'>{0} {1}</td>", item.Amount, item.Unit);
                orderItemsHtml.AppendFormat("<td class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;  white-space: nowrap;'>{0}</td>", PriceService.SimpleRoundPrice(item.Price * item.Amount, currency).FormatPrice(currency));
                if (isHaveAnyDownloadLink && (templateKey == OrderLetterTemplateKey.OrderItemsHtmlDownloadLinks || templateKey == OrderLetterTemplateKey.OrderItemsPlainDownloadLinks))
                        orderItemsHtml.AppendFormat(
                            item.DownloadLink.IsNullOrEmpty() 
                                ? "<td class='download' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;  white-space: nowrap;'></td>" 
                                : "<td class='download' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;  white-space: nowrap;'><a href='{0}'>{1}</a></td>", item.DownloadLink, LocalizationService.GetResource("Core.Orders.Order.Letter.Download"));
                
                orderItemsHtml.Append("</tr>");
            }

            const string footerFormat = "<tr>" +
                                            "<td class='footer-name' colspan='4' style='border-bottom: none; padding: 5px; text-align: right;'>{0}:</td>" +
                                            "<td class='footer-value' style='border-bottom: none; padding: 5px 0; text-align: center;'>{1}</td>" +
                                        "</tr>";

            const string footerMinusFormat = "<tr>" +
                                                "<td class='footer-name' colspan='4' style='border-bottom: none; padding: 5px; text-align: right;'>{0}:</td>" +
                                                "<td class='footer-value' style='border-bottom: none; padding: 5px 0; text-align: center;'>-{1}</td>" +
                                            "</tr>";

            // Стоимость заказа
            orderItemsHtml.AppendFormat(footerFormat, LocalizationService.GetResource("Core.Orders.Order.Letter.OrderCost"), PriceService.SimpleRoundPrice(productsPrice, currency).FormatPrice(currency));

            if (orderDiscountPercent != 0 || orderDiscountValue != 0)
            {
                var productsIgnoreDiscountPrice = orderItems.Where(item => item.IgnoreOrderDiscount).Sum(item => item.Price * item.Amount);
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Discount"), PriceFormatService.FormatDiscountPercent(productsPrice - productsIgnoreDiscountPrice, orderDiscountPercent, orderDiscountValue, false));
            }

            if (bonusPrice != 0)
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Bonuses"), bonusPrice.FormatPrice(currency));

            if (certificate != null)
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Certificate"), certificate.Price.FormatPrice(currency));

            if (coupon != null)
            {
                float couponValue;
                string couponString = null;
                switch (coupon.Type)
                {
                    case CouponType.Fixed:
                        var productPrice = orderItems.Where(p => p.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        couponValue = productPrice >= coupon.Value ? coupon.Value : productPrice;
                        couponString = String.Format("-{0} ", PriceFormatService.FormatPrice(couponValue.RoundPrice(currency.Rate, currency), currency));
                        break;

                    case CouponType.Percent:
                        couponValue = orderItems.Where(p => p.IsCouponApplied).Sum(p => coupon.Value * p.Price / 100 * p.Amount);
                        couponString = String.Format("-{0} ({1}%)", PriceFormatService.FormatPrice(couponValue.RoundPrice(currency.Rate, currency), currency),
                                                       PriceFormatService.FormatPriceInvariant(coupon.Value));
                        break;
                }
                orderItemsHtml.AppendFormat(footerFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Coupon"), couponString);
            }

            // Стоимость доставки
            if (shippingPrice != 0)
                orderItemsHtml.AppendFormat(footerFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.ShippingCost"), shippingPrice.FormatPrice(currency));

            if (paymentPrice != 0)
                orderItemsHtml.AppendFormat(footerFormat,
                    paymentPrice > 0 ? LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentCost") : LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentDiscount"), paymentPrice.FormatPrice(currency));

            var total = productsPrice - totalDiscount - bonusPrice + shippingPrice + paymentPrice;
            if (total < 0) total = 0;

            // Итого
            orderItemsHtml.AppendFormat(footerFormat,
                "<b>" + LocalizationService.GetResource("Core.Orders.Order.Letter.Total") + "</b>",
                "<b>" + PriceService.SimpleRoundPrice(total, currency).FormatPrice(currency) + "</b>");

            if (newBonus > 0)
            {
                orderItemsHtml.AppendFormat(footerFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.NewBonus"), newBonus.ToString("F2"));
            }

            orderItemsHtml.Append("</table>");

            return orderItemsHtml.ToString();
        }

        public static string GetCertificatesLetterHtml(List<GiftCertificate> cetificates, Currency currency, float paymentPrice, float taxesTotal)
        {
            var certificatesHtml = new StringBuilder();

            certificatesHtml.Append("<table class='orders-table' style='border-collapse: collapse; width: 100%;'>");
            certificatesHtml.Append("<tr>");
            certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left;'>{0}</td>", LocalizationService.GetResource("Core.Orders.Order.Letter.Certificate"));
            certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center; width: 150px;'>{0}</td>", LocalizationService.GetResource("Core.Orders.Order.Letter.Price"));
            certificatesHtml.Append("</tr>");

            // Добавление заказанных сертификатов
            foreach (var item in cetificates)
            {
                certificatesHtml.Append("<tr>");
                certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", item.CertificateCode);
                certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", item.Sum.FormatPrice(currency));
                certificatesHtml.Append("</tr>");
            }

            const string footerFormat = "<tr>" +
                                            "<td style='border-bottom: none; padding: 5px 0; text-align: right;'>{0}:</td>" +
                                            "<td style='border-bottom: none; padding: 5px 0; text-align: center;'>{1}</td>" +
                                        "</tr>";

            // Налоги
            float taxesExcluded = 0f;
            TaxElement tax;
            var taxValue = TaxService.CalculateCertificateTax(cetificates.Sum(cert => cert.Sum), out tax);
            if (taxValue.HasValue)
            {
                if (tax.ShowInPrice)
                    taxesExcluded = taxValue.Value;

                certificatesHtml.AppendFormat(footerFormat,
                    (tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") : "") + " " +
                    tax.Name,
                    (tax.ShowInPrice ? "" : "+") + taxValue.Value.FormatPrice(currency));
            }

            if (paymentPrice != 0)
            {
                certificatesHtml.AppendFormat(footerFormat,
                    paymentPrice > 0
                        ? LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentCost")
                        : LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentDiscount"),
                    paymentPrice.FormatPrice(currency));
            }

            // Итого
            certificatesHtml.AppendFormat(footerFormat,
                "<b>" + LocalizationService.GetResource("Core.Orders.Order.Letter.Total") + "</b>",
                "<b>" + (cetificates.Sum(cert => cert.Sum) + paymentPrice + taxesExcluded).FormatPrice(currency) + "</b>");

            certificatesHtml.Append("</table>");

            return certificatesHtml.ToString();
        }

        public static string GenerateCustomerContactsLetterHtml(OrderCustomer customer)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(customer.FirstName))
                sb.AppendFormat("Имя" + " {0}<br/>", customer.FirstName);

            if (!string.IsNullOrEmpty(customer.LastName))
                sb.AppendFormat("Фамилия" + " {0}<br/>", customer.LastName);

            if (!string.IsNullOrEmpty(customer.Country))
                sb.AppendFormat("Страна" + " {0}<br/>", customer.Country);

            if (!string.IsNullOrEmpty(customer.Region))
                sb.AppendFormat("Регион" + " {0}<br/>", customer.Region);

            if (!string.IsNullOrEmpty(customer.City))
                sb.AppendFormat("Город" + " {0}<br/>", customer.City);

            if (!string.IsNullOrEmpty(customer.Zip))
                sb.AppendFormat("Индекс" + " {0}<br/>", customer.Zip);

            if (!string.IsNullOrEmpty(customer.GetCustomerAddress()))
                sb.AppendFormat("Адрес" + ": {0}<br/>", customer.GetCustomerAddress());

            return sb.ToString();
        }
        
        public static string GetCustomerContactsLetterHtml(OrderCustomer customer)
        {
            // Build a new mail
            var customerSb = new StringBuilder();
            customerSb.AppendFormat(CustomerLetterFormatLine, SettingsCheckout.CustomerFirstNameField, customer.FirstName);

            if (SettingsCheckout.IsShowLastName && !string.IsNullOrEmpty(customer.LastName))
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.LastName"), customer.LastName);

            if (SettingsCheckout.IsShowPatronymic && !string.IsNullOrEmpty(customer.Patronymic))
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.Patronymic"), customer.Patronymic);

            if (SettingsCheckout.IsShowPhone && !string.IsNullOrEmpty(customer.Phone))
                customerSb.AppendFormat(CustomerLetterFormatLine, SettingsCheckout.CustomerPhoneField, customer.Phone);

            if (SettingsCheckout.IsShowCountry && !string.IsNullOrEmpty(customer.Country))
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.Country"), customer.Country);

            if (SettingsCheckout.IsShowState && customer.Region.IsNotEmpty())
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.Region"), customer.Region);

            if (SettingsCheckout.IsShowDistrict && !string.IsNullOrEmpty(customer.District))
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.District"), customer.District);

            if (SettingsCheckout.IsShowCity && !string.IsNullOrEmpty(customer.City))
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.City"), customer.City);

            if (SettingsCheckout.IsShowZip && !string.IsNullOrEmpty(customer.Zip))
                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.Zip"), customer.Zip);

            if (SettingsCheckout.IsShowAddress)
            {
                var address = customer.GetCustomerAddress();

                customerSb.AppendFormat(CustomerLetterFormatLine, LocalizationService.GetResource("User.Registration.Address"),
                    string.IsNullOrEmpty(address)
                        ? LocalizationService.GetResource("User.Registration.AddressEmpty")
                        : address);
            }

            if (SettingsCheckout.IsShowCustomShippingField1 && customer.CustomField1.IsNotEmpty())
            {
                customerSb.AppendFormat(CustomerLetterFormatLine, SettingsCheckout.CustomShippingField1, customer.CustomField1);
            }

            if (SettingsCheckout.IsShowCustomShippingField2 && customer.CustomField2.IsNotEmpty())
            {
                customerSb.AppendFormat(CustomerLetterFormatLine, SettingsCheckout.CustomShippingField2, customer.CustomField2);
            }

            if (SettingsCheckout.IsShowCustomShippingField3 && customer.CustomField3.IsNotEmpty())
            {
                customerSb.AppendFormat(CustomerLetterFormatLine, SettingsCheckout.CustomShippingField3, customer.CustomField3);
            }

            return customerSb.ToString();
        }

        public static string GetAdditionalCustomerFieldsLetterHtml(OrderCustomer customer)
        {
            var additionalCustomerFields = string.Empty;

            foreach (var customerField in CustomerFieldService.GetCustomerFieldsWithValue(customer.CustomerID).Where(x => x.ShowInCheckout))
            {
                additionalCustomerFields += string.Format(CustomerLetterFormatLine, customerField.Name, customerField.Value);
            }

            return additionalCustomerFields;
        }

        public static Tuple<string, string, string> GetInnAndCompanyNameLetterHtml(Order order)
        {
            var inn = "";
            var companyName = "";
            var kpp = "";

            if (order.PaymentMethod is Bill method)
            {
                if (method.GetCustomerDataMethod == EGetCustomerDataMethod.FromAdditionalFields)
                {
                    var companyNameField =
                        CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerCompanyNameField.TryParseInt());
                    var innField = 
                        CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerINNField.TryParseInt());
                    
                    var fields = CustomerFieldService.GetMappedCustomerFieldsWithValue(order.OrderCustomer.CustomerID);
                    var kppField = fields.FirstOrDefault(x =>
                        !string.IsNullOrEmpty(x.Value) && x.FieldAssignment == CustomerFieldAssignment.KPP);

                    inn = innField?.Value;
                    companyName = companyNameField?.Value;
                    kpp = kppField?.Value;
                }

                if (string.IsNullOrEmpty(inn) && string.IsNullOrEmpty(companyName))
                {
                    inn = order.PaymentDetails?.INN;
                    companyName = order.PaymentDetails?.CompanyName;
                    kpp = order.PaymentDetails?.Kpp;
                }
            }
            else
            {
                inn = order.PaymentDetails?.INN;
                companyName = order.PaymentDetails?.CompanyName;
                kpp = order.PaymentDetails?.Kpp;
            }

            return new Tuple<string, string, string>(inn, companyName, kpp);
        }

        public static string GetDeliveryDateAndTimeLetterHtml(DateTime? deliveryDate, string deliveryTime, bool withDeliveryDatePrefixWord = true)
        {
            if (deliveryDate == null && string.IsNullOrEmpty(deliveryTime))
                return null;

            return 
                (withDeliveryDatePrefixWord
                    ? LocalizationService.GetResource("Core.Mails.MailFormat.DeliveryDate")
                    : "") +
                (deliveryDate != null ? deliveryDate.Value.ToString("d.MM") : "") + " " + deliveryTime;
        }
        
        public static void FormatOrderLetter(Order order, StringBuilder format, string text, bool replaceIfNull = true)
        {
            if (order != null)
            {
                format.Replace("#ORDER_NUMBER#", order.Number)
                    .Replace("#ORDER_SUM#", PriceFormatService.FormatPrice(order.Sum, order.OrderCurrency))
                    .Replace("#ORDER_STATUS#", order.OrderStatus.StatusName)
                    .Replace("#ORDER_STATUS_COMMENT#", order.StatusComment)
                    .Replace("#ORDER_TRACK_NUMBER#", order.TrackNumber)
                    .Replace("#ORDER_SHIPPING_NAME#", order.ArchivedShippingName)
                    .Replace("#ORDER_PICKPOINT_ADDRESS#", order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : string.Empty)
                    .Replace("#ORDER_PAYMENT_NAME#", order.ArchivedPaymentName)
                    .Replace("#ORDER_PAY_STATUS#",
                        LocalizationService.GetResource(order.Payed ? "Core.Orders.Order.PaySpend" : "Core.Orders.Order.PayCancel").ToLower())
                    .Replace("#ORDER_IS_PAID#",
                        LocalizationService.GetResource(order.Payed ? "Core.Orders.Order.OrderPaid" : "Core.Orders.Order.OrderNotPaid"));

                if (text.Contains("#ORDER_BILLING_SHORTLINK#"))
                {
                    if (string.IsNullOrEmpty(order.PayCode))
                        order.PayCode = OrderService.GeneratePayCode(order.OrderID);

                    format.Replace("#ORDER_BILLING_SHORTLINK#", SettingsMain.SiteUrl.Trim('/') + "/pay/" + order.PayCode);
                }
            }
            else if (replaceIfNull)
            {
                format.Replace("#ORDER_NUMBER#", "")
                    .Replace("#ORDER_SUM#", "")
                    .Replace("#ORDER_STATUS#", "")
                    .Replace("#ORDER_STATUS_COMMENT#", "")
                    .Replace("#ORDER_TRACK_NUMBER#", "")
                    .Replace("#ORDER_SHIPPING_NAME#", "")
                    .Replace("#ORDER_PICKPOINT_ADDRESS#", "")
                    .Replace("#ORDER_PAYMENT_NAME#", "")
                    .Replace("#ORDER_PAY_STATUS#", "")
                    .Replace("#ORDER_IS_PAID#", "")
                    .Replace("#ORDER_BILLING_SHORTLINK#", "");
            }
        }
    }
}