using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Shipping.Yandex.Api;

namespace AdvantShop.Shipping.Yandex
{
    [ShippingKey("Yandex")]
    public partial class YandexDelivery : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingSyncOfOrderStatus, IShippingSupportingPaymentCashOnDelivery
    {
        #region Ctor

        private readonly string _apiToken;
        private readonly string _stationId;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly bool _statusesSync;
        private readonly TypeDeparturePoint _typeDeparturePoint;
        private readonly string _receptionPoint;
        private readonly int? _paymentCodCardId;
        private readonly List<DeliveryType> _deliveryTypes;
        private readonly TypeViewPoints _postOfficeTypeViewPoints;
        private readonly CultureInfo _currentCulture;

        private readonly YandexDeliveryApiService _yandexDeliveryApi;

        public const string KeyNameYandexRequestIdInOrderAdditionalData = "YandexDeliveryRequestId";
        public const string KeyNameYandexOrderIsCanceledInOrderAdditionalData = "YandexOrderIsCanceled";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public YandexDelivery(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _apiToken = _method.Params.ElementOrDefault(YandexDeliveryTemplate.ApiToken);
            _stationId = _method.Params.ElementOrDefault(YandexDeliveryTemplate.StationId);
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(YandexDeliveryTemplate.TypeViewPoints).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(YandexDeliveryTemplate.YaMapsApiKey);
            _statusesSync = method.Params.ElementOrDefault(YandexDeliveryTemplate.StatusesSync).TryParseBool();
            _typeDeparturePoint = (TypeDeparturePoint)_method.Params.ElementOrDefault(YandexDeliveryTemplate.TypeDeparturePoint).TryParseInt();
            _receptionPoint = _method.Params.ElementOrDefault(YandexDeliveryTemplate.ReceptionPoint);
            _deliveryTypes = (method.Params.ElementOrDefault(YandexDeliveryTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => DeliveryType.Parse(x)).ToList();
            if (_typeDeparturePoint == TypeDeparturePoint.Station)
                _yandexDeliveryApi = new YandexDeliveryApiService(_stationId, _apiToken);
            else
                _yandexDeliveryApi = new YandexDeliveryApiService(_receptionPoint, _apiToken);
            _paymentCodCardId = _method.Params.ElementOrDefault(YandexDeliveryTemplate.PaymentCodCardId).TryParseInt(true);
            var statusesReference = method.Params.ElementOrDefault(YandexDeliveryTemplate.StatusesReference);
            if (!string.IsNullOrEmpty(statusesReference))
            {
                string[] arr = null;
                _statusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (arr = x.Split(','))[0],
                            x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<string, int?>();
            _postOfficeTypeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(YandexDeliveryTemplate.PostOfficeTypeViewPoints).TryParseInt();
            _currentCulture = Culture.GetCulture();
        }

        #endregion

        #region IShippingSupportingSyncOfOrderStatus

        public void SyncStatusOfOrder(Order order)
        {
            var requestId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameYandexRequestIdInOrderAdditionalData);
            if (requestId.IsNullOrEmpty())
                return;

            var getStatuses = _yandexDeliveryApi.GetHistoryOfStatuses(requestId);
            if (!(getStatuses?.StateHistory?.Count > 0))
                return;

            var statusInfo =
                getStatuses.StateHistory
                           .Where(x =>
                                StatusesReference.ContainsKey(x.Status)
                                && StatusesReference[x.Status].HasValue)
                           .OrderByDescending(x => x.Timestamp)
                           .FirstOrDefault();
            
            if (statusInfo == null)
                return;

            if (!StatusesReference.ContainsKey(statusInfo.Status))
                return;

            var yandexOrderStatus = StatusesReference[statusInfo.Status];

            if (yandexOrderStatus.HasValue
                && order.OrderStatusId != yandexOrderStatus.Value
                && OrderStatusService.GetOrderStatus(yandexOrderStatus.Value) != null)
            {
                var lastOrderStatusHistory =
                    OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                      .OrderByDescending(item => item.Date)
                                      .FirstOrDefault();
                // Даты может не быть
                if (lastOrderStatusHistory is null || statusInfo.Timestamp == 0 || lastOrderStatusHistory.Date < statusInfo.Timestamp.ToDateTimeFromUnixTime())
                    OrderStatusService.ChangeOrderStatus(order.OrderID, yandexOrderStatus.Value, "Синхронизация статусов для Яндекс.Доставки");
            }
        }

        public bool SyncByAllOrders => false;
        public void SyncStatusOfOrders(IEnumerable<Order> orders) => throw new NotImplementedException();

        public bool StatusesSync => _statusesSync;

        private Dictionary<string, int?> _statusesReference;
        public Dictionary<string, int?> StatusesReference => _statusesReference;

        public static Dictionary<string, string> Statuses => new Dictionary<string, string>
        {
            { "DRAFT", "Черновик" },
            { "VALIDATING", "На проверке" },
            { "VALIDATING_ERROR", "Не прошел проверку" },
            { "CREATED", "Проверен и отправлен в службу доставки" },
            { "DELIVERY_PROCESSING_STARTED", "Создаётся в сортировочном центре" },
            { "DELIVERY_TRACK_RECEIVED", "В службе доставки" },
            { "SENDER_WAIT_FULFILLMENT", "Ожидается поступление на склад" },
            { "SORTING_CENTER_LOADED", "Заказ создан" },
            { "DELIVERY_LOADED", "Подтвержден" },
            { "DELIVERY_AT_START", "В сортировочном центрe" },
            { "DELIVERY_TRANSPORTATION", "Доставляется" },
            { "DELIVERY_ARRIVED", "В населенном пункте получателя" },
            { "DELIVERY_TRANSPORTATION_RECIPIENT", "Доставляется по населенному пункту получателя" },
            { "DELIVERY_STORAGE_PERIOD_EXTENDED", "Срок хранения увеличен" },
            { "DELIVERY_STORAGE_PERIOD_EXPIRED", "Срок хранения истек" },
            { "DELIVERY_UPDATED", "Доставка перенесена по вине магазина" },
            { "DELIVERY_UPDATED_BY_RECIPIENT", "Доставка перенесена по просьбе клиента" },
            { "DELIVERY_UPDATED_BY_DELIVERY", "Доставка перенесена службой доставки" },
            { "DELIVERY_ARRIVED_PICKUP_POINT", "В пункте самовывоза" },
            { "DELIVERY_AT_START_SORT", "Заказ прибыл в регион доставки" },
            { "DELIVERY_TRANSMITTED_TO_RECIPIENT", "Вручен клиенту" },
            { "DELIVERY_DELIVERED", "Доставлен получателю" },
            { "DELIVERY_ATTEMPT_FAILED", "Неудачная попытка вручения заказа" },
            { "DELIVERY_CAN_NOT_BE_COMPLETED", "Не может быть доставлен" },
            { "RETURN_PREPARING", "Готовится к возврату" },
            { "SORTING_CENTER_AT_START", "Поступил на склад сортировочного центра" },
            { "SORTING_CENTER_TRANSMITTED", "Заказ отгружен сортировочным центром в службу доставки" },
            { "SORTING_CENTER_PREPARED", "Заказ на складе сортировочного центра подготовлен к отправке в службу доставки" },
            { "SORTING_CENTER_RETURN_PREPARING", "Сортировочный центр получил данные о планируемом возврате заказа" },
            { "SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT", "Возвратный заказ поступил на склад сортировочного центра" },
            { "SORTING_CENTER_RETURN_ARRIVED", "Возвратный заказ на складе сортировочного центра" },
            { "SORTING_CENTER_RETURN_PREPARING_SENDER", "Возвратный заказ готов для передачи магазину" },
            { "SORTING_CENTER_RETURN_TRANSFERRED", "Возвратный заказ передан на доставку в магазин" },
            { "SORTING_CENTER_RETURN_RETURNED", "Заказ возвращен" },
            { "SORTING_CENTER_CANCELED", "Сортировочный центр: заказ отменен" },
            { "SORTING_CENTER_ERROR", "Ошибка создания заказа в сортировочном центре" },
            { "CANCELLED", "Заказ отменен" },
            { "CANCELED_IN_PLATFORM", "Заказ отменен в логистической платформе" },
        };

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement => true;

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var requestId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameYandexRequestIdInOrderAdditionalData);

