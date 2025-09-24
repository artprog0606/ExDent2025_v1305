using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.RussianPost.CustomsDeclarationProductData;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping.RussianPost.Api;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.RussianPost
{
    public partial class RussianPost : IUnloadOrder
    {
        public const string CustomsDeclarationProductsDataActionName = "fill_additional_data_for_customs_declaration";
        public const string FromAdminFirstActionName = "FromAdminFirstActionName";
        
        public UnloadOrderResult UnloadOrder(Order order) => UnloadOrder(order, null, null);
        
        public UnloadOrderResult UnloadOrder(Order order, string additionalAction, object additionalActionData)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            if (order.OrderCustomer == null)
                return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");

            var orderAdditionalData = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderRussianPostIdInOrderAdditionalData);
            if (orderAdditionalData.IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            CleanAddressResponse address = null;
            var validAddress = false;
            EnValidationCode addressValidationCode = null;

            var calculateOption = order.OrderPickPoint != null &&
                                  order.OrderPickPoint.AdditionalData.IsNotEmpty()
                ? JsonConvert.DeserializeObject<RussianPostCalculateOption>(order.OrderPickPoint.AdditionalData)
                : null;


            if (calculateOption is null)
            {
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах рассчета доставки");

                calculateOption = new RussianPostCalculateOption();

                calculateOption.MailType =
                    EnMailType.AsList()
                              .ToDictionary(x => x, x => x.Localize())
                              .OrderByDescending(x => x.Value.Length)
                              .First(x => order.ArchivedShippingName.Contains(x.Value)).Key;

                calculateOption.MailCategory =
                    EnMailCategory.AsList()
                                  .ToDictionary(x => x, x => x.Localize())
                                  .OrderByDescending(x => x.Value.Length)
                                  .First(x => order.ArchivedShippingName.Contains(x.Value)).Key;
            }

            var countryCode = calculateOption.CountryCode ?? 643;
            var isInternational = countryCode != 643;

            // для доставки ЕКОМ и постаматов не надо, для международки не работает
            if (order.OrderCustomer != null && 
                calculateOption.MailType != EnMailType.ECOM &&
                !IsPochtamatsTariff(calculateOption.MailType, calculateOption.MailCategory) && !isInternational)
            {
                var addressResponse =
                    RussianPostApiService.CleanAddress(new CleanAddress()
                    {
                        Id = "123",
                        Address = calculateOption.ToOps
                            ? string.Join(", ",
                                (new[]
                                {
                                    order.OrderPickPoint?.PickPointId,
                                    order.OrderCustomer.Region, order.OrderCustomer.District,
                                    order.OrderCustomer.City, "До востребования"
                                }).Where(x => !string.IsNullOrEmpty(x)))
                            : string.Join(", ",
                                (new[]
                                {
                                    order.OrderCustomer.Region, order.OrderCustomer.District,
                                    order.OrderCustomer.City, order.OrderCustomer.Street,
                                    order.OrderCustomer.House, order.OrderCustomer.Structure,
                                    order.OrderCustomer.Apartment
                                }).Where(x => !string.IsNullOrEmpty(x)))
                    });

                if (addressResponse != null && addressResponse.Count > 0)
                {
                    address = addressResponse[0];

                    if (address != null)
                    {
                        addressValidationCode = address.ValidationCode;
                        validAddress = address != null &&
                                       (address.ValidationCode == EnValidationCode.ConfirmedManually ||
                                        address.ValidationCode == EnValidationCode.Validated ||
                                        address.ValidationCode == EnValidationCode.Overridden);
                    }
                }
            }

            // для всех нужен нормализованный адрес, кроме международки и доставки в определенную точку
            if ((address is null || !validAddress)
                && calculateOption.MailType != EnMailType.ECOM
                && !IsPochtamatsTariff(calculateOption.MailType, calculateOption.MailCategory)
                && !isInternational)
            {
                if (address is null)
                    return UnloadOrderResult.CreateFailedResult("Неудалось нормализовать адрес");
                if (!validAddress)
                    return UnloadOrderResult.CreateFailedResult($"Неудалось нормализовать адрес ({addressValidationCode})");
                
                return UnloadOrderResult.CreateFailedResult("Что-то пошло не так"); // это сообщение выводить не должно
            }

            var accountSettings = RussianPostApiService.GetAccountSettings();

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var mustPay = !order.Payed && order.PaymentMethod != null
                                       && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            // наложенный платеж
            if (mustPay)
            {
                // Категория РПО не подходит под наложенный платеж
                if (!IsCodCategory(calculateOption.MailCategory))
                {
                    if (calculateOption.MailType == EnMailType.Letter
                        || calculateOption.MailType == EnMailType.LetterClass1)
                    {
                    }
                    else if (calculateOption.MailType == EnMailType.ECOM)
                    {
                        if (accountSettings.ShippingPoints != null)
                        {
                            var shippingPoint =
                                accountSettings.ShippingPoints.First(x =>
                                    x.OperatorIndex == calculateOption.IndexFrom);

                            var cashProduct = shippingPoint.AvailableProducts.First(x =>
                                x.MailType == calculateOption.MailType &&
                                (x.MailCategory == EnMailCategory.WithCompulsoryPayment ||
                                 x.MailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment));

                            calculateOption.MailCategory = cashProduct.MailCategory;
                        }
                    }
                    else
                    {
                        if (accountSettings.ShippingPoints != null)
                        {
                            var shippingPoint =
                                accountSettings.ShippingPoints.First(x =>
                                    x.OperatorIndex == calculateOption.IndexFrom);

                            var cashProduct =
                                shippingPoint.AvailableProducts
                                             .OrderByDescending(x =>
                                                  (DeliveryWithCod
                                                   && (x.MailCategory
                                                       == EnMailCategory.WithCompulsoryPayment || x.MailCategory
                                                       == EnMailCategory.WithDeclaredValueAndCompulsoryPayment))
                                                  || (!DeliveryWithCod
                                                      && x.MailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery)
                                                      ? 1
                                                      : 0)
                                             .First(x =>
                                                  x.MailType == calculateOption.MailType &&
                                                  (x.MailCategory == EnMailCategory.WithCompulsoryPayment
                                                   || x.MailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment
                                                   || x.MailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery));

                            calculateOption.MailCategory =
                                calculateOption.MailCategory.Value.StartsWith("COMBINED_")
                                    ? EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery
                                    : cashProduct.MailCategory;
                        }
                    }
                }
            }

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

            foreach (var item in fiscalItems)
            {
                item.Height = item.Height == 0 ? DefaultHeight : item.Height;
                item.Length = item.Length == 0 ? DefaultLength : item.Length;
                item.Width = item.Width == 0 ? DefaultWidth : item.Width;
                item.Weight = item.Weight == 0 ? DefaultWeight : item.Weight;
            }

            var dimensionsHelp = GetDimensions(rate: 10);
            var dimensions = new Dimension()
            {
                Length = (int) Math.Ceiling(dimensionsHelp[0]),
                Width = (int) Math.Ceiling(dimensionsHelp[1]),
                Height = (int) Math.Ceiling(dimensionsHelp[2]),
            };

            var orderRussionPostAdd = new OrderRussianPostAdd()
            {
                IndexFrom = calculateOption.IndexFrom,
                // часто валится ошибка, что передавать габариты для данного РПО не надо
                Dimension = RussianPostAvailableOption.DimensionsSendWithOrder.Contains(calculateOption.MailType)
                    ? dimensions
                    : null,
                Courier =
                    Courier
                    && RussianPostAvailableOption.CourierOptionAvailable.Contains(calculateOption.MailType)
                        ? true
                        : (bool?) null,
                Fragile =
                    Fragile
                    && RussianPostAvailableOption.FragileOptionAvailable.Contains(calculateOption.MailType)
                        ? true
                        : (bool?) null,
                RecipientName = string.Join(" ",
                    (new[]
                    {
                        order.OrderRecipient.LastName, order.OrderRecipient.FirstName,
                        order.OrderRecipient.Patronymic
                    })
                   .Where(x => !string.IsNullOrEmpty(x))),
                FirstName = order.OrderRecipient.FirstName,
                LastName = order.OrderRecipient.LastName,
                Patronymic = order.OrderRecipient.Patronymic,
                Phone = order.OrderRecipient.StandardPhone,
                DeclaredValue = IsDeclareCategory(calculateOption.MailCategory)
                    ? (long) (orderSum * 100)
                    : (long?) null,
                CashOnDelivery =
                    IsCodCategory(calculateOption.MailCategory)
                        ? (long) (orderSum * 100)
                        : (long?) null,
                DeliveryWithCod =
                    IsCodCategory(calculateOption.MailCategory) 
                    && DeliveryWithCod
                    && RussianPostAvailableOption.DeliveryWithCodAvailable.Contains(calculateOption.MailType)
                        ? true
                        : (bool?) null,
                MailType = calculateOption.MailType,
                MailCategory = calculateOption.MailCategory,
                CountryCode = countryCode,
                TransportType = calculateOption.TransportType,
                //ManualAddressInput = false,
                Weight = (int) GetTotalWeight(1000),
                OrderNum = order.Number,
                SmsNoticeRecipient =
                    SmsNotification &&
                    RussianPostAvailableOption.SmsNoticeOptionAvailable.ContainsKey(calculateOption.MailType) &&
                    RussianPostAvailableOption.SmsNoticeOptionAvailable[calculateOption.MailType]
                                              .Contains(calculateOption.MailCategory)
                        ? 1
                        : (int?) null,
                WithOrderOfNotice =
                    TypeNotification == EnTypeNotification.WithOrderOfNotice &&
                    RussianPostAvailableOption.OrderOfNoticeOptionAvailable.ContainsKey(calculateOption.MailType) &&
                    RussianPostAvailableOption.OrderOfNoticeOptionAvailable[calculateOption.MailType]
                                              .Contains(calculateOption.MailCategory)
                        ? true
                        : (bool?) null,
                WithSimpleNotice =
                    TypeNotification == EnTypeNotification.WithSimpleNotice &&
                    RussianPostAvailableOption.SimpleNoticeOptionAvailable.ContainsKey(calculateOption.MailType) &&
                    RussianPostAvailableOption.SimpleNoticeOptionAvailable[calculateOption.MailType]
                                              .Contains(calculateOption.MailCategory)
                        ? true
                        : (bool?) null,
            };

            if (address != null)
            {
                orderRussionPostAdd.AddressTypeTo = address.AddressType;
                orderRussionPostAdd.NumAddressTypeTo = address.NumAddressType;
                orderRussionPostAdd.RegionTo = address.Region;
                orderRussionPostAdd.AreaTo = address.Area;
                orderRussionPostAdd.PlaceTo = address.Place;
                orderRussionPostAdd.LocationTo = address.Location;
                orderRussionPostAdd.HotelTo = address.Hotel;
                orderRussionPostAdd.StreetTo = address.Street;
                orderRussionPostAdd.HouseTo = address.House;
                orderRussionPostAdd.BuildingTo = address.Building;
                orderRussionPostAdd.CorpusTo = address.Corpus;
                orderRussionPostAdd.SlashTo = address.Slash;
                orderRussionPostAdd.LetterTo = address.Letter;
                orderRussionPostAdd.RoomTo = address.Room;
                orderRussionPostAdd.StrIndexTo = address.Index;
                orderRussionPostAdd.IndexTo = address.Index.TryParseInt(true);
            }

            // для COD и ECOM надо передавать список товаров
            if (orderRussionPostAdd.DeliveryWithCod == true
                || orderRussionPostAdd.MailType == EnMailType.ECOM)
            {
                orderRussionPostAdd.Goods = new Goods
                {
                    Items = fiscalItems.Select(item =>
                        new GoodsItem
                        {
                            ArtNo = item.ArtNo.Reduce(135),
                            Description = item.Name,
                            GoodsType = EnGoodsType.GOODS,
                            Weight = (int) (item.Weight * 1000 * item.Amount),
                            Quantity = (int) item.Amount,
                            Value = (long) (item.Price * 100),
                            VatRate = GetVatRate(item.TaxType, item.PaymentMethodType),
                            DeclareValue =
                                Shipping.RussianPost.RussianPost.IsDeclareCategory(orderRussionPostAdd.MailCategory)
                                    ? (long) (item.Price * 100)
                                    : (long?) null,
                            PaymentSubjectType = (int) item.PaymentSubjectType,
                            PaymentMethodType = (int) item.PaymentMethodType
                        }).ToList()
                };

                if (shippingCost > 0)
                {
                    orderRussionPostAdd.Goods.Items.Add(new GoodsItem
                    {
                        ArtNo = "Доставка",
                        Description = "Доставка",
                        GoodsType = EnGoodsType.GOODS, //передача услуги пока не поддерживается
                        Weight = 1, //передаем грамм, т.к. вес обязателен
                        Quantity = 1,
                        Value = (long) (shippingCost * 100),
                        VatRate = GetVatRate(order.ShippingTaxType, order.ShippingPaymentMethodType),
                        DeclareValue =
                            Shipping.RussianPost.RussianPost.IsDeclareCategory(orderRussionPostAdd.MailCategory)
                                ? (long) (shippingCost * 100)
                                : (long?) null,
                        PaymentSubjectType = (int) order.ShippingPaymentSubjectType,
                        PaymentMethodType = (int) order.ShippingPaymentMethodType
                    });
                }
            }

            // ECOM
            if (orderRussionPostAdd.MailType == EnMailType.ECOM)
            {
                orderRussionPostAdd.DimensionType = EnDimensionType.GetDimensionType(dimensions);

                orderRussionPostAdd.FiscalData = new FiscalData
                {
                    CustomerEmail = order.OrderCustomer.Email,
                    //CustomerName = order.OrderCustomer.Organization // если передавать CustomerName, то надо передать и CustomerInn
                    CustomerPhone = order.OrderCustomer.StandardPhone
                };

                var deliveryPointId = order.OrderPickPoint != null
                    ? order.OrderPickPoint.PickPointId
                    : calculateOption.IndexTo;

                List<DeliveryPoint> points = GetAllPointsAsync(CancellationToken.None)
                                            .GetAwaiter().GetResult();
                DeliveryPoint deliveryPoint = null;

                if (points != null
                    && points.Count > 0)
                {
                    deliveryPoint = deliveryPointId != null
                        ? points.FirstOrDefault(x => x.DeliveryPointIndex == deliveryPointId)
                        : null;
                }

                orderRussionPostAdd.EcomData = new EcomData
                {
                    DeliveryPointIndex = deliveryPoint != null ? deliveryPoint.DeliveryPointIndex : deliveryPointId,
                    Services = new List<EnEcomService> {EnEcomService.WITHOUT_SERVICE}
                };

                // оплата при получении
                if ((orderRussionPostAdd.MailCategory == EnMailCategory.WithCompulsoryPayment
                     || orderRussionPostAdd.MailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment))
                {
                    orderRussionPostAdd.CompulsoryPayment =
                        orderRussionPostAdd.Goods.Items.Sum(x => x.Value * x.Quantity);

                    // Если выбран способ оплаты compulsory-payment, то payattr = только 4
                    orderRussionPostAdd.Goods.Items.ForEach(x =>
                        x.PaymentMethodType = (int) ePaymentMethodType.full_payment);
                    // т.к. оплата при получении то корректируем налоги, если там было передано 10/110 (n/100+n) 
                    orderRussionPostAdd.Goods.Items.Where(x => x.VatRate > 100)
                                       .ForEach(x => x.VatRate -= 100);

                    if (deliveryPoint != null)
                    {
                        if (deliveryPoint.TypePostOffice == EnTypePostOffice.PVZ)
                            orderRussionPostAdd.EcomData.IdentityMethods = new List<EnIdentityMethod>
                                {EnIdentityMethod.PIN};
                        else if (deliveryPoint.TypePostOffice == EnTypePostOffice.GOPS)
                            orderRussionPostAdd.EcomData.IdentityMethods = new List<EnIdentityMethod>
                                {EnIdentityMethod.WITHOUT_IDENTIFICATION};
                        else if (deliveryPoint.TypePostOffice == EnTypePostOffice.POST_OFFICE)
                            orderRussionPostAdd.EcomData.IdentityMethods = new List<EnIdentityMethod>
                                {EnIdentityMethod.PIN};
                    }
                }
                // получение без оплаты
                else if (orderRussionPostAdd.MailCategory == EnMailCategory.Ordinary)
                {
                    orderRussionPostAdd.FiscalData.PaymentAmount =
                        orderRussionPostAdd.Goods.Items.Sum(x => x.Value * x.Quantity);

                    // Если выбран способ оплаты payment-amount, то payattr = 1 или 6
                    orderRussionPostAdd.Goods.Items.ForEach(x =>
                        x.PaymentMethodType = (int) ePaymentMethodType.full_prepayment);

                    if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType)
                    {
                        // т.к. поменяли PaymentMethodType, то нужно скорректировать налоги
                        orderRussionPostAdd.Goods.Items
                                           .Where(x => x.VatRate > 0 && x.VatRate < 100)
                                           .ForEach(x => x.VatRate += 100);
                    }

                    if (deliveryPoint != null)
                    {
                        if (deliveryPoint.TypePostOffice == EnTypePostOffice.PVZ)
                            orderRussionPostAdd.EcomData.IdentityMethods = new List<EnIdentityMethod>
                                {EnIdentityMethod.IDENTITY_DOCUMENT};
                        else if (deliveryPoint.TypePostOffice == EnTypePostOffice.GOPS)
                            orderRussionPostAdd.EcomData.IdentityMethods = new List<EnIdentityMethod>
                                {EnIdentityMethod.ORDER_NUM_AND_FIO};
                        else if (deliveryPoint.TypePostOffice == EnTypePostOffice.POST_OFFICE)
                            orderRussionPostAdd.EcomData.IdentityMethods = new List<EnIdentityMethod>
                                {EnIdentityMethod.PIN};
                    }
                }
            }

            // до почтамата
            if (IsPochtamatsTariff(calculateOption.MailType,
                    calculateOption.MailCategory))
            {
                orderRussionPostAdd.DimensionType = EnDimensionType.GetDimensionType(dimensions);

                var deliveryPointId = order.OrderPickPoint != null
                    ? order.OrderPickPoint.PickPointId
                    : calculateOption.IndexTo;

                orderRussionPostAdd.EcomData = new EcomData
                {
                    DeliveryPointIndex = deliveryPointId,
                    IdentityMethods = new List<EnIdentityMethod> {EnIdentityMethod.PIN}
                };
            }

            // международное отправление
            if (orderRussionPostAdd.CountryCode != 643)
            {
                orderRussionPostAdd.AddressTypeTo = EnAddressType.Default;
                orderRussionPostAdd.RegionTo = order.OrderCustomer.Region.IsNotEmpty()
                    ? StringHelper.Translit(order.OrderCustomer.Region)
                    : null;
                orderRussionPostAdd.PlaceTo = order.OrderCustomer.City.IsNotEmpty()
                    ? StringHelper.Translit(order.OrderCustomer.City)
                    : null;
                if (!string.IsNullOrEmpty(order.OrderCustomer.Street))
                {
                    orderRussionPostAdd.StreetTo =
                        LocalizationService.GetResource("Core.Orders.OrderContact.Street")
                        + " "
                        + order.OrderCustomer.Street
                        + " "
                        + (!string.IsNullOrEmpty(order.OrderCustomer.House)
                            ? LocalizationService.GetResource("Admin.Js.CustomerView.House") + " "
                            + order.OrderCustomer.House + ", "
                            : "")
                        + (!string.IsNullOrEmpty(order.OrderCustomer.Structure)
                            ? LocalizationService.GetResource("Admin.Js.CustomerView.Struct") + " "
                            + order.OrderCustomer.Structure + ", "
                            : "")
                        + (!string.IsNullOrEmpty(order.OrderCustomer.Apartment)
                            ? LocalizationService.GetResource("Admin.Js.CustomerView.Ap") + " "
                            + order.OrderCustomer.Apartment + " "
                            : "");

                    orderRussionPostAdd.StreetTo = StringHelper.Translit(orderRussionPostAdd.StreetTo);
                }

                orderRussionPostAdd.StrIndexTo = order.OrderCustomer.Zip;
                orderRussionPostAdd.IndexTo = order.OrderCustomer.Zip.TryParseInt(true);

                orderRussionPostAdd.RecipientName = StringHelper.Translit(orderRussionPostAdd.RecipientName);
                orderRussionPostAdd.FirstName = StringHelper.Translit(orderRussionPostAdd.FirstName);
                orderRussionPostAdd.LastName = StringHelper.Translit(orderRussionPostAdd.LastName);
                orderRussionPostAdd.Patronymic = StringHelper.Translit(orderRussionPostAdd.Patronymic);

                IList<CustomsDeclarationDto> declarationDtos = null;
                if (additionalAction == CustomsDeclarationProductsDataActionName)
                {
                    declarationDtos = additionalActionData as IList<CustomsDeclarationDto>;

                    // сохраняем значения
                    if (declarationDtos != null)
                        foreach (var item in declarationDtos.Where(x => x.ProductId.HasValue))
                            CustomsDeclarationProductDataService.AddOrUpdate(
                                new CustomsDeclarationProductData
                                    {
                                        ProductId = item.ProductId.Value,
                                        CountryCode = item.CountryCode.TryParseInt(true),
                                        TnvedCode = item.TnvedCode,
                                    });
                }

                // Необходима таможенная декларация
                if (!RussianPostAvailableOption.CountryIso3NumberEurasianEconomicUnion.Contains(orderRussionPostAdd
                       .CountryCode))
                {
                    orderRussionPostAdd.CustomsDeclaration = new CustomsDeclaration
                    {
                        Currency = order.OrderCurrency.CurrencyCode,
                        EntriesType = EnEntriesType.SaleOfGoods,
                        CustomsEntries = new List<CustomsEntry>()
                    };

                    foreach (var orderItem in fiscalItems)
                    {
                        var customsDeclarationItem = declarationDtos?.FirstOrDefault(x => x.OrderItemId == orderItem.OrderItemID);

                        orderRussionPostAdd.CustomsDeclaration.CustomsEntries.Add(new CustomsEntry
                        {
                            Amount = (int) orderItem.Amount,
                            Description = (customsDeclarationItem != null
                                ? StringHelper.Translit(customsDeclarationItem.Name)
                                : StringHelper.Translit(orderItem.Name)).Replace("«", "\"").Replace("»", "\""),
                            Value = (long) (orderItem.Price * 100),
                            Weight = (int) (orderItem.Weight * 1000 * orderItem.Amount),
                            CountryCode = customsDeclarationItem?.CountryCode.TryParseInt(true),
                            TnvedCode = customsDeclarationItem?.TnvedCode
                        });
                    }
                }
            }

            // правим данные, чтобы не словить ошибки по небольшому расхождению
            // после пересчетов
            if (orderRussionPostAdd.Goods != null)
            {
                var goodsWeight = orderRussionPostAdd.Goods.Items.Sum(x => x.Weight ?? 0);
                if (orderRussionPostAdd.Weight < goodsWeight)
                    orderRussionPostAdd.Weight = goodsWeight;

                if (orderRussionPostAdd.DeclaredValue.HasValue)
                    orderRussionPostAdd.DeclaredValue =
                        orderRussionPostAdd.Goods.Items.Sum(x => x.Value * x.Quantity);

                if (orderRussionPostAdd.CashOnDelivery.HasValue)
                    orderRussionPostAdd.CashOnDelivery =
                        orderRussionPostAdd.Goods.Items.Sum(x => x.Value * x.Quantity);
            }

            var result = RussianPostApiService.CreateOrder(orderRussionPostAdd);

            // реагирование на определенные ошибки
            if (result != null
                && result.Errors != null)
            {
                var reSend = false;

                var codes = result.Errors
                                  .Where(x => x.ErrorCodes != null)
                                  .SelectMany(e => e.ErrorCodes.Select(ec => ec.Code))
                                  .Where(code => code.IsNotEmpty()).ToList();

                // не для всех международных отправлений нужна таможенная декларация (например для ЕАЭС)
                // на случай когда RussianPostAvailableOption.CountryIso3NumberEurasianEconomicUnion не актуальный
                if (codes.Exists(err => string.Equals(err, "UNEXPECTED_CUSTOMS_ENTRIES", StringComparison.OrdinalIgnoreCase)))
                {
                    orderRussionPostAdd.CustomsDeclaration = null;
                    reSend = true;
                }
                // нужны дополнительные данные для таможенной декларации (код страны происхождения и ТНВЭД)
                else if (codes.Exists(err => string.Equals(err, "EMPTY_CUSTOMS_ENTRY_COUNTRY_CODE", StringComparison.OrdinalIgnoreCase))
                    || codes.Exists(err => string.Equals(err, "EMPTY_CUSTOMS_ENTRY_TNVED_CODE", StringComparison.OrdinalIgnoreCase)))
                {
                    var requestData = new List<CustomsDeclarationDto>();

                    foreach (var item in fiscalItems)
                    {
                        var customsDeclarationProductsData = item.ProductID.HasValue
                            ? CustomsDeclarationProductDataService.Get(item.ProductID.Value)
                            : null;

                        requestData.Add(
                            new CustomsDeclarationDto
                            {
                                OrderItemId = item.OrderItemID,
                                ProductId = item.ProductID,
                                ArtNo = item.ArtNo,
                                Name = StringHelper.Translit(item.Name),
                                CountryCode = customsDeclarationProductsData != null
                                    ? customsDeclarationProductsData.CountryCode.ToString()
                                    : "",
                                TnvedCode = customsDeclarationProductsData != null
                                    ? customsDeclarationProductsData.TnvedCode
                                    : ""
                            });
                    }

                    // если это автоматическая отправка и данных для отправки достаточно, то пробуем отправить
                    if (additionalAction != FromAdminFirstActionName
                        && requestData.All(x => x.CountryCode.IsNotEmpty() && x.TnvedCode.IsNotEmpty()))
                    {
                        orderRussionPostAdd.CustomsDeclaration.CustomsEntries.Clear();
                        
                        foreach (var orderItem in fiscalItems)
                        {
                            var customsDeclarationItem =
                                requestData.First(x =>
                                    x.OrderItemId == orderItem.OrderItemID);

                            orderRussionPostAdd.CustomsDeclaration.CustomsEntries.Add(new CustomsEntry
                            {
                                Amount = (int) orderItem.Amount,
                                Description = (customsDeclarationItem != null
                                    ? StringHelper.Translit(customsDeclarationItem.Name)
                                    : StringHelper.Translit(orderItem.Name)).Replace("«", "\"").Replace("»", "\""),
                                Value = (long) (orderItem.Price * 100),
                                Weight = (int) (orderItem.Weight * 1000 * orderItem.Amount),
                                CountryCode = customsDeclarationItem.CountryCode.TryParseInt(true),
                                TnvedCode = customsDeclarationItem.TnvedCode
                            });
                        }

                        reSend = true;
                        //return UnloadOrder(order, CustomsDeclarationProductsDataActionName, requestData);
                    }
                    else
                        return UnloadOrderResult.CreateFailedResult(
                            "Нужны дополнительные данные для таможенной декларации (код страны происхождения и ТНВЭД)",
                            CustomsDeclarationProductsDataActionName,
                            requestData);
                }

                if (reSend)
                    result = RussianPostApiService.CreateOrder(orderRussionPostAdd);
            }

            if (result != null
                && result.OrderIds != null
                && result.OrderIds.Count > 0)
            {
                var russianPostOrderId = result.OrderIds[0];
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    KeyNameOrderRussianPostIdInOrderAdditionalData,
                    russianPostOrderId.ToString());

                var orderRussionPost = RussianPostApiService.GetOrder(russianPostOrderId);

                order.TrackNumber = orderRussionPost.Barcode;
                OrderService.UpdateOrderMain(order);

                return UnloadOrderResult.CreateSuccessResult();
            }
            else if (result != null
                     && result.Errors != null)
                return UnloadOrderResult.CreateFailedResult(string.Join("\n", result.Errors.Where(x => x.ErrorCodes != null)
                                        .SelectMany(e => e.ErrorCodes.Select(ec => ec.Description ?? ec.Code))));
            else if (RussianPostApiService.LastActionErrors != null)
                UnloadOrderResult.CreateFailedResult(string.Join("\n", RussianPostApiService.LastActionErrors));

            return UnloadOrderResult.CreateFailedResult("Что-то пошло не так");
        }
        
                
        private int GetVatRate(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return -1;

            if (taxType.Value == TaxType.Vat0)
                return 0;

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 110;
                else
                    return 10;
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 120;
                else
                    return 20;
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 105;
                else
                    return 5;
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 107;
                else
                    return 7;
            }

            return -1;
        }

    }
}