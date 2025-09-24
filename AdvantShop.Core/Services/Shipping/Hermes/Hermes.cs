using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Shipping.Hermes.Api;

namespace AdvantShop.Shipping.Hermes
{
    [ShippingKey("Hermes")]
    public class Hermes : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingSyncOfOrderStatus, IShippingSupportingPaymentCashOnDelivery
    {
        private readonly string _securedToken;
        private readonly string _publicToken;
        private readonly string _parcelShopLocation;
        private readonly string _distributionCenterLocation;
        private readonly bool _locationIsDistributionCenter;
        private readonly string _businessUnitCode;
        private readonly bool _withInsure;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly bool _statusesSync;

        public const string KeyNameBarcodeInOrderAdditionalData = "HermesBarcodeOrder";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public string SecuredToken { get { return _securedToken; } }
        public string PublicToken { get { return _publicToken; } }

        public Hermes(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _securedToken = _method.Params.ElementOrDefault(HermesTemplate.SecuredToken);
            _publicToken = _method.Params.ElementOrDefault(HermesTemplate.PublicToken);
            _businessUnitCode = _method.Params.ElementOrDefault(HermesTemplate.BusinessUnitCode);
            _parcelShopLocation = _method.Params.ElementOrDefault(HermesTemplate.ParcelShopLocation);
            _distributionCenterLocation = _method.Params.ElementOrDefault(HermesTemplate.DistributionCenterLocation);
            _locationIsDistributionCenter = _method.Params.ElementOrDefault(HermesTemplate.LocationIsDistributionCenter).TryParseBool();
            _withInsure = method.Params.ElementOrDefault(HermesTemplate.WithInsure).TryParseBool();
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(HermesTemplate.TypeViewPoints).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(HermesTemplate.YaMapsApiKey);
            _statusesSync = method.Params.ElementOrDefault(HermesTemplate.StatusesSync).TryParseBool();

            var statusesReference = method.Params.ElementOrDefault(HermesTemplate.StatusesReference);
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
        }

        #region IShippingSupportingSyncOfOrderStatus

