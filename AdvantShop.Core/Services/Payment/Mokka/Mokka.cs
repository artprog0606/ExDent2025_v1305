using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Payment.Mokka.Api;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using Order = AdvantShop.Core.Services.Payment.Mokka.Api.Order;

namespace AdvantShop.Payment
{
    [PaymentKey("Mokka")]
    public class Mokka : PaymentMethod, ICreditPaymentMethod
    {
        public string StoreId  { get; set; }
        public string SecretKey { get; set; }
        public float NumberOfParts { get; set; }
        public bool SandboxServer { get; set; }

        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public bool NotSendStatistic { get; set; }
        public bool ActiveCreditPayment => true;
        public bool ShowCreditButtonInProductCard => false;
        public string CreditButtonTextInProductCard => "Оплата Мокка";
        public override string ButtonText => "Оплатить частями";

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[] {"RUB"};

        public override ProcessType ProcessType => ProcessType.Javascript;

        public override NotificationType NotificationType => NotificationType.Handler;

        public override UrlStatus ShowUrls => UrlStatus.None;

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {MokkaTemplate.StoreId, StoreId},
                    {MokkaTemplate.SecretKey, SecretKey},
                    {MokkaTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                    {MokkaTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                    {MokkaTemplate.NumberOfParts, NumberOfParts.ToInvariantString()},
                    {MokkaTemplate.SandboxServer, SandboxServer.ToString()},
                    {MokkaTemplate.NotSendStatistic, NotSendStatistic.ToString()},
                };
            }
            set
            {
                StoreId = value.ElementOrDefault(MokkaTemplate.StoreId);
                SecretKey = value.ElementOrDefault(MokkaTemplate.SecretKey);
                MinimumPrice = value.ElementOrDefault(MokkaTemplate.MinimumPrice).TryParseFloat();
                MaximumPrice = value.ElementOrDefault(MokkaTemplate.MaximumPrice).TryParseFloat(true);
                NumberOfParts = value.ElementOrDefault(MokkaTemplate.NumberOfParts).TryParseFloat();
                SandboxServer = value.ElementOrDefault(MokkaTemplate.SandboxServer).TryParseBool();
                NotSendStatistic = value.ElementOrDefault(MokkaTemplate.NotSendStatistic).TryParseBool();
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            try
            {
                if (NumberOfParts > 0f)
                    return finalPrice / NumberOfParts;
                
                var client = MokkaClient.Create(StoreId, SecretKey, SandboxServer);
                var creditPreSchedule = client.Schedule(new ScheduleParameters {Amount = finalPrice});

                if (creditPreSchedule != null
                    && creditPreSchedule.Status == 0
                    && creditPreSchedule.PaymentSchedule?.Count > 0)
                {
                    var scheduleMinMonthlyPayment =
                        creditPreSchedule.PaymentSchedule
                            .OrderBy(x => x.MonthlyPayment)
                            .FirstOrDefault();

                    return scheduleMinMonthlyPayment?.MonthlyPayment;
                }
            }
            catch (Exception e)
            {
                Debug.Log.Warn(e);
            }
            return null;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice)
        {
            try
            {
                if (NumberOfParts > 0f)
                    return default;
                
                var client = MokkaClient.Create(StoreId, SecretKey, SandboxServer);
                var creditPreSchedule = client.Schedule(new ScheduleParameters {Amount = finalPrice});

                if (creditPreSchedule != null
                    && creditPreSchedule.Status == 0
                    && creditPreSchedule.PaymentSchedule?.Count > 0)
                {
                    var scheduleMinMonthlyPayment =
                        creditPreSchedule.PaymentSchedule
                                         .OrderBy(x => x.Term)
                                         .FirstOrDefault();

                    return scheduleMinMonthlyPayment is null
                        ? default
                        : (scheduleMinMonthlyPayment.MonthlyPayment, scheduleMinMonthlyPayment.Term);
                }
            }
            catch (Exception e)
            {
                Debug.Log.Warn(e);
            }
            return default;
        }

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            if (!Core.Modules.ModulesRepository.IsActiveModule("Mokka"))
                return null;
            
