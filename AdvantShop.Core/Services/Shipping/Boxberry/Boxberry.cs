//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.Boxberry;
using AdvantShop.Orders;
using System;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Repository;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.Boxberry
{
    [ShippingKey("Boxberry")]
    public partial class Boxberry : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingSupportingTheHistoryOfMovement, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery
    {
        #region Ctor

        private readonly string _apiUrl;
        private readonly string _token;
        private readonly string _integrationToken;
        private readonly string _receptionPointCode;
        private readonly bool _calculateCourierOld;
        private readonly bool _statusesSync;
        private readonly int _increaseDeliveryTime;
        private readonly TypeOption _typeOption;
        private readonly List<TypeDelivery> _deliveryTypes;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;

        private readonly BoxberryApiService _boxberryApiService;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Boxberry(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _apiUrl = _method.Params.ElementOrDefault(BoxberryTemplate.ApiUrl);
            _token = _method.Params.ElementOrDefault(BoxberryTemplate.Token);

            if (_apiUrl == null)
                _apiUrl = _token.IsNotEmpty() ? "http://api.boxberry.de/json.php" : "http://api.boxberry.ru/json.php";

            _integrationToken = _method.Params.ElementOrDefault(BoxberryTemplate.IntegrationToken);
            _receptionPointCode = _method.Params.ElementOrDefault(BoxberryTemplate.ReceptionPointCode);
            _statusesSync = method.Params.ElementOrDefault(BoxberryTemplate.StatusesSync).TryParseBool();
            _typeOption = (TypeOption)method.Params.ElementOrDefault(BoxberryTemplate.TypeOption).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(BoxberryTemplate.YaMapsApiKey);
            _withInsure = method.Params.ElementOrDefault(BoxberryTemplate.WithInsure).TryParseBool();
            _boxberryApiService = new BoxberryApiService(_apiUrl, _token, _receptionPointCode);
            _increaseDeliveryTime = _method.ExtraDeliveryTime;

            if (method.Params.ContainsKey(BoxberryTemplate.DeliveryTypes))
            {
                _deliveryTypes = (method.Params.ElementOrDefault(BoxberryTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).Cast<TypeDelivery>().ToList();
            }
            else
            {
                _calculateCourierOld = Convert.ToBoolean(_method.Params.ElementOrDefault(BoxberryTemplate.CalculateCourier));

                _deliveryTypes = Enum.GetValues(typeof(TypeDelivery)).Cast<TypeDelivery>().ToList();
                if (_calculateCourierOld == false)
                    _deliveryTypes.Remove(TypeDelivery.Courier);
            }

            var newStatusesReference = method.Params.ElementOrDefault(BoxberryTemplate.StatusesReference);
            if (newStatusesReference == null)
            {
                var oldStatusesReference = string.Join(";", OldStatusesReference.Where(x => x.Value.HasValue).Select(x => $"{x.Key},{x.Value}"));
                newStatusesReference = oldStatusesReference;
                method.Params.TryAddValue(BoxberryTemplate.StatusesReference, newStatusesReference);
                if (method.ShippingMethodId != 0)
                    ShippingMethodService.UpdateShippingParams(method.ShippingMethodId, method.Params);
            }
            if (!string.IsNullOrEmpty(newStatusesReference))
            {
                string[] arr = null;
                _statusesReference =
                    newStatusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (arr = x.Split(','))[0],
                            x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<string, int?>();

        }

        #endregion

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            // не используется ListStatusesFull, т.к. в нем возвращается время только до минут
            var statusInfoAnswer = _boxberryApiService.ListStatuses(order.TrackNumber.IsNotEmpty() ? order.TrackNumber : order.OrderID.ToString());

            if (statusInfoAnswer == null || statusInfoAnswer.Result == null || statusInfoAnswer.Result.Count <= 0)
                return;

            var statusInfo = 
                statusInfoAnswer.Result
                    .Where(x => StatusesReference.ContainsKey(x.Name) 
                                && StatusesReference[x.Name].HasValue)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
           
            var boxberryOrderStatus = statusInfo != null && StatusesReference.ContainsKey(statusInfo.Name)
                ? StatusesReference[statusInfo.Name]
                : null;

            if (boxberryOrderStatus.HasValue &&
                order.OrderStatusId != boxberryOrderStatus.Value &&
                OrderStatusService.GetOrderStatus(boxberryOrderStatus.Value) != null)
            {
                var lastOrderStatusHistory =
                    OrderStatusService.GetOrderStatusHistory(order.OrderID)
                        .OrderByDescending(item => item.Date)
                        .FirstOrDefault();

                if (lastOrderStatusHistory == null ||
                    lastOrderStatusHistory.Date < statusInfo.Date)
                {
                    OrderStatusService.ChangeOrderStatus(order.OrderID,
                        boxberryOrderStatus.Value, "Синхронизация статусов для Boxberry");
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

        private Dictionary<string, int?> _oldStatusesReference;
        public Dictionary<string, int?> OldStatusesReference
        {
            get
            {
                if (_oldStatusesReference == null)
                {
                    _oldStatusesReference = new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Загружен реестр ИМ", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Created).TryParseInt(true)},
                        { "Принято к доставке", _method.Params.ElementOrDefault(BoxberryTemplate.Status_AcceptedForDelivery).TryParseInt(true)},
                        { "Заказ передан на доставку", _method.Params.ElementOrDefault(BoxberryTemplate.Status_AcceptedForDelivery).TryParseInt(true)},
                        { "Отправлен на сортировочный терминал", _method.Params.ElementOrDefault(BoxberryTemplate.Status_SentToSorting).TryParseInt(true)},
                        { "Передано на сортировку", _method.Params.ElementOrDefault(BoxberryTemplate.Status_TransferredToSorting).TryParseInt(true)},
                        { "Отправлено в город назначения", _method.Params.ElementOrDefault(BoxberryTemplate.Status_SentToDestinationCity).TryParseInt(true)},
                        { "В пути в город получателя", _method.Params.ElementOrDefault(BoxberryTemplate.Status_SentToDestinationCity).TryParseInt(true)},
                        { "Передано на курьерскую доставку", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Courier).TryParseInt(true)},
                        { "Поступил в город для передачи курьеру", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Courier).TryParseInt(true)},
                        { "Поступило в пункт выдачи", _method.Params.ElementOrDefault(BoxberryTemplate.Status_PickupPoint).TryParseInt(true)},
                        { "Доступен к получению в Пункте выдачи", _method.Params.ElementOrDefault(BoxberryTemplate.Status_PickupPoint).TryParseInt(true)},
                        { "Выдано", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Delivered).TryParseInt(true)},
                        { "Успешно Выдан", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Delivered).TryParseInt(true)},
                        { "Готовится к возврату", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnPreparing).TryParseInt(true)},
                        // дубль { "Заказ передан на возврат в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnPreparing).TryParseInt(true)},
                        { "Отправлено в пункт приема", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnSentToReceivingPoint).TryParseInt(true)},
                        { "Заказ в пути в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnSentToReceivingPoint).TryParseInt(true)},
                        { "Возвращено в пункт приема", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnReturnedToReceivingPoint).TryParseInt(true)},
                        { "Возвращено с курьерской доставки", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnByCourier).TryParseInt(true)},
                        // дубль { "Заказ передан на возврат в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnByCourier).TryParseInt(true)},
                        { "Возвращено в ИМ", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnReturned).TryParseInt(true)},
                        { "Заказ возвращен в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnReturned).TryParseInt(true)},
                    };
                }
                return _oldStatusesReference;
            }
        }

        public static Dictionary<string, string> Statuses => new Dictionary<string, string>
        {
            { "Принято к доставке", "Принято к доставке" },
            { "Заказ передан на доставку", "Заказ передан на доставку" },
            { "Отправлен на сортировочный терминал", "Отправлен на сортировочный терминал"},
            { "Передано на сортировку", "Передано на сортировку" },
            { "Отправлено в город назначения", "Отправлено в город назначения" },
            { "В пути в город получателя", "В пути в город получателя" },
            { "Передано на курьерскую доставку", "Передано на курьерскую доставку" },
            { "Поступил в город для передачи курьеру", "Поступил в город для передачи курьеру" },
            { "Поступило в пункт выдачи", "Поступило в пункт выдачи" },
            { "Доступен к получению в Пункте выдачи", "Доступен к получению в Пункте выдачи" },
            { "Выдано", "Выдано" },
            { "Успешно Выдан", "Успешно Выдан" },
            { "Готовится к возврату", "Готовится к возврату" },
            { "Отправлено в пункт приема", "Отправлено в пункт приема" },
            { "Заказ в пути в Интернет-магазин", "Заказ в пути в Интернет-магазин" },
            { "Возвращено в пункт приема", "Возвращено в пункт приема" },
            { "Возвращено с курьерской доставки", "Возвращено с курьерской доставки"},
            { "Возвращено в ИМ", "Возвращено в ИМ" },
            { "Заказ возвращен в Интернет-магазин", "Заказ возвращен в Интернет-магазин"},
        };

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }
        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var statusInfoAnswer = _boxberryApiService.ListStatuses(order.TrackNumber.IsNotEmpty() ? order.TrackNumber : order.OrderID.ToString());

            if (statusInfoAnswer == null || statusInfoAnswer.Result == null || statusInfoAnswer.Result.Count <= 0)
                return null;

            return statusInfoAnswer.Result.OrderByDescending(x => x.Date).Select(statusInfo => new HistoryOfMovement()
            {
                Code = statusInfo.Name,
                Name = statusInfo.Name,
                Date = statusInfo.Date,
                Comment = statusInfo.Comment
            }).ToList();
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (_calculationParameters.City.IsNullOrEmpty())
                return null;
            
            var orderPrice = _totalPrice;
            float orderWeight = (int)GetTotalWeight(1000);// граммы, поэтому целое

            var boxberryOptions = _boxberryApiService.GetBoxberryOptions();
            var hideDeliveryTime = false;
            if (boxberryOptions != null && boxberryOptions.Result != null && boxberryOptions.Result.Settings3 != null)
                hideDeliveryTime = boxberryOptions.Result.Settings3.HideDeliveryDay == 1;

            var result = new List<BaseShippingOption>();

            string countryIso2 = null;
            if (_calculationParameters.Country.IsNotEmpty())
                countryIso2 = CountryService.GetIso2(_calculationParameters.Country);
            var countryCode = CountryService.Iso2ToIso3Number(countryIso2);

            if (!string.IsNullOrEmpty(_calculationParameters.City) && _deliveryTypes.Contains(TypeDelivery.PVZ) && calculationVariants.HasFlag(CalculationVariants.PickPoint))
            {
                var cities = _boxberryApiService.GetListCities(countryCode);
                if (cities != null)
                {
                    cities.ForEach(x => x.Name = x.Name.Replace("ё", "е"));
                    cities.ForEach(x => x.Region = x.Region?.RemoveTypeFromRegion());

                    var normolizeCityName = _calculationParameters.City.Replace("ё", "е");
                    var normolizeRegionDest = _calculationParameters.Region?.RemoveTypeFromRegion();
                    BoxberryCity boxberryCity =
                        FindCity(cities, normolizeCityName, _calculationParameters.District, normolizeRegionDest)// по всем данным
                        ?? FindCity(cities, normolizeCityName, null, normolizeRegionDest);// без района
                    //?? GetCity(cities, normolizeCityName, null, null); // без района и региона (ненадо, считает не втот город)

                    if (boxberryCity != null)
                    {
                        string selectedPoint = null;
                        if (_calculationParameters.ShippingOption != null &&
                            _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Boxberry).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                        {
                            if (_calculationParameters.ShippingOption.GetType() == typeof(BoxberryWidgetOption))
                                selectedPoint = ((BoxberryWidgetOption)_calculationParameters.ShippingOption).PickpointId;

                            if (_calculationParameters.ShippingOption.GetType() == typeof(BoxberryPointDeliveryMapOption))
                                selectedPoint = ((BoxberryPointDeliveryMapOption)_calculationParameters.ShippingOption).PickpointId;
                        }

                        if (_typeOption == TypeOption.WidgetBoxberry || (_typeOption == TypeOption.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                        {

                            var optionWidget = GetShippingWidgentOption(boxberryCity, selectedPoint, orderWeight, orderPrice, hideDeliveryTime);
                            if (optionWidget != null)
                            {
                                result.Add(optionWidget);
                            }
                        }

                        if (_typeOption == TypeOption.YaWidget && _yaMapsApiKey.IsNotEmpty())
                        {
                            var optionWidget = GetShippingYaWidgentOption(boxberryCity, selectedPoint, orderWeight, orderPrice, hideDeliveryTime);
                            if (optionWidget != null)
                            {
                                result.Add(optionWidget);
                            }
                        }
                    }
                }
            }

            if (_deliveryTypes.Contains(TypeDelivery.Courier) && calculationVariants.HasFlag(CalculationVariants.Courier))
            {
                var optionZip = GetShippingOptionZip(orderWeight, orderPrice, hideDeliveryTime, countryCode);
                if (optionZip != null)
                {
                    result.Add(optionZip);
                }
            }

            return result;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
        {
            if (_deliveryTypes.Contains(TypeDelivery.PVZ) is false)
                return null;
            
            var weight = GetTotalWeight();
          
            //до 15 кг
            // if (weight > 15f)
            //     return null;
   
            var dimensions = GetDimensions(rate: 10);

            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var boxberryPoint =
                _boxberryApiService.GetPointDescription(pointId);
            
            if (boxberryPoint is null)
                return null;
            
            // принимает нужный вес
            if (boxberryPoint.LoadLimit == null ? weight > 15f : weight > boxberryPoint.LoadLimit.Value)
                return null;
            
            var filterVolume = MeasureUnits.ConvertVolume(dimensions[0] * dimensions[1] * dimensions[2], MeasureUnits.VolumeUnit.Centimeter, MeasureUnits.VolumeUnit.Metre); // переводим в м3
            // принимает нужный объем
            if (boxberryPoint.VolumeLimit != null && filterVolume > boxberryPoint.VolumeLimit.Value)
                return null;
    
            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                boxberryPoint.Code,
                (int)(weight * 1000f), // переводим в граммы
                _withInsure ? _totalPrice : (float?)null,
                0,
                dimensions[0],
                dimensions[1],
                dimensions[2],
                "",
                null);
       
            if (deliveryCost == null)
                return null;
 
            var deliveryCostCash = boxberryPoint.PrepaidOrdersOnly is false
                ? _boxberryApiService.GetDeliveryCosts(
                    boxberryPoint.Code,
                    (int)(weight * 1000f), // переводим в граммы
                    _totalPrice,
                    0,
                    dimensions[0],
                    dimensions[1],
                    dimensions[2],
                    "",
                    _totalPrice)
                : null;
            
            var boxberryOptions = _boxberryApiService.GetBoxberryOptions();
            var hideDeliveryTime = false;
            if (boxberryOptions != null && boxberryOptions.Result != null && boxberryOptions.Result.Settings3 != null)
            {
                hideDeliveryTime = boxberryOptions.Result.Settings3.HideDeliveryDay == 1;
            }
      
            List<BoxberryPoint> shippingPoints = CastPoints(new List<BoxberryObjectPointDescription>(){boxberryPoint});
            
            var shippingOption = new BoxberryPointDeliveryMapOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                DeliveryTime =
                    hideDeliveryTime
                        ? string.Empty
                        : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                IsAvailablePaymentCashOnDelivery = deliveryCostCash != null,
                CurrentPoints = shippingPoints,
                SelectedPoint = shippingPoints[0],
                PickpointId = shippingPoints[0].Id
            };
            
            return new[] {shippingOption};
        }

        public static BoxberryCity FindCity(IEnumerable<BoxberryCity> cities, string city, string district, string region)
        {
            if (city.IsNotEmpty())
                cities = cities.Where(x => x.Name.Equals(city, StringComparison.OrdinalIgnoreCase));
            if (district.IsNotEmpty())
                cities = cities.Where(x => x.District.Contains(district, StringComparison.OrdinalIgnoreCase));
            if (region.IsNotEmpty())
                cities = cities.Where(x => region.Contains(x.Region, StringComparison.OrdinalIgnoreCase) || x.Region.Contains(region, StringComparison.OrdinalIgnoreCase));

            return cities.FirstOrDefault();
        }

        /*
         *boxberry.open(‘callback_function’,‘api_token’,‘custom_city’,’target_start’,’ordersum’,’weight’,’paysum’,’height’,’width’,’depth’)
         */
        private BoxberryWidgetOption GetShippingWidgentOption(BoxberryCity boxberryCity, string selectedPickpointId, float totalWeight, float totalPrice, bool hideDeliveryTime)
        {
            var dimensions = GetDimensions(rate: 10);
            
            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var points = FilterPoints(_boxberryApiService.GetListPoints(boxberryCity.Code), totalWeight, dimensions);
            if (points == null || points.Count == 0 ||
                (points != null && points.Count > 0 && !string.IsNullOrEmpty(points[0].Error)))
            {
                return null;
            }
            var point = selectedPickpointId != null
                ? points.FirstOrDefault(x => selectedPickpointId == x.Code) ?? points[0]
                : points[0];

            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  _withInsure ? totalPrice : (float?)null,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  null);

            if (deliveryCost == null)
                return null;

            var deliveryCostCash = point.OnlyPrepaidOrders != "Yes" 
                ? _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  totalPrice,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  totalPrice)
                : null;

            return new BoxberryWidgetOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                WidgetConfigData = new Dictionary<string, object>
                {
                    {"api_token", _integrationToken},
                    {"city", boxberryCity.Name }, // для проверки смены города через виджет
                    {"custom_city", string.Join(", ", new[] { _calculationParameters.Region, boxberryCity.Name }.Where(x => x.IsNotEmpty())) }, // для виджета
                    {"targetstart", _receptionPointCode},
                    {"ordersum", _withInsure ? totalPrice : 0f},
                    {"weight", totalWeight},
                    //{"paysum", 0f}, // реализованно на уровне BoxberryWidgetOption
                    {"height", dimensions[0]},
                    {"width", dimensions[1]},
                    {"depth", dimensions[2]}
                },
                DeliveryTime =
                    hideDeliveryTime
                        ? string.Empty
                        : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                HideAddressBlock = true,
                TotalOrderPrice = totalPrice,
                WithInsure = _withInsure,
                IsAvailablePaymentCashOnDelivery = deliveryCostCash != null
            };
        }

        private BoxberryPointDeliveryMapOption GetShippingYaWidgentOption(BoxberryCity boxberryCity, string selectedPickpointId, float totalWeight, float totalPrice, bool hideDeliveryTime)
        {
            var dimensions = GetDimensions(rate: 10);

            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var points = FilterPoints(_boxberryApiService.GetListPoints(boxberryCity.Code), totalWeight, dimensions);
            if (points == null || points.Count == 0 ||
                (points != null && points.Count > 0 && !string.IsNullOrEmpty(points[0].Error)))
            {
                return null;
            }
            var point = selectedPickpointId != null
                ? points.FirstOrDefault(x => selectedPickpointId == x.Code) ?? points[0]
                : points[0];

            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  _withInsure ? totalPrice : (float?)null,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  null);

            if (deliveryCost == null)
                return null;

            var deliveryCostCash = point.OnlyPrepaidOrders != "Yes" 
                ? _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  totalPrice,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  totalPrice)
                : null;

            List<BoxberryPoint> shippingPoints = CastPoints(points);

            var option = new BoxberryPointDeliveryMapOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                DeliveryTime =
                    hideDeliveryTime
                        ? string.Empty
                        : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                IsAvailablePaymentCashOnDelivery = deliveryCostCash != null,
                CurrentPoints = shippingPoints,
            };
            SetMapData(option, boxberryCity.Code, totalWeight, dimensions);

            return option;
        }

        private List<BoxberryObjectPoint> FilterPoints(List<BoxberryObjectPoint> points, float totalWeight, float[] dimensions)
        {
            if (points == null)
                return points;

            var filterWeight = MeasureUnits.ConvertWeight(totalWeight, MeasureUnits.WeightUnit.Grams, MeasureUnits.WeightUnit.Kilogramm);// переводим в кг
            var filterVolume = MeasureUnits.ConvertVolume(dimensions[0] * dimensions[1] * dimensions[2], MeasureUnits.VolumeUnit.Centimeter, MeasureUnits.VolumeUnit.Metre); // переводим в м3
            return points.Where(
                x =>
                (x.LoadLimit == null ? filterWeight <= 15f : filterWeight <= x.LoadLimit.Value) &&
                (x.VolumeLimit == null || filterVolume <= x.VolumeLimit.Value))
                .ToList();
        }

        private List<BoxberryPoint> CastPoints(List<BoxberryObjectPoint> points)
        {
            return points
                  .Select(CastPoint)
                  .ToList();
        }

        private List<BoxberryPoint> CastPoints(List<BoxberryObjectPointDescription> points)
        {
            return points
                  .Select(CastPoint)
                  .ToList();
        }

        private void SetMapData(BoxberryPointDeliveryMapOption option, string boxberryCityCode, float totalWeight, float[] dimensions)
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
            option.MapParams.Destination = string.Join(", ", new[] { _calculationParameters.Country, _calculationParameters.Region, _calculationParameters.District, _calculationParameters.City }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", boxberryCityCode },
                    { "weight", (int)totalWeight },
                    { "dimensions", string.Join("x", dimensions.Select(x => x.ToInvariantString())) },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null
                || !data.ContainsKey("weight") || data["weight"] == null
                || !data.ContainsKey("dimensions") || data["dimensions"] == null)
                return null;

            var city = (string)data["city"];
            var weight = data["weight"].ToString().TryParseInt();
            var dimensions = data["dimensions"].ToString().Split('x').Select(x => x.TryParseFloat()).ToArray();
            var points = CastPoints(FilterPoints(_boxberryApiService.GetListPoints(city), weight, dimensions));

            return GetFeatureCollection(points);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<BoxberryPoint> points)
        {
            return new PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                {
                    var intId = p.Id.GetHashCode();
                    return new PointDelivery.Feature
                    {
                        Id = intId,
                        Geometry = new PointDelivery.PointGeometry
                            {PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f},
                        Options = new PointDelivery.PointOptions {Preset = "islands#dotIcon"},
                        Properties = new PointDelivery.PointProperties
                        {
                            BalloonContentHeader = p.Address,
                            HintContent = p.Address,
                            BalloonContentBody =
                                string.Format(
                                    "{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}')\">Выбрать</a>",
                                    p.TimeWorkStr,
                                    p.TimeWorkStr.IsNotEmpty() ? "<br>" : "",
                                    intId,
                                    p.Id),
                            BalloonContentFooter = p.Description
                        }
                    };
                }).ToList()
            };
        }

        private BoxberryOption GetShippingOptionZip(float totalWeight, float totalPrice, bool hideDeliveryTime, string countryCode)
        {
            if (string.IsNullOrEmpty(_calculationParameters.Zip))
                return new BoxberryOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                    Rate = 0,
                    DisplayIndex = true,
                    HideAddressBlock = false,
                    ErrorMessage = LocalizationService.GetResource("Core.Services.Shipping.Boxberry.IndexNotSpecified"),
                    ZeroPriceMessage = null
                };

            if (!_boxberryApiService.ZipCheck(_calculationParameters.Zip, countryCode))
                return new BoxberryOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                    Rate = 0,
                    DisplayIndex = true,
                    HideAddressBlock = false,
                    ErrorMessage = LocalizationService.GetResource("Core.Services.Shipping.Boxberry.InvalidIndex"),
                    ZeroPriceMessage = null
                };

            //до 15 кг
            if (totalWeight > 15000)
                return null;

            var dimensions = GetDimensions(rate: 10);

            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var cities = _boxberryApiService.GetCourierListCities();
            var normolizeCityName = _calculationParameters.City.Replace("ё", "е");
            if (!cities.Any(item => item.City.Replace("ё", "е").Equals(normolizeCityName, StringComparison.OrdinalIgnoreCase)))
                return null;

            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                     string.Empty,
                     totalWeight,
                     _withInsure ? totalPrice : (float?)null,
                     0, dimensions[0], dimensions[1], dimensions[2],
                     _calculationParameters.Zip,
                     null);

            if (deliveryCost == null)
                return null;

            var deliveryCostCash = _boxberryApiService.GetDeliveryCosts(
                 string.Empty,
                 totalWeight,
                 totalPrice,
                 0, dimensions[0], dimensions[1], dimensions[2],
                 _calculationParameters.Zip,
                 totalPrice);

            var shippingOption = new BoxberryOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                DeliveryTime = hideDeliveryTime ? string.Empty : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                DisplayIndex = true,
                HideAddressBlock = false
            };
            return shippingOption;
        }

        public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude,
            float topLeftLongitude, float bottomRightLatitude, float bottomRightLongitude)
        {
            if (_deliveryTypes.Contains(TypeDelivery.PVZ) is false)
                return null;
            
            var weight = GetTotalWeight();
            var dimensions = GetDimensions(rate: 10);
            var filterVolume = (dimensions[0] * dimensions[1] * dimensions[2]) / 1000000d; // переводим в м3
     
            //до 15 кг
            if (weight > 15f)
                return null;

            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            return _boxberryApiService.GetListPoints(string.Empty)
                               .Where(point => point.LoadLimit == null || weight <= point.LoadLimit.Value)
                               .Where(point => point.VolumeLimit == null || filterVolume <= point.VolumeLimit.Value)
                               .Where(point => point.GPS.IsNotEmpty())
                               .Where(point =>
                                {
                                    var gps = point.GPS.Split(',');
                                    var latitude = gps[0].TryParseFloat();
                                    var longitude = gps[1].TryParseFloat();
                                    return topLeftLatitude > latitude
                                        && topLeftLongitude < longitude
                                        && bottomRightLatitude < latitude
                                        && bottomRightLongitude > longitude;
                                })
                               .Select(CastPoint);
        }

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            var boxberryPoint = _boxberryApiService.GetPointDescription(pointId);
            return boxberryPoint != null
                ? CastPoint(boxberryPoint)
                : null;
        }

        private BoxberryPoint CastPoint(BoxberryObjectPoint point)
        {
            var gps = point.GPS;
            var indexSplitChar = gps.IndexOf(',');
            var latitude = gps.Substring(0, indexSplitChar).TryParseFloat();
            var longitude = gps.Substring(indexSplitChar + 1).TryParseFloat();

            return new BoxberryPoint
            {
                Id = point.Code,
                Code = point.Code,
                Name = point.Name,
                Address = point.Address,
                Description = point.TripDescription,
                Phones = new[] {point.Phone},
                Longitude = longitude,
                Latitude = latitude,
                AvailableCashOnDelivery = point.OnlyPrepaidOrders != "Yes",
                AvailableCardOnDelivery = point.OnlyPrepaidOrders != "Yes" && point.Acquiring == "Yes",
                TimeWorkStr = point.WorkShedule,
                MaxWeightInGrams = point.LoadLimit != null
                    ? MeasureUnits.ConvertWeight(
                        point.LoadLimit.Value, 
                        MeasureUnits.WeightUnit.Kilogramm,
                        MeasureUnits.WeightUnit.Grams)
                    : (float?) null,
                DimensionVolumeInCentimeters = point.VolumeLimit != null
                    ? MeasureUnits.ConvertVolume(
                        point.VolumeLimit.Value, 
                        MeasureUnits.VolumeUnit.Metre,
                        MeasureUnits.VolumeUnit.Centimeter)
                    : (double?) null,
                // DeliveryPeriodInDay = point.DeliveryPeriod, todo
                OnlyPrepaidOrders = point.OnlyPrepaidOrders == "Yes",
            };
        }
        
        private BoxberryPoint CastPoint(BoxberryObjectPointDescription point)
        {
            var gps = point.GPS;
            var indexSplitChar = gps?.IndexOf(',') ?? -1;
            var latitude = indexSplitChar >= 0 ? gps?.Substring(0, indexSplitChar).TryParseFloat(true) : null;
            var longitude = indexSplitChar >= 0 ? gps?.Substring(indexSplitChar + 1).TryParseFloat(true) : null;

            return new BoxberryPoint
            {
                Id = point.Code,
                Code = point.Code,
                Name = point.Name,
                Address = point.Address,
                Description = point.TripDescription,
                Phones = point.Phone != null 
                    ? new[] {point.Phone}
                    : null,
                Longitude = longitude,
                Latitude = latitude,
                AvailableCashOnDelivery = !point.PrepaidOrdersOnly,
                AvailableCardOnDelivery = !point.PrepaidOrdersOnly && point.Acquiring,
                TimeWorkStr = point.WorkShedule,
                MaxWeightInGrams = point.LoadLimit != null
                    ? MeasureUnits.ConvertWeight(
                        point.LoadLimit.Value, 
                        MeasureUnits.WeightUnit.Kilogramm,
                        MeasureUnits.WeightUnit.Grams)
                    : (float?) null,
                DimensionVolumeInCentimeters = point.VolumeLimit != null
                    ? MeasureUnits.ConvertVolume(
                        point.VolumeLimit.Value, 
                        MeasureUnits.VolumeUnit.Metre,
                        MeasureUnits.VolumeUnit.Centimeter)
                    : (double?) null,
                OnlyPrepaidOrders = point.PrepaidOrdersOnly,
            };
        }

        #region ApiMethods

        public BoxberryOrderDeleteAnswer DeleteOrder(string trackNumber)
        {
            return _boxberryApiService.ParselDelete(trackNumber);
        }

        //protected override int GetHashForCache()
        //{
        //    var totalPrice = _items.Sum(item => item.Price * item.Amount);
        //    var str =
        //        string.Format(
        //            "checkout/calculation?ClientId={0}&CityDest={1}&RegionDest={2}&ZipDest={3}&totalSum={4}&assessedSum={5}&totalWeight={6}&itemsCount={7}&paymentMethod=cash",
        //            _token,
        //            _preOrder.CityDest,
        //            _preOrder.RegionDest,
        //            _preOrder.ZipDest,
        //            _preOrder.
        //            Math.Ceiling(totalPrice),
        //            Math.Ceiling(totalPrice),
        //            _items.Sum(item => item.Weight * item.Amount).ToString().Replace(",", "."),
        //            Math.Ceiling(_items.Sum(x => x.Amount)));
        //    var hash = _method.ShippingMethodId ^ str.GetHashCode();
        //    return hash;
        //}

        #endregion
    }

    public enum TypeOption
    {
        [Localize("Через виджет Boxberry")]
        WidgetBoxberry = 0,

        [Localize("Через Яндекс.Карты")]
        YaWidget = 1
    }

    public enum TypeDelivery
    {
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery")]
        PVZ = 0,

        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.Courier")]
        Courier = 1
    }
}