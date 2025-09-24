using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.ApiShip;
using AdvantShop.Core.Services.Shipping.ApiShip.Api;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping.ApiShip.Api;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ApiShip
{
    public partial class ApiShip : IUnloadOrder
    {
        public UnloadOrderResult UnloadOrder(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            if (order.OrderCustomer == null)
                return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");
            
            if (OrderService.GetOrderAdditionalData(order.OrderID, KeyTextSendOrder).IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            
            var orderPickPoint = JsonConvert.DeserializeObject<ApiShipShippingPoint>(order.OrderPickPoint.AdditionalData);
            ApiShipParamsSendOrder parametrs = new ApiShipParamsSendOrder(_method);

            ApiShipAddOrderRequestModel orderModel = new ApiShipAddOrderRequestModel();
            orderModel.Order = new ApiShipOrder();
            orderModel.Sender = new ApiShipSender();
            orderModel.ExtraParams = new List<Extra>();
            orderModel.Recipient = new ApiShipRecipient();
            orderModel.Cost = new ApiShipCost();
            orderModel.Places = new List<ApiShipPlaces>();
            orderModel.ReturnAddress = new ApiShipContact();

            int orderWeight = (int)GetTotalWeight();
            var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                              .AcceptableDifference(0.1f)
                              .WithCurrency(_method.ShippingCurrency)
                              .CeilingAmountToInteger()
                              .GetItems() as List<OrderItem>;

            int tafirrId = Convert.ToInt32(orderPickPoint?.Extra?.FirstOrDefault(x => x.Key == "tariffId")?.Value);
            int pickPointId = 0;
            int.TryParse(orderPickPoint?.Id, out pickPointId);

            orderModel.Order.ClientNumber = order.Number;
            orderModel.Order.ProviderKey = orderPickPoint?.ProviderKey;
            orderModel.Order.PickupType = (int)ApiShipPickupType.fromClientDoor;

            orderModel.Order.DeliveryType = pickPointId > 0 ? (int)ApiShipDeliveryType.toPVZ : (int)ApiShipDeliveryType.toDoor;
            orderModel.Order.TariffId = tafirrId;
            orderModel.Order.Weight = orderWeight;
            if (pickPointId > 0)
                orderModel.Order.PointOutId = pickPointId;

            orderModel.Cost.AssessedCost = order.Sum - order.ShippingCost;
            orderModel.Cost.CodCost = order.Payed ? 0 : order.Sum;
            orderModel.Cost.DeliveryCost = order.Payed ? 0 : order.ShippingCost;

            string сountrySenderIso2 = "RU";
            string сountryRecipientIso2 = "RU";

            if (parametrs.SendedCountry > 0)
            {
                var country = CountryService.GetCountry(parametrs.SendedCountry);
                if(country != null && country.CountryId > 0)
                    сountrySenderIso2 = country.Iso2;
            }
            
            if (order.OrderCustomer != null && !string.IsNullOrEmpty(order.OrderCustomer.Country))
            {
                var country = CountryService.GetCountryByName(order.OrderCustomer.Country);
                if (country != null && country.CountryId > 0)
                {
                    сountryRecipientIso2 = country.Iso2;
                }
            }

            orderModel.Sender.CountryCode = сountrySenderIso2;
            orderModel.Sender.AddressString = parametrs.SenderAddress;
            orderModel.Sender.ContactName = !parametrs.SenderName.IsNullOrEmpty() ? parametrs.SenderName : string.Empty;
            orderModel.Sender.Phone = FormatPhone(!parametrs.SenderPhone.IsNullOrEmpty() ? parametrs.SenderPhone : string.Empty);

            orderModel.Recipient.CountryCode = сountryRecipientIso2;
            orderModel.Recipient.AddressString = order?.OrderCustomer?.Region + " " + order?.OrderCustomer?.City + " " + order?.OrderCustomer?.Street + " " + order?.OrderCustomer?.House + " " + order?.OrderCustomer?.Structure;
            orderModel.Recipient.ContactName = order?.OrderRecipient?.FirstName + " " + order?.OrderRecipient?.LastName;
            orderModel.Recipient.Phone = FormatPhone(order?.OrderRecipient?.Phone);

            string returnCountryCountryIso2 = "";
            if (parametrs.ReturnCountry > 0)
            {
                returnCountryCountryIso2 = CountryService.GetCountry(parametrs.ReturnCountry).Iso2;
            }
            else {
                returnCountryCountryIso2 = сountrySenderIso2;
            }
            orderModel.ReturnAddress.CountryCode = returnCountryCountryIso2;
            orderModel.ReturnAddress.AddressString = parametrs.ReturnAddress;
            orderModel.ReturnAddress.ContactName = !parametrs.SenderName.IsNullOrEmpty() ? parametrs.SenderName : string.Empty;
            orderModel.ReturnAddress.Phone = FormatPhone(!parametrs.SenderPhone.IsNullOrEmpty() ? parametrs.SenderPhone : string.Empty);

            var places = new ApiShipPlaces();
            places.Weight = orderWeight;
            var items = new List<ApiShipItem>();                
            foreach (var orderItem in orderItems)
            {
                var item = new ApiShipItem();
                item.Description = orderItem.Name;
                item.Quantity = (int)Math.Ceiling(orderItem.Amount);
                item.AssessedCost = orderItem.Price;
                item.Cost = order.Payed ? 0 : orderItem.Price;
                items.Add(item);
            }
            places.Items = items;
            orderModel.Places.Add(places);
            var res = ApiShipService.ApiShipSendOrder(orderModel);
            if (res != null && res.Created.IsNotEmpty())
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, KeyTextSendOrder, true.ToString());
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, KeyTextOrderIdApiShip, res.OrderId.ToString());
                return UnloadOrderResult.CreateSuccessResult("Заказ успешно отправлен в ApiShip");
            }

            return UnloadOrderResult.CreateFailedResult("Не удалось создать заказ" + (res != null && res.Errors.Count > 0 ? "\n" + string.Join(",\n", res.Errors.Select(x=>x.Message).ToList()) : string.Empty));
        }

        private string FormatPhone(string phone)
        {
            string result = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (result.StartsWith("+7") && result.Length > 12)
            {
                result = result.Substring(0, 12);
            }
            else if (result.StartsWith("8") && result.Length > 11)
            {
                result = result.Substring(0, 11);
            }
            return result;
        }
    }
}