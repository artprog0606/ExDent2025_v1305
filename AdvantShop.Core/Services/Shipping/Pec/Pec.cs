//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.Pec.Api;

namespace AdvantShop.Shipping.Pec
{
    [ShippingKey("Pec")]
    public partial class Pec : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingSyncOfOrderStatus, IShippingSupportingPaymentCashOnDelivery
    {
        #region Ctor

        private readonly string _login;
        private readonly string _apiKey;
        private readonly long? _senderCityId;
        private readonly int _increaseDeliveryTime;
        private readonly bool _withInsure;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly List<TypeDelivery> _deliveryTypes;
        private readonly List<int> _showTransportDelivery;
        private readonly string _senderInn;
        private readonly string _senderTitle;
        private readonly string _senderFs;
        private readonly string _senderPhone;
        private readonly string _senderPhoneAdditional;
        private readonly bool _statusesSync;

        private readonly PecApiService _pecApi;

        public const string KeyNameCargoCodeInOrderAdditionalData = "PecCargoCode";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Pec(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _login = _method.Params.ElementOrDefault(PecTemplate.Login);
            _apiKey = _method.Params.ElementOrDefault(PecTemplate.ApiKey);
            _senderCityId = _method.Params.ElementOrDefault(PecTemplate.SenderCityId).TryParseLong(true);
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(PecTemplate.TypeViewPoints).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(PecTemplate.YaMapsApiKey);
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
            _withInsure = _method.Params.ElementOrDefault(PecTemplate.WithInsure).TryParseBool();
            _deliveryTypes = (method.Params.ElementOrDefault(PecTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).Cast<TypeDelivery>().ToList();
            _showTransportDelivery = (method.Params.ElementOrDefault(PecTemplate.ShowTransportDelivery) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).ToList();
            _senderInn = _method.Params.ElementOrDefault(PecTemplate.SenderInn);
            _senderTitle = _method.Params.ElementOrDefault(PecTemplate.SenderTitle);
            _senderFs = _method.Params.ElementOrDefault(PecTemplate.SenderFs);
            _senderPhone = _method.Params.ElementOrDefault(PecTemplate.SenderPhone);
            _senderPhoneAdditional = _method.Params.ElementOrDefault(PecTemplate.SenderPhoneAdditional);
            _statusesSync = method.Params.ElementOrDefault(PecTemplate.StatusesSync).TryParseBool();

            var statusesReference = method.Params.ElementOrDefault(PecTemplate.StatusesReference);
            if (!string.IsNullOrEmpty(statusesReference))
            {
                string[] arr = null;
                _statusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (arr = x.Split(','))[0].TryParseLong(),
                            x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<long, int?>();

            _pecApi = new PecApiService(_login, _apiKey);
        }

        #endregion

        #region IShippingSupportingSyncOfOrderStatus

        public void SyncStatusOfOrder(Order order)
        {
            var cargoCode = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameCargoCodeInOrderAdditionalData);
            if (cargoCode.IsNotEmpty())
            {
                var getStatuses = _pecApi.GetCargosStatuses(cargoCode);
                if (getStatuses != null && getStatuses.Success && getStatuses.Result != null)
                {
                    if (getStatuses.Result.Cargos != null && getStatuses.Result.Cargos.Count > 0)
                    {
                        var statusInfo = getStatuses.Result.Cargos[0].Info;

                        var pecOrderStatus = statusInfo != null && statusInfo.CargoStatusId.HasValue && StatusesReference.ContainsKey(statusInfo.CargoStatusId.Value)
                            ? StatusesReference[statusInfo.CargoStatusId.Value]
                            : null;

                        if (pecOrderStatus.HasValue &&
                            order.OrderStatusId != pecOrderStatus.Value &&
                            OrderStatusService.GetOrderStatus(pecOrderStatus.Value) != null)
                        {
                            /*var lastOrderStatusHistory =
                                OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                    .OrderByDescending(item => item.Date).FirstOrDefault();

                            if (lastOrderStatusHistory == null ||
                                lastOrderStatusHistory.Date < statusInfo.Date)
                            {*/
                                OrderStatusService.ChangeOrderStatus(order.OrderID,
                                    pecOrderStatus.Value, "Синхронизация статусов для ПЭК");
                            //}
                        }

                        var updateOrder = false;

                        var cargoInfo = getStatuses.Result.Cargos[0].Cargo;

                        //if (cargoInfo != null && !string.Equals(order.TrackNumber, cargoInfo.Code, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    order.TrackNumber = cargoInfo.Code;
                        //    updateOrder = true;
                        //}

                        if (statusInfo != null && statusInfo.ArrivalPlanDateTime.HasValue)
                        {
                            if (!order.DeliveryDate.HasValue && order.DeliveryTime.IsNullOrEmpty())
                            {
                                order.DeliveryDate = statusInfo.ArrivalPlanDateTime;
                                order.DeliveryTime = statusInfo.ArrivalPlanDateTime.Value.ToString("HH:mm");
                                updateOrder = true;
                            }
                        }

                        if (updateOrder)
                            OrderService.UpdateOrderMain(order, changedBy: new OrderChangedBy("Синхронизация статусов для ПЭК"));
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

        private Dictionary<long, int?> _statusesReference;
        public Dictionary<long, int?> StatusesReference
        {
            get
            {
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
            var cargoCode = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameCargoCodeInOrderAdditionalData);

            if (cargoCode.IsNotEmpty()) 
            {
                var movementResult = _pecApi.GetCargoStatusHistory(cargoCode);
                if (movementResult != null && movementResult.Success && movementResult.Result != null)
                {
                    return movementResult.Result
                        .Select(x => new HistoryOfMovement
                            {
                                Name = x.Status,
                                Date = x.Date.TryParseDateTime(),
                            })
                        .OrderByDescending(x => x.Date)
                        .ToList();
                }
            }

            return null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        public string SenderInn { get { return _senderInn; } }
        public string SenderTitle { get { return _senderTitle; } }
        public string SenderFs { get { return _senderFs; } }
        public string SenderPhone { get { return _senderPhone; } }
        public string SenderPhoneAdditional { get { return _senderPhoneAdditional; } }
        public long? SenderCityId { get { return _senderCityId; } }
        public bool WithInsure { get { return _withInsure; } }

        public PecApiService PecApiService
        {
            get { return _pecApi; }
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var shippingOptions = new List<BaseShippingOption>();

            var city = _calculationParameters.City;
            var district = _calculationParameters.District;
            var region = _calculationParameters.Region;
            var country = _calculationParameters.Country;
            var orderCost = _totalPrice;

            if (_senderCityId.HasValue && city.IsNotEmpty() && _deliveryTypes.Count > 0 && _showTransportDelivery.Count > 0)
            {
                long? deliveryCityId = null;
                long branchId = 0;

                var brancheAndCity = FindBrancheAndCity(city, district, _pecApi);
                if (brancheAndCity != null)
                {
                    deliveryCityId = brancheAndCity.CityId ?? brancheAndCity.BranchId;
                    branchId = brancheAndCity.BranchId;
                }

                if (deliveryCityId.HasValue)
                {
                    var weight = GetTotalWeight();
                    var dimensions = GetDimensions().Select(x => x / 1000d).ToArray();// конвертируем сами, чтобы получить большую точность (float для таких значений сильно ограничен)

                    var tasks = new List<Task<List<BaseShippingOption>>>();

                    if (_deliveryTypes.Contains(TypeDelivery.PVZ) && calculationVariants.HasFlag(CalculationVariants.PickPoint))
                    {
                        string selectedPoint = null;

                        if (_calculationParameters.ShippingOption != null &&
                            _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Pec).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                        {
                            if (_calculationParameters.ShippingOption.GetType() == typeof(PecPointDeliveryMapOption))
                                selectedPoint = ((PecPointDeliveryMapOption)_calculationParameters.ShippingOption).PickpointId;

                            if (_calculationParameters.ShippingOption.GetType() == typeof(PecOption) && ((PecOption)_calculationParameters.ShippingOption).SelectedPoint != null)
                                selectedPoint = ((PecOption)_calculationParameters.ShippingOption).SelectedPoint.Id;
                        }

                        var points = GetPoints(branchId, deliveryCityId, weight, dimensions, _pecApi);
                        if (points != null && points.Count > 0)
                            tasks.Add(CalcOptionsAsync(_senderCityId.Value, deliveryCityId.Value, _withInsure, orderCost, weight, dimensions, false, 
                                points, selectedPoint, branchId, country, region, district, city));
                    }

                    if (_deliveryTypes.Contains(TypeDelivery.Courier) && calculationVariants.HasFlag(CalculationVariants.Courier))
                    {
                        tasks.Add(CalcOptionsAsync(_senderCityId.Value, deliveryCityId.Value, _withInsure, orderCost, weight, dimensions, true, 
                            null, null, branchId, country, region, district, city));
                    }

                    Task.WaitAll(tasks.ToArray(), TimeSpan.FromMinutes(1));
                    tasks.Where(x => x.Exception != null).ForEach(Debug.Log.Warn);
                    tasks.Where(x => x.Exception == null).Select(x => x.Result).Where(x => x != null).ForEach(shippingOptions.AddRange);
                }
            }

            return shippingOptions;
        }
        
        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            if (pointId.IsNullOrEmpty())
                return null;

            var branchs = GetAllBranchs(_pecApi);

            if (branchs == null || branchs.Count == 0)
                return null;

            foreach(var branch in branchs.Where(x => x.Divisions != null))
            foreach (var division in branch.Divisions.Where(x => x.Warehouses != null))
            foreach (var warehouse in division.Warehouses)
                if (warehouse.Id.Equals(pointId, StringComparison.OrdinalIgnoreCase))
                    return CastPoint(warehouse);

            return null;
        }

        private async Task<List<BaseShippingOption>> CalcOptionsAsync(long senderCityId, long deliveryCityId, bool withInsure, float orderCost, float weight, double[] dimensions, bool withDelivery,
            List<PecPoint> points, string selectedPoint, long deliveryBranchId, string country, string region, string district, string city)
        {
            var shippingOptions = new List<BaseShippingOption>();

            var calculateParams = new CalculateParams
            {
                SenderCityId = senderCityId,
                ReceiverCityId = deliveryCityId,
                CalcDate = DateTime.Today,
                IsInsurance = withInsure,
                IsInsurancePrice = withInsure ? orderCost : (double?)null,
                IsPickUp = false,
                IsDelivery = /*курьер*/withDelivery,
                Cargos = new List<CalculateCargo>()
                {
                    new CalculateCargo
                    {
                        Height = dimensions[2],
                        Width = dimensions[1],
                        Length = dimensions[0],

                        Weight = Math.Round(weight, 3)
                    }
                }
            };

            foreach (var transport in _showTransportDelivery)
            {
                calculateParams.TransportingTypes = new[] {EnTransportingType.Parse(transport)};
                var calculateResponse = await _pecApi.CalculateAsync(calculateParams).ConfigureAwait(false);

                ProcessCalculateResponse(calculateResponse, shippingOptions,
                    deliveryCityId, weight, dimensions, withDelivery, points, selectedPoint, deliveryBranchId, country,
                    region, district, city);
            }

            return shippingOptions;
        }

        private void ProcessCalculateResponse(PecBaseResponse<CalculateResponse> calculateResponse, List<BaseShippingOption> shippingOptions, long deliveryCityId,
            float weight, double[] dimensions, bool withDelivery, List<PecPoint> points, string selectedPoint, long deliveryBranchId,
            string country, string region, string district, string city)
        {
            if (calculateResponse != null
                && calculateResponse.Success
                && calculateResponse.Result != null)
            {
                if (calculateResponse.Result.HasError == false)
                {
                    foreach (var transfer in calculateResponse.Result.Transfers)
                    {
                        if (transfer.HasError)
                        {
                            if (transfer.ErrorMessage.IsNotEmpty())
                                Debug.Log.Warn(string.Format("Pec {0}: {1}", transfer.TransportingType.Localize(),
                                    transfer.ErrorMessage));
                            continue;
                        }

                        if (!_showTransportDelivery.Contains(transfer.TransportingType.Value))
                            continue;

                        var rate = transfer.CostTotal;
                        var deliveryTime = GetDeliveryTime(calculateResponse.Result, transfer.TransportingType, /*курьер*/
                            withDelivery);

                        var deliveryTimeStr = deliveryTime != null
                            ? string.Format("{0:. дн\\.}{1}{2:. дн\\.}",
                                deliveryTime.Item1 + _increaseDeliveryTime,
                                deliveryTime.Item2.HasValue ? " - " : "",
                                deliveryTime.Item2.HasValue ? deliveryTime.Item2.Value + _increaseDeliveryTime : (int?) null)
                            : null;


                        var calculateOption = new PecCalculateOption
                        {
                            TransportingType = transfer.TransportingType.Value,
                            WithDelivery = /*курьер*/withDelivery
                        };

                        var deliveryPoint = points != null
                            ? points.FirstOrDefault(x => x.Id.Equals(selectedPoint, StringComparison.OrdinalIgnoreCase))
                            : null;

                        if (points == null
                            || _typeViewPoints == TypeViewPoints.List
                            || (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                        {
                            var option = new PecOption(_method, _totalPrice)
                            {
                                DeliveryId = transfer.TransportingType.Value,
                                Name = string.Format("{0} ({1})",
                                    _method.Name,
                                    withDelivery ? TypeDelivery.Courier.Localize() : TypeDelivery.PVZ.Localize(),
                                    " " + transfer.TransportingType.Localize().ToLower()),
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = rate,
                                DeliveryTime = deliveryTimeStr,
                                IsAvailablePaymentCashOnDelivery = true,
                                CalculateOption = calculateOption,
                                HideAddressBlock = points != null,
                                ShippingPoints = points,
                                SelectedPoint = deliveryPoint
                            };

                            shippingOptions.Add(option);
                        }
                        else if (_typeViewPoints == TypeViewPoints.YaWidget)
                        {
                            var option = new PecPointDeliveryMapOption(_method, _totalPrice)
                            {
                                DeliveryId = transfer.TransportingType.Value,
                                Name = string.Format("{0} ({1})",
                                    _method.Name,
                                    withDelivery ? TypeDelivery.Courier.Localize() : TypeDelivery.PVZ.Localize(),
                                    " " + transfer.TransportingType.Localize().ToLower()),
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = rate,
                                DeliveryTime = deliveryTimeStr,
                                IsAvailablePaymentCashOnDelivery = true,
                                CalculateOption = calculateOption,
                                HideAddressBlock = true,
                                CurrentPoints = points,
                                SelectedPoint = deliveryPoint
                            };

                            SetMapData(option, country, region, district, city, deliveryBranchId, deliveryCityId, weight,
                                dimensions);

                            shippingOptions.Add(option);
                        }
                    }
                }
                else if (calculateResponse.Result.ErrorMessage.IsNotEmpty())
                {
                    Debug.Log.Warn("Pec: " + calculateResponse.Result.ErrorMessage);
                }
            }
        }

        private void SetMapData(PecPointDeliveryMapOption option, string country, string region, string district, string city,
            long deliveryBranchId, long deliveryCityId, float weight, double[] dimensions)
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
                    { "deliveryBranchId", deliveryBranchId },
                    { "deliveryCityId", deliveryCityId },
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

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null ||
                !data.ContainsKey("deliveryBranchId") || !data.ContainsKey("deliveryCityId") ||
                !data.ContainsKey("weight") || !data.ContainsKey("dimensionsH") ||
                !data.ContainsKey("dimensionsW") || !data.ContainsKey("dimensionsL"))
                return null;

            long deliveryBranchId = data["deliveryBranchId"].ToString().TryParseLong();
            long deliveryCityId = data["deliveryCityId"].ToString().TryParseLong();
            var weight = data["weight"].ToString().TryParseFloat();
            var dimensions = new double[]
            {
                (double)data["dimensionsL"].ToString().TryParseDecimal(),
                (double)data["dimensionsW"].ToString().TryParseDecimal(),
                (double)data["dimensionsH"].ToString().TryParseDecimal(),
            };

            var deliveryPoints = GetPoints(deliveryBranchId, deliveryCityId, weight, dimensions, _pecApi);

            return GetFeatureCollection(deliveryPoints);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<PecPoint> points)
        {
            return new PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                {
                    var intId = p.Id.GetHashCode();
                    return new PointDelivery.Feature
                    {
                        Id = intId,
                        Geometry = new PointDelivery.PointGeometry {PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f },
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
                            BalloonContentFooter = /*_showAddressComment
                                ?*/ null
                            //: null
                        }
                    };
                }).ToList()
            };
        }

        private Tuple<int, int?> GetDeliveryTime(CalculateResponse calculateResult, EnTransportingType transportingType, bool delivery)
        {
            if (calculateResult != null && calculateResult.CommonTerms != null)
            {
                var transportTerms = calculateResult.CommonTerms.FirstOrDefault(x => x.TransportingType == transportingType);

                if (transportTerms != null)
                {
                    var sourceTimes = delivery ? transportTerms.TransportingWithDelivery : transportTerms.Transporting;

                    if (sourceTimes != null)
                    {
                        int? min = null;
                        int? max = null;

                        foreach(var times in sourceTimes)
                        {
                            var val = new StringBuilder();
                            for (int i = 0; i < times.Length; i++)
                            {
                                if (Char.IsDigit(times[i]))
                                    val.Append(times[i]);

                                if (!Char.IsDigit(times[i]) || i == times.Length - 1)
                                {
                                    if (val.Length != 0)
                                    {
                                        var numb = val.ToString().TryParseInt();

                                        if (min == null || min.Value > numb)
                                            min = numb;

                                        if (max == null || max < numb)
                                            max = numb;
                                    }
                                }
                            }
                        }

                        if (min.HasValue)
                            return new Tuple<int, int?>(min.Value, max.HasValue && min != max.Value ? max : (int?)null);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Код филиала филиала, Код города
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public BranchesFindByTitleItem FindBrancheAndCity(string city, string district, PecApiService pecApi)
        {
            return CacheManager.Get("Pec-City-" + city.GetHashCode(), 24 * 60, () =>
            {
                var findResult = pecApi.FindBranchesByTitle(city);

                if (findResult != null && findResult.Success && findResult.Result.Success && findResult.Result.Items != null)
                {
                    var cityItem = district.IsNullOrEmpty() 
                    ? findResult.Result.Items.FirstOrDefault()
                    : findResult.Result.Items.FirstOrDefault(x => x.CityTitle.IsNullOrEmpty() || x.CityTitle.IndexOf(district, StringComparison.OrdinalIgnoreCase) > 0); // больше 0, значит не совпадает с названием города и содержит район в названии
                    return cityItem;
                }

                return null;
            });
        }

        private List<Branch> GetAllBranchs(PecApiService pecApi)
        {
            List<Branch> list;
            string cacheName = "Pec-AllBranchs";
            if (!CacheManager.TryGetValue(cacheName, out list))
            {
                var result = pecApi.GetAllBranches();

                if (result != null && result.Success && result.Result != null)
                {
                    list = result.Result.Branches;

                    CacheManager.Insert(cacheName, list, 24 * 60);
                }
            }

            return list;
        }

        private List<PecPoint> GetPoints(long branchId, long? cityId, float weight, double[] dimensions, PecApiService pecApi)
        {
            var points = CacheManager.Get(string.Format("Pec-Points-{0}-{1}", branchId, cityId), 24 * 60, () =>
            {
                var branch = GetAllBranchs(pecApi).FirstOrDefault(b => b.BitrixId == branchId);

                if (branch != null)
                {
                    var cityBitrixId = cityId.HasValue ? cityId.Value : branchId;
                    var city = branch.Cities.FirstOrDefault(c => c.BitrixId == cityBitrixId);
                    if (city != null && city.Divisions != null && city.Divisions.Count > 0)
                    {
                        return branch.Divisions
                            .Where(d => city.Divisions.Contains(d.Id))
                            .SelectMany(d => d.Warehouses.Select(CastPoint))
                            .ToList();
                    }
                }

                return null;
            });

            if (points != null) 
            {
                var maxDimension = dimensions.Max();
                var volume = Math.Round(dimensions[0] * dimensions[1] * dimensions[2], 3);
                var weightInGrams = Repository.MeasureUnits.ConvertWeight(
                    weight,
                    Repository.MeasureUnits.WeightUnit.Kilogramm,
                    Repository.MeasureUnits.WeightUnit.Grams);
                points = points
                    .Where(p => p.MaxWeightInGrams == null || p.MaxWeightInGrams.Value >= weightInGrams)
                    .Where(p=> p.MaxVolume == null || p.MaxVolume >= volume)
                    .Where(p => p.MaxDimension == null || p.MaxDimension >= maxDimension)
                    .ToList();
            }

            return points;
        }

        private PecPoint CastPoint(Warehouse warehouse)
        {
            string[] tempArr;
            var maxDimensionInMillimeter =
                warehouse.IsRestrictions
                    ? Repository.MeasureUnits.ConvertLength(
                        (float)warehouse.MaxDimension,
                        Repository.MeasureUnits.LengthUnit.Metre,
                        Repository.MeasureUnits.LengthUnit.Millimeter)
                    : (float?) null;
            
            return new PecPoint
            {
                Id = warehouse.Id,
                Code = warehouse.Id,
                Name = warehouse.Name,
                Address = warehouse.Address,
                Phones = string.IsNullOrWhiteSpace(warehouse.Telephone)
                    ? null
                    : new []{warehouse.Telephone},
                Description = null,
                TimeWorkStr = GetTimeWork(warehouse.DivisionTimeOfWork),
                // TimeWork = todo
                Latitude = (tempArr = warehouse.Coordinates.Split(","))[0].TryParseFloat(),
                Longitude = tempArr[1].TryParseFloat(),
                MaxWeightInGrams = warehouse.IsRestrictions 
                    ? Repository.MeasureUnits.ConvertWeight(
                        (float)warehouse.MaxWeightPerPlace,
                        Repository.MeasureUnits.WeightUnit.Kilogramm,
                        Repository.MeasureUnits.WeightUnit.Grams)
                    : (float?) null,
                MaxVolume = warehouse.IsRestrictions ? warehouse.MaxVolume : (double?) null,
                MaxDimension = warehouse.IsRestrictions ? warehouse.MaxDimension : (double?) null,
                MaxHeightInMillimeters = maxDimensionInMillimeter,
                MaxWidthInMillimeters = maxDimensionInMillimeter,
                MaxLengthInMillimeters = maxDimensionInMillimeter,
            };
        }

        private string GetTimeWork(List<TimeOfWork> divisionTimeOfWork, string separator = ", ")
        {
            if (divisionTimeOfWork != null)
            {
                return string.Join(separator, divisionTimeOfWork.Select(x => string.Format("{0}: {1}{2}", GetWeekDay(x.DayOfWeek), x.WorkFrom + "-" + x.WorkTo, x.DinnerFrom.IsNotEmpty() ? ", обед с " + x.DinnerFrom + " до " + x.DinnerTo : null)));
            }

            return null;
        }

        private object GetWeekDay(byte dayOfWeek)
        {
            var dofw = dayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)dayOfWeek;
            return Localization.Culture.GetCulture().DateTimeFormat.GetDayName(dofw);
        }
    }

    public enum TypeViewPoints
    {
        [Localize("Через Яндекс.Карты")]
        YaWidget = 0,

        [Localize("Списком")]
        List = 2
    }

    public enum TypeDelivery
    {
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery")]
        PVZ = 0,

        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.Courier")]
        Courier = 1
    }
}
