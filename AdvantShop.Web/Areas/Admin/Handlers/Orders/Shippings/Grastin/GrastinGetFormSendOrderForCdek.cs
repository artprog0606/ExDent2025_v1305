using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders.Grastin;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Grastin
{
    public class GrastinGetFormSendOrderForCdek
    {
        private readonly int _orderId;

        public GrastinGetFormSendOrderForCdek(int orderId)
        {
            _orderId = orderId;
        }
 
        public SendOrderForCdekModel Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.Grastin.Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                //Errors.Add("Неверный метод доставки");
                return null;
            }

            var shippingMethod = order.ShippingMethod;
            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();

            var grastinMethod = new Shipping.Grastin.Grastin(shippingMethod, calculationParameters);

            var orderSum = order.Sum;
            //var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = shippingMethod != null ? shippingMethod.ShippingCurrency : null;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                //order.OrderItems.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                //shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var model = new SendOrderForCdekModel()
            {
                OrderId = _orderId,
                OrderNumber = order.Number,
                OrderPrefix = grastinMethod.OrderPrefix,
                Seats = 1,
                AssessedCost = grastinMethod.Insure ? orderSum : 10f,
            };

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.Cash)),
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.PickPoint))
            };
            model.CashOnDelivery = !order.Payed || (order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey));

            GrastinEventWidgetData grastinEventWidget = null;
            var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
            if (orderPickPoint != null && orderPickPoint.AdditionalData.IsNotEmpty())
            {
                try
                {
                    grastinEventWidget = JsonConvert.DeserializeObject<GrastinEventWidgetData>(orderPickPoint.AdditionalData);
                }
                catch (Exception)
                {
                    // ignored
                }
            }


            if (order.OrderRecipient != null)
            {
                model.Buyer = string.Join(" ",
                    (new[] {order.OrderRecipient.LastName, order.OrderRecipient.FirstName, order.OrderRecipient.Patronymic})
                        .Where(x => !string.IsNullOrEmpty(x)));

                var phone = order.OrderRecipient.StandardPhone.HasValue
                    ? order.OrderRecipient.StandardPhone.Value.ToString()
                    : string.Empty;

                if (phone.StartsWith("7"))
                    phone = "8" + phone.Substring(1);

                if (phone.Length == 10)
                    phone = "8" + phone;

                model.Phone = phone;
            }
            if (order.OrderCustomer != null)
            {
                model.Email = order.OrderCustomer.Email;

                var addresses = new List<string>()
                {
                    order.OrderCustomer.Zip, order.OrderCustomer.Country, order.OrderCustomer.Region,
                    order.OrderCustomer.City, order.OrderCustomer.Street, order.OrderCustomer.House
                };

                if (grastinEventWidget == null || grastinEventWidget.DeliveryType != EnDeliveryType.PickPoint)
                {

                    if (order.OrderCustomer.Apartment.IsNotEmpty())
                        addresses.Add("кв. " + order.OrderCustomer.Apartment);

                    if (order.OrderCustomer.Structure.IsNotEmpty())
                        addresses.Add("строение/Корпус " + order.OrderCustomer.Structure);

                    if (order.OrderCustomer.Entrance.IsNotEmpty())
                        addresses.Add("подъезд " + order.OrderCustomer.Entrance);

                    if (order.OrderCustomer.Floor.IsNotEmpty())
                        addresses.Add("этаж " + order.OrderCustomer.Floor);

                }
                model.Address = string.Join(", ", addresses.Where(x => !string.IsNullOrEmpty(x)));
            }

            model.PointId = grastinEventWidget != null &&
                                 grastinEventWidget.DeliveryType == EnDeliveryType.PickPoint
                ? grastinEventWidget.PickPointId
                : null;

            model.TakeWarehouse = grastinEventWidget != null
                ? grastinEventWidget.CityFrom
                : null;

            model.Services =
                Enum.GetValues(typeof (EnCdekService))
                    .Cast<EnCdekService>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int) x).ToString()
                    })
                    .ToList();

            model.Service = grastinEventWidget != null &&
                                 grastinEventWidget.DeliveryType == EnDeliveryType.PickPoint
                ? EnCdekService.PickPoint
                : EnCdekService.Courier;

            if (string.IsNullOrEmpty(model.TakeWarehouse))
                model.TakeWarehouse = grastinMethod.WidgetFromCity;

            var service = new GrastinApiService(grastinMethod.ApiKey);

            var city = grastinEventWidget != null && !string.IsNullOrEmpty(grastinEventWidget.CityTo)
                ? grastinEventWidget.CityTo
                : order.OrderCustomer != null ? order.OrderCustomer.City : string.Empty;

            var cdekSelfPickups = service.GetCdekSelfPickup(city);
            model.AddressPoints = cdekSelfPickups != null
                ? cdekSelfPickups
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id,
                    }).ToList()
                : new List<SelectListItem>();

            var takeWarehouses = service.GetWarehouses();
            model.TakeWarehouses = takeWarehouses != null
                ? takeWarehouses.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name
                }).ToList()
                : new List<SelectListItem>();

            var cdekCities = service.GetCdekCities();
            model.Cities = cdekCities != null
                ? cdekCities
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id,
                        Selected = string.Equals(x.Name, city, StringComparison.OrdinalIgnoreCase)
                    }).ToList()
                : new List<SelectListItem>();

            return model;
        }
   }
}