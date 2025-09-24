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
using AdvantShop.Shipping.Hermes;
using AdvantShop.Shipping.Hermes.Api;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Hermes
{
    public abstract class HermesCreateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public HermesCreateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId, Shipping.Hermes.Hermes.KeyNameBarcodeInOrderAdditionalData);
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
                typeof(Shipping.Hermes.Hermes).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
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

            HermesCalculateOption hermesCalculateOption = null;

            try
            {
                hermesCalculateOption =
                    JsonConvert.DeserializeObject<HermesCalculateOption>(orderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (hermesCalculateOption == null)
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var hermesMethod = new Shipping.Hermes.Hermes(order.ShippingMethod, calculationParameters);

            var barcode = CreateOrder(order, hermesMethod, hermesCalculateOption, orderPickPoint);
            if (string.IsNullOrEmpty(barcode))
                return false;

            OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                Shipping.Hermes.Hermes.KeyNameBarcodeInOrderAdditionalData,
                barcode);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);

            return CreateOrderContent(order, hermesMethod, barcode);
        }

        public virtual string CreateOrder(Order order, Shipping.Hermes.Hermes hermesMethod, HermesCalculateOption hermesCalculateOption, OrderPickPoint orderPickPoint)
        {
            throw new NotImplementedException();
        }

        private bool CreateOrderContent(Order order, Shipping.Hermes.Hermes hermesMethod, string parcelBarcode)
        {
            var client = new RestApiClient(hermesMethod.SecuredToken, hermesMethod.PublicToken);

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

            var parcelItems = orderItems
                .Select(x => new ParcelContentModel {
                    ClientItemId = x.ArtNo.Reduce(50),
                    Type = "ITEM",
                    Article = x.ArtNo.Reduce(250),
                    Name = x.Name.Reduce(250),
                    Quantity = (int)x.Amount,
                    Price = x.Price,
                    Vat = x.TaxRate
                })
                .ToList();

            if (shippingCost > 0)
                parcelItems.Add(new ParcelContentModel
                {
                    Type = "DELIVERY",
                    Name = "Доставка".Reduce(250),
                    Quantity = 1,
                    Price = shippingCost,
                    Vat = order.ShippingTaxType == Taxes.TaxType.VatWithout
                        ? null
                        : order.ShippingTaxType == Taxes.TaxType.Vat0
                            ? 0f
                            : order.ShippingTaxType == Taxes.TaxType.Vat10
                                ? 10f
                                : order.ShippingTaxType == Taxes.TaxType.Vat18
                                    ? 18f
                                    : order.ShippingTaxType == Taxes.TaxType.Vat20
                                        ? 20f
                                        : (float?)null
                });

            var result = client.CreateOrUpdateParcelContent(new CreateOrUpdateParcelContentParams
            {
                Barcode = parcelBarcode,
                ParcelItems = parcelItems
            });

            if (result != null && result.IsSuccess != true && result.ErrorMessage.IsNotEmpty())
                Errors.Add(result.ErrorMessage);

            return result != null && result.IsSuccess == true;
        }
    }
}
