using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Payment.Kupivkredit;
using System.IO;
using System.Web;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("Kupivkredit")]
    public class Kupivkredit : PaymentMethod, ICreditPaymentMethod
    {
        /*
         * Примечание:
         * Заказы на сумму менее 3000 руб. не обарабатываются
         * 
         * Тестовые данные:
         * Id партнера: 1-178YO4Z
         * API key: 123qwe
         * API secret ($salt или "соль" подписи сообщения): 321ewq
         * СМС код подтверждения - 1010
         * 
         */
        #region Fields
        
        private const int MinOrderPrice = 3000;
        private const int DefFirstPayment = 10;

        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment => true;
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;

        public string ShopId { get; set; }
        public string ShowCaseId { get; set; }
        public string PromoCode { get; set; }
        public bool UseTest { get; set; }
        
        #endregion

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.FailUrl | UrlStatus.ReturnUrl /*| UrlStatus.NotificationUrl*/; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {KupivkreditTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                               {KupivkreditTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                               {KupivkreditTemplate.FirstPayment, FirstPayment.ToInvariantString()},
                               {KupivkreditTemplate.ShopId, ShopId},
                               {KupivkreditTemplate.UseTest, UseTest.ToString()},
                               {KupivkreditTemplate.ShowCaseId, ShowCaseId},
                               {KupivkreditTemplate.PromoCode, PromoCode},
                           };
            }
            set
            {
                MinimumPrice = value.ElementOrDefault(KupivkreditTemplate.MinimumPrice) != null ? value.ElementOrDefault(KupivkreditTemplate.MinimumPrice).TryParseFloat() : MinOrderPrice;
                if (MinimumPrice < MinOrderPrice)
                {
                    MinimumPrice = MinOrderPrice;
                }

                MaximumPrice = value.ElementOrDefault(KupivkreditTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(KupivkreditTemplate.FirstPayment) != null ? value.ElementOrDefault(KupivkreditTemplate.FirstPayment).TryParseFloat() : DefFirstPayment;
                UseTest = value.ElementOrDefault(KupivkreditTemplate.UseTest).TryParseBool();
                ShopId = value.ElementOrDefault(KupivkreditTemplate.ShopId);
                ShowCaseId = value.ElementOrDefault(KupivkreditTemplate.ShowCaseId);
                PromoCode = value.ElementOrDefault(KupivkreditTemplate.PromoCode);
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;

        public override string ProcessServerRequest(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var createOrder = new TinkoffCreateOrder
            {
                ShopId = ShopId,
                ShowcaseId = ShowCaseId.IsNullOrEmpty() ? null : ShowCaseId, // выдает ошибку на пустую строку
                PromoCode = PromoCode.IsNullOrEmpty() ? null : PromoCode, // выдает ошибку на пустую строку
                Sum = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency),
                OrderNumber = order.Number,
                DemoFlow = UseTest
                    ? order.Sum == 1f
                        ? EnDemoFlow.Sms
                        : order.Sum == 2f
                            ? EnDemoFlow.Reject
                            : EnDemoFlow.Appointment
                    : (EnDemoFlow?)null,
                FailUrl = FailUrl,
                SuccessUrl = SuccessUrl,
            };
            if (order.OrderCustomer != null)
            {
                var phone = order.OrderCustomer.StandardPhone.ToString();
                if (phone.Length > 10)
                    phone = phone.Substring(phone.Length - 10);

                createOrder.Values = new Values
                {
                    Contact = new Contact
                    {
                        Email = order.OrderCustomer.Email,
                        MobilePhone = phone,
                        Fio = new Fio
                        {
                            FirstName = order.OrderCustomer.FirstName,
                            LastName = order.OrderCustomer.LastName,
                            MiddleName = order.OrderCustomer.Patronymic
                        }
                    }
                };
            }

            createOrder.Items = order
                                .GetOrderItemsForFiscal(paymentCurrency, toIntegerAmount: true)
                                .Select(item => new Item()
                                {
                                    Name = item.Name,
                                    VendorCode = item.ArtNo,
                                    Price = Math.Round(item.Price, 2),
                                    Quantity = (int)Math.Ceiling(item.Amount),
                                }).ToList();
            
            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                createOrder.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x => new Item
                {
                    Name = LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName"),
                    VendorCode = x.CertificateCode,
                    Price = Math.Round(x.Sum, 2),
                    Quantity = 1,
                }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0)
            {
                createOrder.Items.Add(new Item()
                {
                    Name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    VendorCode = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    Price = Math.Round(orderShippingCostWithDiscount, 2),
                    Quantity = 1,
                });
            }

            double sum = 0d;

            foreach (var item in createOrder.Items)
                sum += item.Price * (double)item.Quantity;

            createOrder.Sum = Math.Round(sum, 2);

            var service = new TinkoffCreditService();
            var response = service.CreateOrder(createOrder, UseTest);

            if (response != null)
                return response.Link;

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            string bodyPost = null;

            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            bodyPost = (new StreamReader(context.Request.InputStream)).ReadToEnd();

            if (!string.IsNullOrEmpty(bodyPost))
            {

                var service = new TinkoffCreditService();

                var notify = service.ReadNotifyData(bodyPost);

                if (notify == null || notify.Id.IsNullOrEmpty() || notify.Demo != UseTest)
                {
                    return NotificationMessahges.InvalidRequestData;
                }

                //ToDo: запросить статус и отметить заказ
                //GetInfoOrder

                //if (notify.Success.Equals("true", StringComparison.OrdinalIgnoreCase) && notify.Status == "CONFIRMED")
                //{
                //    var order = notify.OrderId.IsNotEmpty() ? OrderService.GetOrder(notify.OrderId.Split(new[] { '_' })[0].TryParseInt()) : null;

                //    if (order != null)
                //    {
                //        OrderService.PayOrder(order.OrderID, true);
                //    }
                //}
            }

            return "OK";
        }
    }
}