            var option = base.GetOption(shippingOption, preCoast, customerType);

            if (option.Desc.IsNullOrEmpty())
                option.Desc =
                    "Получи аванс Мокка в два клика и сразу используй его для покупок с оплатой частями. Нужен только паспорт и мобильный телефон.";
            // try
            // {
            //     var client = MokkaClient.Create(StoreId, SecretKey);
            //     var creditPreSchedule = client.Schedule(new ScheduleParameters {Amount = preCoast});
            //
            //     if (creditPreSchedule != null
            //         && creditPreSchedule.Status == 0
            //         && creditPreSchedule.PaymentSchedule?.Count > 0)
            //     {
            //         var scheduleMinMonthlyPayment =
            //             creditPreSchedule.PaymentSchedule
            //                 .OrderBy(x => x.MonthlyPayment)
            //                 .FirstOrDefault();
            //
            //         if (scheduleMinMonthlyPayment != null)
            //         {
            //             option.Desc = (option.Desc ?? string.Empty) + string.Format(
            //                 " Кредит на {0} месяцев с платежом {1}* в месяц. Полная сумма {2}.",
            //                 scheduleMinMonthlyPayment.Term,
            //                 scheduleMinMonthlyPayment.MonthlyPayment,
            //                 scheduleMinMonthlyPayment.Total);
            //         }
            //     }
            // }
            // catch (Exception e)
            // {
            //     Debug.Log.Warn(e);
            // }

            return option;
        }
        
        public override string ProcessServerRequest(AdvantShop.Orders.Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var primaryPhone = order.OrderCustomer.StandardPhone.ToString();
            if (primaryPhone.Length > 10)
                primaryPhone = primaryPhone.Substring(primaryPhone.Length - 10);

            var customer = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
            var birthDate = customer?.BirthDay;
            var address =
                string.Join(
                    ", ",
                    new[]
                    {
                        order.OrderCustomer.Country,
                        order.OrderCustomer.Region,
                        order.OrderCustomer.District,
                        order.OrderCustomer.City,
                        order.OrderCustomer.Street,
                        order.OrderCustomer.House,
                        order.OrderCustomer.Structure,
                    }.Where(x => x.IsNotEmpty())
                );
            
            var fiscalItems = order.GetOrderItemsForFiscal(paymentCurrency, toIntegerAmount: true).ToList();
            var trafficSource = OrderTrafficSourceService.Get(order.OrderID, TrafficSourceType.Order);

            var createOrder = new CheckoutParameters
            {
                CallbackUrl = NotificationUrl,
                RedirectUrl = SettingsMain.SiteUrl,
                PrimaryEmail = order.OrderCustomer.Email,
                PrimaryPhone = primaryPhone,
                SkipResultPage = false,
                CurrentOrder = new CurrentOrderCheckout
                {
                    OrderId = order.Number,
                    Amount = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency)
                        .ToInvariantString(),
                },
                Person = new Person
                {
                    FirstName = order.OrderCustomer.FirstName,
                    Surname = order.OrderCustomer.LastName,
                    Patronymic = order.OrderCustomer.Patronymic,
                    BirthDate = birthDate,
                },
                CartItems = fiscalItems
                    .Select(item =>
                    {
                        var product =
                            item.ProductID.HasValue
                                ? ProductService.GetProduct(item.ProductID.Value)
                                : null;
                        return new CartItem
                        {
                            Sku = item.ArtNo,
                            Name = item.Name,
                            Price = item.Price,
                            Quantity = (int)item.Amount,
                            Unit = item.Unit,
                            Brand = product?.Brand?.Name,
                            Category = product?.MainCategory?.Name,
                        };
                    })
                    .ToList(),
                DeliveryInfo = new DeliveryInfo
                {
                    FirstName = order.OrderCustomer.FirstName,
                    Surname = order.OrderCustomer.LastName,
                    Patronymic = order.OrderCustomer.Patronymic,
                    Type = order.ArchivedShippingName,
                    Address = address,
                    Email = order.OrderCustomer.Email,
                    Phone = primaryPhone,
                },
                AdditionalData = new AdditionalDataCheckout
                {
                    PreviousUrl = trafficSource?.Referrer,
                    Channel = 
                        order.OrderSource?.Type == OrderType.Mobile
                        ? "mobile"
                        : null,
                    Client = new Client
                    {
                        Email = order.OrderCustomer.Email,
                        Phone = order.OrderCustomer.StandardPhone.ToString(),
                        ClientId = order.OrderCustomer.CustomerID.ToString(),
                        RegistrationDate = NotSendStatistic ? null : customer?.RegistrationDateTime,
                        DataChangeDate = null,
                        PurchasesSum = customer != null && NotSendStatistic is false ? Core.Services.Statistic.StatisticService.GetCustomerOrdersSum(customer.Id) : (float?) null,
                        PurchasesVolume = customer != null && NotSendStatistic is false ? Core.Services.Statistic.StatisticService.GetCustomerOrdersCount(customer.Id) : (int?) null,
                        Birthdate = birthDate,
                    },
                    Order = new Order
                    {
                        Country = CountryService.GetCountry(SettingsMain.SellerCountryId)?.Iso2,//todo
                        Currency = "rub",
                        OrderPrice = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    },
                    Delivery = new Delivery
                    {
                        ReceiverName = string.Join(" ", order.OrderCustomer.FirstName, order.OrderCustomer.LastName),
                        DeliveryAddress = address,
                    },
                    Purchase = fiscalItems
                        .Select(item =>
                        {
                            var product =
                                item.ProductID.HasValue
                                    ? ProductService.GetProduct(item.ProductID.Value)
                                    : null;

                            return new Purchase
                            {
                                ProductName = item.Name,
                                Number = (int)item.Amount,
                                ProductPrice = item.Price,
                                Breadcrumbs =
                                    product is null
                                        ? null
                                        : string.Join(" > ",
                                            CategoryService.GetParentCategories(product.CategoryId).Select(x => x.Name))
                            };
                        })
                        .ToList()
                }
            };