            if (string.IsNullOrEmpty(requestId))
                return null;

            var movementResult = _yandexDeliveryApi.GetHistoryOfStatuses(requestId);
            if (movementResult?.StateHistory?.Count > 0)
                return movementResult.StateHistory
                    .Select(x => new HistoryOfMovement
                    {
                        Name = x.Status,
                        Comment = x.Description,
                        Date = x.Timestamp.ToDateTimeFromUnixTime()
                    })
                    .OrderByDescending(x => x.Date)
                    .ToList();
            return null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        public YandexDeliveryApiService YandexDeliveryApiService => _yandexDeliveryApi;

        public int? PaymentCodCardId => _paymentCodCardId;

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var shippingOptions = new List<BaseShippingOption>();

            if (_totalPrice > 250000)
                return shippingOptions;

            if (_typeDeparturePoint == TypeDeparturePoint.Station && _stationId.IsNullOrEmpty())
                return shippingOptions;
            else if (_typeDeparturePoint == TypeDeparturePoint.Dropoff && _receptionPoint.IsNullOrEmpty())
                return shippingOptions;

            var deliveryPoint = string.Join(",", new[] { _calculationParameters.City, _calculationParameters.Region });
            if (deliveryPoint.IsNullOrEmpty())
                return shippingOptions;

            var calcPickPoint = calculationVariants.HasFlag(CalculationVariants.PickPoint) && (_deliveryTypes.Contains(DeliveryType.PVZ) || _deliveryTypes.Contains(DeliveryType.Postamat));
            var calcCourier = calculationVariants.HasFlag(CalculationVariants.Courier) && _deliveryTypes.Contains(DeliveryType.Courier);
            var weight = GetTotalWeight();
            //Вес заказа не должен превышать 200 кг
            if (weight > 200)
                return shippingOptions;

            //Вес заказа в пвз не должен превышать 30 кг
            if (weight > 30)
            {
                if (!calcCourier)
                    return shippingOptions;
                calcPickPoint = false;
            }