        public void SyncStatusOfOrder(Order order)
        {
            var barcode = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameBarcodeInOrderAdditionalData);
            if (barcode.IsNotEmpty())
            {
                var client = new RestApiClient(_securedToken, _publicToken);
                var getStatusesParcel = client.GetStatusesParcel(barcode);
                if (getStatusesParcel != null && getStatusesParcel.IsSuccess == true)
                {
                    var statusInfo = 
                        getStatusesParcel.Statuses
                            .Where(x => StatusesReference.ContainsKey(x.StatusSystemName) 
                                        && StatusesReference[x.StatusSystemName].HasValue)
                            .OrderByDescending(x => x.Current)
                            .ThenByDescending(x => x.StatusTimestamp)
                            .FirstOrDefault();

                    var hermesOrderStatus = statusInfo != null && StatusesReference.ContainsKey(statusInfo.StatusSystemName)
                        ? StatusesReference[statusInfo.StatusSystemName]
                        : null;

                    if (hermesOrderStatus.HasValue &&
                        order.OrderStatusId != hermesOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(hermesOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date)
                                .FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < statusInfo.StatusTimestamp)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                hermesOrderStatus.Value, "Синхронизация статусов для Hermes");
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
        public Dictionary<string, int?> StatusesReference => _statusesReference;

        public static Dictionary<string,string> Statuses => new Dictionary<string, string>
        {
            { "NEW", "Новая" },
            { "ARRIVED_AT_TERMINAL_PICKUP", "Доставлена на терминал (для доставки)" },
            { "OUT_FOR_DELIVERY", "Отправлена в пункт выдачи" },
            { "DELIVERED_TO_DESTINATION", "Доставлена в пункт выдачи" },
            { "ARRIVED_AT_PARCEL_SHOP", "Принята в пункте выдачи" },
            { "RECEIVED", "Выдана" },
            { "NON_PICKED", "Просрочена" },
            { "REFUSED_BY_CUSTOMER", "Отказ" },
            { "UNDELIVERED", "Отправлена на терминал (возврат)" },
            { "ARRIVED_AT_TERMINAL_PICKUP_RETURN_WAY", "Доставлена на терминал (возврат)" },
            { "OUT_FOR_DELIVERY_RETURN_WAY", "Отправлена поставщику" },
            { "DELIVERED_TO_DESTINATION_RETURN_WAY", "Получена поставщиком" },
            { "MISROUTED", "Ошибочно доставлена в пункт выдачи" },
            { "MISDIRECTED", "Перенаправлена в другой пункт выдачи" },
            { "ON_ROAD_BETWEEN_TERMINALS", "В пути между терминалами" },
            { "ARRIVED_AT_TERMINAL_TRANSIT", "Доставлена на транзитный терминал" },
            { "ARRIVED_AT_TERMINAL_DELIVERY", "Доставлена на терминал доставки" },
            { "DELIVERY_NOT_DONE", "Доставка не состоялась" },
            { "LOST_DELIVERY", "Потеряна при доставке" },
            { "PROBLEM_DELIVERY", "Проблемы при доставке" },
            { "RETURNED_FROM_DELIVERY", "Возвращена с маршрута" },
            { "ON_ROAD_BETWEEN_TERMINALS_RETURN_WAY", "В пути между терминалами (возврат)" },
            { "ARRIVED_AT_TERMINAL_TRANSIT_RETURN_WAY", "Доставлена на транзитный терминал (возврат)" },
            { "ARRIVED_AT_TERMINAL_DELIVERY_RETURN_WAY", "Доставлена на терминал доставки (возврат)" },
            { "DELIVERY_NOT_DONE_RETURN_WAY", "Доставка не состоялась (возврат)" },
            { "LOST_DELIVERY_RETURN_WAY", "Потеряна при доставке (возврат)" },
            { "PROBLEM_DELIVERY_RETURN_WAY", "Проблемы при доставке (возврат)" },
            { "RETURNED_FROM_DELIVERY_RETURN_WAY", "Возвращена с маршрута (возврат)" },
            { "RETURN_CONFIRMED", "Возврат поставщику подтвержден" },
            { "CUSTOMER_RETURN_RECEIVED", "Принята в пункте выдачи (клиентский возврат)" },
            { "PICKED_UP_BY_COURIER_CUSTOMER_RETURN", "Отправлена на терминал (клиентский возврат)" },
            { "ARRIVED_AT_TERMINAL_PICKUP_CUSTOMER_RETURN", "Доставлена на терминал (клиентский возврат)" },
            { "ON_ROAD_BETWEEN_TERMINALS_CUSTOMER_RETURN", "В пути между терминалами (клиентский возврат)" },
            { "ARRIVED_AT_TERMINAL_TRANSIT_CUSTOMER_RETURN", "Доставлена на транзитный терминал (клиентский возврат)" },
            { "ARRIVED_AT_TERMINAL_DELIVERY_CUSTOMER_RETURN", "Доставлена на терминал доставки (клиентский возврат)" },
            { "OUT_FOR_DELIVERY_CUSTOMER_RETURN", "Отправлена поставщику (клиентский возврат)" },
            { "DELIVERY_NOT_DONE_CUSTOMER_RETURN", "Доставка не состоялась (клиентский возврат)" },
            { "LOST_DELIVERY_CUSTOMER_RETURN", "Потеряна при доставке (клиентский возврат)" },
            { "PROBLEM_DELIVERY_CUSTOMER_RETURN", "Проблемы при доставке (клиентский возврат)" },
            { "RETURNED_FROM_DELIVERY_CUSTOMER_RETURN", "Возвращена с маршрута (клиентский возврат)" },
            { "DELIVERED_TO_DESTINATION_CUSTOMER_RETURN", "Получена поставщиком (клиентский возврат)" },
            { "CUSTOMER_RETURN_CONFIRMED", "Возврат поставщику подтвержден (клиентский возврат)" },
            { "INVENTORIED", "Инвентаризирована" },
            { "MISSING", "Потеряна" },
            { "LOST", "Окончательно потеряна" },
        };

    #endregion

    #region IShippingSupportingTheHistoryOfMovement

    public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var barcode = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameBarcodeInOrderAdditionalData);
            if (barcode.IsNotEmpty())
            {
                var client = new RestApiClient(_securedToken, _publicToken);
                var getStatusesParcel = client.GetStatusesParcel(barcode);
                if (getStatusesParcel != null && getStatusesParcel.IsSuccess == true)
                {
                    return getStatusesParcel.Statuses
                        .OrderByDescending(x => x.StatusTimestamp)
                        .Select(status => new HistoryOfMovement()
                        {
                            Code = status.StatusSystemName,
                            Name = status.StatusName,
                            Date = status.StatusTimestamp.HasValue ? status.StatusTimestamp.Value : default(DateTime),
                            Comment = status.StorageDateTime.HasValue ? "Хранение до " + status.StorageDateTime.Value.ToString(Configuration.SettingsMain.ShortDateFormat) : string.Empty
                        }).ToList();
                }
            }
            return null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var shippingOptions = new List<BaseShippingOption>();

