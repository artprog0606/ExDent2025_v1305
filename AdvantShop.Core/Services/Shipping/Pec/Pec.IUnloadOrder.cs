using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.Pec.Api;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Pec
{
    public partial class Pec : IUnloadOrder
    {
        public UnloadOrderResult UnloadOrder(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            // if (order.OrderCustomer == null)
            //     return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");
            
            if (OrderService.GetOrderAdditionalData(order.OrderID, KeyNameCargoCodeInOrderAdditionalData).IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            if (order.OrderPickPoint == null || order.OrderPickPoint.AdditionalData.IsNullOrEmpty())
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах расчета доставки.");
      
        
            PecCalculateOption pecCalculateOption = null;

            try
            {
                pecCalculateOption =
                    JsonConvert.DeserializeObject<PecCalculateOption>(order.OrderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (pecCalculateOption is null)
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах расчета доставки.");
            
            
            if (SenderCityId == null)
                return UnloadOrderResult.CreateFailedResult("Не указан город отправления.");

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var isDeliveryTypeToPoint = !pecCalculateOption.WithDelivery;
            var dimensions = GetDimensions().Select(x => x / 1000d).ToArray();// конвертируем сами, чтобы получить большую точность (float для таких значений сильно ограничен);
            var weight = GetTotalWeight();
            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            var senderCity = FindSenderCity();
            if (senderCity.IsNullOrEmpty())
            {
                if (PecApiService.LastActionErrors?.Count > 0)
                    return UnloadOrderResult.CreateFailedResult("Не указан город отправления.\n" + string.Join("\n", PecApiService.LastActionErrors));
                return UnloadOrderResult.CreateFailedResult("Не указан город отправления.");
            }

            string receiverCity = null;
            if (order.OrderCustomer != null)
            {
                // делаем замену только для поиска, сами данные пользователя не изменяем
                string outCountry, outRegion, outDistrict, outCity, outZip;
                ShippingReplaceGeoService.ReplaceGeo(
                        order.ShippingMethod.ShippingType,
                        order.OrderCustomer.Country, order.OrderCustomer.Region, order.OrderCustomer.District, order.OrderCustomer.City, order.OrderCustomer.Zip,
                        out outCountry, out outRegion, out outDistrict, out outCity, out outZip);

                receiverCity = FindReceiverCity(outCity, outDistrict);
            }
            if (receiverCity.IsNullOrEmpty())
                return UnloadOrderResult.CreateFailedResult("Не найден город получения.");

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = order.ShippingMethod.ShippingCurrency;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                //order.OrderItems.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            //var payer = new DeliveryPayer { Type = mustPay ? EnPaymentType.Receiver : EnPaymentType.Seller };
            var payer = new DeliveryPayer { Type = EnPaymentType.Seller };
            var pecOrder = new PreregistrationSubmitParams
            {
                Sender = new Sender
                {
                    City = senderCity,
                    Inn = SenderInn,
                    Title = SenderTitle,
                    Fs = SenderFs,
                    Phone = SenderPhone,
                    PhoneAdditional = SenderPhoneAdditional
                },
                Cargos = new List<PreregistrationSubmitCargo> {
                    new PreregistrationSubmitCargo
                    {
                        Common = new CargoCommon
                        {
                            Type = pecCalculateOption.TransportingType == EnTransportingType.Car.Value
                                ? EnTransportingTypeCargo.Car
                                : pecCalculateOption.TransportingType == EnTransportingType.Avia.Value
                                    ? EnTransportingTypeCargo.Avia
                                    : EnTransportingTypeCargo.Easyway,
                            Height = dimensions[2],
                            Width = dimensions[1],
                            Length = dimensions[0],
                            Weight = Math.Round(weight, 3),
                            DeclaredCost = orderSum - shippingCost,
                            OrderNumber = order.Number,
                            PositionsCount = 1
                        },
                        Receiver = new Receiver
                        {
                            City = receiverCity,
                            Title = order.OrderRecipient != null
                                ? string.Join(" ", (new[] { order.OrderRecipient.LastName, order.OrderRecipient.FirstName, order.OrderRecipient.Patronymic })
                                        .Where(x => !string.IsNullOrEmpty(x)))
                                : null,
                            Person = order.OrderRecipient != null
                                ? string.Join(" ", (new[] { order.OrderRecipient.LastName, order.OrderRecipient.FirstName, order.OrderRecipient.Patronymic })
                                        .Where(x => !string.IsNullOrEmpty(x)))
                                : null,
                            Phone = order.OrderRecipient != null
                                ? order.OrderRecipient.Phone
                                : null,
                            WarehouseId = isDeliveryTypeToPoint
                                ? order.OrderPickPoint.PickPointId
                                : null,
                            AddressStock = isDeliveryTypeToPoint
                                ? null
                                : string.Join(
                                    ", ",
                                    new[] {
                                        order.OrderCustomer.Street,
                                        order.OrderCustomer.House, order.OrderCustomer.Structure,
                                        order.OrderCustomer.Apartment
                                    }.Where(x => x.IsNotEmpty()))
                        },
                        Services = new Services
                        {
                            Transporting = new Transporting{ Payer = payer },
                            HardPacking = new Ing{ Enabled = false },
                            Insurance = new Insurance 
                            {
                                Cost = orderSum - shippingCost,
                                Payer = payer,
                                Enabled = WithInsure
                            },
                            Sealing = new Sealing { Enabled = false },
                            Strapping = new Strapping { Enabled = false },
                            DocumentsReturning = new Ing { Enabled = false },
                            Delivery = new ServicesDelivery 
                            {
                                Enabled = !isDeliveryTypeToPoint,
                                Payer = !isDeliveryTypeToPoint ? payer : null
                            },
                            CashOnDelivery = mustPay
                                ? new CashOnDelivery
                                {
                                    Enabled = true,
                                    IncludeTes = true,
                                    ActualCost = orderSum - shippingCost,
                                    CashOnDeliverySum = orderSum,
                                    OrderNumber = order.Number,
                                    SellerInn = SenderInn,
                                    SellerTitle = string.Format("{0} {1}", SenderFs, SenderTitle),
                                    SellerPhone = SenderPhone,
                                }
                                : null
                        }
                    }
                }
            };

            var result = PecApiService.Preregistration(pecOrder);

            if (result != null)
            {
                if (result.Success
                    && result.Result != null)
                {
                    if (result.Result.Cargos != null && result.Result.Cargos.Count > 0)
                    {
                        OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                            KeyNameCargoCodeInOrderAdditionalData,
                            result.Result.Cargos[0].CargoCode);

                        order.TrackNumber = result.Result.Cargos[0].CargoCode;
                        OrderService.UpdateOrderMain(order);

                        return UnloadOrderResult.CreateSuccessResult();
                    }
                }
            }
            
            if (PecApiService.LastActionErrors?.Count > 0)
                return UnloadOrderResult.CreateFailedResult("Не удалось передать заказ.\n" + string.Join("\n", PecApiService.LastActionErrors));

            return UnloadOrderResult.CreateFailedResult("Не удалось передать заказ.");
        }

        private string FindReceiverCity(string city, string district)
        {
            if (city.IsNotEmpty())
            {
                var brancheAndCity = FindBrancheAndCity(city, district, PecApiService);
                if (brancheAndCity != null)
                    return brancheAndCity.CityTitle.IsNotEmpty() ? brancheAndCity.CityTitle : brancheAndCity.BranchTitle;
            }
            return null;
        }

        private string FindSenderCity()
        {
            var findResult = PecApiService.FindBranchesById(SenderCityId.Value);
            if (findResult != null)
            {
                if (findResult.Success && findResult.Result.Success && findResult.Result.Items != null)
                {
                    var cityItem = findResult.Result.Items.FirstOrDefault();
                    if (cityItem != null)
                        return cityItem.CityTitle.IsNotEmpty() ? cityItem.CityTitle : cityItem.BranchTitle;
                }
            }

            return null;
        }
    }
}