            var dimensions = GetDimensions();
            //Сумма длин всех сторон товара не должна превышать 500 см. При этом длина одной стороны — не более 300 см.
            if (dimensions.Sum() > 5000 || dimensions.Any(x => x > 3000))
                return shippingOptions;

            //Для пвз сумма длин всех сторон товара не должна превышать 300 см. При этом длина одной стороны — не более 150 см.
            if (calcPickPoint && (dimensions.Sum() > 3000 || dimensions.Any(x => x > 1500)))
            {
                if (!calcCourier)
                    return shippingOptions;
                calcPickPoint = false;
            }

            var geoId = GetGeoId(_calculationParameters.City, _calculationParameters.Region);
            if (!geoId.HasValue)
                return shippingOptions;

            var orderCostInKop = (int)Math.Ceiling(_totalPrice * 100);
            var weightInGrams = (int)Math.Ceiling(weight * 1000);

            if (calcPickPoint)
                shippingOptions.AddRange(GetOptionsWithPoints(geoId.Value, orderCostInKop, weightInGrams, dimensions, deliveryPoint));
            if (calcCourier)
            {
                var courierOption = GetOptionCourier(orderCostInKop, weightInGrams, dimensions);
                if (courierOption != null)
                    shippingOptions.Add(courierOption);
            }

            return shippingOptions;
        }