            var city = _calculationParameters.City;
            var district = _calculationParameters.District;
            var region = _calculationParameters.Region;
            var country = _calculationParameters.Country;

            if (city.IsNotEmpty() && calculationVariants.HasFlag(CalculationVariants.PickPoint))
            {
                var orderCost = _totalPrice;
                int weight = (int)GetTotalWeight(1000);
                var dimensions = GetDimensionsHermesInCentimeters();

                var deliveryPoints = GetPointsCity(region, city, weight, dimensions);
                if (deliveryPoints != null && deliveryPoints.Count != 0)
                {
                    string selectedPoint = null;
                    ParcelShop deliveryPoint = null;

                    var businessUnit = GetBusinessUnit(_businessUnitCode);
                    if (businessUnit == null)
                        return shippingOptions;

                    if (_locationIsDistributionCenter)
                    {
                        if (_calculationParameters.ShippingOption != null &&
                            _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Hermes).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                        {
                            if (_calculationParameters.ShippingOption.GetType() == typeof(HermesPointDeliveryMapOption))
                                selectedPoint = ((HermesPointDeliveryMapOption)_calculationParameters.ShippingOption).PickpointId;

                            if (_calculationParameters.ShippingOption.GetType() == typeof(HermesWidgetOption))
                                selectedPoint = ((HermesWidgetOption)_calculationParameters.ShippingOption).PickpointId;

                            if (_calculationParameters.ShippingOption.GetType() == typeof(HermesOption) && ((HermesOption)_calculationParameters.ShippingOption).SelectedPoint != null)
                                selectedPoint = ((HermesOption)_calculationParameters.ShippingOption).SelectedPoint.Id;
                        }

                        deliveryPoint = selectedPoint != null
                           ? deliveryPoints.FirstOrDefault(x => x.ParcelShopCode == selectedPoint) ?? deliveryPoints[0]
                           : deliveryPoints[0];
                    }
                    else
                    {
                        // услуга Drop self
                        selectedPoint = _parcelShopLocation;
                        deliveryPoint = selectedPoint != null
                           ? deliveryPoints.FirstOrDefault(x => x.ParcelShopCode == selectedPoint)
                           : null;
                    }

                    if (deliveryPoint == null)
                        return shippingOptions;

                    var calcResults = CalculateDelivery(orderCost, weight, dimensions, deliveryPoint, businessUnit);

                    //if (ValidateAndLogError(noCodResult))
                    if (ValidateResult(calcResults))
                    {
                        var rate = calcResults.CalculationResult.DeliveryPrice.Value + 
                            (_withInsure && calcResults.CalculationResult.InsurancePrice.HasValue
                                ? calcResults.CalculationResult.InsurancePrice.Value
                                : 0f);

                        var hermesCalculateOption = new HermesCalculateOption
                        {
                            BusinessUnitCode = _businessUnitCode,
                            SourcePointIsDistributionCenter = _locationIsDistributionCenter,
                            SourcePointCode = _locationIsDistributionCenter
                                    ? _distributionCenterLocation
                                    : _parcelShopLocation,
                            WithInsure = _withInsure,
                        };

                        if (_locationIsDistributionCenter)
                        {
                            List<HermesPoint> shippingPoints = CastPoints(deliveryPoints);

                            if (_typeViewPoints == TypeViewPoints.WidgetHermes ||
                                (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                            {
                                var option = new HermesWidgetOption(_method, _totalPrice)
                                {
                                    Rate = rate,
                                    BasePrice = rate,
                                    PriceCash = rate + (calcResults.CalculationResult.PaymentPrice ?? 0f),
                                    //DeliveryTime = deliveryTimeStr,
                                    IsAvailablePaymentCashOnDelivery = calcResults.CalculationResult.PaymentPrice.HasValue,
                                    CurrentPoints = shippingPoints,
                                    CalculateOption = hermesCalculateOption,
                                };
                                SetWidjetConfig(option, country, region, city, weight, dimensions, orderCost);

                                shippingOptions.Add(option);
                            }
                            else if (_typeViewPoints == TypeViewPoints.YaWidget)
                            {
                                var option = new HermesPointDeliveryMapOption(_method, _totalPrice)
                                {
                                    Rate = rate,
                                    BasePrice = rate,
                                    PriceCash = rate + (calcResults.CalculationResult.PaymentPrice ?? 0f),
                                    //DeliveryTime = deliveryTimeStr,
                                    IsAvailablePaymentCashOnDelivery = calcResults.CalculationResult.PaymentPrice.HasValue,
                                    CurrentPoints = shippingPoints,
                                    CalculateOption = hermesCalculateOption,
                                };
                                SetMapData(option, country, region, district, city, weight, dimensions);

                                shippingOptions.Add(option);
                            }
                            else if (_typeViewPoints == TypeViewPoints.List)
                            {
                                var option = new HermesOption(_method, _totalPrice)
                                {
                                    Rate = rate,
                                    BasePrice = rate,
                                    PriceCash = rate + (calcResults.CalculationResult.PaymentPrice ?? 0f),
                                    //DeliveryTime = deliveryTimeStr,
                                    IsAvailablePaymentCashOnDelivery = calcResults.CalculationResult.PaymentPrice.HasValue,
                                    ShippingPoints = shippingPoints.OrderBy(x => x.Address).ToList(),
                                    SelectedPoint = shippingPoints.FirstOrDefault(x => x.Id.Equals(deliveryPoint.ParcelShopCode, StringComparison.OrdinalIgnoreCase)),
                                    CalculateOption = hermesCalculateOption,
                                };

                                shippingOptions.Add(option);
                            }
                        } 
                        else
                        {
                            var option = new HermesDropOption(_method, _totalPrice)
                            {
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = rate + (calcResults.CalculationResult.PaymentPrice ?? 0f),
                                //DeliveryTime = deliveryTimeStr,
                                IsAvailablePaymentCashOnDelivery = calcResults.CalculationResult.PaymentPrice.HasValue,
                                PickpointId = deliveryPoint.ParcelShopCode,
                                PickpointAddress = deliveryPoint.Address,
                                CalculateOption = hermesCalculateOption,
                            };

                            shippingOptions.Add(option);
                        }
                    }
                }
            }

            return shippingOptions;
        }
        
        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            var points = GetAllPoints();

            if (points == null || points.Count == 0)
                return null;

            var hermesPoint = points.FirstOrDefault(x => x.ParcelShopCode == pointId);

            return hermesPoint != null
                ? CastPoint(hermesPoint)
                : null;
        }


