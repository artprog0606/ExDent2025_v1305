//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Grastin
{
    [ShippingKey("Grastin")]
    public class Grastin : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery, IShippingSupportingPaymentPickPoint, IShippingWithBackgroundMaintenance
    {
        #region Ctor

        private readonly string _widgetFromCity;
        private readonly bool _widgetFromCityHide;
        private readonly bool _widgetFromCityNoChange;
        private readonly string _widgetHidePartnersShort;
        private readonly string _widgetHidePartnersFull;
        private readonly bool _widgetHideCost;
        private readonly bool _widgetHideDuration;
        private readonly string _widgetExtrachargeTypen;
        private readonly float _widgetExtracharge;
        private readonly int _widgetAddDuration;
        private readonly string _widgetHidePartnersJson;
        private readonly string _apiKey;
        private readonly string _orderPrefix;
        private readonly EnCourierService _typePaymentDelivery;
        private readonly EnCourierService _typePaymentPickup;
        private readonly EnTypeCalc _typeCalc;
        private readonly List<EnTypeDelivery> _activeDeliveryTypes;
        private readonly bool _insure;
        private readonly bool _statusesSync;
        private readonly string _moscowRegionId;
        private readonly string _saintPetersburgRegionId;
        private readonly string _nizhnyNovgorodRegionId;
        private readonly string _orelRegionId;
        private readonly string _krasnodarRegionId;
        private readonly string _boxberryRegionId;
        private readonly string _partnerRegionId;
        private readonly string _russianPostRegionId;
        private readonly string _cdekRegionId;
        private readonly float _extracharge;
        private readonly int _increaseDeliveryTime;
        private readonly bool _excludeCostOrderprocessing;
        private readonly string _yaMapsApiKey;
        private readonly bool _showDrivingDescriptionPoint;

        private readonly GrastinApiService _grastinApiService;
        private GrastinAggregatorCalcShipingCostRequests _aggregatorCalcShipingCostRequests;
        private readonly List<EnTypeContract> _activeContracts;
        private readonly Dictionary<EnTypeContract, string> _grastinRegionIds;

        public const string KeyNameIsSendOrderInOrderAdditionalData = "GrastinSendOrder";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Grastin(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _widgetFromCity = method.Params.ElementOrDefault(GrastinTemplate.WidgetFromCity);
            _widgetFromCityHide = method.Params.ElementOrDefault(GrastinTemplate.WidgetFromCityHide).TryParseBool();
            _widgetFromCityNoChange = method.Params.ElementOrDefault(GrastinTemplate.WidgetFromCityNoChange).TryParseBool();
            _widgetHidePartnersShort = method.Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersShort);
            _widgetHidePartnersFull = method.Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersFull);
            _widgetHideCost = method.Params.ElementOrDefault(GrastinTemplate.WidgetHideCost).TryParseBool();
            _widgetHideDuration = method.Params.ElementOrDefault(GrastinTemplate.WidgetHideDuration).TryParseBool();
            _widgetExtrachargeTypen = method.Params.ElementOrDefault(GrastinTemplate.WidgetExtrachargeTypen);
            _widgetExtracharge = method.Params.ElementOrDefault(GrastinTemplate.WidgetExtracharge).TryParseFloat();
            _widgetAddDuration = method.Params.ElementOrDefault(GrastinTemplate.WidgetAddDuration).TryParseInt();
            _widgetHidePartnersJson = method.Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersJson);
            _apiKey = method.Params.ElementOrDefault(GrastinTemplate.ApiKey);
            _orderPrefix = method.Params.ElementOrDefault(GrastinTemplate.OrderPrefix);
            _typePaymentDelivery = (EnCourierService)method.Params.ElementOrDefault(GrastinTemplate.TypePaymentDelivery).TryParseInt();
            _typePaymentPickup = (EnCourierService)method.Params.ElementOrDefault(GrastinTemplate.TypePaymentPickup).TryParseInt();
            _typeCalc = (EnTypeCalc)method.Params.ElementOrDefault(GrastinTemplate.TypeCalc).TryParseInt();
            _activeDeliveryTypes = (method.Params.ElementOrDefault(GrastinTemplate.ActiveDeliveryTypes) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).Cast<EnTypeDelivery>().ToList();
            _insure = method.Params.ElementOrDefault(GrastinTemplate.Insure).TryParseBool();
            _statusesSync = method.Params.ElementOrDefault(GrastinTemplate.StatusesSync).TryParseBool();
            _moscowRegionId = method.Params.ElementOrDefault(GrastinTemplate.MoscowRegionId);
            _saintPetersburgRegionId = method.Params.ElementOrDefault(GrastinTemplate.SaintPetersburgRegionId);
            _nizhnyNovgorodRegionId = method.Params.ElementOrDefault(GrastinTemplate.NizhnyNovgorodRegionId);
            _orelRegionId = method.Params.ElementOrDefault(GrastinTemplate.OrelRegionId);
            _krasnodarRegionId = method.Params.ElementOrDefault(GrastinTemplate.KrasnodarRegionId);
            _boxberryRegionId = method.Params.ElementOrDefault(GrastinTemplate.BoxberryRegionId);
            _partnerRegionId = method.Params.ElementOrDefault(GrastinTemplate.PartnerRegionId);
            _russianPostRegionId = method.Params.ElementOrDefault(GrastinTemplate.RussianPostRegionId);
            _cdekRegionId = method.Params.ElementOrDefault(GrastinTemplate.CdekRegionId);
            _extracharge = 0f;
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
            _excludeCostOrderprocessing = method.Params.ElementOrDefault(GrastinTemplate.ExcludeCostOrderprocessing).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(GrastinTemplate.YaMapsApiKey);
            _showDrivingDescriptionPoint = method.Params.ElementOrDefault(GrastinTemplate.ShowDrivingDescriptionPoint).TryParseBool();

            _activeContracts = new List<EnTypeContract>();
            if (!string.IsNullOrEmpty(_moscowRegionId))
                _activeContracts.Add(EnTypeContract.Moscow);
            if (!string.IsNullOrEmpty(_saintPetersburgRegionId))
                _activeContracts.Add(EnTypeContract.SaintPetersburg);
            if (!string.IsNullOrEmpty(_nizhnyNovgorodRegionId))
                _activeContracts.Add(EnTypeContract.NizhnyNovgorod);
            if (!string.IsNullOrEmpty(_orelRegionId))
                _activeContracts.Add(EnTypeContract.Orel);
            if (!string.IsNullOrEmpty(_boxberryRegionId))
                _activeContracts.Add(EnTypeContract.Boxberry);
            if (!string.IsNullOrEmpty(_partnerRegionId))
                _activeContracts.Add(EnTypeContract.Partner);
            if (!string.IsNullOrEmpty(_krasnodarRegionId))
                _activeContracts.Add(EnTypeContract.Krasnodar);
            if (!string.IsNullOrEmpty(_russianPostRegionId))
                _activeContracts.Add(EnTypeContract.RussianPost);
            if (!string.IsNullOrEmpty(_cdekRegionId))
                _activeContracts.Add(EnTypeContract.Cdek);

            _grastinApiService = new GrastinApiService(_apiKey);
            _grastinRegionIds = new Dictionary<EnTypeContract, string>
            {
                {EnTypeContract.Moscow, _moscowRegionId },
                {EnTypeContract.SaintPetersburg, _saintPetersburgRegionId },
                {EnTypeContract.NizhnyNovgorod, _nizhnyNovgorodRegionId },
                {EnTypeContract.Orel, _orelRegionId },
                {EnTypeContract.Krasnodar, _krasnodarRegionId },
                {EnTypeContract.Partner, _partnerRegionId },
            };

            var newStatusesReference = method.Params.ElementOrDefault(GrastinTemplate.StatusesReference);
            if (newStatusesReference == null)
            {
                var oldStatusesReference = string.Join(";", OldStatusesReference.Where(x => x.Value.HasValue).Select(x => $"{x.Key},{x.Value}"));
                newStatusesReference = oldStatusesReference;
                method.Params.TryAddValue(GrastinTemplate.StatusesReference, newStatusesReference);
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

        public void SyncStatusOfOrder(Order order) => throw new NotImplementedException();

        public bool SyncByAllOrders => true;
        public void SyncStatusOfOrders(IEnumerable<Order> orders)
        {
            var orderNumbers = new Dictionary<string, Order>(StringComparer.OrdinalIgnoreCase);
            foreach (var order in orders)
            {
                if (OrderService.GetOrderAdditionalData(order.OrderID, KeyNameIsSendOrderInOrderAdditionalData).IsNotEmpty())
                    orderNumbers.Add($"{OrderPrefix}{order.Number}", order);

                if (orderNumbers.Count >= 100)
                {
                    UpdateStatusOrders(orderNumbers);
                    orderNumbers.Clear();
                }
            }

            UpdateStatusOrders(orderNumbers);
        }

        private void UpdateStatusOrders(Dictionary<string, Order> orderNumbers)
        {
            if (orderNumbers.Count <= 0)
                return;

            var service = new GrastinApiService(ApiKey);
            var ordersInfo = service.GetOrderInfo(new OrderInfoContainer() {Orders = orderNumbers.Keys.ToList()});
            if (ordersInfo != null)
            {
                foreach (var orderInfo in ordersInfo)
                {
                    if (!orderNumbers.ContainsKey(orderInfo.Number))
                        continue;

                    var order = orderNumbers[orderInfo.Number];

                    var grastinOrderStatus = StatusesReference.ContainsKey(orderInfo.Status)
                        ? StatusesReference[orderInfo.Status]
                        : null;

                    if (grastinOrderStatus.HasValue &&
                        order.OrderStatusId != grastinOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(grastinOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date).FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < orderInfo.StatusDateTime)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                grastinOrderStatus.Value, "Синхронизация статусов для Grastin");
                        }
                    }
                }
            }
        }

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
                    _oldStatusesReference = new Dictionary<string, int?>
                    {
                        { "draft", _method.Params.ElementOrDefault(GrastinTemplate.StatusDraft).TryParseInt(true)},
                        { "new", _method.Params.ElementOrDefault(GrastinTemplate.StatusNew).TryParseInt(true)},
                        { "return", _method.Params.ElementOrDefault(GrastinTemplate.StatusReturn).TryParseInt(true)},
                        { "done", _method.Params.ElementOrDefault(GrastinTemplate.StatusDone).TryParseInt(true)},
                        { "shipping", _method.Params.ElementOrDefault(GrastinTemplate.StatusShipping).TryParseInt(true)},
                        { "received", _method.Params.ElementOrDefault(GrastinTemplate.StatusReceived).TryParseInt(true)},
                        { "canceled", _method.Params.ElementOrDefault(GrastinTemplate.StatusCanceled).TryParseInt(true)},
                        { "prepared for shipment", _method.Params.ElementOrDefault(GrastinTemplate.StatusPreparedForShipment).TryParseInt(true)},
                        { "problem", _method.Params.ElementOrDefault(GrastinTemplate.StatusProblem).TryParseInt(true)},
                        { "returned to customer", _method.Params.ElementOrDefault(GrastinTemplate.StatusReturnedToCustomer).TryParseInt(true)},
                        { "decommissioned", _method.Params.ElementOrDefault(GrastinTemplate.StatusDecommissioned).TryParseInt(true)},
                    };
                }
                return _oldStatusesReference;
            }
        }

        public static Dictionary<string, string> Statuses => new Dictionary<string, string>
        {
            { "draft", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.DraftAcceptedViaAPI") },
            { "new", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.ConsideredByLogistician") },
            { "return", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.Return") },
            { "done", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.Completed") },
            { "shipping", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.OnDelivery") },
            { "received", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.OrderWasReceivedAtTheWarehouse") },
            { "canceled", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.OrderCancelled") },
            { "prepared for shipment", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.OrderIsPreparedForShipment") },
            { "problem", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.ProblemOrder") },
            { "returned to customer", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.ReturnedToClient") },
            { "decommissioned", LocalizationService.GetResource("Admin.ShippingMethods.Grastin.WrittenOff") },
        };

        #endregion

        #region Properties

        public string ApiKey
        {
            get { return _apiKey; }
        }

        public string OrderPrefix
        {
            get { return _orderPrefix; }
        }

        public string WidgetFromCity
        {
            get { return _widgetFromCity; }
        }

        public bool Insure
        {
            get { return _insure; }
        }

        public EnCourierService TypePaymentDelivery
        {
            get { return _typePaymentDelivery; }
        }

        public EnCourierService TypePaymentPickup
        {
            get { return _typePaymentPickup; }
        }

        private void DeactivateContract(EnTypeContract typeContract)
        {
            // Отключаем метод, чтобы эконопить кол-во запросов (ограничение 10к в день),
            // для большой нагрузки это очень мало.

            _activeContracts.Remove(typeContract);

            var nameParam = string.Empty;

            if (typeContract == EnTypeContract.Moscow)
                nameParam = GrastinTemplate.MoscowRegionId;

            if (typeContract == EnTypeContract.SaintPetersburg)
                nameParam = GrastinTemplate.SaintPetersburgRegionId;

            if (typeContract == EnTypeContract.NizhnyNovgorod)
                nameParam = GrastinTemplate.NizhnyNovgorodRegionId;

            if (typeContract == EnTypeContract.Orel)
                nameParam = GrastinTemplate.OrelRegionId;

            if (typeContract == EnTypeContract.Krasnodar)
                nameParam = GrastinTemplate.KrasnodarRegionId;

            if (typeContract == EnTypeContract.Partner)
                nameParam = GrastinTemplate.PartnerRegionId;

            if (typeContract == EnTypeContract.Boxberry)
                nameParam = GrastinTemplate.BoxberryRegionId;

            if (typeContract == EnTypeContract.RussianPost)
                nameParam = GrastinTemplate.RussianPostRegionId;

            if (typeContract == EnTypeContract.Cdek)
                nameParam = GrastinTemplate.CdekRegionId;

            if (!string.IsNullOrEmpty(nameParam))
                ShippingMethodService.UpdateShippingParams(_method.ShippingMethodId, new Dictionary<string, string>()
                {
                    {
                        nameParam,
                        string.Empty
                    }
                });
        }

        #endregion

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            LoadTrackNumbers();
        }

        private void LoadTrackNumbers()
        {
            try
            {
                var orders =
                    SQLDataAccess.ExecuteReadList(@"SELECT o.*
                    FROM [Order].[Order] AS o
                        INNER JOIN [Order].[OrderAdditionalData] AS oad ON o.[OrderID] = oad.[OrderID]
                        inner join [Order].[OrderStatus] os ON o.OrderStatusID = os.[OrderStatusID]
                    WHERE oad.[Name] = @KeyNameIsSendOrder AND o.OrderDate >= @MinOrderDate AND os.[IsCanceled] = 0 
                        AND os.[IsCompleted] = 0 AND o.[ShippingMethodID] = @ShippingMethodID
                        AND (o.TrackNumber IS NULL OR o.TrackNumber = '') ",
                        CommandType.Text,
                        OrderService.GetOrderFromReader,
                        new SqlParameter("@ShippingMethodID", _method.ShippingMethodId),
                        new SqlParameter("@KeyNameIsSendOrder", KeyNameIsSendOrderInOrderAdditionalData),
                        new SqlParameter("@MinOrderDate", DateTime.Today.AddMonths(-3)));

                var orderNumbers = new Dictionary<string, Order>(StringComparer.OrdinalIgnoreCase);
                foreach (var order in orders)
                {
                    orderNumbers.Add($"{OrderPrefix}{order.Number}", order);

                    if (orderNumbers.Count >= 100)
                    {
                        UpdateTrackNumberOrders(orderNumbers);
                        orderNumbers.Clear();
                    }
                }
            
                UpdateTrackNumberOrders(orderNumbers);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        private void UpdateTrackNumberOrders(Dictionary<string, Order> orderNumbers)
        {
            if (orderNumbers.Count <= 0)
                return;

            var service = new GrastinApiService(ApiKey);
            var ordersInfo = service.GetOrderInfo(new OrderInfoContainer() {Orders = orderNumbers.Keys.ToList()});
            if (ordersInfo != null)
            {
                foreach (var orderInfo in ordersInfo)
                {
                    if (!orderNumbers.ContainsKey(orderInfo.Number))
                        continue;

                    var order = orderNumbers[orderInfo.Number];
                    
                    if (orderInfo.TrackingNumber.IsNotEmpty() && order.TrackNumber.IsNullOrEmpty())
                    {
                        order.TrackNumber = orderInfo.TrackingNumber;
                        OrderService.UpdateOrderMain(order,
                            changedBy: new OrderChangedBy("Получение трек-номеров Grastin"));
                    }
                }
            }
        }
        
        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var shippingOptions = new List<BaseShippingOption>();
            int weight = (int)GetTotalWeight(rate: 1000); // в граммах
            var dimensions = GetDimensions(rate: 10); // в см

            var orderCost = _totalPrice;

            if (_typeCalc == EnTypeCalc.Widget || _typeCalc == EnTypeCalc.ApiAndWidget)
            {
                // Данный блок нельзя оптимизировать по запросам,
                // т.к. кол-во вызовов зависит от результат предыдущих запросов
                
                if (!string.IsNullOrWhiteSpace(_calculationParameters.City))
                {
                    // при весе больше 25 виджет падает
                    if (weight < 25000)
                    {
                        var widgetOption = new GrastinWidgetOption(_method, _totalPrice, updateRateAndTime: _typeCalc == EnTypeCalc.Widget)
                        {
                            WidgetConfigData = GetConfig(),
                            IsAvailableCashOnDelivery = true
                        };


                        if (_typeCalc == EnTypeCalc.ApiAndWidget)
                        {
                            var preorderOption = _calculationParameters.ShippingOption != null &&
                                                 _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value &&
                                                 _calculationParameters.ShippingOption.GetType() == typeof(GrastinWidgetOption)
                                ? ((GrastinWidgetOption)_calculationParameters.ShippingOption)
                                : null;

                            if (preorderOption != null && preorderOption.PickpointAdditionalDataObj != null)
                            {
                                switch (preorderOption.PickpointAdditionalDataObj.Partner)
                                {
                                    case EnPartner.Partner:
                                    case EnPartner.Grastin:
                                        UpdateGrastinRate(widgetOption, preorderOption, orderCost, weight, dimensions);
                                        break;

                                    case EnPartner.Boxberry:
                                        UpdateBoxberryRate(widgetOption, preorderOption, orderCost, weight);
                                        break;

                                    case EnPartner.RussianPost:
                                        UpdateRussianPostRate(widgetOption, preorderOption, orderCost, weight);
                                        break;

                                    case EnPartner.Cdek:
                                        UpdateCdekRate(widgetOption, preorderOption, orderCost, weight);
                                        break;
                                }
                            }
                            else
                            {
                                SetFirstRate(widgetOption, orderCost, weight, dimensions, calculationVariants);
                            }

                            if (widgetOption.Rate > 0f)
                                shippingOptions.Add(widgetOption);
                        }
                        else
                        {
                            shippingOptions.Add(widgetOption);
                        }
                    }
                }
            }
            else if (_typeCalc == EnTypeCalc.Api || _typeCalc == EnTypeCalc.ApiAndYaWidget)
            {
                if (!string.IsNullOrWhiteSpace(_calculationParameters.City))
                {
                    var city = NormalizeCity(_calculationParameters.City);
                    var typeContracts = GetTypeContractsGrastinByGeoData(city, _calculationParameters.Region);
            
                    using (_aggregatorCalcShipingCostRequests = new GrastinAggregatorCalcShipingCostRequests(_grastinApiService))
                    {
                        var tasks = new List<Task<List<BaseShippingOption>>>();

                        if (typeContracts.Count > 0)
                        {
                            if (typeContracts.Any(x => _activeContracts.Contains(x)))
                            {
                                if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) && (_typeCalc == EnTypeCalc.Api || _yaMapsApiKey.IsNullOrEmpty()))
                                    if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                                        tasks.Add(GetShippingOptionsWithPointsAsync(typeContracts, orderCost, weight, dimensions, city, _insure));

                                if (_activeDeliveryTypes.Contains(EnTypeDelivery.Courier))
                                    if (calculationVariants.HasFlag(CalculationVariants.Courier))
                                        tasks.Add(GetShippingOptionsAsync(typeContracts, orderCost, weight, city, _calculationParameters.Zip, _insure));
                            }
                        }

                        if (_activeContracts.Contains(EnTypeContract.Boxberry))
                        {
                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) && (_typeCalc == EnTypeCalc.Api || _yaMapsApiKey.IsNullOrEmpty()))
                                if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                                    tasks.Add(GetShippingOptionsWithPointsAsync(new List<EnTypeContract> { EnTypeContract.Boxberry }, orderCost, weight, dimensions, city, _insure));

                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Courier))
                                if (calculationVariants.HasFlag(CalculationVariants.Courier))
                                    tasks.Add(GetShippingOptionsAsync(new List<EnTypeContract> { EnTypeContract.Boxberry }, orderCost, weight, city, _calculationParameters.Zip, _insure));
                        }
                        
                        if (_activeContracts.Contains(EnTypeContract.RussianPost))
                        {
                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint))
                                if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                                    tasks.Add(GetShippingOptionsAsync(new List<EnTypeContract> { EnTypeContract.RussianPost }, orderCost, weight, city, _calculationParameters.Zip, _insure));
                        }

                        if (_activeContracts.Contains(EnTypeContract.Cdek))
                        {
                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) && (_typeCalc == EnTypeCalc.Api || _yaMapsApiKey.IsNullOrEmpty()))
                                if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                                    tasks.Add(GetShippingOptionsWithPointsAsync(new List<EnTypeContract> { EnTypeContract.Cdek }, orderCost, weight, dimensions, city, _insure));

                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Courier))
                                if (calculationVariants.HasFlag(CalculationVariants.Courier))
                                    tasks.Add(GetShippingOptionsAsync(new List<EnTypeContract> { EnTypeContract.Cdek }, orderCost, weight, city, _calculationParameters.Zip, _insure));
                        }

                        if (_typeCalc == EnTypeCalc.ApiAndYaWidget && _yaMapsApiKey.IsNotEmpty() && _activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint))
                            if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                                tasks.Add(GrastinPointDeliveryMapOptionAsync(typeContracts, orderCost, weight, dimensions, city, _insure));
                
                        
                        _aggregatorCalcShipingCostRequests.CalcAllRequests();
                        
                        Task.WaitAll(tasks.ToArray(), TimeSpan.FromMinutes(1));
                        tasks.Where(x => x.Exception != null).ForEach(Debug.Log.Warn);
                        tasks.Where(x => x.Exception == null).Select(x => x.Result).Where(x => x != null).ForEach(shippingOptions.AddRange);
                    }
                }
            }

            return shippingOptions;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
        {
            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) is false)
                return null;
            
            int weightInGrams = (int)GetTotalWeight(rate: 1000); // в граммах
            var dimensionsInCentimeter = GetDimensions(rate: 10); // в см
            
            var isActiveMoscow = _activeContracts.Contains(EnTypeContract.Moscow);
            var isActiveSaintPetersburg = _activeContracts.Contains(EnTypeContract.SaintPetersburg);
            var isActiveNizhnyNovgorod = _activeContracts.Contains(EnTypeContract.NizhnyNovgorod);
            var isActiveOrel = _activeContracts.Contains(EnTypeContract.Orel);
            var isActiveKrasnodar = _activeContracts.Contains(EnTypeContract.Krasnodar);
            var isLargeSize = weightInGrams >= 25000 || dimensionsInCentimeter[0] >= 190 || dimensionsInCentimeter[1] >= 50 || dimensionsInCentimeter[2] >= 70;
      
            if (isActiveMoscow
                || isActiveSaintPetersburg
                || isActiveNizhnyNovgorod
                || isActiveOrel
                || isActiveKrasnodar)
            {
                var grastinSelfPickup = 
                    _grastinApiService.GetGrastinSelfPickups()
                                      .Where(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal))
                                       // обработка крупногабаритных заказов
                                      .Where(selfpickup => selfpickup.IssuesLargeSize || isLargeSize is false)
                                      .Where(selfpickup => selfpickup.IssuesOnlyLargeSize is false || isLargeSize)
                                       // только подключенные города
                                      .Where(selfpickup => _grastinRegionIds.ContainsValue(selfpickup.DeliveryContract))
                                      //  // не включаем московские пвз, если москва не активна
                                      // .Where(selfpickup => isActiveMoscow || moscowCities.Contains(selfpickup.City, StringComparer.OrdinalIgnoreCase) is false)
                                      //  // не включаем питерские пвз, если питер не активен
                                      // .Where(selfpickup => isActiveSaintPetersburg || string.Equals(selfpickup.City, "санкт-петербург", StringComparison.OrdinalIgnoreCase) is false)
                                      //  // не включаем пвз нижний новгород, если нижний новгород не активен
                                      // .Where(selfpickup => isActiveNizhnyNovgorod || string.Equals(selfpickup.City, "нижний новгород", StringComparison.OrdinalIgnoreCase) is false)
                                      //  // не включаем пвз орёл, если орёл не активен
                                      // .Where(selfpickup => isActiveOrel || string.Equals(selfpickup.City, "орёл", StringComparison.OrdinalIgnoreCase) is false)
                                      //  // не включаем пвз краснодар, если краснодар не активен
                                      // .Where(selfpickup => isActiveKrasnodar || string.Equals(selfpickup.City, "краснодар", StringComparison.OrdinalIgnoreCase) is false)
                                      .FirstOrDefault();
                if (grastinSelfPickup != null)
                {
                    var deliveryCost = CalcShippingCost(grastinSelfPickup.DeliveryContract, _totalPrice, _totalPrice,
                        weightInGrams, true, grastinSelfPickup.Id, isRegional: grastinSelfPickup.RegionalPoint);
                
                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(_grastinRegionIds.First(kv => kv.Value == grastinSelfPickup.DeliveryContract).Key);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                    {
                        var rate = GetDeliverySum(deliveryCost, _insure);
                        var grastinPoint = CastSelfpickup(grastinSelfPickup);
                        return new[]
                        {
                            new GrastinPointOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "Грастин"),
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = GetDeliverySum(deliveryCost, _insure, true),
                                PickpointAdditionalData = new GrastinEventWidgetData
                                {
                                    DeliveryType = EnDeliveryType.PickPoint,
                                    CityFrom = _widgetFromCity,
                                    CityTo = grastinSelfPickup.City,
                                    Cost = rate,
                                    Partner = grastinSelfPickup.TypePoint,
                                    PickPointId = grastinSelfPickup.Id
                                },
                                ShippingPoints = new List<GrastinPoint>(){grastinPoint},
                                SelectedPoint = grastinPoint
                            }
                        };
                    }
                }
            }
            
            if (_activeContracts.Contains(EnTypeContract.Partner))
            {
                float weightInKg = GetTotalWeight();
                var dimensionsSumInCentimeter = dimensionsInCentimeter.Sum(); // в см
                
                var partnerSelfPickup = 
                    _grastinApiService.GetPartnerSelfPickups()
                                      .Where(x => x.MaxWeight == 0f || weightInKg < x.MaxWeight)
                                      .Where(x => x.MaxDimensions == 0f || dimensionsSumInCentimeter < x.MaxDimensions)
                                      .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));
                
                if (partnerSelfPickup != null)
                {
                    var deliveryCost = CalcShippingCost(_grastinRegionIds[EnTypeContract.Partner], _totalPrice,
                        _totalPrice, weightInGrams, true, partnerSelfPickup.Id,
                        isRegional: partnerSelfPickup.RegionalPoint);
                
                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Partner);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                    {
                        var rate = GetDeliverySum(deliveryCost, _insure);
                        var grastinPoint = CastPartnerSelfpickup(partnerSelfPickup);
                        return new[]
                        {
                            new GrastinPointOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "Грастин"),
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = GetDeliverySum(deliveryCost, _insure, true),
                                PickpointAdditionalData = new GrastinEventWidgetData
                                {
                                    DeliveryType = EnDeliveryType.PickPoint,
                                    CityFrom = _widgetFromCity,
                                    CityTo = partnerSelfPickup.City,
                                    Cost = rate,
                                    Partner = partnerSelfPickup.TypePoint,
                                    PickPointId = partnerSelfPickup.Id
                                },
                                ShippingPoints = new List<GrastinPoint>(){grastinPoint},
                                SelectedPoint = grastinPoint
                            }
                        };
                    }
                }
            }
    
            if (_activeContracts.Contains(EnTypeContract.Boxberry))
            {
                var boxberryPoint = 
                    _grastinApiService.GetBoxberrySelfPickup()
                                      .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));

                if (boxberryPoint != null)
                {
                    var deliveryCost = CalcShippingCost(_boxberryRegionId, _totalPrice, _totalPrice, weightInGrams, true, boxberryPoint.Id);
                
                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Boxberry);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                    {
                        var rate = GetDeliverySum(deliveryCost, _insure);
                        var grastinPoint = CastBoxberrySelfpickup(boxberryPoint);

                        return new[]
                        {
                            new GrastinPointOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "Boxberry"),
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = GetDeliverySum(deliveryCost, _insure, true),
                                DeliveryTime = grastinPoint.DeliveryTime,
                                PickpointAdditionalData = new GrastinEventWidgetData
                                {
                                    DeliveryType = EnDeliveryType.PickPoint,
                                    CityFrom = _widgetFromCity,
                                    CityTo = boxberryPoint.City,
                                    Cost = rate,
                                    Partner = EnPartner.Boxberry,
                                    PickPointId = boxberryPoint.Id
                                },
                                ShippingPoints = new List<GrastinPoint>(){grastinPoint},
                                SelectedPoint = grastinPoint
                            }
                        };
                    }
                }
            }

            if (_activeContracts.Contains(EnTypeContract.Cdek))
            {
                var cdekPoint = 
                    _grastinApiService.GetCdekSelfPickup()
                                      .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));

                if (cdekPoint != null)
                {
                    var deliveryCost = CalcShippingCost(_cdekRegionId, _totalPrice, _totalPrice, weightInGrams, true, cdekPoint.Id);
                                
                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Cdek);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                    {
                        var rate = GetDeliverySum(deliveryCost, _insure);
                        var grastinPoint = CastCdekSelfpickup(cdekPoint);

                        return new[]
                        {
                            new GrastinPointOption(_method, _totalPrice)
                            {
                                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "СДЭК"),
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = GetDeliverySum(deliveryCost, _insure, true),
                                DeliveryTime = grastinPoint.DeliveryTime,
                                PickpointAdditionalData = new GrastinEventWidgetData
                                {
                                    DeliveryType = EnDeliveryType.PickPoint,
                                    CityFrom = _widgetFromCity,
                                    CityTo = cdekPoint.City,
                                    Cost = rate,
                                    Partner = EnPartner.Cdek,
                                    PickPointId = cdekPoint.Id
                                },
                                ShippingPoints = new List<GrastinPoint>(){grastinPoint},
                                SelectedPoint = grastinPoint
                            }
                        };
                    }
                }
            }

            return null;
        }
        

        public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude,
            float topLeftLongitude, float bottomRightLatitude, float bottomRightLongitude)
        {
            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) is false)
                return null;
            
            int weightInGrams = (int)GetTotalWeight(rate: 1000); // в граммах
            float weightInKg = GetTotalWeight();
            var dimensionsInCentimeter = GetDimensions(rate: 10); // в см
            var dimensionsSumInCentimeter = dimensionsInCentimeter.Sum(); // в см

            // var isActiveMoscow = _activeContracts.Contains(EnTypeContract.Moscow);
            // var isActiveSaintPetersburg = _activeContracts.Contains(EnTypeContract.SaintPetersburg);
            // var isActiveNizhnyNovgorod = _activeContracts.Contains(EnTypeContract.NizhnyNovgorod);
            // var isActiveOrel = _activeContracts.Contains(EnTypeContract.Orel);
            // var isActiveKrasnodar = _activeContracts.Contains(EnTypeContract.Krasnodar);
            // List<string> moscowCities = null;
            var isLargeSize = weightInGrams >= 25000 || dimensionsInCentimeter[0] >= 190 || dimensionsInCentimeter[1] >= 50 || dimensionsInCentimeter[2] >= 70;

            // if (isActiveMoscow)
            // {
            //     moscowCities = new List<string>()
            //     {
            //         "Москва", "Балашиха", "Видное", "Бутово", "Долгопрудный", "Ивантеевка", "Королев", "Люберцы",
            //         "Мытищи", "Одинцово", "Реутов", "Фрязино", "Химки", "Щелково",
            //         "Ново-Переделкино" //"Чехов", "Подольск", "Домодедово", "Зеленоград"
            //     };
            //     var moscowReg = RegionService.GetRegionByName("Москва");
            //     if (moscowReg != null)
            //         foreach (var city in CityService.GetCitiesByRegion(moscowReg.RegionId))
            //         {
            //             moscowCities.Add(NormalizeCity(city.Name));
            //         }
            // }

            var grastinSelfPickups = 
                _grastinApiService.GetGrastinSelfPickups()
                                  .Where(point => topLeftLatitude > point.Latitude)
                                  .Where(point => topLeftLongitude < point.Longitude)
                                  .Where(point => bottomRightLatitude < point.Latitude)
                                  .Where(point => bottomRightLongitude > point.Longitude)
                                  // обработка крупногабаритных заказов
                                  .Where(selfpickup => selfpickup.IssuesLargeSize || isLargeSize is false)
                                  .Where(selfpickup => selfpickup.IssuesOnlyLargeSize is false || isLargeSize)
                                  // только подключенные города
                                 .Where(selfpickup => _grastinRegionIds.ContainsValue(selfpickup.DeliveryContract));
                                  // // не включаем московские пвз, если москва не активна
                                  // .Where(selfpickup => isActiveMoscow || moscowCities.Contains(selfpickup.City, StringComparer.OrdinalIgnoreCase) is false)
                                  // // не включаем питерские пвз, если питер не активен
                                  // .Where(selfpickup => isActiveSaintPetersburg || string.Equals(selfpickup.City, "санкт-петербург", StringComparison.OrdinalIgnoreCase) is false)
                                  // // не включаем пвз нижний новгород, если нижний новгород не активен
                                  // .Where(selfpickup => isActiveNizhnyNovgorod || string.Equals(selfpickup.City, "нижний новгород", StringComparison.OrdinalIgnoreCase) is false)
                                  // // не включаем пвз орёл, если орёл не активен
                                  // .Where(selfpickup => isActiveOrel || string.Equals(selfpickup.City, "орёл", StringComparison.OrdinalIgnoreCase) is false)
                                  // // не включаем пвз краснодар, если краснодар не активен
                                  // .Where(selfpickup => isActiveKrasnodar || string.Equals(selfpickup.City, "краснодар", StringComparison.OrdinalIgnoreCase) is false);
            
            var partnerSelfPickups =
                _activeContracts.Contains(EnTypeContract.Partner)
                    ? _grastinApiService.GetPartnerSelfPickups()
                                        .Where(point => topLeftLatitude > point.Latitude)
                                        .Where(point => topLeftLongitude < point.Longitude)
                                        .Where(point => bottomRightLatitude < point.Latitude)
                                        .Where(point => bottomRightLongitude > point.Longitude)
                                        .Where(x => x.MaxWeight == 0f || weightInKg < x.MaxWeight)
                                        .Where(x => x.MaxDimensions == 0f || dimensionsSumInCentimeter < x.MaxDimensions)
                    : null;

            var boxberryPoints =
                _activeContracts.Contains(EnTypeContract.Boxberry)
                    ? _grastinApiService.GetBoxberrySelfPickup()
                                        .Where(point => topLeftLatitude > point.Latitude)
                                        .Where(point => topLeftLongitude < point.Longitude)
                                        .Where(point => bottomRightLatitude < point.Latitude)
                                        .Where(point => bottomRightLongitude > point.Longitude)
                    : null;

            var cdekPoints =
                _activeContracts.Contains(EnTypeContract.Cdek)
                    ? _grastinApiService.GetCdekSelfPickup()
                                        .Where(point => topLeftLatitude > point.Latitude)
                                        .Where(point => topLeftLongitude < point.Longitude)
                                        .Where(point => bottomRightLatitude < point.Latitude)
                                        .Where(point => bottomRightLongitude > point.Longitude)
                    : null;
            
            var shippingPoints = new List<BaseShippingPoint>();
            
            if (grastinSelfPickups != null)
                shippingPoints.AddRange(
                    grastinSelfPickups.Select(CastSelfpickup));
            
            if (partnerSelfPickups != null)
                shippingPoints.AddRange(
                    partnerSelfPickups.Select(CastPartnerSelfpickup));
            
            if (boxberryPoints != null)
                shippingPoints.AddRange(
                    boxberryPoints.Select(CastBoxberrySelfpickup));
            
            if (cdekPoints != null)
                shippingPoints.AddRange(
                    cdekPoints.Select(CastCdekSelfpickup));
  
            return shippingPoints;
        }

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            var grastinSelfPickup = 
                _grastinApiService.GetGrastinSelfPickups()
                                  .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));
            if (grastinSelfPickup != null)
                return CastSelfpickup(grastinSelfPickup);

            var partnerSelfPickup = 
                _grastinApiService.GetPartnerSelfPickups()
                                  .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));
            
            if (partnerSelfPickup != null)
                return CastPartnerSelfpickup(partnerSelfPickup);

            var boxberryPoint = 
                _grastinApiService.GetBoxberrySelfPickup()
                                  .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));
            
            if (boxberryPoint != null)
                return CastBoxberrySelfpickup(boxberryPoint);

            var cdekPoint = 
                _grastinApiService.GetCdekSelfPickup()
                                  .FirstOrDefault(selfpickup => string.Equals(selfpickup.Id, pointId, StringComparison.Ordinal));
            
            if (cdekPoint != null)
                return CastCdekSelfpickup(cdekPoint);

            return null;
        }
        
        public GrastinPoint CastCdekSelfpickup(SelfpickupCdek selfpickupCdek)
        {
            return new GrastinPoint()
            {
                Id = selfpickupCdek.Id,
                Code = selfpickupCdek.Id,
                Name = selfpickupCdek.Name,
                Address = selfpickupCdek.Name,
                Description = selfpickupCdek.DrivingDescription,
                Latitude = selfpickupCdek.Latitude,
                Longitude = selfpickupCdek.Longitude,
                TimeWorkStr = selfpickupCdek.Schedule,
                AvailableCashOnDelivery = true,
                DeliveryPeriodInDay = 
                    selfpickupCdek.DeliveryPeriod.IsInt()
                        ? selfpickupCdek.DeliveryPeriod.TryParseInt() + _increaseDeliveryTime
                        : (int?)null,

                TypePoint = EnPartner.Cdek,
            };
        }

        private GrastinPoint CastBoxberrySelfpickup(SelfpickupBoxberry selfpickupBoxberry)
        {
            return new GrastinPoint()
            {
                Id = selfpickupBoxberry.Id,
                Code = selfpickupBoxberry.Id,
                Name = selfpickupBoxberry.Name,
                Address = selfpickupBoxberry.Name,
                Description = selfpickupBoxberry.DrivingDescription,
                Latitude = selfpickupBoxberry.Latitude,
                Longitude = selfpickupBoxberry.Longitude,
                TimeWorkStr = selfpickupBoxberry.Schedule,
                AvailableCashOnDelivery = selfpickupBoxberry.FullPrePayment is false,
                AvailableCardOnDelivery = selfpickupBoxberry.Acquiring,
                DeliveryPeriodInDay = 
                    selfpickupBoxberry.DeliveryPeriod.IsInt()
                        ? selfpickupBoxberry.DeliveryPeriod.TryParseInt() + _increaseDeliveryTime
                        : (int?)null,
                
                TypePoint = EnPartner.Boxberry,
            };
        }

        private GrastinPoint CastISelfpickup(ISelfpickupGrastin selfpickupGrastin)
        {
            if (selfpickupGrastin.TypePoint == EnPartner.Partner)
                return CastPartnerSelfpickup((SelfpickupPartner) selfpickupGrastin);
            else if (selfpickupGrastin.TypePoint == EnPartner.Grastin)
                return CastSelfpickup((Selfpickup) selfpickupGrastin);
            else
                throw new Exception("Type point is not supported.");
        }

        private GrastinPoint CastPartnerSelfpickup(SelfpickupPartner selfpickupPartner)
        {
            return new GrastinPoint()
            {
                Id = selfpickupPartner.Id,
                Code = selfpickupPartner.Id,
                Name = selfpickupPartner.Name,
                Address = selfpickupPartner.Address,
                Description = selfpickupPartner.DrivingDescription,
                Latitude = selfpickupPartner.Latitude,
                Longitude = selfpickupPartner.Longitude,
                Phones =
                    selfpickupPartner.Phone.IsNotEmpty()
                        ? new[] {selfpickupPartner.Phone}
                        : null,
                TimeWorkStr = selfpickupPartner.TimeTable,
                AvailableCashOnDelivery = true,
                AvailableCardOnDelivery = selfpickupPartner.AcceptsPaymentCards,
                MaxWeightInGrams =
                    selfpickupPartner.MaxWeight == 0f
                        ? (float?)null
                        : MeasureUnits.ConvertWeight(selfpickupPartner.MaxWeight, MeasureUnits.WeightUnit.Kilogramm, MeasureUnits.WeightUnit.Grams),
                DimensionSumInMillimeters = 
                    selfpickupPartner.MaxDimensions == 0f
                        ? (float?)null
                        : MeasureUnits.ConvertLength(selfpickupPartner.MaxDimensions, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter),
                
                LinkDriving = selfpickupPartner.LinkDrivingDescription,
                TypePoint = selfpickupPartner.TypePoint
            };
        }

        private GrastinPoint CastSelfpickup(Selfpickup selfpickup)
        {
            return new GrastinPoint()
            {
                Id = selfpickup.Id,
                Code = selfpickup.Id,
                Name = selfpickup.Name,
                Address = selfpickup.Address,
                Description = selfpickup.DrivingDescription,
                Latitude = selfpickup.Latitude,
                Longitude = selfpickup.Longitude,
                Phones =
                    selfpickup.Phone.IsNotEmpty()
                        ? new[] {selfpickup.Phone}
                        : null,
                TimeWorkStr = selfpickup.TimeTable,
                AvailableCashOnDelivery = true,
                AvailableCardOnDelivery = selfpickup.AcceptsPaymentCards,
                MaxWeightInGrams =
                    selfpickup.IssuesLargeSize is false
                        ? 25_000f
                        : (float?) null,
                MaxLengthInMillimeters =
                    selfpickup.IssuesLargeSize is false
                        ? 190f
                        : (float?) null,
                MaxWidthInMillimeters =
                    selfpickup.IssuesLargeSize is false
                        ? 50f
                        : (float?) null,
                MaxHeightInMillimeters =
                    selfpickup.IssuesLargeSize is false
                        ? 70f
                        : (float?) null,
                
                LinkDriving = selfpickup.LinkDrivingDescription,
                TypePoint = selfpickup.TypePoint
            };
        }

        #region Calc Options By Api

        #region Without points

        private async Task<List<BaseShippingOption>> GetShippingOptionsAsync(List<EnTypeContract> typeContracts, float orderCost, int weight, string cityDest, string zipDest, bool insure)
        {
            var list = new List<BaseShippingOption>();

            var grastinContracts = typeContracts
                .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                .Where(x => x != EnTypeContract.Partner)
                .ToList();
            if (grastinContracts.Count > 0)
            {
                if (grastinContracts.Count > 1)
                    throw new ArgumentException("в списке должен быть один элемент"); // не должно срабатывать (в списке должен быть один)

                list.AddRange(
                    await GetGrastinOptionsAsync(grastinContracts.First(), orderCost, weight, cityDest, insure)
                        .ConfigureAwait(false));
            }
            else if (typeContracts.Contains(EnTypeContract.Boxberry))
            {
                list.AddRange(
                    await GetBoxberryOptionsAsync(orderCost, weight, cityDest, zipDest, insure)
                        .ConfigureAwait(false));
            }
            else if (typeContracts.Contains(EnTypeContract.RussianPost))
            {
                list.AddRange(
                    await GetRussianPostOptionsAsync(orderCost, weight, cityDest, zipDest, insure)
                        .ConfigureAwait(false));
            }
            else if (typeContracts.Contains(EnTypeContract.Cdek))
            {
                list.AddRange(
                    await GetCdekOptionsAsync(orderCost, weight, cityDest, zipDest, insure)
                        .ConfigureAwait(false));
            }

            return list;
        }

        private async Task<List<GrastinOption>> GetRussianPostOptionsAsync(float orderCost, int weight, string cityDest, string zipDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var deliveryCost = await CalcDeliveryCostToPickPointUsingRussianPostAsync(orderCost, weight, cityDest, zipDest)
                .ConfigureAwait(false);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                var rate = GetDeliverySum(deliveryCost, insure);
                list.Add(new GrastinOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "Почта России"),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = GetDeliverySum(deliveryCost, insure, true),
                    PickpointAdditionalData = new GrastinEventWidgetData
                    {
                        DeliveryType = EnDeliveryType.PickPoint,
                        CityFrom = _widgetFromCity,
                        CityTo = cityDest,
                        Cost = rate,
                        Partner = EnPartner.RussianPost,
                        PickPointId = string.Empty
                    },
                });
            }

            return list;
        }

        private async Task<List<GrastinOption>> GetCdekOptionsAsync(float orderCost, int weight, string cityDest, string zipDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var deliveryCost = await CalcDeliveryCostByCourierUsingCdekAsync(orderCost, weight, cityDest, zipDest)
                .ConfigureAwait(false);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                var rate = GetDeliverySum(deliveryCost, insure);
                list.Add(new GrastinOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithDeliveryService", _method.Name, "СДЭК"),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = GetDeliverySum(deliveryCost, insure, true),
                    PickpointAdditionalData = new GrastinEventWidgetData
                    {
                        DeliveryType = EnDeliveryType.Courier,
                        CityFrom = _widgetFromCity,
                        CityTo = cityDest,
                        Cost = rate,
                        Partner = EnPartner.Cdek,
                        PickPointId = string.Empty
                    },
                });
            }

            return list;
        }

        private async Task<List<GrastinOption>> GetBoxberryOptionsAsync(float orderCost, int weight, string cityDest, string zipDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var deliveryCost = await CalcDeliveryCostByCourierUsingBoxberryAsync(orderCost, weight, cityDest, zipDest)
                .ConfigureAwait(false);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                var rate = GetDeliverySum(deliveryCost, insure);
                list.Add(new GrastinOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithDeliveryService", _method.Name, "Boxberry"),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = GetDeliverySum(deliveryCost, insure, true),
                    PickpointAdditionalData = new GrastinEventWidgetData
                    {
                        DeliveryType = EnDeliveryType.Courier,
                        CityFrom = _widgetFromCity,
                        CityTo = cityDest,
                        Cost = rate,
                        Partner = EnPartner.Boxberry,
                        PickPointId = string.Empty
                    },
                });
            }

            return list;
        }

        private async Task<List<GrastinOption>> GetGrastinOptionsAsync(EnTypeContract typeContract, float orderCost, int weight, string cityDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var deliveryCost = await CalcDeliveryCostByCourierUsingGrastinAsync(typeContract, orderCost, weight, cityDest)
                .ConfigureAwait(false);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                var rate = GetDeliverySum(deliveryCost, insure);
                list.Add(new GrastinOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithDeliveryService", _method.Name, "Грастин"),
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = GetDeliverySum(deliveryCost, insure, true),
                    PickpointAdditionalData = new GrastinEventWidgetData
                    {
                        DeliveryType = EnDeliveryType.Courier,
                        CityFrom = _widgetFromCity,
                        CityTo = cityDest,
                        Cost = rate,
                        Partner = EnPartner.Grastin,
                        PickPointId = string.Empty
                    },
                });

            }

            return list;
        }

        #endregion Without points

        #region With points

        private async Task<List<BaseShippingOption>> GetShippingOptionsWithPointsAsync(List<EnTypeContract> typeContracts, float orderCost, int weight, float[] dimensions, string cityDest, bool insure)
        {
            var list = new List<BaseShippingOption>();

            var preorderOption = _calculationParameters.ShippingOption != null &&
                _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof (Grastin).GetCustomAttributes(typeof (ShippingKeyAttribute), false).First()).Value &&
                _calculationParameters.ShippingOption.GetType() == typeof (GrastinPointOption)
                    ? ((GrastinPointOption) _calculationParameters.ShippingOption)
                    : new GrastinPointOption();

            var grastinContracts = typeContracts
                .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                .ToList();
            if (grastinContracts.Count > 0)
            {
                list.AddRange(
                    await GetGrastinOptionsWithPoints(grastinContracts, orderCost, weight, dimensions, cityDest, preorderOption, insure)
                        .ConfigureAwait(false));
            }
            else if (typeContracts.Contains(EnTypeContract.Boxberry))
            {
                list.AddRange(
                    await GetBoxberryOptionsWithPoints(orderCost, weight, cityDest, preorderOption, insure)
                        .ConfigureAwait(false));
            }
            else if (typeContracts.Contains(EnTypeContract.Cdek))
            {
                list.AddRange(
                    await GetCdekOptionsWithPoints(orderCost, weight, cityDest, preorderOption, insure)
                        .ConfigureAwait(false));
            }

            return list;
        }

        private async Task<List<GrastinPointOption>> GetCdekOptionsWithPoints(float orderCost, int weight, string cityDest, GrastinPointOption preorderOption, bool insure)
        {
            var list = new List<GrastinPointOption>();

            var pickPointId = preorderOption != null && preorderOption.SelectedPoint != null
                ? preorderOption.SelectedPoint.Id
                : null;

            var result = await CalcDeliveryCostToPickPointUsingCdekAsync(orderCost, weight, cityDest, pickPointId)
                .ConfigureAwait(false);
            var deliveryCost = result?.DeliveryCost;

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                list.Add(GetCdekOptionWithPoints(cityDest, deliveryCost, result.Points, result.SelectedPoint, insure));
            }

            return list;
        }

        private GrastinPointOption GetCdekOptionWithPoints(string cityDest, List<CostResponse> deliveryCost, List<SelfpickupCdek> points, SelfpickupCdek selectedPoint, bool insure)
        {
            var rate = GetDeliverySum(deliveryCost, insure);

            var shippingPoints = new List<GrastinPoint>();
            var selectedGrastinPoint = new GrastinPoint();

            foreach (var point in points)
            {
                var grastinPoint = CastCdekSelfpickup(point);

                if (selectedPoint == point)
                    selectedGrastinPoint = grastinPoint;

                shippingPoints.Add(grastinPoint);
            }

            var shippingOption = new GrastinPointOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "СДЭК"),
                Rate = rate,
                BasePrice = rate,
                PriceCash = GetDeliverySum(deliveryCost, insure, true),
                DeliveryTime = selectedGrastinPoint.DeliveryTime,
                PickpointAdditionalData = new GrastinEventWidgetData
                {
                    DeliveryType = EnDeliveryType.PickPoint,
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    Cost = rate,
                    Partner = EnPartner.Cdek,
                    PickPointId = selectedGrastinPoint.Id
                },
                ShippingPoints = shippingPoints,
                SelectedPoint = selectedGrastinPoint
            };

            return shippingOption;
        }

        private async Task<List<GrastinPointOption>> GetBoxberryOptionsWithPoints(float orderCost, int weight, string cityDest, GrastinPointOption preorderOption, bool insure)
        {
            var list = new List<GrastinPointOption>();

            var pickPointId = preorderOption != null && preorderOption.SelectedPoint != null
                ? preorderOption.SelectedPoint.Id
                : null;

            var result = await CalcDeliveryCostToPickPointUsingBoxberryAsync(orderCost, weight, cityDest, pickPointId)
                .ConfigureAwait(false);
            var deliveryCost = result?.DeliveryCost;

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                list.Add(GetBoxberryOptionWithPoints(cityDest, deliveryCost, result.Points, result.SelectedPoint, insure));
            }

            return list;
        }

        private GrastinPointOption GetBoxberryOptionWithPoints(string cityDest, List<CostResponse> deliveryCost, List<SelfpickupBoxberry> points, SelfpickupBoxberry selectedPoint, bool insure)
        {
            var rate = GetDeliverySum(deliveryCost, insure);

            var shippingPoints = new List<GrastinPoint>();
            var selectedGrastinPoint = new GrastinPoint();

            foreach (var point in points)
            {
                var grastinPoint = CastBoxberrySelfpickup(point);

                if (selectedPoint == point)
                    selectedGrastinPoint = grastinPoint;

                shippingPoints.Add(grastinPoint);
            }

            var shippingOption = new GrastinPointOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "Boxberry"),
                Rate = rate,
                BasePrice = rate,
                PriceCash = GetDeliverySum(deliveryCost, insure, true),
                DeliveryTime = selectedGrastinPoint.DeliveryTime,
                PickpointAdditionalData = new GrastinEventWidgetData
                {
                    DeliveryType = EnDeliveryType.PickPoint,
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    Cost = rate,
                    Partner = EnPartner.Boxberry,
                    PickPointId = selectedGrastinPoint.Id
                },
                ShippingPoints = shippingPoints,
                SelectedPoint = selectedGrastinPoint
            };

            return shippingOption;
        }

        private async Task<List<GrastinPointOption>> GetGrastinOptionsWithPoints(List<EnTypeContract> typeContracts, float orderCost, int weight, float[] dimensions, string cityDest, GrastinPointOption preorderOption, bool insure)
        {
            var list = new List<GrastinPointOption>();

            var pickPointId = preorderOption != null && preorderOption.SelectedPoint != null
                ? preorderOption.SelectedPoint.Id
                : null;

            var result = await CalcDeliveryCostToPickPointUsingGrastinAsync(typeContracts, orderCost, weight, dimensions, cityDest, pickPointId)
                .ConfigureAwait(false);
            var deliveryCost = result?.DeliveryCost;

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                list.Add(GetGrastinOptionWithPoints(deliveryCost, cityDest, preorderOption, result.Points, result.SelectedPoint, insure));
            }

            return list;
        }

        private GrastinPointOption GetGrastinOptionWithPoints(List<CostResponse> deliveryCost, string cityDest, GrastinPointOption preorderOption, List<ISelfpickupGrastin> points, ISelfpickupGrastin selectedPoint, bool insure)
        {
            var rate = GetDeliverySum(deliveryCost, insure);

            var shippingPoints = new List<GrastinPoint>();
            var selectedGrastinPoint = new GrastinPoint();

            var activePartner = _activeContracts.Contains(EnTypeContract.Partner);
            var activeGrastin = _activeContracts.Contains(EnTypeContract.Moscow) ||
                                _activeContracts.Contains(EnTypeContract.SaintPetersburg) ||
                                _activeContracts.Contains(EnTypeContract.NizhnyNovgorod) ||
                                _activeContracts.Contains(EnTypeContract.Orel) ||
                                _activeContracts.Contains(EnTypeContract.Krasnodar);

            foreach (var point in points)
            {
                if (point.TypePoint == EnPartner.Partner && !activePartner)
                    continue;
                if (point.TypePoint == EnPartner.Grastin && !activeGrastin)
                    continue;

                var grastinPoint = CastISelfpickup(point);

                if (selectedPoint == point)
                    selectedGrastinPoint = grastinPoint;

                shippingPoints.Add(grastinPoint);
            }

            var shippingOption = new GrastinPointOption(_method, _totalPrice)
            {
                Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", _method.Name, "Грастин"),
                Rate = rate,
                BasePrice = rate,
                PriceCash = GetDeliverySum(deliveryCost, insure, true),
                PickpointAdditionalData = new GrastinEventWidgetData
                {
                    DeliveryType = EnDeliveryType.PickPoint,
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    Cost = rate,
                    Partner = selectedPoint != null ? selectedPoint.TypePoint : EnPartner.Grastin,
                    PickPointId = selectedGrastinPoint.Id
                },
                ShippingPoints = shippingPoints,
                SelectedPoint = selectedGrastinPoint
            };
            return shippingOption;
        }

        #endregion With points

        #endregion Calc Options By Api

        #region Calc Options For Widget

        private void SetFirstRate(GrastinWidgetOption widgetOption, float orderCost, int weight, float[] dimensions,
            CalculationVariants calculationVariants)
        {
            var isCalcRate = false;

            var city = _calculationParameters.City;
            city = NormalizeCity(city);
            var typeContracts = GetTypeContractsGrastinByGeoData(city, _calculationParameters.Region);
            if (typeContracts.Any(x => _activeContracts.Contains(x)))
            {
                List<CostResponse> deliveryCost = null;
                // ISelfpickupGrastin selectedPoint;
                // List<ISelfpickupGrastin> points;

                var grastinContracts = typeContracts
                    .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                    .ToList();

                if (grastinContracts.Count > 0 && calculationVariants.HasFlag(CalculationVariants.PickPoint))
                    deliveryCost = CalcDeliveryCostToPickPointUsingGrastinWithoutAggregator(grastinContracts,
                        orderCost, weight, dimensions, city, null, out _, out _);

                if (deliveryCost == null || deliveryCost.Count == 0 || deliveryCost[0].Status != "Ok" || deliveryCost[0].ShippingCost <= 0)
                {
                    grastinContracts = grastinContracts
                        .Where(x => x != EnTypeContract.Partner)
                        .ToList();

                    if (grastinContracts.Count > 1)
                        throw new ArgumentException("в списке должен быть один элемент"); // не должно срабатывать (в списке должен быть один)

                    if (grastinContracts.Count > 0 && calculationVariants.HasFlag(CalculationVariants.Courier))
                        deliveryCost = CalcDeliveryCostByCourierUsingGrastinWithoutAggregator(grastinContracts.First(), orderCost, weight, city);
                }

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    isCalcRate = true;
                }
            }

            if (!isCalcRate && _activeContracts.Contains(EnTypeContract.Boxberry))
            {
                List<CostResponse> deliveryCost = null;
                SelfpickupBoxberry selectedPoint = null;
                // List<SelfpickupBoxberry> points;

                if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                    deliveryCost = CalcDeliveryCostToPickPointUsingBoxberryWithoutAggregator(orderCost, weight, city,
                        null, out _, out selectedPoint);

                if (deliveryCost == null || deliveryCost.Count == 0 || deliveryCost[0].Status != "Ok" || deliveryCost[0].ShippingCost <= 0)
                    if (calculationVariants.HasFlag(CalculationVariants.Courier))
                        deliveryCost = CalcDeliveryCostByCourierUsingBoxberryWithoutAggregator(orderCost, weight, city, _calculationParameters.Zip);


                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    if (selectedPoint != null)
                    {
                        //widgetOption.IsAvailableCashOnDelivery = selectedPoint.FullPrePayment;
                        widgetOption.DeliveryTime =
                            !string.IsNullOrWhiteSpace(selectedPoint.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPoint.DeliveryPeriod.IsInt()
                                    ? (selectedPoint.DeliveryPeriod.TryParseInt() +
                                       _increaseDeliveryTime).ToString()
                                    : selectedPoint.DeliveryPeriod)
                                : null;
                    }
                    isCalcRate = true;
                }
            }

            if (!isCalcRate && _activeContracts.Contains(EnTypeContract.RussianPost))
            {
                List<CostResponse> deliveryCost = null;

                if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                    deliveryCost = CalcDeliveryCostToPickPointUsingRussianPostWithoutAggregator(orderCost, weight, city, null);

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    isCalcRate = true;
                }
            }

            if (!isCalcRate && _activeContracts.Contains(EnTypeContract.Cdek))
            {
                List<CostResponse> deliveryCost = null;
                SelfpickupCdek selectedPoint = null;
                // List<SelfpickupCdek> points;

                if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                    deliveryCost = CalcDeliveryCostToPickPointUsingCdekWithoutAggregator(orderCost, weight, city,
                        null, out _, out selectedPoint);

                if (deliveryCost == null || deliveryCost.Count == 0 || deliveryCost[0].Status != "Ok" || deliveryCost[0].ShippingCost <= 0)
                    if (calculationVariants.HasFlag(CalculationVariants.Courier))
                        deliveryCost = CalcDeliveryCostByCourierUsingCdekWithoutAggregator(orderCost, weight, city, _calculationParameters.Zip);


                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    if (selectedPoint != null)
                    {
                        //widgetOption.IsAvailableCashOnDelivery = selectedPoint.FullPrePayment;
                        widgetOption.DeliveryTime =
                            !string.IsNullOrWhiteSpace(selectedPoint.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPoint.DeliveryPeriod.IsInt()
                                    ? (selectedPoint.DeliveryPeriod.TryParseInt() +
                                       _increaseDeliveryTime).ToString()
                                    : selectedPoint.DeliveryPeriod)
                                : null;
                    }
                    isCalcRate = true;
                }
            }
        }

        private void UpdateGrastinRate(GrastinWidgetOption widgetOption, GrastinWidgetOption preorderOption, float orderCost,
            int weight, float[] dimensions)
        {
            var city = _calculationParameters.City;
            city = NormalizeCity(city);
            var typeContracts = GetTypeContractsGrastinByGeoData(city, _calculationParameters.Region);
            if (typeContracts.Any(x => _activeContracts.Contains(x)))
            {
                List<CostResponse> deliveryCost = null;
                // ISelfpickupGrastin selectedPoint;
                // List<ISelfpickupGrastin> points;

                var grastinContracts = typeContracts
                    .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                    .ToList();

                if (grastinContracts.Count > 0 && preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.PickPoint)
                    deliveryCost = CalcDeliveryCostToPickPointUsingGrastinWithoutAggregator(grastinContracts,
                        orderCost, weight, dimensions, city, preorderOption.PickpointId, out _, out _);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.Courier)
                {
                    grastinContracts = grastinContracts
                        .Where(x => x != EnTypeContract.Partner)
                        .ToList();

                    if (grastinContracts.Count > 1)
                        throw new ArgumentException("в списке должен быть один элемент"); // не должно срабатывать (в списке должен быть один)

                    if (grastinContracts.Count > 0)
                        deliveryCost = CalcDeliveryCostByCourierUsingGrastinWithoutAggregator(grastinContracts.First(), orderCost, weight, city);
                }

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                }
            }
        }

        private void UpdateBoxberryRate(GrastinWidgetOption widgetOption, GrastinWidgetOption preorderOption, float orderCost,
            int weight)
        {
            if (_activeContracts.Contains(EnTypeContract.Boxberry))
            {
                List<CostResponse> deliveryCost = null;
                SelfpickupBoxberry selectedPoint = null;
                // List<SelfpickupBoxberry> points;
                var city = preorderOption.PickpointAdditionalDataObj.CityTo.IsNotEmpty()
                    ? preorderOption.PickpointAdditionalDataObj.CityTo
                    : _calculationParameters.City;

                city = NormalizeCity(city);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.PickPoint)
                    deliveryCost = CalcDeliveryCostToPickPointUsingBoxberryWithoutAggregator(orderCost, weight, city,
                        preorderOption.PickpointId, out _, out selectedPoint);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.Courier)
                    deliveryCost =
                        CalcDeliveryCostByCourierUsingBoxberryWithoutAggregator(orderCost, weight, city, _calculationParameters.Zip);


                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    if (selectedPoint != null)
                    {
                        //widgetOption.IsAvailableCashOnDelivery = selectedPoint.FullPrePayment;
                        widgetOption.DeliveryTime =
                            !string.IsNullOrWhiteSpace(selectedPoint.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPoint.DeliveryPeriod.IsInt()
                                    ? (selectedPoint.DeliveryPeriod.TryParseInt() +
                                       _increaseDeliveryTime).ToString()
                                    : selectedPoint.DeliveryPeriod)
                                : null;
                    }
                }
            }
        }

        private void UpdateCdekRate(GrastinWidgetOption widgetOption, GrastinWidgetOption preorderOption, float orderCost,
            int weight)
        {
            if (_activeContracts.Contains(EnTypeContract.Cdek))
            {
                List<CostResponse> deliveryCost = null;
                SelfpickupCdek selectedPoint = null;
                // List<SelfpickupCdek> points;
                var city = preorderOption.PickpointAdditionalDataObj.CityTo.IsNotEmpty()
                    ? preorderOption.PickpointAdditionalDataObj.CityTo
                    : _calculationParameters.City;

                city = NormalizeCity(city);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.PickPoint)
                    deliveryCost = CalcDeliveryCostToPickPointUsingCdekWithoutAggregator(orderCost, weight, city,
                        preorderOption.PickpointId, out _, out selectedPoint);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.Courier)
                    deliveryCost =
                        CalcDeliveryCostByCourierUsingCdekWithoutAggregator(orderCost, weight, city, _calculationParameters.Zip);


                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    if (selectedPoint != null)
                    {
                        //widgetOption.IsAvailableCashOnDelivery = selectedPoint.FullPrePayment;
                        widgetOption.DeliveryTime =
                            !string.IsNullOrWhiteSpace(selectedPoint.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPoint.DeliveryPeriod.IsInt()
                                    ? (selectedPoint.DeliveryPeriod.TryParseInt() +
                                       _increaseDeliveryTime).ToString()
                                    : selectedPoint.DeliveryPeriod)
                                : null;
                    }
                }
            }
        }

        private void UpdateRussianPostRate(GrastinWidgetOption widgetOption, GrastinWidgetOption preorderOption, float orderCost,
            int weight)
        {
            if (_activeContracts.Contains(EnTypeContract.RussianPost))
            {
                List<CostResponse> deliveryCost = null;
                var city = preorderOption.PickpointAdditionalDataObj.CityTo.IsNotEmpty()
                    ? preorderOption.PickpointAdditionalDataObj.CityTo
                    : _calculationParameters.City;

                city = NormalizeCity(city);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.PickPoint)
                    deliveryCost = CalcDeliveryCostToPickPointUsingRussianPostWithoutAggregator(orderCost, weight, city, _calculationParameters.Zip);

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                }
            }
        }

        private Dictionary<string, string> GetConfig()
        {
            var _widgetConfigData = new Dictionary<string, string>();

            _widgetConfigData.Add("data-no-weight", "1");

            if (!string.IsNullOrEmpty(_widgetFromCity))
                _widgetConfigData.Add("data-from-city", _widgetFromCity);

            if (_widgetFromCityHide)
                _widgetConfigData.Add("data-from-hide", "1");

            //if (_widgetFromCityNoChange)
            _widgetConfigData.Add("data-from-single", "1");

            if (_calculationParameters.City.IsNotEmpty())
            {
                _widgetConfigData.Add("data-to-city", _calculationParameters.City);
                _widgetConfigData.Add("data-to-hide", "1");
            }

            if (_typeCalc == EnTypeCalc.ApiAndWidget)
            {
                var hidePartners = new List<string>() { "hermespikup", "dpdpikup", "post", "postpackageonline", "postcourieronline" };
                if (!_activeContracts.Contains(EnTypeContract.Moscow) && 
                    !_activeContracts.Contains(EnTypeContract.SaintPetersburg) && 
                    !_activeContracts.Contains(EnTypeContract.NizhnyNovgorod) &&
                    !_activeContracts.Contains(EnTypeContract.Orel) && 
                    !_activeContracts.Contains(EnTypeContract.Krasnodar))
                    hidePartners.AddRange(new []{ "grastinpikup", "grastincourier"});
                else
                {
                    var typeContract = GetTypeContractsGrastinByGeoData(_calculationParameters.City, _calculationParameters.Region, doNotCheckPartner: true);//вернет пустой или с одним элементом список
                    if (!typeContract.All(x => _activeContracts.Contains(x)))
                        hidePartners.AddRange(new[] { "grastinpikup", "grastincourier" });
                }
                if (!_activeContracts.Contains(EnTypeContract.Partner))
                    hidePartners.AddRange(new[] { "partnerpikup" });
                if (!_activeContracts.Contains(EnTypeContract.Boxberry))
                    hidePartners.AddRange(new[] { "boxberrypikup", "boxberrycourier" });

                if (_widgetHidePartnersShort.IsNotEmpty())
                    hidePartners.AddRange(
                        _widgetHidePartnersShort
                            .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                            .Where(x => !hidePartners.Contains(x, StringComparer.OrdinalIgnoreCase)));

                _widgetConfigData.Add("data-no-partners", string.Join(",", hidePartners));
            }
            else
            {
                if (!string.IsNullOrEmpty(_widgetHidePartnersFull))
                    _widgetConfigData.Add("data-no-partners", _widgetHidePartnersFull);
            }

            if (_widgetHideCost)
                _widgetConfigData.Add("data-no-cost", "1");

            if (_widgetHideDuration)
                _widgetConfigData.Add("data-no-duration", "1");

            if (_widgetExtracharge > 0f)
                _widgetConfigData.Add("data-add-cost", string.Format("{0}{1}", _widgetExtracharge.ToString(CultureInfo.InvariantCulture), _widgetExtrachargeTypen));
            //if (_method.Extracharge > 0f)
            //{
            //    if (_method.ExtrachargeType == Payment.ExtrachargeType.Percent)
            //    {
            //        if (_method.ExtrachargeFromOrder)
            //            _widgetConfigData.Add("data-add-cost", Math.Round(_method.Extracharge * _totalPrice / 100, 2).ToInvariantString() + "Руб");
            //        else
            //            _widgetConfigData.Add("data-add-cost", _method.Extracharge.ToInvariantString() + "%");
            //    }
            //    else
            //        _widgetConfigData.Add("data-add-cost", _method.Extracharge.ToInvariantString() + "Руб");
            //}

            //if (_widgetAddDuration > 0f)
            //    _widgetConfigData.Add("data-add-duration", _widgetAddDuration.ToString());
            if (_increaseDeliveryTime > 0f)
                _widgetConfigData.Add("data-add-duration", _increaseDeliveryTime.ToString());

            var weight = GetTotalWeight();
            if (weight > 0f)
                _widgetConfigData.Add("data-weight-base", Math.Ceiling(weight).ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(_widgetHidePartnersJson))
                _widgetConfigData.Add("data-no-partners-obj", Uri.EscapeDataString(_widgetHidePartnersJson));

            return _widgetConfigData;
        }

        #region ApiAndYaWidget

        private void SetMapData(GrastinPointDeliveryMapOption option, int weight, float[] dimensions)
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
            var pointsCount = (option.GrastinPoints != null ? option.GrastinPoints.Count : 0) 
                              + (option.BoxberryPoints != null ? option.BoxberryPoints.Count : 0)
                              + (option.CdekPoints != null ? option.CdekPoints.Count : 0);
            option.PointParams.IsLazyPoints = pointsCount > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", option.CityTo },
                    { "region", _calculationParameters.Region },
                    { "grastinPoints", option.GrastinPoints != null }, // null - значит не активен или неактуально для данного города
                    { "boxberryPoints", option.BoxberryPoints != null },// null - значит не активен
                    { "cdekPoints", option.CdekPoints != null },// null - значит не активен
                    { "weight", weight },
                    { "dimensions0", dimensions[0] },
                    { "dimensions1", dimensions[1] },
                    { "dimensions2", dimensions[2] },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.GrastinPoints, option.BoxberryPoints, option.CdekPoints);
            }
        }

        private PointDelivery.FeatureCollection GetFeatureCollection(List<GrastinPoint> GrastinPoints, List<GrastinPoint> BoxberryPoints, List<GrastinPoint> CdekPoints)
        {
            var featureCollection = new PointDelivery.FeatureCollection() { Features = new List<PointDelivery.Feature>() };

            if (GrastinPoints != null)
            {
                featureCollection.Features.AddRange(GrastinPoints.Select(p =>
                        new PointDelivery.Feature
                        {
                            Id = p.Id.GetHashCode(),
                            Geometry = new PointDelivery.PointGeometry { PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f },
                            Options = new PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                            Properties = new PointDelivery.PointProperties
                            {
                                BalloonContentHeader = p.ToString(),
                                HintContent = p.ToString(),
                                BalloonContentBody =
                                    string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}#{4}')\">Выбрать</a>",
                                                    string.Join("<br>", new[] 
                                                        {
                                                            p.TimeWorkStr,
                                                            p.AvailableCardOnDelivery is true ? "Возможна оплата банковской картой" : null,
                                                            p.Phones?[0],
                                                        }.Where(x => !string.IsNullOrEmpty(x))),
                                                    "<br>",
                                                    p.Id.GetHashCode(),
                                                    p.TypePoint,
                                                    p.Id),
                                BalloonContentFooter = _showDrivingDescriptionPoint 
                                    ? string.Join("<br>", new[]
                                        {
                                            p.Description,
                                            p.LinkDriving.IsNotEmpty() ? string.Format("<a target=\"_blank\" href=\"{0}\">Показать схему</a>", p.LinkDriving) : null
                                        }.Where(x => !string.IsNullOrEmpty(x)))
                                    : null
                            }
                        }));
            }

            if (BoxberryPoints != null)
            {
                featureCollection.Features.AddRange(BoxberryPoints.Select(p =>
                        new PointDelivery.Feature
                        {
                            Id = p.Id.GetHashCode(),
                            Geometry = new PointDelivery.PointGeometry { PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f },
                            Options = new PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                            Properties = new PointDelivery.PointProperties
                            {
                                BalloonContentHeader = p.Name,
                                HintContent = p.Name,
                                BalloonContentBody =
                                    string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}#{4}')\">Выбрать</a>",
                                                    string.Join("<br>", new[] 
                                                        {
                                                            p.TimeWorkStr,
                                                            p.AvailableCardOnDelivery is true ? "Возможна оплата банковской картой" : null,
                                                        }.Where(x => !string.IsNullOrEmpty(x))),
                                                    "<br>",
                                                    p.Id.GetHashCode(),
                                                    EnPartner.Boxberry,
                                                    p.Id),
                                BalloonContentFooter = _showDrivingDescriptionPoint
                                    ? string.Join("<br>", new[]
                                        {
                                            p.Description,
                                        }.Where(x => !string.IsNullOrEmpty(x)))
                                    :null
                            }
                        }));
            }

            if (CdekPoints != null)
            {
                featureCollection.Features.AddRange(CdekPoints.Select(p =>
                        new PointDelivery.Feature
                        {
                            Id = p.Id.GetHashCode(),
                            Geometry = new PointDelivery.PointGeometry { PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f },
                            Options = new PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                            Properties = new PointDelivery.PointProperties
                            {
                                BalloonContentHeader = p.Name,
                                HintContent = p.Name,
                                BalloonContentBody =
                                    string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}#{4}')\">Выбрать</a>",
                                                    p.TimeWorkStr,
                                                    "<br>",
                                                    p.Id.GetHashCode(),
                                                    EnPartner.Cdek,
                                                    p.Id),
                                BalloonContentFooter = _showDrivingDescriptionPoint
                                    ? string.Join("<br>", new[]
                                        {
                                            p.Description,
                                        }.Where(x => !string.IsNullOrEmpty(x)))
                                    :null
                            }
                        }));
            }

            return featureCollection;
        }

        private async Task<List<BaseShippingOption>> GrastinPointDeliveryMapOptionAsync(List<EnTypeContract> typeContracts, float orderCost, int weight, float[] dimensions, string cityDest, bool insure)
        {
            var list = new List<BaseShippingOption>();
            
            var preorderOption = _calculationParameters.ShippingOption != null &&
                                 _calculationParameters.ShippingOption.GetType() == typeof(GrastinPointDeliveryMapOption) &&
                                 _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value
                ? ((GrastinPointDeliveryMapOption)_calculationParameters.ShippingOption)
                : null;

            float? rate = null, priceCash = null;
            string deliveryTime = null;

            var pickPointId = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                ? preorderOption.PickpointAdditionalDataObj.PickPointId
                : null;
            EnPartner typePoint = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                ? preorderOption.PickpointAdditionalDataObj.Partner
                : EnPartner.None;

            List<ISelfpickupGrastin> grastinPoints = null;
            List<SelfpickupBoxberry> boxberryPoints = null;
            List<SelfpickupCdek> cdekPoints = null;

            Task<CalcDeliveryCostToPickPointUsingGrastinResult> taskGrastin = null;
            Task<CalcDeliveryCostToPickPointUsingBoxberryResult> taskBoxberry = null;
            Task<CalcDeliveryCostToPickPointUsingCdekResult> taskCdek = null;
            

            var grastinContracts = typeContracts
                .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                .ToList();
            if (grastinContracts.Count > 0)
            {

                if (typePoint == EnPartner.None ||
                    typePoint == EnPartner.Grastin ||
                    typePoint == EnPartner.Partner)
                {
                    taskGrastin = CalcDeliveryCostToPickPointUsingGrastinAsync(grastinContracts, orderCost, weight, dimensions, cityDest, pickPointId);
                }
                else
                    grastinPoints = GetPointsGrastin(cityDest, weight, dimensions, typeContracts);
            }

            if (_activeContracts.Contains(EnTypeContract.Boxberry))
            {
                if (typePoint == EnPartner.None ||
                    typePoint == EnPartner.Boxberry)
                {
                    taskBoxberry = CalcDeliveryCostToPickPointUsingBoxberryAsync(orderCost, weight, cityDest, pickPointId);
                }
                else
                    boxberryPoints = GetPointsBoxberry(cityDest);
            }

            if (_activeContracts.Contains(EnTypeContract.Cdek))
            {
                if (typePoint == EnPartner.None ||
                    typePoint == EnPartner.Cdek)
                {
                    taskCdek = CalcDeliveryCostToPickPointUsingCdekAsync(orderCost, weight, cityDest, pickPointId);
                }
                else
                    cdekPoints = GetPointsCdek(cityDest);
            }

            var tasks = new List<Task>();
            
            if (taskGrastin != null)
                tasks.Add(taskGrastin);
            if (taskBoxberry != null)
                tasks.Add(taskBoxberry);
            if (taskCdek != null)
                tasks.Add(taskCdek);

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);

                if (taskGrastin?.Result != null)
                {
                    var result = taskGrastin.Result;
                    grastinPoints = result.Points;
                    
                    if (typePoint == EnPartner.None || 
                        ((typePoint == EnPartner.Grastin || typePoint == EnPartner.Partner) &&
                         result.SelectedPoint.Id == pickPointId))
                    {
                        var deliveryCost = result.DeliveryCost;
                        if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                        {
                            rate = GetDeliverySum(deliveryCost, insure);
                            priceCash = GetDeliverySum(deliveryCost, insure, true);
                        }
                    }
                }

                if (taskBoxberry?.Result != null)
                {
                    var result = taskBoxberry.Result;
                    boxberryPoints = result.Points;
                     var selectedPointBoxberry = result.SelectedPoint;

                    if ((typePoint == EnPartner.None && rate == null) || (typePoint == EnPartner.Boxberry && selectedPointBoxberry.Id == pickPointId))
                    {
                        var deliveryCost = result.DeliveryCost;
                        if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                        {
                            rate = GetDeliverySum(deliveryCost, insure);
                            priceCash = GetDeliverySum(deliveryCost, insure, true);
                            deliveryTime = !string.IsNullOrWhiteSpace(selectedPointBoxberry.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPointBoxberry.DeliveryPeriod.IsInt()
                                    ? (selectedPointBoxberry.DeliveryPeriod.TryParseInt() + _increaseDeliveryTime).ToString()
                                    : selectedPointBoxberry.DeliveryPeriod)
                                : null;
                        }
                    }
                }

                if (taskCdek?.Result != null)
                {
                    var result = taskCdek.Result;
                    cdekPoints = result.Points;
                     var selectedPointCdek = result.SelectedPoint;

                    if ((typePoint == EnPartner.None && rate == null) || (typePoint == EnPartner.Cdek && selectedPointCdek.Id == pickPointId))
                    {
                        var deliveryCost = result.DeliveryCost;
                        if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                        {
                            rate = GetDeliverySum(deliveryCost, insure);
                            priceCash = GetDeliverySum(deliveryCost, insure, true);
                            deliveryTime = !string.IsNullOrWhiteSpace(selectedPointCdek.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPointCdek.DeliveryPeriod.IsInt()
                                    ? (selectedPointCdek.DeliveryPeriod.TryParseInt() + _increaseDeliveryTime).ToString()
                                    : selectedPointCdek.DeliveryPeriod)
                                : null;
                        }
                    }
                }
            }

            if ((grastinPoints != null && grastinPoints.Count > 0) || (boxberryPoints != null && boxberryPoints.Count > 0)
                || (cdekPoints != null && cdekPoints.Count > 0))
            {
                var option = new GrastinPointDeliveryMapOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
                    Rate = rate ?? 0f,
                    BasePrice = rate ?? 0f,
                    PriceCash = priceCash ?? 0f,
                    DeliveryTime = deliveryTime,
                    GrastinPoints = grastinPoints
                                   ?.Select(CastISelfpickup)
                                   .ToList(),
                    BoxberryPoints = boxberryPoints
                                    ?.Select(CastBoxberrySelfpickup)
                                    .ToList(),
                    CdekPoints = cdekPoints
                                ?.Select(CastCdekSelfpickup)
                                .ToList(),
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    IsAvailableCashOnDelivery = true,
                    ShowDrivingDescriptionPoint = _showDrivingDescriptionPoint
                };

                SetMapData(option, weight, dimensions);

                list.Add(option);
            }

            return list;
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null || !data.ContainsKey("region") || data["region"] == null
                || !data.ContainsKey("weight") || !data.ContainsKey("dimensions0") || 
                !data.ContainsKey("dimensions1") || !data.ContainsKey("dimensions2"))
                return null;

            var city = (string)data["city"];
            var region = (string)data["region"];
            var weight = data["weight"].ToString().TryParseInt();
            var dimensions = new float[]
            {
                data["dimensions0"].ToString().TryParseFloat(),
                data["dimensions1"].ToString().TryParseFloat(),
                data["dimensions2"].ToString().TryParseFloat(),
            };
            var grastinPoints = data.ContainsKey("grastinPoints") && data["grastinPoints"] != null && (bool)data["grastinPoints"]
                ? GetPointsGrastin(
                      city, weight, dimensions, GetTypeContractsGrastinByGeoData(city, region))
                  .Select(CastISelfpickup).ToList()
                : null;
            var boxberryPoints = data.ContainsKey("boxberryPoints") && data["boxberryPoints"] != null && (bool)data["boxberryPoints"]
                ? GetPointsBoxberry(city)
                 .Select(CastBoxberrySelfpickup).ToList()
                : null;
            var cdekPoints = data.ContainsKey("cdekPoints") && data["cdekPoints"] != null && (bool)data["cdekPoints"]
                ? GetPointsCdek(city)
                 .Select(CastCdekSelfpickup).ToList()
                : null;

            return GetFeatureCollection(grastinPoints, boxberryPoints, cdekPoints);
        }

        #endregion

        #endregion

        #region Help methods

        private async Task<List<CostResponse>> CalcShippingCostAsync(string regionId, float orderSum, float assessedCost, int weight, bool isSelfPickup, string pickupId = null, string postcodeId = null, bool isRegional = false, string postcode = null)
        {
            var deliveryCost = await _aggregatorCalcShipingCostRequests.CalcShippingCostAsync(
                new CalcShippingCostOrder()
                {
                    Number = "123",
                    RegionId = regionId,
                    Weight = weight,
                    OrderSum = orderSum,
                    AssessedCost = assessedCost,
                    IsSelfPickup = isSelfPickup,
                    IsRegional = isRegional,
                    PickupId = pickupId, // уточнеие от поддержки: Idpickup используется только для региона Боксберри
                    PostcodeId = postcodeId,
                    Postcode = postcode
                }).ConfigureAwait(false);
            return deliveryCost;
        }
        
        private List<CostResponse> CalcShippingCost(string regionId, float orderSum, float assessedCost, int weight, bool isSelfPickup, string pickupId = null, string postcodeId = null, bool isRegional = false, string postcode = null)
        {
            var deliveryCost = _grastinApiService.CalcShippingCost(new CalcShippingCostContainer()
            {
                Orders = new List<CalcShippingCostOrder>()
                {
                    new CalcShippingCostOrder()
                    {
                        Number = "123",
                        RegionId = regionId,
                        Weight = weight,
                        OrderSum = orderSum,
                        AssessedCost = assessedCost,
                        IsSelfPickup = isSelfPickup,
                        IsRegional = isRegional,
                        PickupId = pickupId, // уточнеие от поддержки: Idpickup используется только для региона Боксберри
                        PostcodeId = postcodeId,
                        Postcode = postcode
                    }
                }
            });
            return deliveryCost;
        }  
        
        private float GetDeliverySum(List<CostResponse> deliveryCost, bool withInsurance, bool cachOnDelivery = false)
        {
            var rate =
                deliveryCost[0].ShippingCost +
                deliveryCost[0].ShippingCostDistance +
                (cachOnDelivery ? deliveryCost[0].Commission : 0f) +
                (withInsurance ? deliveryCost[0].SafetyStock : 0f) +
                deliveryCost[0].AdditionalShippingCosts +
                (_excludeCostOrderprocessing ? 0f : deliveryCost[0].OrderProcessing) +
                _extracharge;
            return rate;
        }

        #region Grastin

        private List<EnTypeContract> GetTypeContractsGrastinByGeoData(string cityDesc, string regionDest, bool doNotCheckPartner = false)
        {
            var result = new List<EnTypeContract>();

            var isMoscow = cityDesc.Equals("москва", StringComparison.OrdinalIgnoreCase) ||
                           (!string.IsNullOrWhiteSpace(regionDest) &&
                            (regionDest.Equals("москва", StringComparison.OrdinalIgnoreCase) ||
                             regionDest.Equals("московская область", StringComparison.OrdinalIgnoreCase)));

            var isSaintPetersburg =
                cityDesc.Equals("санкт-петербург", StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(regionDest) &&
                 (regionDest.Equals("санкт-петербург", StringComparison.OrdinalIgnoreCase)));

            var isNizhnyNovgorod = cityDesc.Equals("нижний новгород", StringComparison.OrdinalIgnoreCase);
            var isOrel = cityDesc.Equals("орёл", StringComparison.OrdinalIgnoreCase) ||
                         cityDesc.Equals("орел", StringComparison.OrdinalIgnoreCase);
            var isKrasnodar = cityDesc.Equals("краснодар", StringComparison.OrdinalIgnoreCase);

            if (isMoscow || isSaintPetersburg || isNizhnyNovgorod || isOrel || isKrasnodar)
            {
                result.Add(isMoscow
                    ? EnTypeContract.Moscow
                    : isSaintPetersburg
                        ? EnTypeContract.SaintPetersburg
                        : isNizhnyNovgorod
                            ? EnTypeContract.NizhnyNovgorod
                            : isOrel
                                ? EnTypeContract.Orel
                                : EnTypeContract.Krasnodar);
            }

            // Партнерские ПВЗ также могут быть в Питере, Москве и др.
            if (!doNotCheckPartner && _activeContracts.Contains(EnTypeContract.Partner))
            {
                var city = cityDesc;
                city = NormalizeCity(city);

                if (_grastinApiService.GetPartnerSelfPickups()
                    .Any(x => string.Equals(city, x.City, StringComparison.OrdinalIgnoreCase)))
                    result.Add(EnTypeContract.Partner);
            }

            return result;
        }
        
        private List<CostResponse> CalcDeliveryCostToPickPointUsingGrastinWithoutAggregator(List<EnTypeContract> typeContracts, float orderCost,
            int weight, float[] dimensions, string cityDest, string pickPointId, out List<ISelfpickupGrastin> points, out ISelfpickupGrastin SelectedPoint)
        {
            var result = _CalcDeliveryCostToPickPointUsingGrastinAsync(typeContracts, orderCost, weight, dimensions, cityDest,
                pickPointId, withoutAggregator: true).GetAwaiter().GetResult();

            points = result?.Points;
            SelectedPoint = result?.SelectedPoint;
            return result?.DeliveryCost;
        }

        private async Task<CalcDeliveryCostToPickPointUsingGrastinResult> CalcDeliveryCostToPickPointUsingGrastinAsync(List<EnTypeContract> typeContracts, float orderCost,
            int weight, float[] dimensions, string cityDest, string pickPointId)
        {
            return await _CalcDeliveryCostToPickPointUsingGrastinAsync(typeContracts, orderCost, weight, dimensions, cityDest,
                pickPointId, withoutAggregator: false).ConfigureAwait(false);
        }
        
        private async Task<CalcDeliveryCostToPickPointUsingGrastinResult> _CalcDeliveryCostToPickPointUsingGrastinAsync(List<EnTypeContract> typeContracts, float orderCost,
            int weight, float[] dimensions, string cityDest, string pickPointId, bool withoutAggregator)
        {
            var points = GetPointsGrastin(cityDest, weight, dimensions, typeContracts);
            var result = new CalcDeliveryCostToPickPointUsingGrastinResult {Points = points};

            if (points.Count > 0)
            {
                var selectedPoint = result.SelectedPoint = pickPointId.IsNotEmpty()
                    ? points.FirstOrDefault(x => x.Id == pickPointId) ?? points[0]
                    : points[0];

                var typeContract = selectedPoint.TypePoint == EnPartner.Partner
                    ? typeContracts.Select(x => (EnTypeContract?)x).FirstOrDefault(x => x == EnTypeContract.Partner)
                    : typeContracts.Select(x => (EnTypeContract?)x).FirstOrDefault(x => x != EnTypeContract.Partner);

                if (typeContract.HasValue && _grastinRegionIds.ContainsKey(typeContract.Value) &&
                    !string.IsNullOrEmpty(_grastinRegionIds[typeContract.Value]))
                {
                    var deliveryCost = result.DeliveryCost = 
                        withoutAggregator
                            ? CalcShippingCost(_grastinRegionIds[typeContract.Value], orderCost, orderCost, weight, true, selectedPoint.Id, isRegional: selectedPoint.RegionalPoint)
                            : await CalcShippingCostAsync(_grastinRegionIds[typeContract.Value], orderCost, orderCost, weight, true, selectedPoint.Id, isRegional: selectedPoint.RegionalPoint)
                                .ConfigureAwait(false);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(typeContract.Value);
                }
            }

            return result;
        }

        private List<ISelfpickupGrastin> GetPointsGrastin(string cityDest, int weight, float[] dimensions, List<EnTypeContract> typeContracts)
        {
            var moscowCities = new List<string>()
            {
                "Москва", "Балашиха", "Видное", "Бутово", "Долгопрудный", "Ивантеевка", "Королев", "Люберцы", "Мытищи", "Одинцово", "Реутов", "Фрязино", "Химки", "Щелково", "Ново-Переделкино" //"Чехов", "Подольск", "Домодедово", "Зеленоград"
            };

            var isMoscow = cityDest.Equals("москва", StringComparison.OrdinalIgnoreCase);

            if (isMoscow)
            {
                var moscowReg = RegionService.GetRegionByName("Москва");
                if(moscowReg != null)
                    foreach (var city in CityService.GetCitiesByRegion(moscowReg.RegionId))
                    {
                        moscowCities.Add(NormalizeCity(city.Name));
                    }
            }

            var points = new List<ISelfpickupGrastin>();
            if ((_activeContracts.Contains(EnTypeContract.Moscow) && typeContracts.Contains(EnTypeContract.Moscow)) ||
                (_activeContracts.Contains(EnTypeContract.SaintPetersburg) && typeContracts.Contains(EnTypeContract.SaintPetersburg)) ||
                (_activeContracts.Contains(EnTypeContract.NizhnyNovgorod) && typeContracts.Contains(EnTypeContract.NizhnyNovgorod)) ||
                (_activeContracts.Contains(EnTypeContract.Orel) && typeContracts.Contains(EnTypeContract.Orel)) ||
                (_activeContracts.Contains(EnTypeContract.Krasnodar) && typeContracts.Contains(EnTypeContract.Krasnodar)))
            {
                var isLargeSize = weight >= 25000 || dimensions[0] >= 190 || dimensions[1] >= 50 || dimensions[2] >= 70;
                if (isMoscow)
                {
                    points.AddRange(_grastinApiService.GetGrastinSelfPickups()
                        .Where(x =>
                            moscowCities.Contains(x.City, StringComparer.OrdinalIgnoreCase))
                        .Where(x => x.IssuesLargeSize || isLargeSize is false)
                        .Where(x => x.IssuesOnlyLargeSize is false || isLargeSize)
                        .OrderBy(x => x.Name)
                    );
                }
                else
                {
                    points.AddRange(_grastinApiService.GetGrastinSelfPickups()
                        .Where(x =>
                            string.Equals(x.City, cityDest, StringComparison.OrdinalIgnoreCase))
                        .Where(x => x.IssuesLargeSize || isLargeSize is false)
                        .Where(x => x.IssuesOnlyLargeSize is false || isLargeSize)
                        .OrderBy(x => x.Name)
                    );
                }
            }

            if (_activeContracts.Contains(EnTypeContract.Partner) && typeContracts.Contains(EnTypeContract.Partner))
            {
                var weightInKg = MeasureUnits.ConvertWeight(weight, MeasureUnits.WeightUnit.Grams, MeasureUnits.WeightUnit.Kilogramm);
                var dimensionsSum = dimensions.Sum();
                if(isMoscow)
                {
                    points.AddRange(_grastinApiService.GetPartnerSelfPickups()
                        .Where(x =>
                            moscowCities.Contains(x.City, StringComparer.OrdinalIgnoreCase))
                        .Where(x => x.MaxWeight == 0f || weightInKg < x.MaxWeight)
                        .Where(x => x.MaxDimensions == 0f || dimensionsSum < x.MaxDimensions)
                        .OrderBy(x => x.Address));
                }
                else
                {
                    points.AddRange(_grastinApiService.GetPartnerSelfPickups(cityDest)
                        //.Where(x =>
                        //    !string.IsNullOrWhiteSpace(x.City) &&
                        //    x.City.Equals(cityDest, StringComparison.OrdinalIgnoreCase))
                        .Where(x => x.MaxWeight == 0f || weightInKg < x.MaxWeight)
                        .Where(x => x.MaxDimensions == 0f || dimensionsSum < x.MaxDimensions)
                        .OrderBy(x => x.Address));
                }
            }

            return points;
        }

        private List<CostResponse> CalcDeliveryCostByCourierUsingGrastinWithoutAggregator(EnTypeContract typeContract, float orderCost, int weight, 
            string cityDest)
        {
            return _CalcDeliveryCostByCourierUsingGrastinAsync(typeContract, orderCost, weight, cityDest,
                withoutAggregator: true).GetAwaiter().GetResult();
        }

        private async Task<List<CostResponse>> CalcDeliveryCostByCourierUsingGrastinAsync(EnTypeContract typeContract, float orderCost, int weight, 
            string cityDest)
        {
            return await _CalcDeliveryCostByCourierUsingGrastinAsync(typeContract, orderCost, weight, cityDest, withoutAggregator: false)
                .ConfigureAwait(false);
        }
        
        private async Task<List<CostResponse>> _CalcDeliveryCostByCourierUsingGrastinAsync(EnTypeContract typeContract, float orderCost, int weight, 
            string cityDest, bool withoutAggregator)
        {
            if (_grastinRegionIds.ContainsKey(typeContract) && 
                !string.IsNullOrEmpty(_grastinRegionIds[typeContract]))
            {
                var regionId = 
                    typeContract == EnTypeContract.Moscow && !IsMoscow() // расчет в московскую область
                        ? cityDest // вместо id контракта передаем название города
                        : _grastinRegionIds[typeContract];
                var deliveryCost =
                    withoutAggregator
                        ? CalcShippingCost(regionId, orderCost, orderCost, weight, false)
                        : await CalcShippingCostAsync(regionId, orderCost, orderCost, weight, false)
                            .ConfigureAwait(false);

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                    if (typeContract != EnTypeContract.Moscow || IsMoscow()) // не деактивируем для МО 
                        DeactivateContract(typeContract);

                return deliveryCost;
            }

            return null;
        }
        
        private class CalcDeliveryCostToPickPointUsingGrastinResult
        {
            public List<CostResponse> DeliveryCost { get; set; }
            public List<ISelfpickupGrastin> Points { get; set; }
            public ISelfpickupGrastin SelectedPoint { get; set; }
        }

        #endregion Grastin

        #region Boxberry

        private List<CostResponse> CalcDeliveryCostToPickPointUsingBoxberryWithoutAggregator(float orderCost, int weight,
            string cityDest, string pickPointId, out List<SelfpickupBoxberry> points, out SelfpickupBoxberry selectedPoint)
        {
            var result =
                _CalcDeliveryCostToPickPointUsingBoxberryAsync(orderCost, weight, cityDest, pickPointId, withoutAggregator: true)
                    .GetAwaiter().GetResult();

            points = result?.Points;
            selectedPoint = result?.SelectedPoint;
            return result?.DeliveryCost;
        }
        

        private async Task<CalcDeliveryCostToPickPointUsingBoxberryResult> CalcDeliveryCostToPickPointUsingBoxberryAsync(float orderCost, int weight,
            string cityDest, string pickPointId)
        {
            return await _CalcDeliveryCostToPickPointUsingBoxberryAsync(orderCost, weight, cityDest, pickPointId, withoutAggregator: false)
                .ConfigureAwait(false);
        }

        private async Task<CalcDeliveryCostToPickPointUsingBoxberryResult> _CalcDeliveryCostToPickPointUsingBoxberryAsync(float orderCost, int weight,
            string cityDest, string pickPointId, bool withoutAggregator)
        {
            var result = new CalcDeliveryCostToPickPointUsingBoxberryResult();
            if (!string.IsNullOrEmpty(_boxberryRegionId))
            {
                var points = result.Points = GetPointsBoxberry(cityDest);

                if (points.Count > 0)
                {
                    var selectedPoint= result.SelectedPoint = pickPointId.IsNotEmpty()
                        ? points.FirstOrDefault(x => x.Id == pickPointId) ?? points[0]
                        : points[0];

                    var deliveryCost = result.DeliveryCost =
                        withoutAggregator
                            ? CalcShippingCost(_boxberryRegionId, orderCost, orderCost, weight, true, selectedPoint.Id)
                            : await CalcShippingCostAsync(_boxberryRegionId, orderCost, orderCost, weight, true, selectedPoint.Id)
                                .ConfigureAwait(false);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Boxberry);
                }
            }

            return result;
        }

        private List<SelfpickupBoxberry> GetPointsBoxberry(string cityDest)
        {
            return _grastinApiService.GetBoxberrySelfPickup(cityDest)
                .OrderBy(x => x.Name)
                .ToList();
        }

        private List<CostResponse> CalcDeliveryCostByCourierUsingBoxberryWithoutAggregator(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            return _CalcDeliveryCostByCourierUsingBoxberryAsync(orderCost, weight, cityDest, zipDest, withoutAggregator: true)
                .GetAwaiter().GetResult();
        }

        private async Task<List<CostResponse>> CalcDeliveryCostByCourierUsingBoxberryAsync(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            return await _CalcDeliveryCostByCourierUsingBoxberryAsync(orderCost, weight, cityDest, zipDest, withoutAggregator: false)
                .ConfigureAwait(false);
        }

        private async Task<List<CostResponse>> _CalcDeliveryCostByCourierUsingBoxberryAsync(float orderCost, int weight,
            string cityDest, string zipDest, bool withoutAggregator)
        {
            if (!string.IsNullOrEmpty(_boxberryRegionId))
            {
                var postCodes = _grastinApiService.GetBoxberryPostCode(cityDest);

                if (postCodes != null && postCodes.Count > 0)
                {
                    var selectedPostCode = postCodes.FirstOrDefault(x => x.Name.StartsWith(zipDest ?? string.Empty)) ?? postCodes[0];

                    var deliveryCost =
                        withoutAggregator
                            ? CalcShippingCost(_boxberryRegionId, orderCost, orderCost, weight, false, null, selectedPostCode.Id)
                            : await CalcShippingCostAsync(_boxberryRegionId, orderCost, orderCost, weight, false, null, selectedPostCode.Id)
                                .ConfigureAwait(false);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Boxberry);

                    return deliveryCost;
                }
            }

            return null;
        }

        private class CalcDeliveryCostToPickPointUsingBoxberryResult
        {
            public List<CostResponse> DeliveryCost { get; set; }
            public List<SelfpickupBoxberry> Points { get; set; }
            public SelfpickupBoxberry SelectedPoint { get; set; }
        }

        #endregion Boxberry

        #region Cdek

        private List<CostResponse> CalcDeliveryCostToPickPointUsingCdekWithoutAggregator(float orderCost, int weight,
            string cityDest, string pickPointId, out List<SelfpickupCdek> points, out SelfpickupCdek selectedPoint)
        {
            var result =
                _CalcDeliveryCostToPickPointUsingCdekAsync(orderCost, weight, cityDest, pickPointId, withoutAggregator: true)
                    .GetAwaiter().GetResult();

            points = result?.Points;
            selectedPoint = result?.SelectedPoint;
            return result?.DeliveryCost;
        }
        

        private async Task<CalcDeliveryCostToPickPointUsingCdekResult> CalcDeliveryCostToPickPointUsingCdekAsync(float orderCost, int weight,
            string cityDest, string pickPointId)
        {
            return await _CalcDeliveryCostToPickPointUsingCdekAsync(orderCost, weight, cityDest, pickPointId, withoutAggregator: false)
                .ConfigureAwait(false);
        }

        private async Task<CalcDeliveryCostToPickPointUsingCdekResult> _CalcDeliveryCostToPickPointUsingCdekAsync(float orderCost, int weight,
            string cityDest, string pickPointId, bool withoutAggregator)
        {
            var result = new CalcDeliveryCostToPickPointUsingCdekResult();
            if (!string.IsNullOrEmpty(_cdekRegionId))
            {
                var points = result.Points = GetPointsCdek(cityDest);

                if (points.Count > 0)
                {
                    var selectedPoint= result.SelectedPoint = pickPointId.IsNotEmpty()
                        ? points.FirstOrDefault(x => x.Id == pickPointId) ?? points[0]
                        : points[0];

                    var deliveryCost = result.DeliveryCost =
                        withoutAggregator
                            ? CalcShippingCost(_cdekRegionId, orderCost, orderCost, weight, true, selectedPoint.Id)
                            : await CalcShippingCostAsync(_cdekRegionId, orderCost, orderCost, weight, true, selectedPoint.Id)
                                .ConfigureAwait(false);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Cdek);
                }
            }

            return result;
        }

        private List<SelfpickupCdek> GetPointsCdek(string cityDest)
        {
            return _grastinApiService.GetCdekSelfPickup(cityDest)
                .OrderBy(x => x.Name)
                .ToList();
        }

        private List<CostResponse> CalcDeliveryCostByCourierUsingCdekWithoutAggregator(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            return _CalcDeliveryCostByCourierUsingCdekAsync(orderCost, weight, cityDest, zipDest, withoutAggregator: true)
                .GetAwaiter().GetResult();
        }

        private async Task<List<CostResponse>> CalcDeliveryCostByCourierUsingCdekAsync(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            return await _CalcDeliveryCostByCourierUsingCdekAsync(orderCost, weight, cityDest, zipDest, withoutAggregator: false)
                .ConfigureAwait(false);
        }

        private async Task<List<CostResponse>> _CalcDeliveryCostByCourierUsingCdekAsync(float orderCost, int weight,
            string cityDest, string zipDest, bool withoutAggregator)
        {
            if (!string.IsNullOrEmpty(_cdekRegionId))
            {
                var cityCdek = _grastinApiService.GetCdekCities()
                    ?.FirstOrDefault(x => string.Equals(x.Name, cityDest, StringComparison.OrdinalIgnoreCase));

                if (cityCdek != null)
                {
                    var deliveryCost =
                        withoutAggregator
                            ? CalcShippingCost(_cdekRegionId, orderCost, orderCost, weight, false, null, cityCdek.Id)
                            : await CalcShippingCostAsync(_cdekRegionId, orderCost, orderCost, weight, false, null, cityCdek.Id)
                                .ConfigureAwait(false);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Cdek);

                    return deliveryCost;
                }
            }

            return null;
        }

        private class CalcDeliveryCostToPickPointUsingCdekResult
        {
            public List<CostResponse> DeliveryCost { get; set; }
            public List<SelfpickupCdek> Points { get; set; }
            public SelfpickupCdek SelectedPoint { get; set; }
        }

        #endregion Cdek

        #region RussianPost

        private List<CostResponse> CalcDeliveryCostToPickPointUsingRussianPostWithoutAggregator(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            return _CalcDeliveryCostToPickPointUsingRussianPostAsync(orderCost, weight, cityDest, zipDest, withoutAggregator: true)
                .GetAwaiter().GetResult();
        }

        private async Task<List<CostResponse>> CalcDeliveryCostToPickPointUsingRussianPostAsync(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            return await _CalcDeliveryCostToPickPointUsingRussianPostAsync(orderCost, weight, cityDest, zipDest, withoutAggregator: false)
                .ConfigureAwait(false);
        }

        private async Task<List<CostResponse>> _CalcDeliveryCostToPickPointUsingRussianPostAsync(float orderCost, int weight,
            string cityDest, string zipDest, bool withoutAggregator)
        {
            if (!string.IsNullOrEmpty(_russianPostRegionId))
            {
                if (zipDest.IsNotEmpty())
                {
                    var deliveryCost =
                        withoutAggregator
                            ? CalcShippingCost(_russianPostRegionId, orderCost, orderCost, weight, false, postcode: zipDest)
                            : await CalcShippingCostAsync(_russianPostRegionId, orderCost, orderCost, weight, false, postcode: zipDest)
                                .ConfigureAwait(false);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.RussianPost);

                    return deliveryCost;
                }
            }

            return null;
        }

        #endregion RussianPost

        private bool IsMoscow()
        {
            // нельзя расчитать доставку курьером за Москву
            return !string.IsNullOrWhiteSpace(_calculationParameters.City) && _calculationParameters.City.Equals("москва", StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizeCity(string city)
        {
            if (city.Equals("орёл", StringComparison.OrdinalIgnoreCase))
                return "Орел";
            //if (city.Contains("ё") || city.Contains("Ё"))
            //    city = city.Replace('ё', 'е').Replace('Ё', 'Е');
            return city;
        }

        #endregion Help methods
    }

    public enum EnTypeCalc
    {
        /// <summary>
        /// Через Api
        /// </summary>
        [Localize("Через Api")]
        Api = 0,

        /// <summary>
        /// Через Api с выбором через виджет
        /// </summary>
        [Localize("Через Api с выбором через виджет")]
        ApiAndWidget = 1,

        /// <summary>
        /// Через виджет
        /// </summary>
        [Localize("Через виджет, со всеми типами доставки (не рекомендуется)")]
        Widget = 2,

        /// <summary>
        /// Через Api с выбором через наш виджет
        /// </summary>
        [Localize("Через Api с выбором через Яндекс.Карты")]
        ApiAndYaWidget = 3,
    }

    public enum EnTypeContract
    {
        /// <summary>
        /// Грастин Москва
        /// </summary>
        [Localize("Грастин Москва")]
        Moscow = 0,

        /// <summary>
        /// Грастин Санкт-Петербург
        /// </summary>
        [Localize("Грастин Санкт-Петербург")]
        SaintPetersburg = 1,

        /// <summary>
        /// Грастин Нижний Новгород
        /// </summary>
        [Localize("Грастин Нижний Новгород")]
        NizhnyNovgorod = 2,

        /// <summary>
        /// Грастин Орёл
        /// </summary>
        [Localize("Грастин Орёл")]
        Orel = 3,

        /// <summary>
        /// Грастин Краснодар
        /// </summary>
        [Localize("Грастин Краснодар")]
        Krasnodar = 5,

        /// <summary>
        /// Boxberry
        /// </summary>
        [Localize("Партнерские ПВЗ")]
        Partner = 4,

        /// <summary>
        /// Boxberry
        /// </summary>
        [Localize("Boxberry")]
        Boxberry = 30,

        /// <summary>
        /// Почта России
        /// </summary>
        [Localize("RussianPost")]
        RussianPost = 31,

        /// <summary>
        /// СДЭК
        /// </summary>
        [Localize("Cdek")]
        Cdek = 32,

    }

    public enum EnTypeDelivery
    {
        /// <summary>
        /// Самовывоз
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery")]
        Pickpoint = 0,

        /// <summary>
        /// Курьер
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.Courier")]
        Courier = 1,
    }

    [JsonConverter(typeof(GrastinEnumConverter))]
    public enum EnPartner
    {
        None = 0,
        Grastin = 1,
        Hermes = 2,
        RussianPost = 3,
        Boxberry = 4,
        DPD = 5,
        Partner = 6,
        Cdek = 7,
    }
}
