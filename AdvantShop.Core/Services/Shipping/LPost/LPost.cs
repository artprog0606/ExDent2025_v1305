using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.LPost.Api;
using AdvantShop.Shipping.LPost.PickPoints;

namespace AdvantShop.Shipping.LPost
{
    [ShippingKey("LPost")]
    public class LPost : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery, IShippingWithBackgroundMaintenance
    {
        private readonly string _secretKey;
        private readonly List<TypeDelivery> _deliveryTypes;
        private readonly LPostApiService _apiService;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly int _warehouseId;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }
        public LPost(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _secretKey = _method.Params.ElementOrDefault(LPostTemplate.SecretKey);
            _warehouseId = _method.Params.ElementOrDefault(LPostTemplate.ReceivePoint).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(LPostTemplate.YandexMapApiKey);
            _withInsure = _method.Params.ElementOrDefault(LPostTemplate.WithInsure).TryParseBool();
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(LPostTemplate.TypeViewPoints).TryParseInt();
            _deliveryTypes = (method.Params.ElementOrDefault(LPostTemplate.DeliveryTypes) ?? string.Empty)
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (TypeDelivery)x.TryParseInt())
                .ToList();

            _apiService = new LPostApiService(_secretKey);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (_calculationParameters.City.IsNullOrEmpty())
                return null;

            List<BaseShippingOption> optionList = new List<BaseShippingOption>();
            int weight = (int)Math.Round(GetTotalWeight(1000));
            int volume = CalcVolume();
            decimal totalPrice = (decimal)_items.Sum(x => x.Price * x.Amount);
            string address = $"г. {_calculationParameters.City}";

            if (_deliveryTypes.Contains(TypeDelivery.Courier) && calculationVariants.HasFlag(CalculationVariants.Courier))
            {
                var lPostCalculateCourierParam = new LPostOptionParams
                {
                    Weight = weight,
                    Address = address,
                    Volume = volume,
                    SumPayment = totalPrice,
                    WarehouseId = _warehouseId,
                    DateShipment = DateTime.Now.AddDays(_method.ExtraDeliveryTime),
                    IsNotExactAddress = 1,
                    Value = _withInsure ? totalPrice : 0
                };

                var deliveryCostCash = _apiService.GetDeliveryCost(lPostCalculateCourierParam);
                lPostCalculateCourierParam.SumPayment = 0;
                var deliveryCost = _apiService.GetDeliveryCost(lPostCalculateCourierParam);
                if (deliveryCost != null || deliveryCostCash != null)
                {
                    var basePrice = deliveryCost != null ? deliveryCost.SumCost : deliveryCostCash.SumCost;
                    var priceCash = deliveryCostCash != null ? deliveryCostCash.SumCost : deliveryCost.SumCost;

                    optionList.Add(new LPostOption(_method, _totalPrice)
                    {
                        Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
                        Rate = basePrice,
                        DeliveryTime = GetDeliveryTime((deliveryCostCash.DateClose.Date - DateTime.Now.Date).Days + deliveryCostCash.DayLogistic),
                        DeliveryId = (int)TypeDelivery.Courier,
                        BasePrice = basePrice,
                        PriceCash = priceCash
                    });
                } 
            }

