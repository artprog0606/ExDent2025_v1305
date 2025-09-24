using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.PickPoint.Api;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PickPoint
{
    public class PickPointCreateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PickPointCreateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId, Shipping.PickPoint.PickPoint.KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData);
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
                typeof(Shipping.PickPoint.PickPoint).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
            if (orderPickPoint == null)
            {
                Errors.Add("Нет данных о выбранном ПВЗ доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var pickPointMethod = new Shipping.PickPoint.PickPoint(order.ShippingMethod, calculationParameters);

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var dimensions = pickPointMethod.GetDimensions(rate: 10);
            var weight = pickPointMethod.GetTotalWeight();
            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            var phone = order.OrderRecipient != null && order.OrderRecipient.StandardPhone.HasValue
                ? order.OrderRecipient.StandardPhone.Value.ToString()
                : string.Empty;

            if (phone.StartsWith("8"))
                phone = "7" + phone.Substring(1);

            if (phone.Length == 10)
                phone = "7" + phone;

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
            var subEncloses = fiscalItems.Select(item =>
                new SubEnclose
                {
                    ProductCode = item.ArtNo.Reduce(50),
                    GoodsCode = item.BarCode.Reduce(50),
                    Name = item.Name.Reduce(200),
                    Price = item.Price,
                    Quantity = (int)item.Amount,
                    Vat = GetVat(item.TaxType),

                }
            ).ToList();

            var sumSubEncloses = subEncloses.Sum(item => item.Price * item.Quantity);

            var shipment = new Shipment
            {
                IdOfRequest = Guid.NewGuid().ToString(),
                Ikn = pickPointMethod.Ikn,
                Invoice = new InvoiceShipment
                {
                    OrderNumber = order.Number.Length <= 50 ? order.Number : order.OrderID.ToString(),
                    Description = string.Format("Заказ {0}", order.Number),
                    RecipientName = order.OrderRecipient != null
                        ? string.Join(" ", (new[] { order.OrderRecipient.LastName, order.OrderRecipient.FirstName, order.OrderRecipient.Patronymic })
                                        .Where(x => !string.IsNullOrEmpty(x)))
                        : null,
                    PostamatNumber = orderPickPoint.PickPointId,
                    MobilePhone = phone.IsNotEmpty()
                        ? "+" + phone
                        : null,
                    Email = order.OrderCustomer != null && order.OrderCustomer.Email.IsNotEmpty()
                        ? order.OrderCustomer.Email.Reduce(256)
                        : null,
                    PostageType = mustPay ? PostageType.Cod : PostageType.Paid,
                    Sum = mustPay ? sumSubEncloses + shippingCost : 0f,
                    GettingType = pickPointMethod.GettingType,
                    DeliveryMode = pickPointMethod.DeliveryMode,
                    DeliveryFee = mustPay ? shippingCost : 0f,
                    DeliveryVat = GetVat(order.ShippingTaxType),
                    SenderCity = new SenderCity
                    {
                        RegionName = pickPointMethod.FromRegion,
                        CityName = pickPointMethod.FromCity,
                    },
                    Places = new List<InvoicePlace>
                    {
                        new InvoicePlace
                        {
                            Depth = dimensions[0],
                            Width = dimensions[1],
                            Height = dimensions[2],
                            Weight = weight,
                            SubEncloses = subEncloses
                        }
                    }
                }
            };

            var result = pickPointMethod.PickPointApiService.CreateShipment(new CreateShipmentParams { Sendings = new List<Shipment> { shipment } });

            if (result != null && result.CreatedSendings != null && result.CreatedSendings.Count > 0)
            {
                var invoiceNumber = result.CreatedSendings[0].InvoiceNumber;
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.PickPoint.PickPoint.KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData,
                    invoiceNumber);

                order.TrackNumber = invoiceNumber;
                OrderService.UpdateOrderMain(order);

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);

                return true;
            }
            else if (pickPointMethod.PickPointApiService.LastActionErrors != null)
                Errors.AddRange(pickPointMethod.PickPointApiService.LastActionErrors);

            return false;
        }

        private Vat GetVat(TaxType? taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat18:
                case TaxType.Vat20:
                    return Vat.Vat20;
                case TaxType.Vat10:
                    return Vat.Vat10;
                case TaxType.Vat0:
                    return Vat.Vat0;
            }
            return Vat.WithoutVat;
        }
    }
}
