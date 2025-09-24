using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.Measoft.Api;

namespace AdvantShop.Shipping.Measoft
{
    [ShippingKey("Measoft")]
    public partial class Measoft : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery
    {
        private readonly MeasoftAuthOption _authOption;
        private readonly MeasoftApiService _apiService;
        private readonly bool _statusesSync;
        private readonly List<TypeDelivery> _deliveryTypes;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;
        private readonly int _extraDeliveryDays;
        private readonly List<int> _activeDeliveryServices;
        private readonly List<int> _activeStoreList;
        private readonly int? _paymentCodCardId;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }
        public const string TrackNumberOrderAdditionalDataName = "MeasoftTrackNumber";

        public Measoft(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(MeasoftTemplate.TypeViewPoints).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(MeasoftTemplate.YaMapsApiKey);
            _withInsure = _method.Params.ElementOrDefault(MeasoftTemplate.WithInsure).TryParseBool();
            _extraDeliveryDays = _method.ExtraDeliveryTime;
            _paymentCodCardId = _method.Params.ElementOrDefault(MeasoftTemplate.PaymentCodCardId).TryParseInt(true);
            _authOption = new MeasoftAuthOption
            {
                Login = method.Params.ElementOrDefault(MeasoftTemplate.Login),
                Password = method.Params.ElementOrDefault(MeasoftTemplate.Password),

                Extra = method.Params.ElementOrDefault(MeasoftTemplate.Extra)
            };
            var xmlConverter = new MeasoftXmlConverter(_authOption);
            _apiService = new MeasoftApiService(xmlConverter);
            _deliveryTypes = (method.Params.ElementOrDefault(MeasoftTemplate.DeliveryTypes) ?? string.Empty)
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (TypeDelivery)x.TryParseInt())
                .ToList();