            if (_deliveryTypes.Contains(TypeDelivery.PVZ) && calculationVariants.HasFlag(CalculationVariants.PickPoint))
            {
                var deliveryPoints = CastPickPoints(LPostPickPointService.Find(_calculationParameters.Region, _calculationParameters.City, false));
                if (deliveryPoints != null && deliveryPoints.Count > 0)
                {
                    string selectedPointId = null;
                    LPostPoint deliveryPoint = null;

                    if (_calculationParameters.ShippingOption != null &&
                                _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(LPost).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        if (_calculationParameters.ShippingOption is LPostPointOption pointOption)
                            selectedPointId = pointOption.SelectedPoint.Id;
                        if (_calculationParameters.ShippingOption is LPostDeliveryMapOption mapOption)
                            selectedPointId = mapOption.PickpointId;
                    }

                    deliveryPoint = selectedPointId != null
                               ? deliveryPoints.FirstOrDefault(x => x.Id == selectedPointId) ?? deliveryPoints[0]
                               : deliveryPoints[0];

                    if (deliveryPoint != null)
                    {
                        var lPostCalculatePickPointParam = new LPostOptionParams
                        {
                            Weight = weight,
                            Volume = volume,
                            SumPayment = totalPrice,
                            WarehouseId = _warehouseId,
                            DateShipment = DateTime.Now.AddDays(_method.ExtraDeliveryTime),
                            Value = _withInsure ? totalPrice : 0,
                            PickPointId = int.Parse(deliveryPoint.Id)
                        };

                        var deliveryCostCash = _apiService.GetDeliveryCost(lPostCalculatePickPointParam);
                        lPostCalculatePickPointParam.SumPayment = 0;
                        var deliveryCost = _apiService.GetDeliveryCost(lPostCalculatePickPointParam);
                        if (deliveryCost == null && deliveryCostCash == null)
                            return optionList;

                        var basePrice = deliveryCost != null ? deliveryCost.SumCost : deliveryCostCash.SumCost;
                        var priceCash = deliveryCostCash != null ? deliveryCostCash.SumCost : deliveryCost.SumCost;

                        BaseShippingOption option = null;

                        if (_typeViewPoints == TypeViewPoints.List)
                        {
                            option = new LPostPointOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                                DeliveryId = (int)TypeDelivery.PVZ,
                                DeliveryTime = GetDeliveryTime((deliveryCostCash.DateClose.Date - DateTime.Now.Date).Days + deliveryCostCash.DayLogistic),
                                Rate = basePrice,
                                BasePrice = basePrice,
                                PriceCash = priceCash,
                                ShippingPoints = deliveryPoints,
                                SelectedPoint = deliveryPoint,
                            };
                        }
                        else
                        {
                            option = CreatePointDeliveryMapOption(
                                basePrice,
                                priceCash,
                                GetDeliveryTime((deliveryCostCash.DateClose.Date - DateTime.Now.Date).Days + deliveryCostCash.DayLogistic),
                                deliveryPoint.AvailableCashOnDelivery is true,
                                deliveryPoints,
                                (int)TypeDelivery.PVZ);

                            SetMapData((LPostDeliveryMapOption)option, _calculationParameters.Country, _calculationParameters.Region, _calculationParameters.District, _calculationParameters.City,
                                weight);

                        }

                        optionList.Add(option);
                    }
                }
            }

            return optionList;
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<LPostPoint> points)
        {
            if (points == null)
                return null;

            return new PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                {
                    var intId = p.Id.TryParseInt(p.Id.GetHashCode());
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
            if (data == null || !data.ContainsKey("region") || !data.ContainsKey("city"))
                return null;

            var region = (string)data["region"];
            var city = (string)data["city"];

            var points = CastPickPoints(LPostPickPointService.Find(region, city, false));

            return GetFeatureCollection(points);
        }

        private int CalcVolume()
        {
            var dimensions = GetDimensions(10);
            var volume = dimensions[0] * dimensions[1] * dimensions[2];

            return (int)Math.Round(volume);
        }

        private LPostDeliveryMapOption CreatePointDeliveryMapOption(float basePrice, float priceCash, string deliveryTime, bool codAvailable,
            List<LPostPoint> points, int deliveryId)
        {
            return new LPostDeliveryMapOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                DeliveryId = deliveryId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,
                DeliveryTime = deliveryTime,
                IsAvailablePaymentCashOnDelivery = codAvailable,
                CurrentPoints = points
            };
        }

        private void SetMapData(LPostDeliveryMapOption option, string country, string region, string district, string city, float weight)
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
                    { "city", city },
                    { "weight", weight },
                    { "region", region }
               };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        private List<LPostPoint> CastPickPoints(List<PickPoints.LPostPickPoint> points)
        {
            return points
                .Select(CastPickPoint)
                .OrderBy(x => x.Address)
                .ToList();
        }

        private LPostPoint CastPickPoint(PickPoints.LPostPickPoint point)
        {
            return new LPostPoint
            {
                Id = point.Id.ToString(),
                Code = point.Id.ToString(),
                Address = point.Address,
                Description = point.AddressDescription,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                AvailableCardOnDelivery = point.Card,
                AvailableCashOnDelivery = point.Cash
            };
        }

        private string GetDeliveryTime(int dayLogistic)
        {
            if (dayLogistic == 0)
                return "До 1 дн.";
            return dayLogistic + " дн.";
        }

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            if (_secretKey.IsNotEmpty())
                SyncPickPoints(_apiService);
        }

        public static void SyncPickPoints(LPostApiService apiClient)
        {
            // общая настройка, т.к. справочники общие, не зависят от настроек
            var lastDateSync = Configuration.SettingProvider.Items["LPostLastDateSyncPickPoints"].TryParseDateTime(true);
            try
            {
                var currentDateTime = DateTime.UtcNow;

                if (!lastDateSync.HasValue || (currentDateTime - lastDateSync.Value.ToUniversalTime() > TimeSpan.FromHours(23)))
                {
                    // пишем в начале импорта, чтобы, если запустят в паралель еще
                    // то не прошло по условию времени последнего запуска
                    Configuration.SettingProvider.Items["LPostLastDateSyncPickPoints"] = currentDateTime.ToString("O");

                    LPostPickPointService.Sync(apiClient);
                }
            }
            catch (Exception ex)
            {
                // возвращаем предыдущее заначение, чтобы при следующем запуске снова сработало
                Configuration.SettingProvider.Items["LPostLastDateSyncPickPoints"] = lastDateSync.HasValue ? lastDateSync.Value.ToString("O") : null;
                Diagnostics.Debug.Log.Warn(ex);
            }
        }

        #endregion
    }
}
