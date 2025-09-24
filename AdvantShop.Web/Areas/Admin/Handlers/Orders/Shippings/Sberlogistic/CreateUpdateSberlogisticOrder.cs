using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;
using System;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Shipping.Sberlogistic.Api;
using AdvantShop.Shipping.Sberlogistic;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Attributes;
using System.Collections.Generic;
using AdvantShop.Core.Common;
using AdvantShop.Taxes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sberlogistic
{
	public class CreateUpdateSberlogisticOrder
	{
        private readonly int _orderId;

        public CreateUpdateSberlogisticOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Sberlogistic")
                return new CommandResult() { Error = "Order shipping method is not 'Sberlogistic' type" };

            var uuid = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderUuidInOrderAdditionalData);

            var isUpdate = uuid.IsNotEmpty();
            try
            {
                SberlogisticCalculateOption sberlogisticCalculateOption = null;

                try
                {
                    sberlogisticCalculateOption =
                        JsonConvert.DeserializeObject<SberlogisticCalculateOption>(order.OrderPickPoint.AdditionalData);
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }
                var shippingCurrency = order.ShippingMethod.ShippingCurrency;

                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();
                var sberlogistic = new Shipping.Sberlogistic.Sberlogistic(shippingMethod, calculationParameters);

                var paymentsCash = new[]
                {
                    AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.Cash)),
                    AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
                };
                var recipientPhone = order.OrderRecipient != null 
                    && order.OrderRecipient.StandardPhone.HasValue
                        ? order.OrderRecipient.StandardPhone.Value.ToString()
                        : string.Empty;

                if (recipientPhone.StartsWith("8"))
                    recipientPhone = "7" + recipientPhone.Substring(1);

                if (recipientPhone.Length == 10)
                    recipientPhone = "7" + recipientPhone;

                var dimensionsInSm = sberlogistic.GetDimensions();
                var dimensions = new Dimensions {
                    Length = (int)Math.Ceiling(dimensionsInSm[0]),
                    Width = (int)Math.Ceiling(dimensionsInSm[1]),
                    Height = (int)Math.Ceiling(dimensionsInSm[2]),
                };
                Shipment shipment = new Shipment
                {
                    OrderNumber = order.Number,
                    CourierAddress = string.Join(",", new[] { sberlogistic.CityFrom, sberlogistic.StreetFrom, sberlogistic.HouseFrom }),
                    RecipientName = order.OrderRecipient.FirstName + " " + order.OrderRecipient.LastName,
                    RecipientNumber = recipientPhone,
                    Weight = (int)sberlogistic.GetTotalWeight(1000),
                    Dimensions = dimensions
                };

                if (paymentsCash.Contains(order.PaymentMethod.PaymentKey))
				{
                    var listItems = new List<Item>();
					foreach (var product in order.GetOrderItemsWithDiscountsAndFee()
                                                 .AcceptableDifference(0.1f)
                                                 .WithCurrency(shippingCurrency)
                                                 .CeilingAmountToInteger()
                                                 .GetItems())
					{
                        listItems.Add(new Item
                        {
                            CostPerUnit = (int)product.Price * 100,
                            Name = product.Name,
                            Quantity = (int)product.Amount,
                            VatRate = GetVatRate(product.TaxType)
                        });
					}
                    if (listItems.Count > 0)
					{
                        shipment.Items = listItems;
                        shipment.CashOnDelivery = listItems.Sum(x => x.CostPerUnit * x.Quantity);
                        shipment.InsuranceSum = shipment.CashOnDelivery;
                    }
				}
                    
                var orderPickPoint = order.OrderPickPoint;
                if (sberlogisticCalculateOption.IsCourier)
				{
                    if (order.OrderCustomer.Street.IsNullOrEmpty() 
                        || order.OrderCustomer.House.IsNullOrEmpty() 
                        && order.OrderCustomer.Street.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                        return new CommandResult { Error = "Необходимо указать улицу и дом получателя" };
                    shipment.RecipientAddress = order.OrderCustomer != null
                        ? string.Join(
                            ", ",
                            new[] {
                                order.OrderCustomer.Zip, order.OrderCustomer.Country,
                                order.OrderCustomer.Region,
                                order.OrderCustomer.City, order.OrderCustomer.Street,
                                order.OrderCustomer.House, order.OrderCustomer.Structure,
                                order.OrderCustomer.Apartment
                            }.Where(x => x.IsNotEmpty()))
                        : null;
                    shipment.MailType = "SBER_COURIER";
                }
				else if(orderPickPoint != null)
				{
                    shipment.DeliveryPointAddress = orderPickPoint.PickPointAddress;
                    shipment.DeliveryPointId = orderPickPoint.PickPointId.TryParseInt();
                    shipment.MailType = "SBER_PACKAGE";
                }

                var result = isUpdate
                    ? sberlogistic.SberlogisticApiService.UpdateOrderDraft(shipment, uuid) 
                    : sberlogistic.SberlogisticApiService.CreateShipment(shipment);

                var hasErrors = result == null || result.ErrorDescriptions != null && result.ErrorDescriptions.Count > 0;

                if (!hasErrors)
                {
                    OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                        Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderUuidInOrderAdditionalData,
                        result.Uuid);
                    if (!string.IsNullOrEmpty(result.Barcode))
                    {
                        OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                            Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderBarcodeInOrderAdditionalData,
                            result.Barcode);
                        OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                            Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderUuidInOrderAdditionalData,
                            result.Uuid);
                        OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                            Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderIsCanceledInOrderAdditionalData, String.Empty);

                        order.TrackNumber = result.Barcode;
                        OrderService.UpdateOrderMain(order);
                    }
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);
                }
                return !hasErrors
                    ? new CommandResult() { Result = true, Message = $"Черновик заказа успешно {(isUpdate ? "обновлен" : "создан")}" }
                    : new CommandResult() { Error = $"Не удалось {(isUpdate ? "обновить" : "создать")} черновик заказа" };
            }
            catch (BlException ex)
            {
                return new CommandResult() { Error = $"Не удалось {(isUpdate ? "обновить" : "создать")} черновик заказа: " + ex.Message };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = $"Не удалось {(isUpdate ? "обновить" : "создать")} черновик заказа: " + ex.Message };
            }
        }
        private int GetVatRate(TaxType? taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat0:
                    return 5;
                case TaxType.Vat10:
                    return 2;
                case TaxType.Vat20:
                    return 1;
                default:
                    return 6;
            }
        }
    }
}