            _statusesSync = method.Params.ElementOrDefault(MeasoftTemplate.StatusesSync).TryParseBool();
            var statusesReference = method.Params.ElementOrDefault(MeasoftTemplate.StatusesReference);
            if (!string.IsNullOrEmpty(statusesReference))
            {
                string[] arr = null;
                _statusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (arr = x.Split(','))[0].TryParseInt(),
                            x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<int, int?>();

            var activeDeliveryServices = method.Params.ElementOrDefault(MeasoftTemplate.ActiveDeliveryServices);
            _activeDeliveryServices = activeDeliveryServices.IsNotEmpty() 
                ? activeDeliveryServices.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.TryParseInt())
                .ToList()
                : MeasoftApiService.GetDeliveryServices(_authOption.Extra)
                .Select(x => x.Code)
                .ToList();
            var activeStoreList = method.Params.ElementOrDefault(MeasoftTemplate.ActiveStoreList);
            _activeStoreList = activeStoreList.IsNotEmpty()
                ? activeStoreList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.TryParseInt())
                .ToList()
                : _apiService.GetStoreList(_authOption.Extra)?
                .Select(x => x.Code)
                .ToList();
        }

        #region IShippingSupportingSyncOfOrderStatus

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

        public void SyncStatusOfOrder(Order order)
        {
            var trackNumber = OrderService.GetOrderAdditionalData(order.OrderID, TrackNumberOrderAdditionalDataName);
            var result = _apiService.SyncStatusOrder(trackNumber);

            if (string.IsNullOrEmpty(result.Error))
            {
                int measoftStatus = (int)result.Status;
                if (StatusesReference.ContainsKey(measoftStatus))
                {
                    int newStatus = StatusesReference[measoftStatus].Value;
                    OrderStatusService.ChangeOrderStatus(order.OrderID, newStatus, "Синхронизация статусов для Measoft");
                }
            }    
        }

        public void SyncStatusOfOrders(IEnumerable<Order> orders)
        {
            orders = orders.Where(x => x.ShippingMethodName == "Measoft");
            var orderStatusList = _apiService.SyncStatusOrderList();

            foreach (var order in orders)
            {
                var trackNumber = OrderService.GetOrderAdditionalData(order.OrderID, TrackNumberOrderAdditionalDataName);
                var orderStatus = orderStatusList.FirstOrDefault(x => x.OrderNo == trackNumber);
                if (orderStatus != null && string.IsNullOrEmpty(orderStatus.Error))
                {
                    int measoftStatus = (int)orderStatus.Status;
                    if (StatusesReference.ContainsKey(measoftStatus))
                    {
                        int newStatus = StatusesReference[measoftStatus].Value;
                        OrderStatusService.ChangeOrderStatus(order.OrderID, newStatus, "Синхронизация статусов для Measoft");
                    }
                }
            }         
        }

        #endregion Statuses

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var measoftCity = _apiService.GetCity(
                _calculationParameters.City, 
                _calculationParameters.District,
                _calculationParameters.Region, 
                _calculationParameters.Country);
            if (measoftCity == null)
                return null;

            List<BaseShippingOption> optionList = new List<BaseShippingOption>();
            float weight = GetTotalWeight();
            var dimensionsInSm = GetDimensions(rate: 10).Select(x => (int)Math.Ceiling(x)).ToArray();

            var items = _items.Select(x =>
                    new MeasoftItem
                    {
                        Price = x.Price,
                        Amount = x.Amount,
                        Weight = x.Weight,
                        Height = x.Height,
                        Length = x.Length,
                        Width = x.Width
                    }).ToList();

            if (_deliveryTypes.Contains(TypeDelivery.Courier) && calculationVariants.HasFlag(CalculationVariants.Courier))
            {
                MeasoftCalcOptionParams measoftCourierCalcOption = new MeasoftCalcOptionParams
                {
                    Weight = weight,
                    Dimensions = dimensionsInSm,
                    City = measoftCity.Name,
                    RegionCode = measoftCity.Region.Code,
                    WithInsure = _withInsure,
                    ExtraDeliveryDays = _extraDeliveryDays,
                    DeliveryServiceIds = _activeDeliveryServices
                };

                var options = _apiService.CalcOptions(measoftCourierCalcOption, items)
                    .Select(x => new MeasoftShippingOption(_method, _totalPrice)
                    {
                        Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                        Rate = x.BasePrice,
                        DeliveryId = x.DeliveryId,
                        DeliveryTime = x.DeliveryTime,
                        DateOfDelivery = x.MinDeliveryDate.AddDays(_extraDeliveryDays),
                        BasePrice = x.BasePrice,
                        PriceCash = x.PriceCash,
                        CalculateOption = x,
                        IsAvailablePaymentCashOnDelivery = true,
                        CashOnDeliveryCardAvailable = true,
                        PaymentCodCardId = _paymentCodCardId
                    });
                optionList.AddRange(options);
            }
            
            if (_deliveryTypes.Contains(TypeDelivery.PVZ) && calculationVariants.HasFlag(CalculationVariants.PickPoint))
            {
                var deliveryPoints = GetPoints(measoftCity, weight);
                if (deliveryPoints != null && deliveryPoints.Count > 0)
                {
                    string selectedPointId = null;
                    MeasoftPoint deliveryPoint = null;

                    if (_calculationParameters.ShippingOption != null &&
                                _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Measoft).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        if (_calculationParameters.ShippingOption is MeasoftPointOption option)
                            selectedPointId = option.SelectedPoint?.Id;
                        if (_calculationParameters.ShippingOption is MeasoftDeliveryMapOption mapOption)
                            selectedPointId = mapOption.PickpointId;
                    }

                    deliveryPoint = selectedPointId != null
                               ? deliveryPoints.FirstOrDefault(x => x.Id == selectedPointId) ?? deliveryPoints[0]
                               : deliveryPoints[0];

                    if (deliveryPoint != null)
                    {
                        var measoftPickPointCalcOption = new MeasoftCalcOptionParams
                        {
                            Address = deliveryPoint.Address,
                            PvzCode = deliveryPoint.Id,
                            Weight = weight,
                            Dimensions = dimensionsInSm,
                            City = measoftCity.Name,
                            RegionCode = measoftCity.Region.Code,
                            WithInsure = _withInsure,
                            ExtraDeliveryDays = _extraDeliveryDays,
                            DeliveryServiceIds = _activeDeliveryServices
                        };

                        var deliveryPointIsAvailable = true;
                        IEnumerable<BaseShippingOption> options = null;
                        var measoftCalcOptions = _apiService.CalcOptions(measoftPickPointCalcOption, items);
                        if (!measoftCalcOptions.Any())
                        {
                            deliveryPointIsAvailable = false;
                            measoftPickPointCalcOption.Address = string.Join(" ",
                                _calculationParameters.Country,
                                _calculationParameters.Region,
                                _calculationParameters.City,
                                _calculationParameters.District);
                            measoftPickPointCalcOption.PvzCode = null;
                            measoftCalcOptions = _apiService.CalcOptions(measoftPickPointCalcOption, items);
                        }
                        if (_typeViewPoints == TypeViewPoints.List)
                        {
                            options = measoftCalcOptions.Select(x => new MeasoftPointOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                                Rate = x.BasePrice,
                                DeliveryId = x.DeliveryId,
                                DeliveryTime = x.DeliveryTime,
                                ShippingPoints = deliveryPoints,
                                SelectedPoint = deliveryPointIsAvailable ? deliveryPoint : null,
                                DateOfDelivery = x.MinDeliveryDate.AddDays(_extraDeliveryDays),
                                BasePrice = x.BasePrice,
                                PriceCash = x.PriceCash,
                                CalculateOption = x,
                                IsAvailablePaymentCashOnDelivery = !deliveryPointIsAvailable || deliveryPoint.AvailableCardOnDelivery is true || deliveryPoint.AvailableCashOnDelivery is true,
                                CashOnDeliveryCardAvailable = !deliveryPointIsAvailable || deliveryPoint.AvailableCardOnDelivery is true,
                                PaymentCodCardId = _paymentCodCardId,
                                ErrorMessage = deliveryPointIsAvailable || selectedPointId == null
                                ? null 
                                : LocalizationService.GetResourceFormat("Core.Services.Shipping.Measoft.SelectedPointNotAvailable", deliveryPoint.Address)
                            });
                        }
                        else
                        {
                            options = measoftCalcOptions.Select(x => new MeasoftDeliveryMapOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                                DeliveryId = x.DeliveryId,
                                Rate = x.BasePrice,
                                BasePrice = x.BasePrice,
                                PriceCash = x.PriceCash,
                                DeliveryTime = x.DeliveryTime,
                                DateOfDelivery = x.MinDeliveryDate.AddDays(_extraDeliveryDays),
                                IsAvailablePaymentCashOnDelivery = !deliveryPointIsAvailable || deliveryPoint.AvailableCashOnDelivery is true || deliveryPoint.AvailableCardOnDelivery is true,
                                CurrentPoints = deliveryPoints,
                                SelectedPoint = deliveryPointIsAvailable ? deliveryPoint : null,
                                CalculateOption = x,
                                CashOnDeliveryCardAvailable = !deliveryPointIsAvailable || deliveryPoint.AvailableCardOnDelivery is true,
                                PaymentCodCardId = _paymentCodCardId,
                                ErrorMessage = deliveryPointIsAvailable || selectedPointId == null
                                ? null
                                : LocalizationService.GetResourceFormat("Core.Services.Shipping.Measoft.SelectedPointNotAvailable", deliveryPoint.Address)
                            });
                            foreach (var option in options)
                                SetMapData((MeasoftDeliveryMapOption)option, _calculationParameters.Country, measoftCity.Region.Code, _calculationParameters.District, measoftCity.Name, weight);
                        }

                        optionList.AddRange(options);
                    }
                }  
            }

            return optionList;
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<MeasoftPoint> points)
        {
            if (points == null)
                return null;

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

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("regionCode") || !data.ContainsKey("city"))
                return null;

            var regionCode = (string)data["regionCode"];
            var city = (string)data["city"];
            var weight = data["weight"].ToString().TryParseFloat();
            MeasoftCity measoftCity = new MeasoftCity
            {
                Region = new MeasoftRegion{ Code = regionCode},
                Name = city
            };

            var points = GetPoints(measoftCity, weight);

            return GetFeatureCollection(points);
        }

        private void SetMapData(MeasoftDeliveryMapOption option, string country, string regionCode, string district, string city, float weight)
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
            option.MapParams.Destination = string.Join(", ", new[] { country, regionCode, district, city }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", city },
                    { "weight", weight },
                    { "regionCode", regionCode }
               };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        #region api methods

        private List<MeasoftPoint> GetPoints(MeasoftCity measoftCity, float weightInKilogramm)
        {
            return _apiService.GetPoints(measoftCity, weightInKilogramm)
                .Where(x => _activeStoreList.Contains(x.ParentCode))
                .ToList();
        }

        public MeasoftDeleteOrderResult DeleteOrder(string trackNumber)
        {
            return _apiService.DeleteOrder(trackNumber);
        }

        #endregion
    }
}
