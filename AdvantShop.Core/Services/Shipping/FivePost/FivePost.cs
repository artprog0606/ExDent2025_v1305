using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.FivePost.CalculateCost;
using AdvantShop.Core.Services.Shipping.FivePost.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping.FivePost.Api;
using AdvantShop.Shipping.FivePost.CalculateCost;
using AdvantShop.Shipping.FivePost.PickPoints;
using AdvantShop.Shipping.PointDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdvantShop.Shipping.FivePost
{
    [ShippingKey("FivePost")]
    public partial class FivePost : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery, IShippingWithBackgroundMaintenance, IShippingSupportingSyncOfOrderStatus
    {
        #region ctor

        private readonly string _apiKey;
        private readonly int? _paymentCodCardId;
        private readonly EFivePostBarcodeEnrichment _barcodeEnrichment;
        private readonly EFivePostUndeliverableOption _undeliverableOption;

        private readonly ETypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly string _widgetKey;
        private readonly bool _withInsure;
        private readonly List<int> _activeTarifs;
        private readonly Dictionary<string, int?> _warehouseDeliveryTypeReferences;
        private readonly Regex _timeWorkRegex = new Regex(@"^([^:]+): (\d{2}:\d{2}) - (\d{2}:\d{2})$", RegexOptions.Singleline);
        private readonly Dictionary<int, List<EFivePostDeliveryType>> _rateDeliverySLReference;

        private readonly FivePostApiService _apiService;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }
        public const string TrackNumberOrderAdditionalDataName = "FivePostTrackNumber";


        public FivePost(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _apiKey = _method.Params.ElementOrDefault(FivePostTemplate.ApiKey);
            _paymentCodCardId = _method.Params.ElementOrDefault(FivePostTemplate.PaymentCodCardId).TryParseInt(true);
            _yaMapsApiKey = _method.Params.ElementOrDefault(FivePostTemplate.YandexMapApiKey);
            _widgetKey = _method.Params.ElementOrDefault(FivePostTemplate.WidgetKey);
            _withInsure = _method.Params.ElementOrDefault(FivePostTemplate.WithInsure).TryParseBool();
            _typeViewPoints = (ETypeViewPoints)_method.Params.ElementOrDefault(FivePostTemplate.TypeViewPoints).TryParseInt();
            _barcodeEnrichment = (EFivePostBarcodeEnrichment)_method.Params.ElementOrDefault(FivePostTemplate.BarcodeEnrichment).TryParseInt();
            _undeliverableOption = (EFivePostUndeliverableOption)_method.Params.ElementOrDefault(FivePostTemplate.UndeliverableOption).TryParseInt();
            _statusesSync = _method.Params.ElementOrDefault(FivePostTemplate.StatusesSync).TryParseBool();
            var statusesReference = _method.Params.ElementOrDefault(FivePostTemplate.StatusesReference);
            if (statusesReference.IsNotEmpty())
            {
                string[] referenceTmp = null;
                _statusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (referenceTmp = x.Split(','))[0].TryParseInt(),
                            x => referenceTmp.Length > 1 ? referenceTmp[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<int, int?>();

            var warehouseDeliveryReference = _method.Params.ElementOrDefault(FivePostTemplate.WarehouseDeliveryTypeReferences);
            if (warehouseDeliveryReference.IsNotEmpty())
            {
                string[] referenceTmp = null;
                _warehouseDeliveryTypeReferences =
                    warehouseDeliveryReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(x => (referenceTmp = x.Split(','))[0],
                        x => referenceTmp.Length > 1 ? referenceTmp[1].TryParseInt(true) : null);
            }
            else
                _warehouseDeliveryTypeReferences = new Dictionary<string, int?>();

            _apiService = new FivePostApiService(_apiKey);

            var activeTarifs = _method.Params.ElementOrDefault(FivePostTemplate.ActiveTarifs);
            _activeTarifs = activeTarifs.IsNotEmpty()
                ? activeTarifs
                .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.TryParseInt())
                .ToList()
                : _apiService.GetRateList()?
                .Select(x => x.TypeCode)
                .ToList();

            var rateDeliverySLReferenceStr = _method.Params.ElementOrDefault(FivePostTemplate.RateDeliverySLReference);
            var rateDeliveryArr = new string[2];
            if (rateDeliverySLReferenceStr == null && _activeTarifs != null)
            {
                _rateDeliverySLReference = FivePostHelper.DefaultRateDeliveryReference.Where(x => _activeTarifs.Contains((int)x.Key)).ToDictionary(x => (int)x.Key, y => y.Value);
                method.Params.TryAddValue(FivePostTemplate.RateDeliverySLReference, string.Join(";", _rateDeliverySLReference.Select(x => x.Key + ":" + string.Join(",", x.Value.Select(val => ((int)val).ToString())))));
                if (method.ShippingMethodId != 0)
                    ShippingMethodService.UpdateShippingParams(method.ShippingMethodId, method.Params);
            }
            else if (rateDeliverySLReferenceStr != null)
                _rateDeliverySLReference = rateDeliverySLReferenceStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(
                        key => (rateDeliveryArr = key.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))[0].TryParseInt(),
                        value => rateDeliveryArr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(deliverySl => deliverySl.TryParseEnum<EFivePostDeliveryType>()).ToList());
        }

        #endregion

        #region public properties

        public FivePostApiService FivePostApiService => _apiService;
        public bool WithInsure => _withInsure;
        public EFivePostBarcodeEnrichment BarCodeEnrichment => _barcodeEnrichment;
        public EFivePostUndeliverableOption UndeliverableOption => _undeliverableOption;
        public int? PaymentCodCardId => _paymentCodCardId;

        #endregion

        #region IShippingSupportingSyncOfOrderStatus

        private readonly bool _statusesSync;
        public bool StatusesSync
        {
            get => _statusesSync;
        }
        public bool SyncByAllOrders => true;

        private Dictionary<int, int?> _statusesReference;
        public Dictionary<int, int?> StatusesReference
        {
            get => _statusesReference;
        }

        public void SyncStatusOfOrders(IEnumerable<Order> orders)
        {
            var syncStatusParams = new FivePostSyncStatusParams
            {
                OrderIds = orders.Select(x => x.OrderID).ToList()
            };
            var synchronizedStatusList = _apiService.SyncStatuses(syncStatusParams);

            foreach (var syncStatus in synchronizedStatusList)
            {
                var fivePostStatus = (int)syncStatus.Status;
                if (!StatusesReference.ContainsKey(fivePostStatus))
                    continue;

                var newStatus = StatusesReference[fivePostStatus].Value;
                OrderStatusService.ChangeOrderStatus(syncStatus.OrderId, newStatus, "Синхронизация статусов FivePost");
            }
        }

        public void SyncStatusOfOrder(Order order) => throw new NotImplementedException();

        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            List<BaseShippingOption> optionList = new List<BaseShippingOption>();

            if (calculationVariants.HasFlag(CalculationVariants.PickPoint) is false)
                return optionList;

            var weightInKg = GetTotalWeight();
            var dimensionsInMillimeters = GetDimensions()
                                         .OrderByDescending(x => x)
                                         .Select(x => (int) Math.Ceiling(x))
                                         .ToArray();

            var weightInMg = (long)(weightInKg * 1_000_000);
            var deliveryPoints = GetPickPoints(_calculationParameters.Region, _calculationParameters.City, dimensionsInMillimeters, weightInMg);

            if (deliveryPoints == null || deliveryPoints.Count == 0)
                return optionList;

            string selectedPointId = null;
            FivePostPoint deliveryPoint = null;

            if (_calculationParameters.ShippingOption != null &&
                        _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(FivePost).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                if (_calculationParameters.ShippingOption is FivePostPointOption pointOption)
                    selectedPointId = pointOption.SelectedPoint.Id;
                if (_calculationParameters.ShippingOption is FivePostPointMapOption mapOption)
                    selectedPointId = mapOption.PickpointId;
                if (_calculationParameters.ShippingOption is FivePostWidgetOption widgetOption)
                    selectedPointId = widgetOption.PickpointId;
            }

            deliveryPoint = selectedPointId != null
                        ? deliveryPoints.FirstOrDefault(x => x.Id == selectedPointId) ?? deliveryPoints[0]
                        : deliveryPoints[0];

            if (deliveryPoint == null)
                return optionList;

            var pickPoint = FivePostPickPointService.Get(deliveryPoint.Id);

            var tarifResult = GetTarif(pickPoint, weightInKg);
            if (tarifResult == null)
                return optionList;

            var calcParams = new FivePostCalculationParams
            {
                Rate = tarifResult.Rate,
                Weight = weightInKg,
                WithInsure = _withInsure,
                WarehouseId = tarifResult.WarehouseId,
                InsureValue = _totalPrice
            };
            var calculationResult = new FivePostCalculationHandler(calcParams).Execute();

            BaseShippingOption option = null;
            var deliveryTime = (tarifResult.PossibleDelivery.MaxDeliveryDays > tarifResult.PossibleDelivery.MinDeliveryDays
                                   ? tarifResult.PossibleDelivery.MinDeliveryDays + _method.ExtraDeliveryTime + "-"
                                   : string.Empty)
                               + (tarifResult.PossibleDelivery.MaxDeliveryDays + _method.ExtraDeliveryTime) + " дн.";

            var isAvailablePaymentCashOnDelivery = deliveryPoint.AvailableCashOnDelivery is true || deliveryPoint.AvailableCardOnDelivery is true;
            var cashOnDeliveryCardAvailable = deliveryPoint.AvailableCardOnDelivery is true;

            switch (_typeViewPoints)
            {
                case ETypeViewPoints.List:
                    option = new FivePostPointOption(_method, _totalPrice)
                    {
                        Name = $"{_method.Name}",
                        DeliveryId = (int)ETypeViewPoints.List,
                        DeliveryTime = deliveryTime,
                        Rate = calculationResult.DeliveryCost,
                        BasePrice = calculationResult.DeliveryCost,
                        PriceCash = calculationResult.DeliveryCostWithInsure,
                        ShippingPoints = deliveryPoints,
                        SelectedPoint = deliveryPoint,
                        CalculateOption = calcParams,
                        IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                        CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                        PaymentCodCardId = _paymentCodCardId
                    }; 
                    break;
                case ETypeViewPoints.YandexMap:
                    option = new FivePostPointMapOption(_method, _totalPrice)
                    {
                        Name = $"{_method.Name}",
                        DeliveryId = (int)ETypeViewPoints.YandexMap,
                        Rate = calculationResult.DeliveryCost,
                        BasePrice = calculationResult.DeliveryCost,
                        PriceCash = calculationResult.DeliveryCostWithInsure,
                        DeliveryTime = deliveryTime,
                        IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                        CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                        PaymentCodCardId = _paymentCodCardId,
                        CurrentPoints = deliveryPoints,
                        CalculateOption = calcParams
                    };

                    SetMapData(
                        (FivePostPointMapOption)option, 
                        _calculationParameters.Country,
                        _calculationParameters.Region, 
                        _calculationParameters.District, 
                        _calculationParameters.City,
                        weightInMg,
                        dimensionsInMillimeters);
                    break;
                //case ETypeViewPoints.FivePostWidget:
                //    option = new FivePostWidgetOption(_method, _totalPrice)
                //    {
                //        Name = $"{_method.Name}",
                //        Rate = calculationResult.DeliveryCost,
                //        BasePrice = calculationResult.DeliveryCost,
                //        PriceCash = calculationResult.DeliveryCostWithInsure,
                //        IsAvailablePaymentCashOnDelivery = isAvailablePaymentCashOnDelivery,
                //        CashOnDeliveryCardAvailable = cashOnDeliveryCardAvailable,
                //        PaymentCodCardId = _paymentCodCardId,
                //        CurrentPoints = deliveryPoints,
                //        CalculateOption = calcParams,
                //        DeliveryTime = deliveryTime,
                //        SelectedPoint = deliveryPoint
                //    };

                //    SetWidjetConfig(option as FivePostWidgetOption, _calculationParameters.City, _widgetKey, 
                //        _calculationParameters.Latitude, _calculationParameters.Longitude);
                //    break;
            }

            optionList.Add(option);

            return optionList;
        }

        public FeatureCollection GetFeatureCollection(List<FivePostPoint> points)
        {
            if (points == null)
                return null;

            return new FeatureCollection
            {
                Features = points.Select(p =>
                {
                    var intId = p.Id.GetHashCode();
                    return new Feature
                    {
                        Id = intId,
                        Geometry = new PointGeometry {PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f},
                        Options = new PointOptions {Preset = "islands#dotIcon"},
                        Properties = new PointProperties
                        {
                            BalloonContentHeader = p.Address,
                            HintContent = p.Address,
                            BalloonContentBody =
                                string.Format(
                                    "{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}')\">Выбрать</a>",
                                    p.Description,
                                    p.Description.IsNotEmpty() ? "<br>" : "",
                                    intId,
                                    p.Id),
                            BalloonContentFooter = p.Description
                        }
                    };
                }).ToList()
            };
        }

        private List<FivePostPoint> CastPickPoints(List<PickPoints.FivePostPickPoint> points)
        {
            return points
                .OrderBy(x => x.FullAddress)
                .OrderByDescending(x => x.RateList.Count)
                .OrderByDescending(x => x.PossibleDeliveryList.Count)
                .Select(CastPickPoint)
                .ToList();
        }

        private void SetMapData(FivePostPointMapOption option, string country, string region, string district, string city, long weightInMg, int[] dimensionsInMillimeters)
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
            option.MapParams = new MapParams();
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = string.Join(", ", new[] { country, region, district, city }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", city },
                    { "weight", weightInMg },
                    { "region", region },
                    { "dimensions", string.Join(" ", dimensionsInMillimeters) },
               };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        private void SetWidjetConfig(FivePostWidgetOption option, string city, string widgetKey, float? lat, float? lng)
        {
            option.WidgetConfigParams = new Dictionary<string, object>
            {
                { "city", city },
                { "widgetKey", widgetKey }
            };

            if (lat.HasValue && lng.HasValue)
                option.WidgetConfigParams.Add("mapCenter", new float[] {lat.Value, lng.Value});
        }

        #region IShippingLazyData

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("region") || !data.ContainsKey("city") || !data.ContainsKey("weight") || !data.ContainsKey("dimensions"))
                return null;

            var region = (string)data["region"];
            var city = (string)data["city"];
            var weightInMg = data["weight"].ToString().TryParseLong();
            var dimensionsInMillimeters = ((string)data["dimensions"])
                                .Split(" ")
                                .Select(x => x.TryParseInt())
                                .ToArray();

            var points = GetPickPoints(region, city, dimensionsInMillimeters, weightInMg);

            return GetFeatureCollection(points);
        }

        #endregion

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            if (_apiKey.IsNotEmpty())
                SyncPickPoints(_apiService);
        }

        public static void SyncPickPoints(FivePostApiService apiClient)
        {
            var syncSettingName = "FivePostLastDateSyncPickPoints";
            // общая настройка, т.к. справочники общие, не зависят от настроек
            var lastDateSync = SettingProvider.Items[syncSettingName].TryParseDateTime(true);

            try
            {
                var currentDateTime = DateTime.UtcNow;
                // 5пост рекомендует обновлять список пвз после 6 утра
                var after6Am = currentDateTime.TimeOfDay > TimeSpan.FromHours(6);

                if (lastDateSync is null || 
                    ((currentDateTime - lastDateSync.Value.ToUniversalTime() > TimeSpan.FromHours(23)) 
                        && after6Am))
                {
                    // пишем в начале импорта, чтобы, если запустят в паралель еще
                    // то не прошло по условию времени последнего запуска
                    SettingProvider.Items[syncSettingName] = currentDateTime.ToString("O");

                    FivePostPickPointService.Sync(apiClient);
                }
            }
            catch (Exception ex)
            {
                // возвращаем предыдущее заначение, чтобы при следующем запуске снова сработало
                SettingProvider.Items[syncSettingName] = lastDateSync.HasValue ? lastDateSync.Value.ToString("O") : null;
                Debug.Log.Warn(ex);
            }
        }

        #endregion

        #region LoadShippingPointInfo

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            var pickPoint = FivePostPickPointService.Get(pointId);
            return CastPickPoint(pickPoint);
        }

        public FivePostPoint CastPickPoint(PickPoints.FivePostPickPoint point)
        {
            return new FivePostPoint
            {
                Id = point.Id,
                Code = point.Id,
                Name = point.Name,
                Address = point.FullAddress,
                Description = point.Description,
                Longitude = point.Longitude,
                Latitude = point.Lattitude,
                AvailableCashOnDelivery = point.IsCash,
                AvailableCardOnDelivery = point.IsCard,
                TimeWorkStr = point.TimeWork,
                TimeWork = GetTimeWorksFromString(point.TimeWork),
                Phones = point.Phone.IsNotEmpty() ? new[] { point.Phone } : null,
                MaxWeightInGrams = MeasureUnits.ConvertWeight(point.WeightDimensionsLimit.MaxWeightInMilligrams, MeasureUnits.WeightUnit.Kilogramm, MeasureUnits.WeightUnit.Grams),
                MaxHeightInMillimeters = MeasureUnits.ConvertLength(point.WeightDimensionsLimit.MaxHeightInMillimeters, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter),
                MaxWidthInMillimeters = MeasureUnits.ConvertLength(point.WeightDimensionsLimit.MaxWidthInMillimeters, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter),
                MaxLengthInMillimeters = MeasureUnits.ConvertLength(point.WeightDimensionsLimit.MaxLengthInMillimeters, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter),
            };
        }

        public List<TimeWork> GetTimeWorksFromString(string selfDeliveryTimes)
        {
            var selfDeliveryTimesParts = selfDeliveryTimes.Split(new[] { "; " }, StringSplitOptions.None);
            if (selfDeliveryTimesParts.All(part => _timeWorkRegex.IsMatch(part)))
            {
                return
                    selfDeliveryTimesParts
                       .SelectMany(part =>
                       {
                           var match = _timeWorkRegex.Match(part);
                           return match.Groups[1].Value.Split(new[] { ", " }, StringSplitOptions.None)
                                .Select(x => new TimeWork
                                {
                                    Label = x,
                                    From = TimeSpan.Parse(match.Groups[2].Value),
                                    To = TimeSpan.Parse(match.Groups[3].Value),
                                });
                       })
                       .ToList();
            }
            else
                return null;
        }

        #endregion

        private List<FivePostPoint> GetPickPoints(string region, string city, int[] dimensionsInMillimeters, long weightInMg)
        {
            var weightDimensions = new FivePostWeightDimension
            {
                MaxWeightInMilligrams = weightInMg,
                MaxWidthInMillimeters = dimensionsInMillimeters[0],
                MaxHeightInMillimeters = dimensionsInMillimeters[1],
                MaxLengthInMillimeters = dimensionsInMillimeters[2]
            };

            return CastPickPoints(
                FivePostPickPointService.Find(region, city, weightDimensions));
        }

        public FivePostGetTarifResult GetTarif(PickPoints.FivePostPickPoint pickPoint, float weight)
        {
            if (pickPoint == null)
                return null;

            var getTarifParams = new FivePostGetTarifParams
            {
                Weight = weight,
                PossibleDeliveryList = pickPoint.PossibleDeliveryList,
                RateList = pickPoint.RateList,
                ActiveTarifTypes = _activeTarifs,
                WarehouseDeliveryTypeReference = _warehouseDeliveryTypeReferences,
                RateDeliverySLReference = _rateDeliverySLReference
            };

            return new FivePostGetTarifHandler(getTarifParams).Execute();
        }
    }
}
