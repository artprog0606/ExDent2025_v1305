using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using AdvantShop.Taxes;
using AdvantShop.ViewModel.GiftCertificate;

namespace AdvantShop.Handlers.GiftCertificate
{
    public class CreateCertificateOrderHandler
    {
        private readonly GiftCertificateViewModel _model;

        public CreateCertificateOrderHandler(GiftCertificateViewModel model)
        {
            _model = model;
        }

        public Order Execute()
        {
            var customer = CustomerContext.CurrentCustomer;
            var linkedCustomerId = default(Guid?);

            if (!customer.RegistredUser)
            {
                var existCustomer = CustomerService.GetCustomerByEmail(HttpUtility.HtmlEncode(_model.EmailFrom));
                
                if (existCustomer != null)
                    customer = existCustomer;
                else
                {
                    var c = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        Id = CustomerContext.CustomerId,
                        Password = StringHelper.GeneratePassword(8),
                        FirstName = _model.NameFrom.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.NameFrom) : "",
                        EMail = _model.EmailFrom.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.EmailFrom) : "",
                        CustomerRole = Role.User,
                        Phone = _model.Phone.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Phone) : "",
                        StandardPhone = StringHelper.ConvertToStandardPhone(_model.Phone),
                        IsAgreeForPromotionalNewsletter = false
                    };

                    CustomerService.InsertNewCustomer(c);

                    if (c.Id != Guid.Empty)
                    {
                        AuthorizeService.SignIn(c.EMail, c.Password, false, true);
                        customer = c;
                    }
                    else
                    {
                        if (_model.EmailFrom.IsNotEmpty())
                        {
                            var customerByEmail = CustomerService.GetCustomerByEmail(HttpUtility.HtmlEncode(_model.EmailFrom));
                            if (customerByEmail != null && customerByEmail.Enabled)
                                linkedCustomerId = customerByEmail.Id;
                        }
                        else if (_model.Phone.IsNotEmpty())
                        {
                            var phone = HttpUtility.HtmlEncode(_model.Phone);
                            var customerByPhone = CustomerService.GetCustomerByPhone(phone, StringHelper.ConvertToStandardPhone(phone));
                            if (customerByPhone != null && customerByPhone.Enabled)
                                linkedCustomerId = customerByPhone.Id;
                        }
                    }
                }
            }

            TaxElement tax;
            var taxValue = TaxService.CalculateCertificateTax(_model.Sum, out tax);

            var taxOverPay = taxValue.HasValue && !tax.ShowInPrice ? taxValue.Value : 0f;

            var orderSum = _model.Sum + taxOverPay;

            var payment = PaymentService.GetPaymentMethod(_model.PaymentMethod);
            //var paymentPrice = payment.Extracharge == 0 ? 0 : (payment.ExtrachargeType == ExtrachargeType.Fixed ? payment.Extracharge : payment.Extracharge / 100 * certificate.Sum + taxOverPay);
            var paymentPrice = payment.GetExtracharge(orderSum);


            var currency = CurrencyService.CurrentCurrency;
            var orderSource = OrderSourceService.GetOrderSource(OrderType.ShoppingCart);

            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderCustomer = new OrderCustomer
                {
                    CustomerID = linkedCustomerId ?? customer.Id,
                    Email = _model.EmailFrom.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.EmailFrom) : customer.EMail,
                    FirstName = _model.NameFrom,
                    Phone = _model.Phone.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Phone) : "",
                    StandardPhone = StringHelper.ConvertToStandardPhone(_model.Phone),
                    CustomerIP = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : string.Empty,

                    Country = IpZoneContext.CurrentZone.CountryName,
                    Region = IpZoneContext.CurrentZone.Region,
                    District = IpZoneContext.CurrentZone.District,
                    City = IpZoneContext.CurrentZone.City,
                    Zip = IpZoneContext.CurrentZone.Zip,
                },
                OrderRecipient = new OrderRecipient()
                {
                    FirstName = HttpUtility.HtmlEncode(_model.NameTo)
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
                OrderCertificates = new List<Orders.GiftCertificate>
                {
                    new Orders.GiftCertificate
                    {
                        CertificateCode = GiftCertificateService.GenerateCertificateCode(),
                        ToName = HttpUtility.HtmlEncode(_model.NameTo ?? string.Empty),
                        FromName = HttpUtility.HtmlEncode(_model.NameFrom ?? string.Empty),
                        Sum = _model.Sum,
                        CertificateMessage = HttpUtility.HtmlEncode(_model.Message ?? string.Empty),
                        Enable = true,
                        ToEmail = HttpUtility.HtmlEncode(_model.EmailTo ?? string.Empty)
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

            var email = _model.EmailFrom;
            var mail = NewOrderMailTemplate.CreateForCertificate(order);

            MailService.SendMailNow(CustomerContext.CustomerId, email, mail);
            MailService.SendMailNow(SettingsMail.EmailForOrders, mail, replyTo: email);

            return order;

        }
    }
}