using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using AdvantShop.Web.Admin.Models.Marketing.Certificates;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Certificates
{
    public class AddCertificateHandler: ICommandHandler
    {
        private readonly CertificatesFilterModel _model;
        private readonly int _paymentId;

        public AddCertificateHandler(CertificatesFilterModel model, int paymentId)
        {
            _model = model;
            _paymentId = paymentId;
        }

        public void Execute()
        {
            var customer = CustomerContext.CurrentCustomer;

            TaxElement tax;
            var taxValue = TaxService.CalculateCertificateTax(_model.Sum.TryParseFloat(), out tax);

            var taxOverPay = taxValue.HasValue && !tax.ShowInPrice ? taxValue.Value : 0f;

            var orderSum = _model.Sum.TryParseFloat() + taxOverPay;

            var payment = PaymentService.GetPaymentMethod(_paymentId);
            //var paymentPrice = payment.Extracharge == 0 ? 0 : (payment.ExtrachargeType == ExtrachargeType.Fixed ? payment.Extracharge : payment.Extracharge / 100 * certificate.Sum + taxOverPay);
            var paymentPrice = payment.GetExtracharge(orderSum);


            var currency = CurrencyService.CurrentCurrency;
            var orderSource = OrderSourceService.GetOrderSource(OrderType.ShoppingCart);

            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderCustomer = new OrderCustomer
                {
                    CustomerID = customer.Id,
                    Email = _model.FromEmail.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.FromEmail) : customer.EMail,
                    FirstName = _model.FromName,
                },
                OrderRecipient = new OrderRecipient()
                {
                    FirstName = HttpUtility.HtmlEncode(_model.ToName)
                },
                GroupName = customer.CustomerGroup.GroupName,
                GroupDiscount = customer.CustomerGroup.GroupDiscount,
                OrderCurrency = currency,
                OrderStatusId = OrderStatusService.DefaultOrderStatus,
                AffiliateID = 0,
                ArchivedShippingName = LocalizationService.GetResource("Core.Orders.GiftCertificate.DeliveryByEmail"),
                PaymentMethodId = payment.PaymentMethodId,
                ArchivedPaymentName = payment.Name,
                PaymentDetails = null,
                Sum = orderSum + paymentPrice,
                PaymentCost = paymentPrice,
                OrderCertificates = new List<GiftCertificate>
                {
                    new GiftCertificate()
                    {
                        CertificateCode = _model.CertificateCode,
                        FromName = HttpUtility.HtmlEncode(_model.FromName ?? string.Empty),
                        ToName = HttpUtility.HtmlEncode(_model.ToName ?? string.Empty),
                        Sum = _model.Sum.TryParseFloat(),
                        ToEmail = HttpUtility.HtmlEncode(_model.ToEmail ?? string.Empty),
                        Enable = _model.Enable ?? false,
                        CertificateMessage = HttpUtility.HtmlEncode(_model.CertificateMessage ?? string.Empty)
                    }
                },
                TaxCost = taxValue ?? 0f,
                Taxes = taxValue.HasValue 
                    ? new List<OrderTax> {new OrderTax { TaxId = tax.TaxId, Name = tax.Name, ShowInPrice = tax.ShowInPrice, Sum = taxValue.Value }}
                    : new List<OrderTax>(),
                OrderSourceId = orderSource.Id
            };

            order.PaymentDetails = order.PaymentMethod.PaymentDetails();

            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

            order.OrderID = OrderService.AddOrder(order, changedBy);

            OrderStatusService.ChangeOrderStatusForNewOrder(order.OrderID);

            var email = _model.FromEmail;
            var mail = NewOrderMailTemplate.CreateForCertificate(order);

            MailService.SendMailNow(CustomerContext.CustomerId, email, mail);
            MailService.SendMailNow(SettingsMail.EmailForOrders, mail, replyTo: email);
        }
    }
}