using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.PecEasyway;
using AdvantShop.Shipping.PecEasyway.Api;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PecEasyway
{
    public class PecEasywayCreateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PecEasywayCreateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId, Shipping.PecEasyway.PecEasyway.KeyNameOrderIdInOrderAdditionalData);
            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                Errors.Add("Заказ уже передан");
                return false;
            }

            var order = OrderService.GetOrder(_orderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.PecEasyway.PecEasyway).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
            if (orderPickPoint == null || orderPickPoint.AdditionalData.IsNullOrEmpty())
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            PecEasywayCalculateOption pecCalculateOption = null;

            try
            {
                pecCalculateOption =
                    JsonConvert.DeserializeObject<PecEasywayCalculateOption>(orderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (pecCalculateOption == null)
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var pecMethod = new Shipping.PecEasyway.PecEasyway(order.ShippingMethod, calculationParameters);

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var isDeliveryTypeToPoint = pecMethod.IsDeliveryTypeToPoint(new EnDeliveryType(pecCalculateOption.DeliveryType));
            var dimensions = pecMethod.GetDimensions(10).Select(x => (int)Math.Ceiling(x)).ToList();
            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = order.ShippingMethod.ShippingCurrency;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                  .AcceptableDifference(0.1f)
                                  .WithCurrency(shippingCurrency)
                                  .CeilingAmountToInteger()
                                  .GetItems();


            var pecOrder = new PecOrder
            {
                Id = order.Number,
                LocationFrom = pecCalculateOption.LocationFrom,
                LocationTo = isDeliveryTypeToPoint
                    ? orderPickPoint.PickPointAddress
                    : order.OrderCustomer != null
                        ? string.Join(
                            ", ",
                            new[] {
                                order.OrderCustomer.Zip, order.OrderCustomer.Country,
                                order.OrderCustomer.Region,
                                order.OrderCustomer.City, order.OrderCustomer.Street,
                                order.OrderCustomer.House, order.OrderCustomer.Structure,
                                order.OrderCustomer.Apartment
                            }.Where(x => x.IsNotEmpty()))
                        : null,
                PickupPointCode = isDeliveryTypeToPoint
                    ? orderPickPoint.PickPointId
                    : null,
                CargoCount = "1",
                Weight = pecMethod.GetTotalWeight(),
                Length = dimensions[2],
                Width = dimensions[1],
                Height = dimensions[0],
                AssessedCost = mustPay
                    ? orderSum
                    : orderSum - shippingCost,
                Total = mustPay
                    ? orderSum
                    : 0f,
                PaymentMethod = mustPay
                    ? EnPaymentMethod.Cash
                    : EnPaymentMethod.WithoutPayment,
                DeliveryType = pecCalculateOption.DeliveryType,
                Items = orderItems
                    .Select(x => new Item
                    {
                        Article = x.ArtNo,
                        Name = x.Name,
                        Count = (int)x.Amount,
                        Price = x.Price,
                        Sum = x.Price * x.Amount,
                        Vat = !x.TaxType.HasValue || !x.TaxRate.HasValue || x.TaxType.Value == Taxes.TaxType.VatWithout
                            ? "none"
                            : x.TaxRate.Value.ToString()
                    }).ToList(),
                Recipient = order.OrderRecipient != null
                    ? new Recipient
                    {
                        Name = string.Join(" ", (new[] { order.OrderRecipient.LastName, order.OrderRecipient.FirstName, order.OrderRecipient.Patronymic })
                                        .Where(x => !string.IsNullOrEmpty(x))),
                        Phone = order.OrderRecipient.Phone,
                    }
                    : null,
                NoPickup = pecMethod.OrderNoPickup
            };

            var result = pecMethod.PecEasywayApiService.CreateOrder(pecOrder);

            if (result != null)
            {
                if (result.Success
                    && result.Result.Data != null && result.Result.Data.Id.IsNotEmpty())
                {
                    OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                        Shipping.PecEasyway.PecEasyway.KeyNameOrderIdInOrderAdditionalData,
                        result.Result.Data.Id);


                    order.TrackNumber = result.Result.Data.Id;
                    OrderService.UpdateOrderMain(order);

                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);

                    return true;
                }
                else if (result.Error != null)
                {
                    Errors.AddRange(result.Error.Errors.Select(x => x.Description));
                }
            }

            return false;
        }
    }
}
