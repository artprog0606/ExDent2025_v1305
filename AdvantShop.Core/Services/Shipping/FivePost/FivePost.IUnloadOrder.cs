using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping.FivePost.Api;
using AdvantShop.Shipping.FivePost.CalculateCost;
using AdvantShop.Shipping.FivePost.PickPoints;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.FivePost
{
    public partial class FivePost : IUnloadOrder
    {
        public UnloadOrderResult UnloadOrder(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            if (order.OrderCustomer == null)
                return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");
            
            if (OrderService.GetOrderAdditionalData(order.OrderID, TrackNumberOrderAdditionalDataName).IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            if (order.OrderPickPoint == null || order.OrderPickPoint.AdditionalData.IsNullOrEmpty())
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах расчета доставки.");

            var fivePostCalcParams = JsonConvert.DeserializeObject<FivePostCalculationParams>(order.OrderPickPoint.AdditionalData);
            var fivePostRate = fivePostCalcParams?.Rate;

            if (fivePostRate == null)
                return UnloadOrderResult.CreateFailedResult("Не удалось определить тариф.");

            var pickPoint = FivePostPickPointService.Get(order.OrderPickPoint.PickPointId);
            var dimensionsInMillimeters = GetDimensions();

            var orderSumWithDelivery = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingTaxType = _method.TaxType;
            var vatRate = GetVatRate(_method.TaxType);

            var shippingCurrency = _method.ShippingCurrency;
            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSumWithDelivery = orderSumWithDelivery.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.CashOnDelivery)),
            };

            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            var createOrderParams = new FivePostCreateOrderParams
            {
                OrderDetails = new FivePostOrderDetails
                {
                    OrderId = order.OrderID.ToString(),
                    TrackNumber = order.Number,
                    CustomerFIO = $"{order.OrderRecipient.LastName} {order.OrderRecipient.FirstName} {order.OrderRecipient.Patronymic}",
                    CustomerStandartPhone = order.OrderRecipient.StandardPhone,
                    CustomerEmail = order.OrderCustomer.Email,
                    WarehouseId = fivePostCalcParams.WarehouseId,
                    PickPointId = order.OrderPickPoint.PickPointId,
                    UndeliverableOption = UndeliverableOption,
                    RateType = fivePostRate.Type,
                    CostDetails = new FivePostOrderDetailsCost
                    {
                        DeliveryCost = mustPay ? shippingCost : 0,
                        SumWithDelivery = mustPay ? orderSumWithDelivery : 0,
                        PaymentCurrencyIso3 = shippingCurrency?.Iso3,
                        InsurePrice = fivePostCalcParams.WithInsure ? orderSumWithDelivery - shippingCost : 0,
                        PaymentType = mustPay
                            ? order.PaymentMethodId == PaymentCodCardId
                            ? EFivePostPaymentType.Cashless.StrName() 
                            : EFivePostPaymentType.Cash.StrName()
                            : EFivePostPaymentType.Prepayment.StrName()
                    },
                    Cargo = new FivePostCargoDetails
                    {
                        CargoId = order.Number,
                        WidthInMillimeters = (int)Math.Ceiling(dimensionsInMillimeters[0]),
                        HeightInMillimeters = (int)Math.Ceiling(dimensionsInMillimeters[1]),
                        LengthInMillimeters = (int)Math.Ceiling(dimensionsInMillimeters[2]),
                        WeightInMilligrams = 
                            (long)MeasureUnits.ConvertWeight(
                                GetTotalWeight(), 
                                MeasureUnits.WeightUnit.Kilogramm, 
                                MeasureUnits.WeightUnit.Milligram),
                        Sum = fivePostCalcParams.WithInsure ? orderSumWithDelivery - shippingCost : 0f,
                        CurrencyIso3 = shippingCurrency?.Iso3,
                        Products = order.OrderItems.Select(x => new FivePostProduct
                        {
                            Name = x.Name,
                            Amount = x.Amount,
                            PriceWithVat = mustPay ? (float)Math.Round(x.Price, 2) : 0f,
                            Vat = x.TaxRate.HasValue ? x.TaxRate.Value : -1f,
                            ArtNo = x.ArtNo
                        }).ToList(),
                        BarcodeDetails = GetBarcodeDetails(BarCodeEnrichment, order.OrderItems)
                    },
                }
            };

            var result = FivePostApiService.CreateOrder(createOrderParams);

            if (result is null)
                return UnloadOrderResult.CreateFailedResult("Не удалось передать заказ в 5Пост.(1)");

            // заказ с таким номером уже был передан, передаем заказ с постфиксом
            if (result.Errors != null && result.Errors.FirstOrDefault(x => x.Code == "20") != null)
            {
                var newOrderNumber = $"{order.Number}_{DateTime.Now}";
                createOrderParams.OrderDetails.OrderId = newOrderNumber;
                createOrderParams.OrderDetails.Cargo.CargoId = newOrderNumber;
                result = FivePostApiService.CreateOrder(createOrderParams);
            }
            if (result == null)
                return UnloadOrderResult.CreateFailedResult("Не удалось передать заказ в 5Пост.(2)");
            if (result.Errors != null)
                return UnloadOrderResult.CreateFailedResult(string.Join(", ", result.Errors.Select(x => x.FullError)));
            if (result.Created)
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    TrackNumberOrderAdditionalDataName,
                    result.OrderId);
                order.TrackNumber = result.OrderId;
                OrderService.UpdateOrderMain(order);

                return UnloadOrderResult.CreateSuccessResult("Заказ успешно создан.", result.FivePostOrderId);
            }

            return UnloadOrderResult.CreateFailedResult("Не удалось передать заказ в 5Пост.(3)");

        }

        private List<FivePostBarcodeDetails> GetBarcodeDetails(EFivePostBarcodeEnrichment barcodeEnrichment, List<OrderItem> orderItems)
        {
            if (barcodeEnrichment == EFivePostBarcodeEnrichment.None)
                return null;

            if (barcodeEnrichment == EFivePostBarcodeEnrichment.Partial &&
                orderItems.Any(x => x.BarCode.IsNullOrEmpty()))
                return null;

            return orderItems.Select(x => new FivePostBarcodeDetails
            {
                Barcode = x.BarCode
            }).ToList();
        }

        private float GetVatRate(TaxType taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat0:
                    return 0f;
                case TaxType.Vat10:
                    return 10f;
                case TaxType.Vat18:
                    return 18f;
                case TaxType.Vat20:
                    return 20f;
                case TaxType.Vat5:
                    return 5f;
                case TaxType.Vat7:
                    return 7f;
                default:
                    return -1f;
            }
        }
    }
}