            var client = MokkaClient.Create(StoreId, SecretKey, SandboxServer);
            var response = client.Checkout(createOrder);
            
            // если получаем ошибку 23 ("Order expired")
            // то заказ протух, нужно генерировать новый CurrentOrder.OrderId
            return response?.IframeUrl;
        }

        public override string ProcessResponse(HttpContext context)
        {
            string payload = null;

            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            payload = (new StreamReader(context.Request.InputStream)).ReadToEnd();
            
            if (payload.IsNullOrEmpty())
                return NotificationMessahges.InvalidRequestData;

            var client = MokkaClient.Create(StoreId, SecretKey, SandboxServer);
            var callback = client.Deserializer.DeserializeObject<CheckoutCallback>(payload);
            
            if (string.IsNullOrEmpty(callback?.OrderId))
                return NotificationMessahges.InvalidRequestData;
            
            if (callback?.Decision != "approved")
                return "Уведомление принято";
              
            var paymentStatus = client.GetStatus(new StatusParameters(){OrderId = callback.OrderId});
            
            if (paymentStatus?.CurrentOrder?.Decision != "approved"
                && paymentStatus?.CurrentOrder?.Status != "hold")
                return "Уведомление принято";
            
            var order = OrderService.GetOrderByNumber(paymentStatus.CurrentOrder.OrderId);

            if (
                order != null &&
                Math.Abs(paymentStatus.CurrentOrder.Amount - order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)) < 1)
            {
                OrderService.PayOrder(order.OrderID, true);
                return NotificationMessahges.SuccessfullPayment(order.Number);
            }
            
            return NotificationMessahges.InvalidRequestData;
        }
    }
}