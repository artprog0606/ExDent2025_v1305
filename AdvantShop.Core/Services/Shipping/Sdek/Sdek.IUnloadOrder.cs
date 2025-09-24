using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Shipping.Sdek.Api;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Sdek
{
    public partial class Sdek : IUnloadOrder
    {
        public UnloadOrderResult UnloadOrder(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            if (order.OrderCustomer == null)
                return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");
            
            if (OrderService.GetOrderAdditionalData(order.OrderID, KeyNameSdekOrderUuidInOrderAdditionalData).IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            int? tariffId = null;
            bool? withInsure = null;
            bool? allowInspection = null;
            bool? partialDelivery = null;
            bool? tryingOn = null;

            if (order.OrderPickPoint != null && order.OrderPickPoint.AdditionalData.IsNotEmpty())
            {
                var calcOption = JsonConvert.DeserializeObject<SdekCalculateOption>(order.OrderPickPoint.AdditionalData);
                if (calcOption != null)
                {
                    if (calcOption.TariffId != 0)
                        tariffId = calcOption.TariffId;

                    withInsure = calcOption.WithInsure;
                    allowInspection = calcOption.AllowInspection;
                    partialDelivery = calcOption.PartialDelivery;
                    tryingOn = calcOption.TryingOn;
                }

                if (!tariffId.HasValue)
                    // поддержка старых заказов с выбором одного тарифа (старых настроек)
                    tariffId = _method.Params[SdekTemplate.TariffOldParam].TryParseInt();
            }

            if (!tariffId.HasValue)
                return UnloadOrderResult.CreateFailedResult("Не удалось определить тариф");
            
            SdekParamsSendOrder parametrs = new SdekParamsSendOrder(_method);
            
            ShippingReplaceGeoService.ReplaceGeo(_method.ShippingType, order.OrderCustomer);
            
            var cityFrom = CityFromId ?? (SdekService.GetSdekCityId(CityFrom, string.Empty, string.Empty, string.Empty, SdekApiService20, out _, allowedObsoleteFindCity: true) ?? 0);
            var cityTo = SdekService.GetSdekCityId(order.OrderCustomer.City, order.OrderCustomer.District, order.OrderCustomer.Region, order.OrderCustomer.Country, SdekApiService20, out _) ?? 0;

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingTaxType = _method.TaxType;

            var shippingCurrency = _method.ShippingCurrency;
            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            string phone;
            if (order.OrderRecipient is null)
                phone = order.OrderCustomer.StandardPhone != null &&
                        order.OrderCustomer.StandardPhone.ToString().Length == 11
                        && order.OrderCustomer.StandardPhone.ToString()[0] == '7'
                    ? "+" + order.OrderCustomer.StandardPhone
                    : order.OrderCustomer.Phone;
            else
                phone = order.OrderRecipient.StandardPhone != null &&
                        order.OrderRecipient.StandardPhone.ToString().Length == 11
                        && order.OrderRecipient.StandardPhone.ToString()[0] == '7'
                    ? "+" + order.OrderRecipient.StandardPhone
                    : order.OrderRecipient.Phone;

            var sdekTariff = SdekTariffs.Tariffs.FirstOrDefault(item => item.TariffId == tariffId);

            var dimensionsInSm = GetDimensions(rate:10);
            var vatRate = GetVatRate(_method.TaxType);

            var newOrder = new NewOrder()
            {
                DeveloperKey = "96a1f68557c674d0224d760ed5455419",
                Type = 1,
                Number = order.Number,
                TariffCode = tariffId.Value,
                Comment = order.CustomerComment.Reduce(255),
                FromLocation = new OrderLocation {Code = cityFrom},
                DeliveryPoint = order.OrderPickPoint.PickPointId.IsNotEmpty()
                    ? order.OrderPickPoint.PickPointId
                    : null,
                ToLocation = order.OrderPickPoint.PickPointId.IsNullOrEmpty()
                    ? new OrderLocation()
                    {
                        Code = cityTo,
                        PostalCode = order.OrderCustomer.Zip,
                        Address = string.Join(", ", new[]
                        {
                            order.OrderCustomer.Street,
                            string.Join(" ", new[]
                            {
                                order.OrderCustomer.House,
                                order.OrderCustomer.Structure.IsNotEmpty()
                                    ? $"стр/корп {order.OrderCustomer.Structure}"
                                    : null,
                            }.Where(x => x.IsNotEmpty())),
                            order.OrderCustomer.Apartment.IsNotEmpty()
                                ? $"кв. {order.OrderCustomer.Apartment}"
                                : null
                        }.Where(x => x.IsNotEmpty()))

                    }
                    : null,
                DeliveryRecipientCost = !order.Payed && shippingCost > 0f
                    ? new MoneyParams()
                    {
                        Value = shippingCost,
                        VatRate = vatRate,
                        VatSum = vatRate.HasValue 
                            ? shippingCost * vatRate.Value / (100 + vatRate.Value)
                            : (float?)null
                    }
                    : null,
                Recipient = new Recipient()
                {
                    Name = order.OrderRecipient is null
                        ? $"{order.OrderCustomer.LastName} {order.OrderCustomer.FirstName} {order.OrderCustomer.Patronymic}"
                        : $"{order.OrderRecipient.LastName} {order.OrderRecipient.FirstName} {order.OrderRecipient.Patronymic}",
                    Email = order.OrderCustomer.Email,
                    Phones = new List<Phone> {new Phone {Number = phone}}
                },
                Packages = new List<OrderPackage>()
                {
                    new OrderPackage()
                    {
                        Number = "1",
                        Weight = (long) GetTotalWeight(1000),
                        Length = (long) Math.Ceiling(dimensionsInSm[0]),
                        Width = (long) Math.Ceiling(dimensionsInSm[1]),
                        Height = (long) Math.Ceiling(dimensionsInSm[2]),
                    }
                },
                Services = new List<ServiceParams>()
            };

            if (parametrs.SellerAddress.IsNotEmpty() ||
                parametrs.SellerName.IsNotEmpty() ||
                parametrs.SellerINN.IsNotEmpty() ||
                parametrs.SellerPhone.IsNotEmpty() ||
                parametrs.SellerOwnershipForm.IsNotEmpty())
            {
                newOrder.Seller = new Seller()
                {
                    Name = parametrs.SellerName,
                    Inn = parametrs.SellerINN,
                    Phone = parametrs.SellerPhone,
                    OwnershipForm = parametrs.SellerOwnershipForm.IsNotEmpty()
                        ? (EnOwnershipForm) parametrs.SellerOwnershipForm.TryParseInt()
                        : (EnOwnershipForm?) null,
                    Address = parametrs.SellerAddress
                };
            }

            if (parametrs.SenderCompany.IsNotEmpty() || parametrs.SenderName.IsNotEmpty() ||
                parametrs.SenderEmail.IsNotEmpty() || parametrs.SenderPhone.IsNotEmpty())
            {
                newOrder.Sender = new Sender()
                {
                    Name = parametrs.SenderName,
                    Company = parametrs.SenderCompany,
                    Email = parametrs.SenderEmail,
                    Phones = new List<Phone>() {new Phone() {Number = parametrs.SenderPhone}}
                };
            }

            if ((allowInspection ?? AllowInspection) &&// поддержка старых заказов без запоминания в SdekCalculateOption
                (sdekTariff == null || !sdekTariff.Mode.EndsWith("-П"))) // недоступно для постаматов
                newOrder.Services.Add(new ServiceParams{Code = "INSPECTION_CARGO"});

            if ((partialDelivery ?? PartialDelivery) &&// поддержка старых заказов без запоминания в SdekCalculateOption
                (sdekTariff == null || !sdekTariff.Mode.EndsWith("-П"))) // недоступно для постаматов
                newOrder.Services.Add(new ServiceParams{Code = "PART_DELIV"});

            if ((tryingOn ?? TryingOn) &&// поддержка старых заказов без запоминания в SdekCalculateOption
                (sdekTariff == null || !sdekTariff.Mode.EndsWith("-П"))) // недоступно для постаматов
                newOrder.Services.Add(new ServiceParams{Code = "TRYING_ON"});
            
            // переносим вес из items, т.к. там учтены настройки метода
            var orderItems = order.OrderItems;
            foreach (var orderItem in orderItems)
                orderItem.Weight = _calculationParameters.PreOrderItems.First(x => x.Id == orderItem.OrderItemID).Weight;
            
            orderItems = order.GetOrderItemsWithDiscountsAndFee()
                              .AcceptableDifference(0.1f)
                              .WithCurrency(shippingCurrency)
                              .CeilingAmountToInteger()
                              .GetItems() as List<OrderItem>;
            newOrder.Packages[0].Items = new List<OrderPackageItem>();

            var insure = withInsure ?? WithInsure; // поддержка старых заказов без запоминания в SdekCalculateOption
            
            foreach (var orderItem in orderItems)
            {
                if (orderItem.Amount > 999f)
                {
                    var newAmount = 1f;
                    orderItem.ConvertOrderItemToNewAmount(newAmount);
                    orderItem.Amount = newAmount;
                }
                var product = orderItem.ProductID != null ? Catalog.ProductService.GetProduct(orderItem.ProductID.Value) : null;
                vatRate = orderItem.TaxType.HasValue ? GetVatRate(orderItem.TaxType.Value) : null;
                newOrder.Packages[0].Items.Add(new OrderPackageItem
                {
                    Name = orderItem.Name,
                    NameI18n = orderItem.Name,
                    WareKey = orderItem.ArtNo,
                    Cost = insure// || !order.Payed 
                        ? orderItem.Price 
                        : 0f,
                    Payment = new MoneyParams
                    {
                        Value = order.Payed 
                            ? 0f 
                            : orderItem.Price,
                        VatRate = !order.Payed
                            ? vatRate 
                            : null,
                        VatSum = !order.Payed && vatRate.HasValue 
                            ? orderItem.Price * vatRate.Value / (100 + vatRate.Value)
                            : (float?)null
                    },
                    Weight = (long)(Math.Ceiling(orderItem.Weight * 1000)),
                    Amount = (int)orderItem.Amount,
                    Url = product != null 
                        ? Configuration.SettingsMain.SiteUrl.Trim('/') + "/products/" + product.UrlPath 
                        : null
                });
            }

            var result = SdekApiService20.CreateOrder(newOrder);

            if (result?.Entity?.Uuid != null && result?.Requests != null && result.Requests[0].Errors == null)
            {
                // запрашиваем данные по заказу
                var getOrderResult = SdekApiService20.GetOrder(result.Entity.Uuid.Value, null, null);
                var requestCreate = getOrderResult?.Requests?.FirstOrDefault(x => 
                    string.Equals("CREATE", x.Type, StringComparison.OrdinalIgnoreCase));

                // если нет конкретного результата, то запрашиваем заказ повторно через 1сек.
                if (requestCreate?.State.Equals("SUCCESSFUL", StringComparison.OrdinalIgnoreCase) == false &&
                    requestCreate?.State.Equals("INVALID", StringComparison.OrdinalIgnoreCase) == false)
                {
                    Task.Delay(1000).Wait();
                    getOrderResult = SdekApiService20.GetOrder(result.Entity.Uuid.Value, null, null);
                    requestCreate = getOrderResult?.Requests?.FirstOrDefault(x =>
                        string.Equals("CREATE", x.Type, StringComparison.OrdinalIgnoreCase));
                }

                // если возникла ошибка при добавлении
                if (requestCreate?.State.Equals("INVALID", StringComparison.OrdinalIgnoreCase) == true)
                    return requestCreate.Errors != null
                        ? UnloadOrderResult.CreateFailedResult(
                            string.Join(". ", requestCreate.Errors.Select(x => x.Message))
                                  .Replace("выбранного тарифа вес должен быть меньше",
                                       "выбранного тарифа вес и объемный вес должен быть меньше",
                                       StringComparison.OrdinalIgnoreCase))
                        : UnloadOrderResult.CreateFailedResult("Запрос на создание заказа обработался с ошибкой");
                
                // либо еще нет конкретного результата добавления, либо заказ добавился
                string sdekNumber = null;
                if (getOrderResult?.Entity?.CdekNumber.IsNotEmpty() == true)
                    sdekNumber = getOrderResult.Entity.CdekNumber;
                
                OrderService.AddUpdateOrderAdditionalData(
                    order.OrderID, 
                    KeyNameSdekOrderUuidInOrderAdditionalData,
                    result.Entity.Uuid.Value.ToString());
                
                if (sdekNumber.IsNotEmpty())
                {
                    OrderService.AddUpdateOrderAdditionalData(
                        order.OrderID, 
                        KeyNameDispatchNumberInOrderAdditionalData,
                        sdekNumber);
                    
                    order = OrderService.GetOrder(order.OrderID);
                    order.TrackNumber = sdekNumber;
                    OrderService.UpdateOrderMain(order);
                }

                return UnloadOrderResult.CreateSuccessResult();
            }

            return result?.Requests != null && result.Requests[0].Errors != null
                ? UnloadOrderResult.CreateFailedResult(
                    string.Join(". ", result.Requests[0].Errors.Select(x => x.Message))
                          .Replace("выбранного тарифа вес должен быть меньше",
                               "выбранного тарифа вес и объемный вес должен быть меньше",
                               StringComparison.OrdinalIgnoreCase))
                : SdekApiService20.LastActionErrors != null &&
                  SdekApiService20.LastActionErrors.Count > 0
                    ? UnloadOrderResult.CreateFailedResult(string.Join(". ", SdekApiService20.LastActionErrors))
                    : UnloadOrderResult.CreateFailedResult("Что-то пошло не так");
        }
        
        private int? GetVatRate(TaxType taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat0:
                    return 0;
                case TaxType.Vat10:
                    return 10;
                case TaxType.Vat18:
                    return 18;
                case TaxType.Vat20:
                    return 20;
                case TaxType.Vat5:
                    return 5;
                case TaxType.Vat7:
                    return 7;
                default:
                    return null;
            }
        }
    }
}