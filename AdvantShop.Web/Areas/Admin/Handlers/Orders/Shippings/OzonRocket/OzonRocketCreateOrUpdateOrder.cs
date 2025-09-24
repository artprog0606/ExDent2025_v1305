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
using AdvantShop.Shipping.OzonRocket;
using AdvantShop.Shipping.OzonRocket.Api;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.OzonRocket
{
    public class OzonRocketCreateOrUpdateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public OzonRocketCreateOrUpdateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var ozonRocketOrderId = OrderService.GetOrderAdditionalData(_orderId, Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIdInOrderAdditionalData);
            // if (!string.IsNullOrEmpty(ozonRocketOrderId))
            // {
            //     Errors.Add("Заказ уже передан");
            //     return false;
            // }
       
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.OzonRocket.OzonRocket).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }
        
            var orderPickPoint = order.OrderPickPoint;
            if (orderPickPoint == null || orderPickPoint.AdditionalData.IsNullOrEmpty())
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }
      
            OzonRocketCalculateOption ozonRocketCalculateOption = null;

            try
            {
                ozonRocketCalculateOption =
                    JsonConvert.DeserializeObject<OzonRocketCalculateOption>(orderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (ozonRocketCalculateOption == null)
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var ozonRocketMethod = new Shipping.OzonRocket.OzonRocket(order.ShippingMethod, calculationParameters);

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.Cash)),
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var dimensions = ozonRocketMethod.GetDimensions();
            var weightInGrams = ozonRocketMethod.GetTotalWeight(1000);
            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            var customerPhone = order.OrderRecipient?.StandardPhone != null
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
            var orderLines = fiscalItems.Select(item =>
                new OrderLine()
                {
                    ArticleNumber = item.ArtNo,
                    Name = item.Name,
                    SellingPrice = item.Price,
                    EstimatedPrice = item.Price,
                    Quantity = (int)item.Amount,
                    Vat = GetVat(item.TaxType, item.Price),
                    ResideInPackages = new List<string>(){"1"}
                }
            ).ToList();
            var sumOrderLines = orderLines.Sum(item => item.SellingPrice * item.Quantity);

            var ozonOrder = ozonRocketOrderId.IsNullOrEmpty()
                ? new Shipping.OzonRocket.Api.NewOrder() 
                : (NewOrderBase)new Shipping.OzonRocket.Api.UpdateOrder();
            
            if (ozonRocketOrderId.IsNullOrEmpty())
                ((NewOrder)ozonOrder).OrderNumber = order.Number;
            else
                ((Shipping.OzonRocket.Api.UpdateOrder)ozonOrder).OrderId = ozonRocketOrderId.TryParseLong();
            
            ozonOrder.Buyer = new Buyer()
            {
                Name = order.OrderRecipient != null
                    ? string.Join(" ", (new[]
                        {
                            order.OrderRecipient.LastName, order.OrderRecipient.FirstName,
                            order.OrderRecipient.Patronymic
                        })
                        .Where(x => !string.IsNullOrEmpty(x)))
                    : null,
                Type = TypeBuyer.NaturalPerson,
                Email = order.OrderCustomer != null
                    ? order.OrderCustomer.Email
                    : null,
                Phone = customerPhone
            };
            ozonOrder.Recipient = null;
            ozonOrder.FirstMileTransfer = new FirstMileTransfer()
            {
                Type = EnFirstMileTransferType.DropOff,
                FromPlaceId = ozonRocketCalculateOption.FromPlaceId.HasValue
                    ? ozonRocketCalculateOption.FromPlaceId.Value.ToString()
                    : ozonRocketMethod.FromPlaceId.ToString(),// для поддержки старых заказов (с передачей по PickUp)
            };
            ozonOrder.Payment = new Shipping.OzonRocket.Api.Payment()
            {
                Type = mustPay 
                    ? EnPaymentType.Postpay 
                    : EnPaymentType.FullPrepayment,
                PrepaymentAmount = mustPay
                    ? 0f
                    : (float)Math.Round(sumOrderLines + shippingCost, 2),
                RecipientPaymentAmount = mustPay
                    ? (float)Math.Round(sumOrderLines + shippingCost, 2)
                    : 0f,
                DeliveryPrice = shippingCost,
                DeliveryVat = GetVat(order.ShippingTaxType, shippingCost)
            };
            ozonOrder.DeliveryInformation = new DeliveryInformation()
            {
                DeliveryVariantId = ozonRocketCalculateOption.DeliveryVariantId.ToString(), //orderPickPoint.PickPointId
                Address = ozonRocketCalculateOption.IsCourier
                    ? order.OrderCustomer != null
                        ? string.Join(
                            ", ",
                            new[] {
                                order.OrderCustomer.Region,
                                order.OrderCustomer.District,
                                order.OrderCustomer.City,
                                order.OrderCustomer.Street,
                                order.OrderCustomer.House, 
                                order.OrderCustomer.Structure,
                            }.Where(x => x.IsNotEmpty()))
                        : null
                    : orderPickPoint.PickPointAddress,
                AdditionalAddress = ozonRocketCalculateOption.IsCourier
                    ? order.OrderCustomer != null
                        ? string.Join(
                            ", ",
                            new[] {
                                order.OrderCustomer.Apartment
                            }.Where(x => x.IsNotEmpty()))
                        : null
                    : null
            };
            ozonOrder.Packages = new List<Package>()
            {
                new Package()
                {
                    PackageNumber = "1",
                    Dimensions = new DimensionsPackage()
                    {
                        Height = (int)dimensions[2],
                        Width = (int)dimensions[1],
                        Length = (int)dimensions[0],
                        Weight = (int)weightInGrams,
                    }
                }
            };
            ozonOrder.OrderLines = orderLines;
            ozonOrder.Comment = order.CustomerComment;
            ozonOrder.AllowPartialDelivery = ozonRocketMethod.AllowPartialDelivery;
            ozonOrder.AllowUncovering = ozonRocketMethod.AllowUncovering;

            var result = ozonRocketOrderId.IsNullOrEmpty() 
                ? ozonRocketMethod.OzonRocketClient.Orders.Create((Shipping.OzonRocket.Api.NewOrder)ozonOrder)
                : ozonRocketMethod.OzonRocketClient.Orders.Update((Shipping.OzonRocket.Api.UpdateOrder)ozonOrder);
            if (result != null)
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIdInOrderAdditionalData,
                    result.Id.ToString());
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketLogisticOrderNumberInOrderAdditionalData,
                    result.LogisticOrderNumber);
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderPostingIdInOrderAdditionalData,
                    result.Packages[0].PostingId.ToString());
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderPostingNumberInOrderAdditionalData,
                    result.Packages[0].PostingNumber);

                order.TrackNumber = result.LogisticOrderNumber;
                OrderService.UpdateOrderMain(order);

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);

                return true;
            }
            else if (ozonRocketMethod.OzonRocketClient.LastActionErrors != null)
                Errors.AddRange(ozonRocketMethod.OzonRocketClient.LastActionErrors);

            return false;
        }

        private Vat GetVat(TaxType? taxType, float cost)
        {
            switch (taxType)
            {
                case TaxType.Vat18:
                case TaxType.Vat20:
                    return new Vat
                    {
                        Rate = 20f,
                        Sum = cost * 20f / (100f + 20f)
                    };
                case TaxType.Vat10:
                    return new Vat
                    {
                        Rate = 10f,
                        Sum = cost * 10f / (100f + 10f)
                    };
                case TaxType.Vat0:
                    return new Vat
                    {
                        Rate = 0f,
                        Sum = 0f
                    };
            }
            return null;
        }
    }
}