        public int[] GetDimensionsHermesInCentimeters()
        {
            return GetDimensions(rate: 10).Select(x => (int)Math.Ceiling(x)).ToArray();
        }

        private bool ValidateResult(GetPriceCalculationByPointResponse result)
        {
            //if (result != null && result.ErrorCode.IsNotEmpty())
            //    Debug.Log.Warn(string.Format("Hermes ErrorCode: {0}, Message: {1}", result.ErrorCode, result.ErrorMessage));
            return result != null && result.IsSuccess == true && result.CalculationResult != null && result.CalculationResult.DeliveryPrice.HasValue;
        }

        private GetPriceCalculationByPointResponse CalculateDelivery(float orderCost, int weight, int[] dimensions, ParcelShop deliveryPoint, BusinessUnit businessUnit)
        {
            var result = new List<GetPriceCalculationByPointResponse>();

            var client = new RestApiClient(_securedToken, _publicToken);

            string product = "ParcelShopDelivery";

            // точка привоза совпадает с точкой выдачи
            if (!_locationIsDistributionCenter && _parcelShopLocation == deliveryPoint.ParcelShopCode)
            {
                product = "DropOffToTargetParcelShop";

                // если не доступна улуга Drop
                if (businessUnit.Services == null || !businessUnit.Services.Contains("DROP_OFF_TO_TARGET_PARCELSHOP")) 
                    return null;
            }

            var request = new GetPriceCalculationByPointParams
                {
                    RequestId = "Delivery",
                    Product = product,
                    BusinessUnitCode = _businessUnitCode,
                    SourcePoint = new TransitPoint
                    {
                        Type = _locationIsDistributionCenter ? "ClientDistributionCenter" : "Parcelshop",
                        Code = _locationIsDistributionCenter ? _distributionCenterLocation : _parcelShopLocation
                    },
                    DestinationPoint = new TransitPoint
                    {
                        Type = "Parcelshop",
                        Code = deliveryPoint.ParcelShopCode
                    },
                    InsuranceValue = orderCost,
                    CashOnDeliveryValue = orderCost,
                    ParcelMeasurements = new ParcelMeasurements
                    {
                        WeightInGrams = weight,
                        HeightInCentimeters = dimensions[2],
                        WidthInCentimeters = dimensions[1],
                        LengthInCentimeters = dimensions[0]
                    },
                };
            return client.GetPriceCalculationByPoint(request);
        }