        private List<BaseShippingOption> GetOptionsWithPoints(int geoId, int orderCostInKop, int weightInGrams, float[] dimensions, string deliveryPoint)
        {
            var shippingOptions = new List<BaseShippingOption>();
            var points = GetPickPoints(geoId, weightInGrams, dimensions);
            if (points == null || points.Count == 0)
                return shippingOptions;

            string selectedPointId = null;

            if (_calculationParameters.ShippingOption?.ShippingType == ((ShippingKeyAttribute)typeof(YandexDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                if (_calculationParameters.ShippingOption.GetType() == typeof(YandexDeliveryMapOption))
                    selectedPointId = ((YandexDeliveryMapOption)_calculationParameters.ShippingOption).PickpointId;

                if (_calculationParameters.ShippingOption.GetType() == typeof(YandexDeliveryWidgetOption))
                    selectedPointId = ((YandexDeliveryWidgetOption)_calculationParameters.ShippingOption).PickpointId;

                if (_calculationParameters.ShippingOption.GetType() == typeof(YandexDeliveryOption) && ((YandexDeliveryOption)_calculationParameters.ShippingOption).SelectedPoint != null)
                    selectedPointId = ((YandexDeliveryOption)_calculationParameters.ShippingOption).SelectedPoint.Id;
            }

            var selectedPoint = selectedPointId.IsNotEmpty()
               ? points.FirstOrDefault(x => x.Id.Equals(selectedPointId, StringComparison.OrdinalIgnoreCase))
               : null;

            if (selectedPoint == null)
            {
                selectedPoint = points[0];
                selectedPointId = selectedPoint.Id;
            }

            var destination = new Location();
            if (selectedPointId.IsNotEmpty())
                destination.PlatformStationId = selectedPointId;
            else
                destination.Address = deliveryPoint;

            var deliveryTime = GetDeliveryTime(pickupId: selectedPointId);
            if (deliveryTime == null)
                return shippingOptions;

            var calculateParams = new CalculateParams
            {
                ClientPrice = orderCostInKop,
                TotalAssessedPrice = orderCostInKop,
                Destination = destination,
                Tariff = TariffType.SelfPickup,
                TotalWeight = weightInGrams,
                PaymentMethod = PaymentMethodType.CashOnReceipt,
                Places = new List<CalculatePlace>
                {
                    new CalculatePlace
                    {
                        PhysicalDims = new PhysicalDims
                        {
                            WeightGross = weightInGrams,
                            Dx = (int)Math.Ceiling(dimensions[0] / 10),
                            Dz = (int)Math.Ceiling(dimensions[1] / 10),
                            Dy = (int)Math.Ceiling(dimensions[2] / 10),
                        }
                    }
                }
            };

            var cashOnDeliveryCashAvailable = true;
            var calculateResponse = _yandexDeliveryApi.Calculate(calculateParams);

            if (calculateResponse == null)
            {
                cashOnDeliveryCashAvailable = false;
                calculateParams.PaymentMethod = PaymentMethodType.AlreadyPaid;
                calculateResponse = _yandexDeliveryApi.Calculate(calculateParams);
            }
            if (calculateResponse == null || calculateResponse.Error)
            {
                if (!string.IsNullOrEmpty(calculateResponse?.Message))
                    Debug.Log.Warn("YandexDelivery: " + calculateResponse?.Message);
                return shippingOptions;
            }

            bool cashOnDeliveryCardAvailable = true;
            calculateParams.PaymentMethod = PaymentMethodType.CardOnReceipt;
            var calculateResponseWithCard = _yandexDeliveryApi.Calculate(calculateParams);
            if (calculateResponseWithCard == null || calculateResponse.Error)
                cashOnDeliveryCardAvailable = false;

            float rateCash = GetRateFromString(calculateResponse.PricingTotal);
            float rate = rateCash - GetRateFromString(calculateResponse.PricingCommissionOnDeliveryPaymentAmount);

            var deliveryTimeStr = deliveryTime != null
                ? deliveryTime.Item1 == deliveryTime.Item2
                    ? $"{deliveryTime.Item1} - {deliveryTime.Item2 + 1} дн."
                    : $"{deliveryTime.Item1} - {deliveryTime.Item2} дн."
                : null;

            var calculateOption = new YandexDeliveryCalculateOption { IsCourier = false };
            var isAvailablePaymentCashOnDelivery =
                selectedPoint != null
                    ? selectedPoint.AvailableCardOnDelivery is true || selectedPoint.AvailableCashOnDelivery is true
                    : points.Any(x => x.AvailableCardOnDelivery is true || x.AvailableCashOnDelivery is true);
            var endMethodName = string.Empty;
            var typeViewPoints = _typeViewPoints;
            if (typeViewPoints == TypeViewPoints.YaWidget)
            {
                if (points.Any(x => x.IsPostOffice))
                {
                    endMethodName = LocalizationService.GetResource("Core.Services.Shipping.FromRussianPost");
                    typeViewPoints = _postOfficeTypeViewPoints;
                    points = points.Where(x => x.IsPostOffice).ToList();
                }
                var option = new YandexDeliveryWidgetOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = rateCash,
                    IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                    CurrentPoints = points,
                    SelectedPoint = selectedPoint,
                    CalculateOption = calculateOption,
                    DeliveryTime = deliveryTimeStr,
                    CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                    PaymentCodCardId = _paymentCodCardId,
                    CashOnDeliveryCashAvailable = cashOnDeliveryCashAvailable
                };

                SetWidjetConfig(option, _calculationParameters.City);
                shippingOptions.Add(option);
            }
            if (typeViewPoints == TypeViewPoints.List)
                shippingOptions.Add(new YandexDeliveryOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, endMethodName),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = rateCash,
                    DeliveryTime = deliveryTimeStr,
                    IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                    CalculateOption = calculateOption,
                    HideAddressBlock = points != null,
                    ShippingPoints = points,
                    SelectedPoint = selectedPoint,
                    CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                    PaymentCodCardId = _paymentCodCardId,
                    CashOnDeliveryCashAvailable = cashOnDeliveryCashAvailable
                });
            else if (typeViewPoints == TypeViewPoints.YaMap && _yaMapsApiKey.IsNotEmpty())
            {
                var option = new YandexDeliveryMapOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, endMethodName),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = rateCash,
                    IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                    CurrentPoints = points,
                    CalculateOption = calculateOption,
                    DeliveryTime = deliveryTimeStr,
                    CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                    PaymentCodCardId = _paymentCodCardId,
                    CashOnDeliveryCashAvailable = cashOnDeliveryCashAvailable
                };
                SetMapData(option, deliveryPoint, geoId, weightInGrams, dimensions);
                shippingOptions.Add(option);
            }
            return shippingOptions;
        }


        private BaseShippingOption GetOptionCourier(int orderCost, int weight, float[] dimensions)
        {
            if (_calculationParameters.City.IsNullOrEmpty() || _calculationParameters.Street.IsNullOrEmpty() || _calculationParameters.House.IsNullOrEmpty() && SettingsCheckout.IsShowFullAddress)
                return new BaseShippingOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                    ErrorMessage = LocalizationService.GetResource("Core.Services.Shipping.Yandex.IncompleteAddress"),
                    ZeroPriceMessage = LocalizationService.GetResource("Core.Services.Shipping.Yandex.IncompleteAddress")
                };

            var fullAddress = string.Join(", ", new string[4] { _calculationParameters.City, _calculationParameters.Region, _calculationParameters.Street, _calculationParameters.House }.Where(x => x.IsNotEmpty()));
            var intervals = _yandexDeliveryApi.GetDeliveryInterval(new DeliveryIntervalParams { FullAddress = fullAddress });
            if (intervals == null || intervals.Count == 0)
                return new BaseShippingOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                    ErrorMessage = LocalizationService.GetResource("Core.Services.Shipping.Yandex.IntervalsNotAvailable"),
                    ZeroPriceMessage = LocalizationService.GetResource("Core.Services.Shipping.Yandex.IntervalsNotAvailable")
                };

            var calculateParams = new CalculateParams
            {
                ClientPrice = orderCost,
                TotalAssessedPrice = orderCost,
                Destination = new Location { Address = fullAddress },
                Tariff = TariffType.TimeInterval,
                TotalWeight = weight,
                PaymentMethod = PaymentMethodType.CashOnReceipt,
                Places = new List<CalculatePlace>
                {
                    new CalculatePlace
                    {
                        PhysicalDims = new PhysicalDims
                        {
                            WeightGross = weight,
                            Dx = (int)Math.Ceiling(dimensions[0] / 10),
                            Dz = (int)Math.Ceiling(dimensions[1] / 10),
                            Dy = (int)Math.Ceiling(dimensions[2] / 10),
                        }
                    }
                }
            };

            var cashOnDeliveryCashAvailable = true;
            var calculateResponse = _yandexDeliveryApi.Calculate(calculateParams);

            if (calculateResponse == null)
            {
                cashOnDeliveryCashAvailable = false;
                calculateParams.PaymentMethod = PaymentMethodType.AlreadyPaid;
                calculateResponse = _yandexDeliveryApi.Calculate(calculateParams);
            }
            if (calculateResponse == null || calculateResponse.Error)
            {
                if (!string.IsNullOrEmpty(calculateResponse?.Message))
                    Debug.Log.Warn("YandexDelivery: " + calculateResponse?.Message);
                return null;
            }

            bool cashOnDeliveryCardAvailable = true;
            calculateParams.PaymentMethod = PaymentMethodType.CardOnReceipt;
            var calculateResponseWithCard = _yandexDeliveryApi.Calculate(calculateParams);
            if (calculateResponseWithCard == null || calculateResponse.Error)
                cashOnDeliveryCardAvailable = false;

            float rateCash = GetRateFromString(calculateResponse.PricingTotal);
            float rate = rateCash - GetRateFromString(calculateResponse.PricingCommissionOnDeliveryPaymentAmount);

            if (intervals?.Count > 0)
            {
                var timesOfDelivery = GetTimesOfDelivery(intervals);
                if (timesOfDelivery.Count == 0)
                    return null;
                var timeOfDelivery = timesOfDelivery.FirstOrDefault();
                return new YandexDeliveryOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = rateCash,
                    IsAvailablePaymentCashOnDelivery = true,
                    CalculateOption = new YandexDeliveryCalculateOption { IsCourier = true },
                    Intervals = intervals,
                    SelectedInterval = intervals.FirstOrDefault(),
                    CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                    PaymentCodCardId = _paymentCodCardId,
                    TimesOfDelivery = timesOfDelivery,
                    FormattedDateOfDelivery = timeOfDelivery.Key,
                    CashOnDeliveryCashAvailable = cashOnDeliveryCashAvailable
                };
            }

            return null;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
        {
            bool allowPickPoint = _deliveryTypes.Contains(DeliveryType.PVZ);
            bool allowPostamats = _deliveryTypes.Contains(DeliveryType.Postamat);
            if (!(allowPostamats || allowPickPoint))
                return null;

            var weight = GetTotalWeight();
            //Вес заказа не должен превышать 30 кг
            if (weight > 30)
                return null;
            var dimensions = GetDimensions();
            var dimSum = dimensions.Sum();
            //Сумма длин всех сторон товара не должна превышать 300 см. При этом длина одной стороны — не более 110 см.
            if (dimSum > 3000 || dimensions.Any(x => x > 1100))
                return null;
            var weightInGrams = (int)Math.Ceiling(weight * 1000);

            //Для постаматов - Масса не превышает 30 кг, Габариты — не более 40×38×40 см, Сумма длин всех сторон — до 118 см
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();
            allowPostamats = _deliveryTypes.Contains(DeliveryType.Postamat) && dimSum <= 1180 && dimensionsValidate[0] <= 400 && dimensionsValidate[1] <= 400 && dimensionsValidate[2] <= 380;
            if (!(allowPostamats || allowPickPoint))
                return null;

            var deliveryTime = GetDeliveryTime(pickupId: pointId);
            if (deliveryTime == null)
                return null;

            var pickPoints = _yandexDeliveryApi.GetPickPoints(new PickPointParams { PickupPointIds = new List<string> { pointId } });
            if (!(pickPoints?.Points?.Count > 0 
                && (allowPostamats || pickPoints.Points[0].Type != PickPointType.Terminal) 
                && (allowPickPoint || pickPoints.Points[0].Type != PickPointType.PickupPoint || pickPoints.Points[0].Type != PickPointType.PostOffice)))
                return null;

            var point = CastPoint(pickPoints.Points[0]);

            var orderCostInKop = (int)Math.Ceiling(_totalPrice * 100);

            var calculateParams = new CalculateParams
            {
                ClientPrice = orderCostInKop,
                TotalAssessedPrice = orderCostInKop,
                Destination = new Location { PlatformStationId = pointId },
                Tariff = TariffType.SelfPickup,
                TotalWeight = weightInGrams,
                PaymentMethod = PaymentMethodType.CashOnReceipt,
                Places = new List<CalculatePlace>
                {
                    new CalculatePlace
                    {
                        PhysicalDims = new PhysicalDims
                        {
                            WeightGross = weightInGrams,
                            Dx = (int)Math.Ceiling(dimensions[0] / 10),
                            Dz = (int)Math.Ceiling(dimensions[1] / 10),
                            Dy = (int)Math.Ceiling(dimensions[2] / 10),
                        }
                    }
                }
            };

            var calculateResponse = _yandexDeliveryApi.Calculate(calculateParams);
            if (calculateResponse == null || calculateResponse.Error)
            {
                if (!string.IsNullOrEmpty(calculateResponse?.Message))
                    Debug.Log.Warn("YandexDelivery: " + calculateResponse?.Message);
                return null;
            }

            float rateCash = GetRateFromString(calculateResponse.PricingTotal);
            float rate = rateCash - GetRateFromString(calculateResponse.PricingCommissionOnDeliveryPaymentAmount);

            var deliveryTimeStr = deliveryTime != null
                ? deliveryTime.Item1 == deliveryTime.Item2
                    ? $"{deliveryTime.Item1} - {deliveryTime.Item2 + 1} дн."
                    : $"{deliveryTime.Item1} - {deliveryTime.Item2} дн."
                : null;

            var calculateOption = new YandexDeliveryCalculateOption { IsCourier = false };
            var isAvailablePaymentCashOnDelivery =
                point != null
                    ? point.AvailableCardOnDelivery is true || point.AvailableCashOnDelivery is true
                    : false;
            var shippingOption = new YandexDeliveryOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                Rate = rate,
                BasePrice = rate,
                PriceCash = rateCash,
                DeliveryTime = deliveryTimeStr,
                IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                CalculateOption = calculateOption,
                ShippingPoints = new List<YandexDeliveryShippingPoint> { point },
                SelectedPoint = point
            };

            return new[] { shippingOption };
        }

        private void SetMapData(YandexDeliveryMapOption option, string deliveryPoint, int geoId, int weightInGrams, float[] dimensions)
        {
            string lang = "en_US";
            switch (Culture.Language)
            {
                case Culture.SupportLanguage.Russian:
                    lang = "ru_RU";
                    break;
                case Culture.SupportLanguage.English:
                    lang = "en_US";
                    break;
                case Culture.SupportLanguage.Ukrainian:
                    lang = "uk_UA";
                    break;
            }
            option.MapParams = new PointDelivery.MapParams();
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = deliveryPoint;

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "geoId", geoId },
                    { "weightInGrams", weightInGrams },
                    { "dimensions", string.Join("x", dimensions.Select(x => x.ToInvariantString())) },
                };
            else
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null
                || !data.ContainsKey("geoId")
                || !data.ContainsKey("weightInGrams")
                || !data.ContainsKey("dimensions"))
                return null;
            var weight = data["weightInGrams"].ToString().TryParseInt();
            var dimensions = data["dimensions"].ToString().Split('x').Select(x => x.TryParseFloat()).ToArray();
            var geoId = data["geoId"].ToString().TryParseInt();
            var deliveryPoints = GetPickPoints(geoId, weight, dimensions);

            return GetFeatureCollection(deliveryPoints);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<YandexDeliveryShippingPoint> points)
        {
            return new PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                {
                    var intId = p.Id.GetHashCode();
                    return new PointDelivery.Feature
                    {
                        Id = intId,
                        Geometry = new PointDelivery.PointGeometry {PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f},
                        Options = new PointDelivery.PointOptions {Preset = "islands#dotIcon"},
                        Properties = new PointDelivery.PointProperties
                        {
                            BalloonContentHeader = p.Address,
                            HintContent = p.Address,
                            BalloonContentBody =
                                string.Format(
                                    "{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}')\">Выбрать</a>",
                                    string.Join("<br />", new[]
                                    {
                                        p.TimeWorkStr,
                                        !(p.AvailableCashOnDelivery is true && p.AvailableCardOnDelivery is true)
                                            ? "Оплата при получении недоступна"
                                            : p.AvailableCashOnDelivery is true
                                                ? "Способы оплаты: наличные" + (p.AvailableCardOnDelivery is true
                                                    ? ", банковские карты"
                                                    : String.Empty)
                                                : "Способы оплаты: банковские карты"
                                    }.Where(x => x.IsNotEmpty())),
                                    "<br />",
                                    intId,
                                    p.Id),
                            BalloonContentFooter = p.Description
                        }
                    };
                }).ToList()
            };
        }

        private Tuple<int, int> GetDeliveryTime(string fullAddress = null, int? geoId = null, string pickupId = null)
        {
            DeliveryIntervalParams deliveryIntervalParams = null;

            if (!pickupId.IsNullOrEmpty())
                deliveryIntervalParams = new DeliveryIntervalParams { SelfPickupId = pickupId };
            else if (!fullAddress.IsNullOrEmpty())
                deliveryIntervalParams = new DeliveryIntervalParams { FullAddress = fullAddress };
            else if (geoId.HasValue)
                deliveryIntervalParams = new DeliveryIntervalParams { GeoId = geoId };

            if (deliveryIntervalParams == null)
                return null;
            var intervals = _yandexDeliveryApi.GetDeliveryInterval(deliveryIntervalParams);
            if (intervals == null || intervals.Count == 0)
                return null;
            var interval = intervals.FirstOrDefault();
            var now = DateTime.UtcNow.Date;
            var min = interval.From.TryParseDateTime() - now;
            var max = interval.To.TryParseDateTime() - now;
            return new Tuple<int, int>(min.Days + _method.ExtraDeliveryTime, max.Days + _method.ExtraDeliveryTime);
        }


        public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude,
            float topLeftLongitude, float bottomRightLatitude, float bottomRightLongitude)
        {
            bool allowPickPoint = _deliveryTypes.Contains(DeliveryType.PVZ);
            bool allowPostamats = _deliveryTypes.Contains(DeliveryType.Postamat);
            if (!(allowPostamats || allowPickPoint))
                return null;

            var weight = GetTotalWeight();
            //Вес заказа не должен превышать 30 кг
            if (weight > 30)
                return null;
            var dimensions = GetDimensions();
            var dimSum = dimensions.Sum();
            //Сумма длин всех сторон товара не должна превышать 300 см. При этом длина одной стороны — не более 110 см.
            if (dimSum > 3000 || dimensions.Any(x => x > 1100))
                return null;
            var weightInGrams = (int)Math.Ceiling(weight * 1000);

            //Для постаматов - Масса не превышает 30 кг, Габариты — не более 40×38×40 см, Сумма длин всех сторон — до 118 см
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();
            allowPostamats = _deliveryTypes.Contains(DeliveryType.Postamat) && dimSum <= 1180 && dimensionsValidate[0] <= 400 && dimensionsValidate[1] <= 400 && dimensionsValidate[2] <= 380;
            if (!(allowPostamats || allowPickPoint))
                return null;

            var pickPoints = allowPostamats && !allowPickPoint 
                ? _yandexDeliveryApi.GetPickPoints(new PickPointParams { Type = PickPointType.Terminal }) 
                : _yandexDeliveryApi.GetPickPoints();

            return pickPoints?.Points?
                .Where(point => point.Position != null)
                // не сортировочный центр
                .Where(x => x.Type != PickPointType.SortingCenter)
                // если постамат, то проверяем можно ли в него
                .Where(x => x.Type != PickPointType.Terminal || allowPostamats)
                .Where(point => topLeftLatitude > point.Position.Latitude
                    && topLeftLongitude < point.Position.Longitude
                    && bottomRightLatitude < point.Position.Latitude
                    && bottomRightLongitude > point.Position.Longitude)
                .Select(CastPoint);
        }

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            var pickPoints = _yandexDeliveryApi.GetPickPoints(new PickPointParams { PickupPointIds = new List<string> { pointId } });
            return pickPoints?.Points?.Count > 0 ? CastPoint(pickPoints.Points.FirstOrDefault()) : null;
        }

        private YandexDeliveryShippingPoint CastPoint(PickupPoint point)
        {
            return new YandexDeliveryShippingPoint
            {
                Id = point.Id,
                Code = point.Id,
                Name = point.Name,
                Address = point.Address.FullAddress,
                Description = point.Instruction ?? point.Address.Comment,
                Phones = new[] { point.Contact.Phone },
                Longitude = point.Position.Longitude,
                Latitude = point.Position.Latitude,
                AvailableCashOnDelivery = point.PaymentMethods.Contains(PaymentMethodType.CashOnReceipt),
                AvailableCardOnDelivery = point.PaymentMethods.Any(x => x == PaymentMethodType.CardOnReceipt || x == PaymentMethodType.Cashless),
                TimeWorkStr = GetTimeWork(point.Schedule.Restrictions),
                MaxWeightInGrams = 30000,
                DimensionVolumeInCentimeters = point.Type == PickPointType.Terminal ? 118 : 300,
                MaxHeightInMillimeters = point.Type == PickPointType.Terminal ? 400 : 110,
                MaxLengthInMillimeters = point.Type == PickPointType.Terminal ? 380 : 110,
                MaxWidthInMillimeters = point.Type == PickPointType.Terminal ? 400 : 110,
                IsPostOffice = point.IsPostOffice,
            };
        }

        private int? GetGeoId(string city, string region)
        {
            var geoIds = _yandexDeliveryApi.GetCityGeoId(city);
            if (!(geoIds?.Variants?.Count > 0))
                return null;

            //Если по городу больше 1 варианта, то запрашиваем еще и по региону, пример город Заречный
            //Не запрашиваем всегда по городу и региону, потому что нужно точное совпадение(Яндекс рекомендовал запрашивать по городу)
            //Пример "Ульяновск, Ульяновская область" не находит, но просто "Ульяновск" находит
            if (geoIds.Variants.Count > 1 && region.IsNotEmpty())
            {
                var geoIdsByCityAndRegion = _yandexDeliveryApi.GetCityGeoId($"{city} {region}");
                if (geoIdsByCityAndRegion?.Variants?.Count > 0 is true)
                    geoIds = geoIdsByCityAndRegion;
            }
            return geoIds.Variants.FirstOrDefault()?.GeoId;
        }

        public List<PickupPoint> GetPointsByGeoId(int geoId)
        {
            var pointsCacheKey = $"YandexDeliveryPoints-GeoId" + geoId;
            if (!CacheManager.TryGetValue(pointsCacheKey, out List<PickupPoint> points))
            {
                var getPickPoints = _yandexDeliveryApi.GetPickPoints(new PickPointParams { GeoId = geoId });
                if (getPickPoints?.Points?.Count > 0)
                {
                    points = getPickPoints.Points;
                    CacheManager.Insert(pointsCacheKey, points, 60 * 24);
                }
            }
            return points ?? new List<PickupPoint>();
        }

        private List<YandexDeliveryShippingPoint> GetPickPoints(int geoId, int weightInGrams, float[] dimensions)
        {
            var shippingPoint = new List<YandexDeliveryShippingPoint>();

            //Вес заказа не должен превышать 30 кг
            if (weightInGrams > 30000)
                return shippingPoint;
            //Сумма длин всех сторон товара не должна превышать 300 см. При этом длина одной стороны — не более 110 см.
            var dimSum = dimensions.Sum();
            if (dimSum > 3000 || dimensions.Any(x => x > 1100))
                return shippingPoint;
            //Для постаматов - Масса не превышает 30 кг, Габариты — не более 40×38×40 см, Сумма длин всех сторон — до 118 см
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();
            bool allowPostamats = _deliveryTypes.Contains(DeliveryType.Postamat) && dimSum <= 1180 && dimensionsValidate[0] <= 400 && dimensionsValidate[1] <= 400 && dimensionsValidate[2] <= 380;
            bool allowPickPoint = _deliveryTypes.Contains(DeliveryType.PVZ);
            if (!(allowPostamats || allowPickPoint))
                return null;

            var points = new List<PickupPoint>();
            
            points.AddRange(GetPointsByGeoId(geoId));

            if (points.Count > 0)
            {
                shippingPoint = points
                    // не сортировочный центр
                    .Where(x => x.Type != PickPointType.SortingCenter)
                    // если постамат, то проверяем можно ли в него
                    .Where(x => x.Type != PickPointType.Terminal || allowPostamats)
                    .Select(CastPoint)
                    .ToList();
            }
            return shippingPoint;
        }

        public Dictionary<string, List<IntervalOffer>> GetTimesOfDelivery(List<IntervalOffer> intervals)
        {
            var timesOfDelivery = new Dictionary<string, List<IntervalOffer>>();
            var minDate = DateTime.Now.AddDays(_method.ExtraDeliveryTime);
            foreach (var interval in intervals)
            {
                var date = interval.From.TryParseDateTime();
                if (date.Date < minDate.Date)
                    continue;
                var dateStr = Culture.ConvertShortDate(date);
                if (timesOfDelivery.ContainsKey(dateStr))
                    timesOfDelivery[dateStr].Add(interval);
                else
                    timesOfDelivery.Add(dateStr, new List<IntervalOffer> { interval });
            }
            return timesOfDelivery;
        }

        private float GetRateFromString(string formattedRate)
        {
            if (formattedRate.IsNullOrEmpty())
                return 0f;
            var match = Regex.Match(formattedRate, @"(\d+(?:\.\d+)?)\D*$");
            var rate = match.Groups[1].Value;

            return rate.TryParseFloat();
        }

        private string GetTimeWork(List<Restriction> restrictions, string separator = ", ")
        {
            if (restrictions?.Count > 0)
            {
                var schedule = new Dictionary<string, List<int>>();
                
                foreach (var restriction in restrictions)
                {
                    var workTime = GetWorkTime(restriction.TimeFrom, restriction.TimeTo);
                    if (schedule.ContainsKey(workTime))
                        schedule[workTime].AddRange(restriction.Days);
                    else
                        schedule.Add(workTime, restriction.Days);
                }
                return String.Join(separator, 
                    schedule.Select(x => string.Format("{0}: {1}",
                        GetWorkDays(x.Value.Distinct().ToList()),
                        x.Key))
                    );
            }
            return null;
        }

        private string GetWorkDays(List<int> days)
        {
            var strDays = new List<string>();
            int i = 0;
            bool newDaysInterval = true; // новая последовательность дней
            int firstDay = 0;
            while (i < days.Count)
            {
                if (newDaysInterval)
                    firstDay = days[i];
                if (i + 1 !=  days.Count && days[i] == days[i + 1] - 1) // если не последний день в списке и дни идут друг за другом
                {
                    newDaysInterval = false;
                    i++;
                    continue;
                }
                else if (firstDay == days[i]) // последовательность прервалась или прошлись по всем дням 
                    strDays.Add(GetWeekDay(firstDay));
                else
                    strDays.Add($"{GetWeekDay(firstDay)} - {GetWeekDay(days[i])}");
                newDaysInterval = true;
                i++;
            }
            return String.Join(", ", strDays);
        }

        private string GetWorkTime(WorkTime from, WorkTime to)
        {
            return string.Format("{0}:{1}-{2}:{3}",
                        from.Hours,
                        (from.Minutes < 10) ? "0" + from.Minutes : from.Minutes.ToString(),
                        to.Hours,
                        (to.Minutes < 10) ? "0" + to.Minutes : to.Minutes.ToString());
        }

        private string GetWeekDay(int dayOfWeek)
        {
            var dofw = dayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)dayOfWeek;
            return _currentCulture.DateTimeFormat.GetShortestDayName(dofw);
        }

        private void SetWidjetConfig(YandexDeliveryWidgetOption option, string city)
        {
            option.WidgetConfigParams = new Dictionary<string, object>();

            option.WidgetConfigParams.Add("city", city);
            option.WidgetConfigParams.Add("deliveryPVZ", _deliveryTypes.Contains(DeliveryType.PVZ));
            option.WidgetConfigParams.Add("deliveryPostamat", _deliveryTypes.Contains(DeliveryType.Postamat));
        }
    }

    public enum TypeViewPoints
    {
        [Localize("Через Яндекс.Карты")]
        YaMap = 0,

        [Localize("Через виджет Яндекс")]
        YaWidget = 1,

        [Localize("Списком")]
        List = 2,
    }

    public enum TypeDeparturePoint
    {
        [Localize("Станция, заведенная в Яндекс.Доставке")]
        Station = 0,

        [Localize("Пункт самопривоза")]
        Dropoff = 1
    }
}
