using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.YandexDelivery;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexDelivery
{
    [ShippingKey("YandexDelivery")]
    public class YandexDelivery : BaseShippingWithCargo/*AndCache*/, IShippingSupportingSyncOfOrderStatus, IShippingSupportingTheHistoryOfMovement, IShippingSupportingPaymentCashOnDelivery
    {
        #region Ctor

        private readonly string _clientId;
        private readonly string _cityFrom;
        private readonly string _senderId;
        private readonly string _warehousId;
        private readonly string _requisiteId;
        private readonly string _secretKeyDelivery;
        private readonly string _secretKeyCreateOrder;
        private readonly string _widgetCode;
        private readonly bool _showAssessedValue;
        private readonly bool _statusesSync;

        private readonly Dictionary<string, string> _serializedApiKeys;

        private readonly YandexDeliveryApiService _yandexDeliveryApiService;

        private const string Url = "https://delivery.yandex.ru/api/last";
        public const string KeyNameOrderIdInOrderAdditionalData = "YandexDeliveryOrderId";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public YandexDelivery(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _cityFrom = _method.Params.ElementOrDefault(YandexDeliveryTemplate.CityFrom, "Москва");
            _clientId = _method.Params.ElementOrDefault(YandexDeliveryTemplate.ClientId);
            _senderId = _method.Params.ElementOrDefault(YandexDeliveryTemplate.SenderId);
            _warehousId = _method.Params.ElementOrDefault(YandexDeliveryTemplate.WarehouseId);
            _requisiteId = _method.Params.ElementOrDefault(YandexDeliveryTemplate.RequisiteId);

            _secretKeyDelivery = _method.Params.ElementOrDefault(YandexDeliveryTemplate.SecretKeyDelivery);
            _secretKeyCreateOrder = _method.Params.ElementOrDefault(YandexDeliveryTemplate.SecretKeyCreateOrder);

            _showAssessedValue = _method.Params.ElementOrDefault(YandexDeliveryTemplate.ShowAssessedValue).TryParseBool();

            _widgetCode = method.Params.ElementOrDefault(YandexDeliveryTemplate.WidgetCode);

            _serializedApiKeys = JsonConvert.DeserializeObject<Dictionary<string, string>>(method.Params.ElementOrDefault(YandexDeliveryTemplate.ApiKeys) ?? string.Empty);

            _yandexDeliveryApiService = new YandexDeliveryApiService(_clientId, _senderId, _cityFrom, _warehousId, _requisiteId, _serializedApiKeys);
            _statusesSync = method.Params.ElementOrDefault(YandexDeliveryTemplate.StatusesSync).TryParseBool();
        }

        #endregion

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            var yaOrderId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData);
            if (yaOrderId.IsNotEmpty())
            {
                var statusInfoAnswer = _yandexDeliveryApiService.GetOrderStatuses(yaOrderId);
                if (statusInfoAnswer != null && statusInfoAnswer.Count > 0)
                {
                    var statusInfo = 
                        statusInfoAnswer
                            .Where(x => 
                                StatusesReference.ContainsKey(x.Status)
                                && StatusesReference[x.Status].HasValue)
                            .OrderByDescending(x => x.Time)
                            .FirstOrDefault();
                    var yandexOrderStatus = statusInfo != null && StatusesReference.ContainsKey(statusInfo.Status)
                        ? StatusesReference[statusInfo.Status]
                        : null;

                    if (yandexOrderStatus.HasValue &&
                        order.OrderStatusId != yandexOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(yandexOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date).FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < statusInfo.Time)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                yandexOrderStatus.Value, "Синхронизация статусов для Яндекс.Доставки");
                        }
                    }
                }

            }
        }

        public bool SyncByAllOrders => false;
        public void SyncStatusOfOrders(IEnumerable<Order> orders) => throw new NotImplementedException();

        public bool StatusesSync
        {
            get { return _statusesSync; }
        }

        private Dictionary<string, int?> _statusesReference;
        public Dictionary<string, int?> StatusesReference
        {
            get
            {
                if (_statusesReference == null)
                {
                    _statusesReference = new Dictionary<string, int?>
                    {
                        { "DRAFT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DRAFT).TryParseInt(true)},
                        { "CREATED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_CREATED).TryParseInt(true)},
                        { "SENDER_SENT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_SENDER_SENT).TryParseInt(true)},
                        { "DELIVERY_LOADED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_LOADED).TryParseInt(true)},
                        { "SENDER_WAIT_FULFILMENT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_SENDER_WAIT_FULFILMENT).TryParseInt(true)},
                        { "SENDER_WAIT_DELIVERY", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_SENDER_WAIT_DELIVERY).TryParseInt(true)},
                        { "FULFILMENT_LOADED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_LOADED).TryParseInt(true)},
                        { "FULFILMENT_ARRIVED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_ARRIVED).TryParseInt(true)},
                        { "FULFILMENT_PREPARED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_PREPARED).TryParseInt(true)},
                        { "FULFILMENT_TRANSMITTED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_TRANSMITTED).TryParseInt(true)},
                        { "DELIVERY_AT_START", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_AT_START).TryParseInt(true)},
                        { "DELIVERY_TRANSPORTATION", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_TRANSPORTATION).TryParseInt(true)},
                        { "DELIVERY_ARRIVED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_ARRIVED).TryParseInt(true)},
                        { "DELIVERY_TRANSPORTATION_RECIPIENT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_TRANSPORTATION_RECIPIENT).TryParseInt(true)},
                        { "DELIVERY_ARRIVED_PICKUP_POINT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_ARRIVED_PICKUP_POINT).TryParseInt(true)},
                        { "DELIVERY_DELIVERED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_DELIVERED).TryParseInt(true)},
                        { "RETURN_PREPARING", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_PREPARING).TryParseInt(true)},
                        { "RETURN_ARRIVED_DELIVERY", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_ARRIVED_DELIVERY).TryParseInt(true)},
                        { "RETURN_ARRIVED_FULFILMENT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_ARRIVED_FULFILMENT).TryParseInt(true)},
                        { "RETURN_PREPARING_SENDER", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_PREPARING_SENDER).TryParseInt(true)},
                        { "RETURN_RETURNED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_RETURNED).TryParseInt(true)},
                        { "RETURN_TRANSMITTED_FULFILMENT", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_TRANSMITTED_FULFILMENT).TryParseInt(true)},
                        { "LOST", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_LOST).TryParseInt(true)},
                        { "UNEXPECTED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_UNEXPECTED).TryParseInt(true)},
                        { "CANCELED", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_CANCELED).TryParseInt(true)},
                        { "ERROR", _method.Params.ElementOrDefault(YandexDeliveryTemplate.Status_ERROR).TryParseInt(true)},
                    };
                }
                return _statusesReference;
            }
        }

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }
        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var yaOrderId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData);
            if (yaOrderId.IsNotEmpty())
            {
                var statusInfoAnswer = _yandexDeliveryApiService.GetOrderStatuses(yaOrderId);
                if (statusInfoAnswer != null && statusInfoAnswer.Count > 0)
                {
                    return statusInfoAnswer
                        .OrderByDescending(x => x.Time)
                        .Select(statusInfo => new HistoryOfMovement()
                        {
                            Code = statusInfo.Status,
                            Name = statusInfo.StatusComment,
                            Date = statusInfo.Time,
                            Comment = statusInfo.Description
                        }).ToList();
                }
            }

            return null;
        }


        #endregion IShippingSupportingTheHistoryOfMovement

        private Dictionary<string, object> GetParam()
        {
            var dimensions = GetDimensions(rate: 10);
            var weight = GetTotalWeight();
            var totalPrice = _totalPrice;// _items.Sum(item => item.Price * item.Amount);

            var cityGeoIdTo = _yandexDeliveryApiService.GetCityGeoId(_calculationParameters.City + ", " + _calculationParameters.Region);

            var data = new Dictionary<string, object>()
            {
                {"client_id", _clientId},
                {"sender_id", _senderId},
                {"city_from", _cityFrom},
                {"city_to", _calculationParameters.Region.IsNullOrEmpty() ? _calculationParameters.City : _calculationParameters.Region + ", " + _calculationParameters.City},
                {"height", Convert.ToInt32(dimensions[2]).ToString("F2", CultureInfo.InvariantCulture)},
                {"width", Convert.ToInt32(dimensions[1]).ToString("F2", CultureInfo.InvariantCulture)},
                {"length", Convert.ToInt32(dimensions[0]).ToString("F2", CultureInfo.InvariantCulture)},
                {"weight", (weight != 0 ? weight : _defaultWeight).ToString("F2", CultureInfo.InvariantCulture)},
                {"total_cost", totalPrice.ToString("F2",CultureInfo.InvariantCulture)},
                {"order_cost", totalPrice.ToString("F2",CultureInfo.InvariantCulture)}
            };

            if (!string.IsNullOrEmpty(cityGeoIdTo))
            {
                data.Add("geo_id_to", cityGeoIdTo);
            }

            if (_showAssessedValue)
                data.Add("assessed_value", totalPrice.ToString("F2", CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(_calculationParameters.Zip))
                data.Add("index_city", _calculationParameters.Zip);

            data.Add("secret_key", YandexDeliveryService.GetSign(data, _secretKeyDelivery));
            return data;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var shippingOptions = new List<BaseShippingOption>();

            var postData = GetParam();
            var postDataString = postData.Keys.Where(key => postData[key] != null)
                .Aggregate("", (current, t) => current + string.Format("&{0}={1}", t, HttpUtility.UrlEncode(postData[t].ToString())));

            var responseData = YandexDeliveryService.MakeRequest(Url + "/searchDeliveryList", postDataString);

            if (!string.IsNullOrEmpty(responseData) && !responseData.Contains("\"status\":\"error\""))
            {
                try
                {
                    responseData = responseData.Replace("settings\":[]", "settings\":{}");
                    var dto = JsonConvert.DeserializeObject<YandexDeliveryListDto>(responseData);
                    if (dto.Status == "ok")
                    {
                        var onlypicks = dto.Data.Where(x => x.Type == "PICKUP" && x.PickupPoints != null && x.PickupPoints.Count > 0).ToList();
                        if (onlypicks.Any())
                        {
                            var dimensions = GetDimensions(rate: 10);
                            var dimensionsStr = string.Format("[{0}, {1}, {2}, {3}]",
                                        dimensions[2].ToString("F2", CultureInfo.InvariantCulture),
                                        dimensions[1].ToString("F2", CultureInfo.InvariantCulture),
                                        dimensions[0].ToString("F2", CultureInfo.InvariantCulture),
                                        1);

                            string selectedTariffId = null;
                            if (_calculationParameters.ShippingOption != null &&
                                _calculationParameters.ShippingOption.MethodId == _method.ShippingMethodId &&
                                _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(YandexDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                            {
                                if (_calculationParameters.ShippingOption.GetType() == typeof(YandexDeliveryPickupPointOption))
                                    selectedTariffId = ((YandexDeliveryPickupPointOption)_calculationParameters.ShippingOption).TariffId;
                            }

                            float? rate = null;
                            if (selectedTariffId.IsNotEmpty()) {
                                var selectedTariff = onlypicks.FirstOrDefault(x => x.TariffId == selectedTariffId);
                                if (selectedTariff != null)
                                    rate = selectedTariff.CostWithRules;
                            }

                            if (!rate.HasValue)
                                rate = onlypicks.Min(x => x.CostWithRules);

                            shippingOptions.Add(new YandexDeliveryPickupPointOption(_method, _totalPrice, dto.Data)
                            {
                                WidgetCodeYa = _widgetCode,
                                ShowAssessedValue = _showAssessedValue,
                                Weight = (string)postData["weight"],
                                Cost = (string)postData["total_cost"],
                                Dimensions = "[" + dimensionsStr + "]",
                                Amount = _totalPrice.ToString("F2", CultureInfo.InvariantCulture),
                                Rate = rate.Value,
                            });
                        }

                        var notpicks = dto.Data.Where(x => x.Type != "PICKUP").ToList();
                        if (notpicks.Any())
                        {
                            shippingOptions.AddRange(notpicks.Select(item =>
                                                    item.Settings != null && item.Settings.ToYDWarehouse == "1"
                                                    ? new YandexDeliveryPickupWarehouseOption(_method, _totalPrice, item)
                                                    : new YandexDeliveryOption(_method, _totalPrice, item)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(responseData, ex);
                }
            }

            else
            {
                Debug.Log.Warn("searchDeliveryList response: " + responseData);
            }

            return shippingOptions;
        }

        /// <summary>
        /// Создание заказа
        /// </summary>
        public bool CreateOrder(Order order)
        {
            var dimensions = GetDimensions(rate: 10);

            var length = (int)dimensions[0];
            var width = (int)dimensions[1];
            var height = (int)dimensions[2];
            var weight = GetTotalWeight();

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = order.ShippingMethod.ShippingCurrency;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            // Если есть скидки, наценки и тд включаем их в стоимость товаров
            // Яндекс.Доставка работает только с целыми значениями цен, никаких копеек
            var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                  .AcceptableDifference(0.1f)
                                  .WithCurrency(shippingCurrency)
                                  .CeilingAmountToInteger()
                                  .GetItems();

            var sum = (float)Math.Round(orderItems.Sum(item => item.Price * item.Amount) + shippingCost);

            YandexDeliveryAdditionalData additionalData = null;
            try
            {
                if (order.OrderPickPoint != null && order.OrderPickPoint.AdditionalData.IsNotEmpty())
                {
                    additionalData =
                        JsonConvert.DeserializeObject<YandexDeliveryAdditionalData>(order.OrderPickPoint.AdditionalData);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            var data = new Dictionary<string, object>()
            {
                {"order_date", order.OrderDate.ToUniversalTime()},

                {"order_weight", weight.ToString("F2").Replace(",", ".")},
                {"order_height", height.ToString("F2", CultureInfo.InvariantCulture)},
                {"order_width", width.ToString("F2", CultureInfo.InvariantCulture)},
                {"order_length", length.ToString("F2", CultureInfo.InvariantCulture)},

                {"order_amount_prepaid", order.Payed ? sum.ToString("F2", CultureInfo.InvariantCulture) : "0"},
                {"order_delivery_cost", shippingCost.ToString("F2", CultureInfo.InvariantCulture)},
                {"is_manual_delivery_cost", "1"}, // будет взята стоимость доставки переданной нами
                {"order_assessed_value", orderItems.Sum(x => x.Amount*x.Price).ToString("F2", CultureInfo.InvariantCulture)},

                {"order_requisite", _requisiteId},
                {"order_warehouse", _warehousId},

                {"recipient_first_name", order.OrderRecipient.FirstName},
                {"recipient_last_name", order.OrderRecipient.LastName ?? ""},
                {"recipient_middle_name", order.OrderRecipient.Patronymic ?? ""},
                {"recipient_phone", order.OrderRecipient.Phone.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")},
                {"recipient_email", order.OrderCustomer.Email},
                {"recipient_comment", order.CustomerComment},

                {"delivery_pickuppoint", order.OrderPickPoint != null ? order.OrderPickPoint.PickPointId : null},

                {"delivery_tariff", additionalData != null ? additionalData.tariffId : (order.OrderPickPoint != null ? order.OrderPickPoint.AdditionalData.TryParseInt() : 0)},

                {"deliverypoint_city", order.OrderCustomer.City},
                {"deliverypoint_index", order.OrderCustomer.Zip},
                {"deliverypoint_street", order.OrderCustomer.GetCustomerAddress()},

                {"order_num", order.OrderID},

                {"client_id", _clientId},
                {"sender_id", _senderId},
            };

            if (additionalData != null)
            {
                data.Add("delivery_direction", additionalData.direction);
                data.Add("delivery_delivery", additionalData.delivery);
                data.Add("delivery_price", additionalData.price);

                if (additionalData.to_ms_warehouse != null)
                {
                    var to_ms_warehouse = additionalData.to_ms_warehouse.Value;
                    if (additionalData.delivery > 0)
                    {
                        var deliveries = _yandexDeliveryApiService.GetDeliveries();
                        if (deliveries != null)
                        {
                            var orderDeliveryIdStr = additionalData.delivery.ToString();
                            var typeWarehouse = to_ms_warehouse == 0
                                ? "delivery"     // склад службы доставки
                                : "fulfillment"; // единый склад
                            var orderDeliveries = deliveries.Where(delivery =>
                                delivery.Id.Equals(orderDeliveryIdStr, StringComparison.Ordinal)).ToList();

                            if (typeWarehouse == "delivery" &&
                                orderDeliveries.Count > 0 && 
                                orderDeliveries.Any(delivery => delivery.Name.Contains("Почта Онлайн", StringComparison.OrdinalIgnoreCase)))
                            {
                                to_ms_warehouse = 1;  // единый склад
                            }
                        }
                    }

                    data.Add("delivery_to_yd_warehouse", to_ms_warehouse);
                }
            }

            var dict2 = new Dictionary<string, object>(data) { { "order_items", orderItems.ToArray() } };
            var sign = YandexDeliveryService.GetSign(dict2, _secretKeyCreateOrder);


            var postDataString = String.Join("&", data.Where(x => x.Value != null).Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value.ToString()))) +
                                 "&" + YandexDeliveryService.GetOrderItems(orderItems) +
                                 "&secret_key=" + sign;

            var resultData = YandexDeliveryService.MakeRequest(Url + "/createOrder", postDataString);

            if (string.IsNullOrEmpty(resultData) || !resultData.Contains("\"status\":\"ok\""))
            {
                Debug.Log.Warn("Can't create order in yandex delivery: " + resultData);
                return false;
            }

            var resultCreateOrder = JsonConvert.DeserializeObject<YandexDeliveryResponseCreateOrder>(resultData);

            if (resultCreateOrder.Data != null && resultCreateOrder.Data.Order != null &&
                !string.IsNullOrEmpty(resultCreateOrder.Data.Order.Id))
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData, resultCreateOrder.Data.Order.Id);
                order.TrackNumber = resultCreateOrder.Data.Order.Id;
            }
            else
            {
                Debug.Log.Warn("Created order in yandex delivery. There is no order id in the response: " + resultData);
                return false;
            }

            return true;
        }

    }
}
