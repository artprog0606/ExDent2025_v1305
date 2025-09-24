using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.Yandex.Api;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Yandex
{
    public partial class YandexDelivery : IUnloadOrder
    {
        public const string KeyConfirmModifyArtNo = "confirm_modify_artno";
        public const string KeySelectValidDeliveryDate = "select_valid_delivery_date";

        public UnloadOrderResult UnloadOrder(Order order) => UnloadOrder(order, null, null);
        
        public UnloadOrderResult UnloadOrder(Order order, string additionalAction, object additionalActionData)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            if (order.OrderCustomer == null)
                return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");
            
            if (OrderService.GetOrderAdditionalData(order.OrderID, KeyNameYandexRequestIdInOrderAdditionalData).IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            if (order.OrderPickPoint == null || order.OrderPickPoint.AdditionalData.IsNullOrEmpty())
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах расчета доставки.");
    
            YandexDeliveryCalculateOption yandexDeliveryCalculateOption = null;

            try
            {
                yandexDeliveryCalculateOption =
                    JsonConvert.DeserializeObject<YandexDeliveryCalculateOption>(order.OrderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (yandexDeliveryCalculateOption is null)
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах расчета доставки");

            var address = string.Empty;
            var availableInterval = new IntervalOffer();
            if (yandexDeliveryCalculateOption.IsCourier)
            {
                if(additionalAction == KeySelectValidDeliveryDate &&  additionalActionData is ChangeDeliveryDateDto dto)
                {
                    var additionalData = dto;
                    order.DeliveryDate = additionalData.SelectedInterval.From.TryParseDateTime();
                    order.DeliveryTime = additionalData.TimeOfDelivery;

                    yandexDeliveryCalculateOption.IntervalFrom = additionalData.SelectedInterval.From;
                    yandexDeliveryCalculateOption.IntervalTo = additionalData.SelectedInterval.To;
                    order.OrderPickPoint.AdditionalData = JsonConvert.SerializeObject(yandexDeliveryCalculateOption);
                    OrderService.AddUpdateOrderPickPoint(order.OrderID, order.OrderPickPoint, new OrderChangedBy("YandexDelivery"));
                }

                address = string.Join(", ",
                                new[] {
                                    order.OrderCustomer.Country,
                                    order.OrderCustomer.City,
                                    order.OrderCustomer.Region,
                                    order.OrderCustomer.District,
                                    order.OrderCustomer.Street,
                                    order.OrderCustomer.House,
                                    order.OrderCustomer.Structure,
                }.Where(x => x.IsNotEmpty()));

                var intervalIsSelected = yandexDeliveryCalculateOption.IntervalFrom != null && yandexDeliveryCalculateOption.IntervalTo != null;
                var intervals = YandexDeliveryApiService.GetDeliveryInterval(new DeliveryIntervalParams { FullAddress = address });
                availableInterval = intervalIsSelected
                    ? intervals.FirstOrDefault(x => x.From == yandexDeliveryCalculateOption.IntervalFrom && x.To == yandexDeliveryCalculateOption.IntervalTo)
                    : null;

                if (availableInterval is null)
                {
                    var timesOfDelivery = GetTimesOfDelivery(intervals);
                    if (timesOfDelivery.Count == 0)
                        return UnloadOrderResult.CreateFailedResult("Интервалы доставки не доступны");
                    
                    var datesOfDelivery = timesOfDelivery?.Keys;
                    var requestData = new RequestChangeDeliveryDate { TimesOfDelivery = timesOfDelivery, DatesOfDelivery = datesOfDelivery };
                    return UnloadOrderResult.CreateFailedResult(
                        "Ранее выбранное время доставки не доступно.",
                        KeySelectValidDeliveryDate,
                        requestData);
                }
            }

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.Cash)),
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.CashOnDelivery)),
            };

            var dimensions = GetDimensions(10).Select(x => (int)Math.Ceiling(x)).ToList();
            var weightInGrams = (int)Math.Ceiling(GetTotalWeight(1000));

            var customerPhone = order.OrderRecipient != null && order.OrderRecipient.StandardPhone.HasValue
                ? order.OrderRecipient.StandardPhone.Value.ToString()
                : string.Empty;

            if (customerPhone.StartsWith("8"))
                customerPhone = "7" + customerPhone.Substring(1);

            if (customerPhone.Length == 10)
                customerPhone = "7" + customerPhone;

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = order.ShippingMethod.ShippingCurrency;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var fiscalItems = order.GetOrderItemsWithDiscountsAndFee()
                                   .AcceptableDifference(0.1f)
                                   .WithCurrency(shippingCurrency)
                                   .CeilingAmountToInteger()
                                   .GetItems();

            var addArtNoPostfix = additionalAction == KeyConfirmModifyArtNo && additionalActionData is true;
            var itemIndex = 0;
            var items = new List<Item>();
            var placeBarcode = $"yd-{order.OrderID}-1";
            foreach (var item in fiscalItems)
            {
                items.Add(new Item
                {
                    Article = item.ArtNo + (addArtNoPostfix && items.Any(x => x.Article == item.ArtNo)
                                                ? $"-yd-position-{itemIndex}"
                                                : string.Empty),
                    Name = item.Name,
                    BillingDetails = new BillingDetails { AssessedUnitPrice = (int)Math.Ceiling(item.Price * 100), UnitPrice = (int)Math.Ceiling(item.Price * 100), NDS = GetVat(item.TaxType) },
                    Count = (int)item.Amount,
                    PhysicalDims = new PhysicalDims { Dx = (int)Math.Ceiling(item.Length / 10), Dy = (int)Math.Ceiling(item.Height / 10), Dz = (int)Math.Ceiling(item.Width / 10), WeightGross = (int)Math.Ceiling(item.Weight * 1000) },
                    PlaceBarcode = placeBarcode,
                    Barcode = item.BarCode
                });
                itemIndex++;
            }
                         
            var yandexOrder = new CreateOrderParams
            {
                BillingInfo = new BillingInfo(),
                Destination = new Destination(),
                Items = items,
                Info = new Info { OperatorRequestId = order.OrderID.ToString(), ReferralSource = "advantshop" },
                Places = new List<Place> 
                { 
                    new Place 
                    { 
                        Barcode = placeBarcode,
                        PhysicalDims = new PhysicalDims { Dx = dimensions[0], Dy = dimensions[2], Dz = dimensions[1], WeightGross = weightInGrams }
                    } 
                },
                RecipientInfo = new RecipientInfo {
                    FirstName = order.OrderRecipient?.FirstName,
                    LastName = order.OrderRecipient?.LastName,
                    Partonymic = order.OrderRecipient?.Patronymic,
                    Email = order.OrderCustomer?.Email,
                    Phone = customerPhone
                }
            };

            if (!order.Payed &&
                (order.PaymentMethod != null &&
                    paymentsCash.Contains(order.PaymentMethod.PaymentKey)))
            {
                yandexOrder.BillingInfo.DeliveryCost = (int)Math.Ceiling(shippingCost * 100);
                yandexOrder.BillingInfo.PaymentMethod =
                    order.PaymentMethod.PaymentMethodId == PaymentCodCardId
                        ? PaymentMethodType.CardOnReceipt
                        : PaymentMethodType.CashOnReceipt;
            }
            else
                yandexOrder.BillingInfo.PaymentMethod = PaymentMethodType.AlreadyPaid;

            var interval = new IntervalUtc();
            if (yandexDeliveryCalculateOption.IsCourier)
            {
                yandexOrder.Destination.Type = DestinationType.CustomLocation;
                yandexOrder.Destination.CustomLocation = new CustomLocation
                {
                    Details = new Details
                    {
                        FullAddress = address,
                        Room = order.OrderCustomer.Apartment,
                        Comment = order.CustomerComment
                    }
                };
                yandexOrder.LastMilePolicy = LastMilePolicyType.TimeInterval;
                interval.From = availableInterval.From;
                interval.To = availableInterval.To;
            }
            else if (order.OrderPickPoint.PickPointId.IsNotEmpty())
            {
                yandexOrder.Destination.Type = DestinationType.PlatformStation;
                yandexOrder.Destination.PlatformStation = new PlatformStation { PlatformId = order.OrderPickPoint.PickPointId };
                yandexOrder.LastMilePolicy = LastMilePolicyType.SelfPickup;
                var availableIntervals = YandexDeliveryApiService.GetDeliveryInterval(
                    new DeliveryIntervalParams { SelfPickupId = order.OrderPickPoint.PickPointId });
                if (availableIntervals?.Count > 0)
                {
                    // берем ближайший доступный интервал для доставки
                    availableInterval = availableIntervals.FirstOrDefault();
                    interval.From = availableInterval.From;
                    interval.To = availableInterval.To;
                }
                else
                {
                    return UnloadOrderResult.CreateFailedResult("Нет доступных интервалов доставки");
                }
            }
            yandexOrder.Destination.IntervalUtc = interval;
            
            var result = YandexDeliveryApiService.CreateOrder(yandexOrder);

            if(result?.Offers.Count > 0)
            {
                var offer = result.Offers.FirstOrDefault();
                if (offer != null)
                {
                    var requestId = YandexDeliveryApiService.ConfirmOrder(offer.OfferId);
                    if (!requestId.IsNullOrEmpty())
                    {
                        OrderService.AddUpdateOrderAdditionalData(order.OrderID, KeyNameYandexRequestIdInOrderAdditionalData, requestId);
                        order.TrackNumber = requestId;
                        OrderService.UpdateOrderMain(order);

                        return UnloadOrderResult.CreateSuccessResult();
                    }
                }
            }
            if (YandexDeliveryApiService.LastActionErrors != null)
            {
                if (YandexDeliveryApiService.LastActionErrors.Any(x => x.StartsWith("there are items duplication", StringComparison.OrdinalIgnoreCase)))
                    return UnloadOrderResult.CreateFailedResult(string.Join("\n", YandexDeliveryApiService.LastActionErrors), KeyConfirmModifyArtNo);
                
                return UnloadOrderResult.CreateFailedResult(string.Join("\n", YandexDeliveryApiService.LastActionErrors));
            }

            return UnloadOrderResult.CreateFailedResult("Что-то пошло не так");
        }

        private int GetVat(TaxType? taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat18:
                case TaxType.Vat20:
                    return 20;
                case TaxType.Vat10:
                    return 10;
                case TaxType.Vat5:
                    return 5;
                case TaxType.Vat7:
                    return 7;
                case TaxType.Vat0:
                case TaxType.VatWithout:
                    return 0;
            }
            return 20;
        }
    }
}