        private void SetWidjetConfig(HermesWidgetOption option, string country, string region, string city, int weight, int[] dimensions, float orderCost)
        {
            option.WidgetConfigParams = new Dictionary<string, object>();

            option.WidgetConfigParams.Add("businessUnitCode", _businessUnitCode);
            option.WidgetConfigParams.Add("address", string.Join(", ", new[] { country, region, city }.Where(x => x.IsNotEmpty())));
            option.WidgetConfigParams.Add("city", city);
            option.WidgetConfigParams.Add("weight", weight / 1000);
            option.WidgetConfigParams.Add("dimensionSum", dimensions.Sum());
            option.WidgetConfigParams.Add("orderCost", orderCost);

        }

        private void SetMapData(HermesPointDeliveryMapOption option, string country, string region, string district, string city, int weight, int[] dimensions)
        {
            string lang = "en_US";
            switch (Localization.Culture.Language)
            {
                case Localization.Culture.SupportLanguage.Russian:
                    lang = "ru_RU";
                    break;
                case Localization.Culture.SupportLanguage.English:
                    lang = "en_US";
                    break;
                case Localization.Culture.SupportLanguage.Ukrainian:
                    lang = "uk_UA";
                    break;
            }
            option.MapParams = new PointDelivery.MapParams();
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = string.Join(", ", new[] { country, region, district, city }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "region", region },
                    { "city", city },
                    { "weight", weight },
                    { "dimensionsH", dimensions[2] },
                    { "dimensionsW", dimensions[1] },
                    { "dimensionsL", dimensions[0] },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public BusinessUnit GetBusinessUnit(string businessUnitCode)
        {
            List<BusinessUnit> businessUnits = null;
            var businessUnitsCacheKey = string.Format("Hermes-{0}-BusinessUnits", (_securedToken + _publicToken).GetHashCode());
            if (!CacheManager.TryGetValue(businessUnitsCacheKey, out businessUnits))
            {
                var client = new RestApiClient(_securedToken, _publicToken);
                var getBusinessUnits = client.GetBusinessUnits();
                if (getBusinessUnits != null && getBusinessUnits.IsSuccess == true)
                {
                    businessUnits = getBusinessUnits.BusinessUnits.ToList();
                    if (businessUnits != null)
                        CacheManager.Insert(businessUnitsCacheKey, businessUnits, 60 * 24);
                }
            }

            return businessUnits != null 
                ? businessUnits.FirstOrDefault(x => x.Code == businessUnitCode)
                : null;
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("region") || !data.ContainsKey("city")
                || !data.ContainsKey("weight") || !data.ContainsKey("dimensionsH") ||
                !data.ContainsKey("dimensionsW") || !data.ContainsKey("dimensionsL"))
                return null;

            var region = (string)data["region"];
            var city = (string)data["city"];
            var weight = data["weight"].ToString().TryParseInt();
            var dimensions = new int[]
            {
                data["dimensionsL"].ToString().TryParseInt(),
                data["dimensionsW"].ToString().TryParseInt(),
                data["dimensionsH"].ToString().TryParseInt(),
            };

            var deliveryPoints = GetPointsCity(region, city, weight, dimensions);
            var points = CastPoints(deliveryPoints);

            return GetFeatureCollection(points);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<HermesPoint> points)
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
                                    string.Join(" ", new[] { p.Name, p.TimeWorkStr }.Where(x => x.IsNotEmpty())),
                                    "<br>",
                                    intId,
                                    p.Id),
                            BalloonContentFooter = /*_showAddressComment
                                ?*/ p.Description
                            //: null
                        }
                    };
                }).ToList()
            };
        }

        public List<ParcelShop> GetAllPoints()
        {
            List<ParcelShop> points = null;
            var pointsCacheKey = string.Format("Hermes-{0}-ParcelShops", (_securedToken + _publicToken + _businessUnitCode).GetHashCode());
            if (!CacheManager.TryGetValue(pointsCacheKey, out points))
            {
                var client = new RestApiClient(_securedToken, _publicToken);
                var getParcelShops = client.GetParcelShops(_businessUnitCode);
                if (getParcelShops != null && getParcelShops.IsSuccess == true)
                {
                    points = getParcelShops.ParcelShops;
                    if (points != null)
                        CacheManager.Insert(pointsCacheKey, points, 60 * 24);
                }
            }

            return points ?? new List<ParcelShop>();
        }

        private List<ParcelShop> GetPointsCity(string region, string city, int weight, int[] dimensions)
        {
            if (city.IsNullOrEmpty())
                return null;

            List<ParcelShop> points = GetAllPoints();

            if (points != null && points.Count > 0)
            {
                weight = weight / 1000; // переводим в кг
                var regionFind = region.RemoveTypeFromRegion();

                var dimensionSum = dimensions.Sum();

                points = points
                    // есть координаты
                    .Where(x => x.Latitude.HasValue && x.Longitude.HasValue)
                    // нужный регион
                    .Where(x => regionFind.IsNullOrEmpty() || (x.Region.IsNotEmpty() && x.Region.IndexOf(regionFind, StringComparison.OrdinalIgnoreCase) != -1))
                    // нужный город
                    .Where(x => x.City.IsNotEmpty() && x.City.IndexOf(city, StringComparison.OrdinalIgnoreCase) != -1)
                    // проходит по весу
                    .Where(x => x.MaxParcelWeight.HasValue == false || weight <= x.MaxParcelWeight.Value)
                    // проходит по габаритам
                    .Where(x => x.MaxParcelOverallSize.HasValue == false || dimensionSum <= x.MaxParcelOverallSize.Value)
                    // работает на выдачу
                    .Where(x => x.Services != null && x.Services.Contains("HAND_OUT_IN_PARCEL_SHOP"))
                    .ToList();
            }

            return points;
        }

        private List<HermesPoint> CastPoints(List<ParcelShop> points)
        {
            return points
                  .Select(CastPoint)
                  .ToList();
        }

        private HermesPoint CastPoint(ParcelShop point)
        {
            return new HermesPoint
            {
                Id = point.ParcelShopCode,
                Code = point.ParcelShopCode,
                Name = $"{point.ParcelShopName} {point.ParcelShopCode}",
                Address = point.Address,
                Description = point.AddressInfo,
                TimeWorkStr = point.WorkTime,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
            };
        }
    }

    public enum TypeViewPoints
    {
        [Localize("Через виджет Hermes")]
        WidgetHermes = 0,

        [Localize("Через Яндекс.Карты")]
        YaWidget = 1,

        [Localize("Списком")]
        List = 2
    }
}
