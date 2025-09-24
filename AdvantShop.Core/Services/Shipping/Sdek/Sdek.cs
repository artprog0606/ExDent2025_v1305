//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Shipping.Sdek.Api;

namespace AdvantShop.Shipping.Sdek
{
    [ShippingKey("Sdek")]
    public partial class Sdek : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingPaymentCashOnDelivery, IShippingWithBackgroundMaintenance
    {
        #region Ctor

        private readonly string _authLogin;
        private readonly string _authPassword;
        private readonly List<string> _tariffs;
        private readonly string _cityFrom;
        private readonly int? _cityFromId;
        private readonly int _deliveryNote;
        private readonly bool _statusesSync;
        private readonly bool _showPointsAsList;
        private readonly bool _allowInspection;
        private readonly bool _showSdekWidjet;
        private readonly bool _showAddressComment;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;
        private readonly bool _partialDelivery;
        private readonly bool _tryingOn;

        private readonly SdekApiService20 _sdekApiService20;

        public const string KeyNameDispatchNumberInOrderAdditionalData = "SdekDispatchNumber";
        public const string KeyNameSdekOrderUuidInOrderAdditionalData = "SdekOrderUuid";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB", "KZT", "USD", "EUR", "GBP", "CNY", "BYR", "UAH", "KGS", "AMD", "TRY", "THB", "KRW", "AED", "UZS", "MNT", "PLN", "AZN", "GEL", "JPY", "VND" }; } }

        public Sdek(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _authLogin = _method.Params.ElementOrDefault(SdekTemplate.AuthLogin);
            _authPassword = _method.Params.ElementOrDefault(SdekTemplate.AuthPassword);
            if (_method.Params.ContainsKey(SdekTemplate.CalculateTariffs))
                _tariffs = (_method.Params.ElementOrDefault(SdekTemplate.CalculateTariffs) ?? string.Empty).Split(",").ToList();
            else
                _tariffs = (_method.Params.ElementOrDefault(SdekTemplate.TariffOldParam) ?? string.Empty).Split(",").ToList();
            _cityFrom = _method.Params.ElementOrDefault(SdekTemplate.CityFrom);
            _cityFromId = _method.Params.ElementOrDefault(SdekTemplate.CityFromId).TryParseInt(true);
            _deliveryNote = _method.Params.ElementOrDefault(SdekTemplate.DeliveryNote).TryParseInt();
            _statusesSync = method.Params.ElementOrDefault(SdekTemplate.StatusesSync).TryParseBool();
            _allowInspection = method.Params.ElementOrDefault(SdekTemplate.AllowInspection).TryParseBool();
            _showPointsAsList = method.Params.ElementOrDefault(SdekTemplate.ShowPointsAsList).TryParseBool();
            _showSdekWidjet = method.Params.ElementOrDefault(SdekTemplate.ShowSdekWidjet).TryParseBool();
            _showAddressComment = method.Params.ElementOrDefault(SdekTemplate.ShowAddressComment).TryParseBool();
            _withInsure = method.Params.ElementOrDefault(SdekTemplate.WithInsure).TryParseBool();
            _partialDelivery = method.Params.ElementOrDefault(SdekTemplate.PartialDelivery).TryParseBool();
            _tryingOn = method.Params.ElementOrDefault(SdekTemplate.TryingOn).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(SdekTemplate.YaMapsApiKey);
            _sdekApiService20 = new SdekApiService20(_authLogin, _authPassword);

            var newStatusesReference = method.Params.ElementOrDefault(SdekTemplate.StatusesReference);
            if (newStatusesReference == null)
            {
                var oldStatusesReference = string.Join(";", OldStatusesReference.Where(x => x.Value.HasValue).Select(x => $"{x.Key},{x.Value}"));
                newStatusesReference = oldStatusesReference;
                method.Params.TryAddValue(SdekTemplate.StatusesReference, newStatusesReference);
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

        public string CityFrom => _cityFrom;
        public int? CityFromId => _cityFromId;
        public bool AllowInspection => _allowInspection;
        public bool WithInsure => _withInsure;
        public bool PartialDelivery => _partialDelivery;
        public bool TryingOn => _tryingOn;
        public SdekApiService20 SdekApiService20 => _sdekApiService20;

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameSdekOrderUuidInOrderAdditionalData);
            var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameDispatchNumberInOrderAdditionalData);

            GetOrderResult orderResult = null;
            if (sdekOrderUuid.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(sdekOrderUuid.TryParseGuid(), null, null);
            if (orderResult == null && sdekOrderNumber.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(null, sdekOrderNumber, null);

            if (orderResult?.Entity != null)
            {
                if (sdekOrderUuid.IsNullOrEmpty())
                    OrderService.AddUpdateOrderAdditionalData(
                        order.OrderID, 
                        KeyNameSdekOrderUuidInOrderAdditionalData,
                        orderResult.Entity.Uuid.ToString());
                
                if (sdekOrderNumber.IsNullOrEmpty() && orderResult.Entity.CdekNumber.IsNotEmpty())
                {
                    OrderService.AddUpdateOrderAdditionalData(
                        order.OrderID, 
                        KeyNameDispatchNumberInOrderAdditionalData,
                        orderResult.Entity.CdekNumber);
                    if (order.TrackNumber.IsNullOrEmpty())
                    {
                        order.TrackNumber = orderResult.Entity.CdekNumber;
                        OrderService.UpdateOrderMain(order,
                            changedBy: new OrderChangedBy("Синхронизация статусов для СДЭК"));
                    }
                }
                
                if (orderResult.Entity.Statuses != null && orderResult.Entity.Statuses.Count > 0)
                {
                    var lastStatus = orderResult.Entity.Statuses
                        .Where(x => 
                            StatusesReference.ContainsKey(x.Code) 
                            && StatusesReference[x.Code].HasValue)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();
                    
                    var sdekOrderStatus = lastStatus != null && StatusesReference.ContainsKey(lastStatus.Code)
                        ? StatusesReference[lastStatus.Code]
                        : null;

                    if (sdekOrderStatus.HasValue &&
                        order.OrderStatusId != sdekOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(sdekOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date).FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < lastStatus.DateTime)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                sdekOrderStatus.Value, "Синхронизация статусов для СДЭК");
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

        private Dictionary<string, int?> _oldStatusesReference;
        public Dictionary<string, int?> OldStatusesReference => _oldStatusesReference
            ?? (_oldStatusesReference = new Dictionary<string, int?>
                    {
                        { "CREATED", _method.Params.ElementOrDefault(SdekTemplate.StatusCreated).TryParseInt(true)},
                        // { "", _method.Params.ElementOrDefault(SdekTemplate.StatusDeleted).TryParseInt(true)},
                        { "RECEIVED_AT_SHIPMENT_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtWarehouseOfSender).TryParseInt(true)},
                        { "READY_FOR_SHIPMENT_IN_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentFromSenderWarehouse).TryParseInt(true)},
                        { "RETURNED_TO_SENDER_CITY_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToWarehouseOfSender).TryParseInt(true)},
                        { "TAKEN_BY_TRANSPORTER_FROM_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierFromSenderWarehouse).TryParseInt(true)},
                        { "SENT_TO_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToTransitWarehouse).TryParseInt(true)},
                        { "ACCEPTED_IN_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtTransitWarehouse).TryParseInt(true)},
                        { "ACCEPTED_AT_TRANSIT_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtTransitWarehouse).TryParseInt(true)},
                        { "RETURNED_TO_TRANSIT_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToTransitWarehouse).TryParseInt(true)},
                        { "READY_FOR_SHIPMENT_IN_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentInTransitWarehouse).TryParseInt(true)},
                        { "TAKEN_BY_TRANSPORTER_FROM_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierInTransitWarehouse).TryParseInt(true)},
                        { "SENT_TO_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToSenderCity).TryParseInt(true)},
                        { "SENT_TO_RECIPIENT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToWarehouseOfRecipient).TryParseInt(true)},
                        { "ACCEPTED_IN_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtSenderCity).TryParseInt(true)},
                        { "ACCEPTED_IN_RECIPIENT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtConsigneeWarehouse).TryParseInt(true)},
                        { "ACCEPTED_AT_RECIPIENT_CITY_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery).TryParseInt(true)},
                        { "ACCEPTED_AT_PICK_UP_POINT", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient).TryParseInt(true)},
                        { "TAKEN_BY_COURIER", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForDelivery).TryParseInt(true)},
                        { "RETURNED_TO_RECIPIENT_CITY_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToConsigneeWarehouse).TryParseInt(true)},
                        { "DELIVERED", _method.Params.ElementOrDefault(SdekTemplate.StatusAwarded).TryParseInt(true)},
                        { "NOT_DELIVERED", _method.Params.ElementOrDefault(SdekTemplate.StatusNotAwarded).TryParseInt(true)},
                        // { "1", _method.Params.ElementOrDefault(SdekTemplate.StatusCreated).TryParseInt(true)},
                        // { "2", _method.Params.ElementOrDefault(SdekTemplate.StatusDeleted).TryParseInt(true)},
                        // { "3", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtWarehouseOfSender).TryParseInt(true)},
                        // { "6", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentFromSenderWarehouse).TryParseInt(true)},
                        // { "16", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToWarehouseOfSender).TryParseInt(true)},
                        // { "7", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierFromSenderWarehouse).TryParseInt(true)},
                        // { "21", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToTransitWarehouse).TryParseInt(true)},
                        // { "22", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtTransitWarehouse).TryParseInt(true)},
                        // { "13", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtTransitWarehouse).TryParseInt(true)},
                        // { "17", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToTransitWarehouse).TryParseInt(true)},
                        // { "19", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentInTransitWarehouse).TryParseInt(true)},
                        // { "20", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierInTransitWarehouse).TryParseInt(true)},
                        // { "27", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToSenderCity).TryParseInt(true)},
                        // { "8", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToWarehouseOfRecipient).TryParseInt(true)},
                        // { "28", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtSenderCity).TryParseInt(true)},
                        // { "9", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtConsigneeWarehouse).TryParseInt(true)},
                        // { "10", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery).TryParseInt(true)},
                        // { "12", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient).TryParseInt(true)},
                        // { "11", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForDelivery).TryParseInt(true)},
                        // { "18", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToConsigneeWarehouse).TryParseInt(true)},
                        // { "4", _method.Params.ElementOrDefault(SdekTemplate.StatusAwarded).TryParseInt(true)},
                        // { "5", _method.Params.ElementOrDefault(SdekTemplate.StatusNotAwarded).TryParseInt(true)},
                    });

        public static Dictionary<string, string> Statuses => new Dictionary<string, string>
        {
            { "CREATED", "Создан" },
            { "RECEIVED_AT_SHIPMENT_WAREHOUSE", "Принят на склад отправителя" },
            { "READY_FOR_SHIPMENT_IN_SENDER_CITY", "Выдан на отправку в г. отправителе" },
            { "RETURNED_TO_SENDER_CITY_WAREHOUSE", "Возвращен на склад отправителя" },
            { "TAKEN_BY_TRANSPORTER_FROM_SENDER_CITY", "Создаётся в сортировочном центре" },
            // { "DELIVERY_TRACK_RECEIVED", "Сдан перевозчику в г. отправителя" },
            { "SENT_TO_TRANSIT_CITY", "Отправлен в г. транзит" },
            { "ACCEPTED_IN_TRANSIT_CITY", "Встречен в г. транзите" },
            { "ACCEPTED_AT_TRANSIT_WAREHOUSE", "Принят на склад транзита" },
            { "RETURNED_TO_TRANSIT_WAREHOUSE", "Возвращен на склад транзита" },
            { "READY_FOR_SHIPMENT_IN_TRANSIT_CITY", "Выдан на отправку в г. транзите" },
            { "TAKEN_BY_TRANSPORTER_FROM_TRANSIT_CITY", "Сдан перевозчику в г. транзите" },
            { "SENT_TO_SENDER_CITY", "Отправлен в г. отправитель" },
            { "SENT_TO_RECIPIENT_CITY", "Отправлен в г. получатель" },
            { "ACCEPTED_IN_SENDER_CITY", "Встречен в г. отправителе" },
            { "ACCEPTED_IN_RECIPIENT_CITY", "Встречен в г. получателе" },
            { "ACCEPTED_AT_RECIPIENT_CITY_WAREHOUSE", "Принят на склад доставки" },
            { "ACCEPTED_AT_PICK_UP_POINT", "Принят на склад до востребования" },
            { "TAKEN_BY_COURIER", "Выдан на доставку" },
            { "RETURNED_TO_RECIPIENT_CITY_WAREHOUSE", "Возвращен на склад доставки" },
            { "DELIVERED", "Вручен" },
            { "NOT_DELIVERED", "Не вручен" },
            { "IN_CUSTOMS_INTERNATIONAL", "Таможенное оформление в стране отправления" },
            { "SHIPPED_TO_DESTINATION", "Отправлено в страну назначения" },
            { "PASSED_TO_TRANSIT_CARRIER", "Передано транзитному перевозчику" },
            { "IN_CUSTOMS_LOCAL", "Таможенное оформление в стране назначения" },
            { "CUSTOMS_COMPLETE", "Таможенное оформление завершено" },
            { "POSTOMAT_POSTED", "Заложен в постамат" },
            { "POSTOMAT_SEIZED", "Изъят из постамата курьером" },
            { "POSTOMAT_RECEIVED", "Изъят из постамата клиентом" },
        };

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameSdekOrderUuidInOrderAdditionalData);
            var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameDispatchNumberInOrderAdditionalData);

            GetOrderResult orderResult = null;
            if (sdekOrderUuid.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(sdekOrderUuid.TryParseGuid(), null, null);
            if (orderResult == null && sdekOrderNumber.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(null, sdekOrderNumber, null);

            if (orderResult?.Entity?.Statuses != null)
            {
                return orderResult.Entity.Statuses
                    .OrderByDescending(x => x.DateTime)
                    .Select(stateInfo => new HistoryOfMovement()
                    {
                        Code = stateInfo.Code,
                        Name = stateInfo.Name,
                        Date = stateInfo.DateTime,
                        Comment = stateInfo.City
                    }).ToList();
            }

            return null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            LoadSdekNumbers();
        }

        private void LoadSdekNumbers()
        {
            try
            {
                // получаем номер заказа сдэк для заказов, где его еще нет
                var list =
                    SQLDataAccess.ExecuteReadList(@"SELECT oad.* 
                    FROM [Order].[OrderAdditionalData] AS oad
                        INNER JOIN [Order].[Order] AS o ON o.[OrderID] = oad.[OrderID]
                        inner join [Order].[OrderStatus] os ON o.OrderStatusID = os.[OrderStatusID]
                    WHERE oad.[Name] = @KeyNameSdekOrderUuid AND o.OrderDate >= @MinOrderDate AND os.[IsCanceled] = 0 
                        AND os.[IsCompleted] = 0 AND o.[ShippingMethodID] = @ShippingMethodID 
                        AND NOT EXISTS(SELECT * FROM [Order].[OrderAdditionalData] AS oad2 WHERE oad2.[OrderID] = oad.[OrderID] AND oad2.[Name] = @KeyNameDispatchNumber)",
                        CommandType.Text,
                        reader => new Tuple<int, string>(SQLDataHelper.GetInt(reader, "OrderID"),
                            SQLDataHelper.GetString(reader, "Value")),
                        new SqlParameter("@ShippingMethodID", _method.ShippingMethodId),
                        new SqlParameter("@KeyNameSdekOrderUuid", KeyNameSdekOrderUuidInOrderAdditionalData),
                        new SqlParameter("@KeyNameDispatchNumber", KeyNameDispatchNumberInOrderAdditionalData),
                        new SqlParameter("@MinOrderDate", DateTime.Today.AddDays(-3)));

                foreach (var item in list)
                {
                    var orderId = item.Item1;
                    var sdekOrderUuid = item.Item2;

                    var order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        GetOrderResult orderResult = null;

                        if (sdekOrderUuid.IsNotEmpty())
                            orderResult = _sdekApiService20.GetOrder(sdekOrderUuid.TryParseGuid(), null, null);

                        if (orderResult?.Entity != null)
                        {
                            if (orderResult.Entity.CdekNumber.IsNotEmpty())
                            {
                                OrderService.AddUpdateOrderAdditionalData(
                                    order.OrderID,
                                    KeyNameDispatchNumberInOrderAdditionalData,
                                    orderResult.Entity.CdekNumber);
                                order.TrackNumber = orderResult.Entity.CdekNumber;
                                OrderService.UpdateOrderMain(order,
                                    changedBy: new OrderChangedBy("Получение трек-номеров СДЭК"));
                            }
                            else
                            {
                                var requestCreate = orderResult?.Requests?.FirstOrDefault(x => 
                                    string.Equals("CREATE", x.Type, StringComparison.OrdinalIgnoreCase));
                                if (requestCreate?.State.Equals("INVALID", StringComparison.OrdinalIgnoreCase) == true)
                                    OrderService.DeleteOrderAdditionalData(orderId, KeyNameSdekOrderUuidInOrderAdditionalData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var options = new List<BaseShippingOption>();
            if (string.IsNullOrEmpty(_calculationParameters.City) || string.IsNullOrEmpty(_cityFrom))
                return options;

            var sdekCityToId = SdekService.GetSdekCityId(_calculationParameters.City, _calculationParameters.District, _calculationParameters.Region, _calculationParameters.Country, service20: _sdekApiService20, city: out CityInfo cityTo);
            var sdekCityFromId = _cityFromId.HasValue ? _cityFromId.Value : SdekService.GetSdekCityId(_cityFrom, string.Empty, string.Empty, string.Empty, service20: _sdekApiService20, city: out _, allowedObsoleteFindCity: true);

            if (!sdekCityToId.HasValue || !sdekCityFromId.HasValue)
                return options;

            var calcTarifs = SdekTariffs.Tariffs.Where(item => _tariffs.Contains(item.TariffId.ToString())).ToList();
            // var dateExecute = DateTime.Now.Date.ToString("yyyy-MM-dd");
            var totalWeightInKg = GetTotalWeight();
            var totalWeightInG = (int)GetTotalWeight(1000);
            var dimensionsInSm = GetDimensions(rate:10);
            var pointsCity = calculationVariants.HasFlag(CalculationVariants.PickPoint) && calcTarifs.Any(x => x.Mode.EndsWith("-С") || x.Mode.EndsWith("-П")) 
                ? PreparePoint(sdekCityToId.Value, totalWeightInKg, dimensionsInSm)
                : null;
             var goods = PrepareGoods(dimensionsInSm, totalWeightInKg);

            var services = new List<ServiceParams>();
            // if (_withInsure)
            services.Add(new ServiceParams { Code = "INSURANCE", Parameter = ((int)Math.Ceiling(_totalPrice)).ToString() });

            foreach (var sdekTariff in calcTarifs)
            {
                var tariffIsPvz = !sdekTariff.Mode.EndsWith("-Д");
                if (tariffIsPvz && !calculationVariants.HasFlag(CalculationVariants.PickPoint))
                    continue;;
                if (!tariffIsPvz && !calculationVariants.HasFlag(CalculationVariants.Courier))
                    continue;;
                
                if ((sdekTariff.MaxWeight.HasValue && sdekTariff.MaxWeight.Value < totalWeightInKg) ||
                    (sdekTariff.MinWeight.HasValue && sdekTariff.MinWeight.Value > totalWeightInKg))
                    continue;

                var typePoints = sdekTariff.Mode.EndsWith("-С")
                    ? "PVZ"
                    : sdekTariff.Mode.EndsWith("-П")
                        ? "POSTAMAT"
                        : null;

                var points = typePoints.IsNotEmpty() && pointsCity != null
                    ? pointsCity.Where(x => x.Type == typePoints).ToList()
                    : null;
                if (sdekTariff == null || ((sdekTariff.Mode.EndsWith("-С") || sdekTariff.Mode.EndsWith("-П")) && (points == null || points.Count == 0)))
                    continue;

                var serviceTariff = services.ToList(); //clone list
                if (_allowInspection && typePoints != "POSTAMAT")
                    serviceTariff.Add(new ServiceParams {Code = "INSPECTION_CARGO"});
                if (_tryingOn && typePoints != "POSTAMAT")
                    serviceTariff.Add(new ServiceParams {Code = "TRYING_ON"});
                if (_partialDelivery && typePoints != "POSTAMAT")
                    serviceTariff.Add(new ServiceParams {Code = "PART_DELIV"});
                
                var calculateResult = _sdekApiService20.CalculatorTariff(new TariffParams()
                {
                    Type = 1,
                    TariffCode = sdekTariff.TariffId,
                    FromLocation = new TariffParamsLocation() {Code = sdekCityFromId.Value},
                    ToLocation = new TariffParamsLocation() {Code = sdekCityToId.Value},
                    Services = serviceTariff,
                    Packages = new List<TariffParamsPackage>()
                    {
                        new TariffParamsPackage()
                        {
                            Weight = totalWeightInG,
                            Length = (long)Math.Ceiling(dimensionsInSm[0]),
                            Width = (long)Math.Ceiling(dimensionsInSm[1]),
                            Height = (long)Math.Ceiling(dimensionsInSm[2]),
                        }
                    }
                });

                if (calculateResult != null && calculateResult.Errors == null)
                {
                    if (!string.Equals(
                        calculateResult.Currency, 
                        _method.ShippingCurrency?.Iso3, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log.Warn(
                            $"Sdek: Валюты не совпадают. Указана {_method.ShippingCurrency?.Iso3}, " +
                            $"а возвращается {calculateResult.Currency}. Метод {_method.ShippingMethodId}.");
                        continue;
                    }
                    
                    string selectedPickpointId = null;
                    if (_calculationParameters.ShippingOption != null &&
                        _calculationParameters.ShippingOption.ShippingType ==
                        ((ShippingKeyAttribute)typeof(Sdek).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        if (_calculationParameters.ShippingOption.GetType() == typeof(SdekOption) && ((SdekOption)_calculationParameters.ShippingOption).SelectedPoint != null)
                            selectedPickpointId = ((SdekOption)_calculationParameters.ShippingOption).SelectedPoint.Id;

                        if (_calculationParameters.ShippingOption.GetType() == typeof(SdekWidjetOption))
                            selectedPickpointId = ((SdekWidjetOption)_calculationParameters.ShippingOption).PickpointId;

                        if (_calculationParameters.ShippingOption.GetType() == typeof(SdekPointDeliveryMapOption))
                            selectedPickpointId = ((SdekPointDeliveryMapOption)_calculationParameters.ShippingOption).PickpointId;
                    }

                    var insurePrice = calculateResult?.Services?.FirstOrDefault(x =>
                        string.Equals(x.Code, "INSURANCE", StringComparison.OrdinalIgnoreCase))?.Sum;
                    var basePrice = _withInsure
                        ? calculateResult.TotalSum
                        : calculateResult.TotalSum - (insurePrice ?? 0f);
                    var priceCash = calculateResult.TotalSum;

                    if (!tariffIsPvz || _showPointsAsList || _yaMapsApiKey.IsNullOrEmpty())
                        options.Add(CreateOption(points, sdekTariff, calculateResult, selectedPickpointId, tariffIsPvz, 
                            cityTo, basePrice, priceCash, insurePrice));
                    else
                    {
                        if (_showSdekWidjet)
                            options.Add(CreateWidjetOption(points, goods, sdekTariff, calculateResult, selectedPickpointId, 
                                cityTo, basePrice, priceCash, insurePrice));
                        else
                            options.Add(CreatePointDeliveryMapOption(sdekCityToId, totalWeightInKg, dimensionsInSm, points, 
                                typePoints, sdekTariff, calculateResult, cityTo, basePrice, priceCash, insurePrice));
                    }
                }
            }

            return options;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
        {
            if (string.IsNullOrEmpty(_cityFrom))
                return null;
            
            var totalWeightInKg = GetTotalWeight();
            var calcTarifs = SdekTariffs.Tariffs
                                         // только тарифы самовывоза
                                        .Where(tariff => tariff.Mode.EndsWith("-С") || tariff.Mode.EndsWith("-П"))
                                         // только выбранные тарифы
                                        .Where(tariff => _tariffs.Contains(tariff.TariffId.ToString()))
                                         // перевозящие такой вес
                                        .Where(tariff => tariff.MaxWeight is null || tariff.MaxWeight.Value >= totalWeightInKg)
                                        .Where(tariff => tariff.MinWeight is null || tariff.MinWeight.Value <= totalWeightInKg)
                                        .ToList();

            if (calcTarifs.Count == 0)
                return null;
            
            var sdekCityFromId = _cityFromId.HasValue ? _cityFromId.Value : SdekService.GetSdekCityId(_cityFrom, string.Empty, string.Empty, string.Empty, service20: _sdekApiService20, city: out _, allowedObsoleteFindCity: true);
            if (!sdekCityFromId.HasValue)
                return null;

            var dimensionsInInCentimeter = GetDimensions(rate:10);
            var deliveryPoint = GetCityPoints(null)
                               // указанная точна
                               .Where(point => string.Equals(point.Code, pointId, StringComparison.OrdinalIgnoreCase))
                               // примет посылку такого веса
                               .Where(point => !point.WeightMax.HasValue || point.WeightMax >= totalWeightInKg)
                               .Where(point => !point.WeightMin.HasValue || point.WeightMin < totalWeightInKg)
                               // примет посылку таких габаритов
                               .Where(point => point.Dimensions == null || point.Dimensions.Any(d =>
                                    dimensionsInInCentimeter[2] <= d.Height &&
                                    dimensionsInInCentimeter[0] <= d.Depth &&
                                    dimensionsInInCentimeter[1] <= d.Width))
                               .FirstOrDefault();
            if (deliveryPoint is null)
                return null;
            
            var options = new List<BaseShippingOption>();
            var services = new List<ServiceParams>();
            // if (_withInsure)
            services.Add(new ServiceParams { Code = "INSURANCE", Parameter = ((int)Math.Ceiling(_totalPrice)).ToString() });
            var totalWeightInGramms = (int)GetTotalWeight(1000);
            var cityTo = _sdekApiService20.GetCities(new CitiesFilter()
                                           {
                                               Code = deliveryPoint.Location.CityCode
                                           })
                                         ?.FirstOrDefault();
            
            if (cityTo is null)
                return options;

            foreach (var sdekTariff in calcTarifs)
            {
                var typePoint =
                    sdekTariff.Mode.EndsWith("-С")
                        ? "PVZ"
                        : sdekTariff.Mode.EndsWith("-П")
                            ? "POSTAMAT"
                            : null;

                if (deliveryPoint.Type != typePoint)
                    continue;
       
                var serviceTariff = services.ToList(); //clone list
                if (_allowInspection && typePoint != "POSTAMAT")
                    serviceTariff.Add(new ServiceParams {Code = "INSPECTION_CARGO"});
                if (_tryingOn && typePoint != "POSTAMAT")
                    serviceTariff.Add(new ServiceParams {Code = "TRYING_ON"});
                if (_partialDelivery && typePoint != "POSTAMAT")
                    serviceTariff.Add(new ServiceParams {Code = "PART_DELIV"});

                var calculateResult = _sdekApiService20.CalculatorTariff(new TariffParams()
                {
                    Type = 1,
                    TariffCode = sdekTariff.TariffId,
                    FromLocation = new TariffParamsLocation() {Code = sdekCityFromId.Value},
                    ToLocation = new TariffParamsLocation() {Code = cityTo.Code},
                    Services = serviceTariff,
                    Packages = new List<TariffParamsPackage>()
                    {
                        new TariffParamsPackage()
                        {
                            Weight = totalWeightInGramms,
                            Length = (long)Math.Ceiling(dimensionsInInCentimeter[0]),
                            Width = (long)Math.Ceiling(dimensionsInInCentimeter[1]),
                            Height = (long)Math.Ceiling(dimensionsInInCentimeter[2]),
                        }
                    }
                });      

                if (calculateResult != null && calculateResult.Errors == null)
                {
                    if (!string.Equals(
                        calculateResult.Currency, 
                        _method.ShippingCurrency?.Iso3, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log.Warn(
                            $"Sdek: Валюты не совпадают. Указана {_method.ShippingCurrency?.Iso3}, " +
                            $"а возвращается {calculateResult.Currency}. Метод {_method.ShippingMethodId}.");
                        continue;
                    }
                    
                    var insurePrice = calculateResult?.Services?.FirstOrDefault(x =>
                        string.Equals(x.Code, "INSURANCE", StringComparison.OrdinalIgnoreCase))?.Sum;
                    var basePrice = _withInsure
                        ? calculateResult.TotalSum
                        : calculateResult.TotalSum - (insurePrice ?? 0f);
                    var priceCash = calculateResult.TotalSum;

                    options.Add(
                        CreateOption(points: new List<SdekShippingPoint>() {CastPoint(deliveryPoint, Localization.Culture.GetCulture())},
                            sdekTariff, calculateResult, deliveryPoint.Code, true, cityTo, basePrice, priceCash,
                            insurePrice));
                }
            }
 
            return options;
        }

        private SdekPointDeliveryMapOption CreatePointDeliveryMapOption(int? sdekCityToId, float goodsWeight,
            float[] dimensionsInSm, List<SdekShippingPoint> points, string typePoints, SdekTariff sdekTariff, 
            TariffResult calculateResult, CityInfo cityTo, float basePrice, float priceCash, float? insurePrice)
        {
            var option = new SdekPointDeliveryMapOption(_method, _totalPrice)
            {
                Name = _method.Name + (sdekTariff.Mode.EndsWith("-С") ? LocalizationService.GetResource("Core.Services.Shipping.PickupPointsWithSpace") : LocalizationService.GetResource("Core.Services.Shipping.ParcelLockersWithSpace")),
                DeliveryId = sdekTariff.TariffId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,

                DeliveryTime = (calculateResult.PeriodMax > calculateResult.PeriodMin
                                   ? (calculateResult.PeriodMin + _method.ExtraDeliveryTime) + "-"
                                   : string.Empty)
                               + (calculateResult.PeriodMax + _method.ExtraDeliveryTime) + " дн.",

                CurrentPoints = points,
                CityTo = sdekCityToId.Value,
                IsAvailablePaymentCashOnDelivery = insurePrice.HasValue && (cityTo == null || !cityTo.PaymentLimit.HasValue || cityTo.PaymentLimit.Value == -1f || cityTo.PaymentLimit.Value >= _totalPrice),
                CalculateOption = new SdekCalculateOption
                {
                    TariffId = sdekTariff.TariffId, 
                    WithInsure = _withInsure, 
                    AllowInspection = _allowInspection,
                    PartialDelivery = _partialDelivery,
                    TryingOn = _tryingOn
                }
            };

            //shippingOption.IsAvailableCashOnDelivery &= реализованно в SdekPointDeliveryMapOption

            SetMapData(option, goodsWeight, dimensionsInSm, typePoints);
            return option;
        }

        private SdekWidjetOption CreateWidjetOption(List<SdekShippingPoint> points, List<SdekGoods> goods,
            SdekTariff sdekTariff, TariffResult calculateResult, string selectedPickpointId, CityInfo cityTo, 
            float basePrice, float priceCash, float? insurePrice)
        {
            var shippingOption = new SdekWidjetOption(_method, _totalPrice)
            {
                Name = _method.Name + (sdekTariff.Mode.EndsWith("-С") ? LocalizationService.GetResource("Core.Services.Shipping.PickupPointsWithSpace") : LocalizationService.GetResource("Core.Services.Shipping.ParcelLockersWithSpace")),
                DeliveryId = sdekTariff.TariffId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,

                DeliveryTime = (calculateResult.PeriodMax > calculateResult.PeriodMin
                                   ? (calculateResult.PeriodMin + _method.ExtraDeliveryTime) + "-"
                                   : string.Empty)
                               + (calculateResult.PeriodMax + _method.ExtraDeliveryTime) + " дн.",

                TariffId = sdekTariff.TariffId.ToString(),
                WidgetConfigParams = ConfigWidget(goods),
                CurrentPoints = points, // не меняю тип точек, чтобы после обновления у клиента не падал checkout 500 при конвертации типа из бд
                IsAvailablePaymentCashOnDelivery = insurePrice.HasValue && (cityTo == null || !cityTo.PaymentLimit.HasValue || cityTo.PaymentLimit.Value == -1f || cityTo.PaymentLimit.Value >= _totalPrice),
                CalculateOption = new SdekCalculateOption
                {
                    TariffId = sdekTariff.TariffId, 
                    WithInsure = _withInsure, 
                    AllowInspection = _allowInspection,
                    PartialDelivery = _partialDelivery,
                    TryingOn = _tryingOn
                }
            };

            SdekShippingPoint point;
            shippingOption.IsAvailablePaymentCashOnDelivery &= selectedPickpointId != null
                ? points.Any(
                    p => (p.AvailableCashOnDelivery is true || p.AvailableCardOnDelivery is true) && p.Id == selectedPickpointId)
                : (point = points.First()).AvailableCashOnDelivery is true || point.AvailableCardOnDelivery is true;
            return shippingOption;
        }

        private SdekOption CreateOption(List<SdekShippingPoint> points, SdekTariff sdekTariff, TariffResult calculateResult,
            string selectedPickpointId, bool tariffIsPvz, CityInfo cityTo, float basePrice, float priceCash, float? insurePrice)
        {
            var shippingOption = new SdekOption(_method, _totalPrice)
            {
                Name = _method.Name + (tariffIsPvz ? (sdekTariff.Mode.EndsWith("-С") ? LocalizationService.GetResource("Core.Services.Shipping.PickupPointsWithSpace") : LocalizationService.GetResource("Core.Services.Shipping.ParcelLockersWithSpace")): LocalizationService.GetResource("Core.Services.Shipping.ByCourierWithSpace")),
                DeliveryId = sdekTariff.TariffId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,

                DeliveryTime = (calculateResult.PeriodMax > calculateResult.PeriodMin
                                   ? (calculateResult.PeriodMin + _method.ExtraDeliveryTime) + "-"
                                   : string.Empty)
                               + (calculateResult.PeriodMax + _method.ExtraDeliveryTime) + " дн.",

                ShippingPoints = tariffIsPvz ? points.Select(point => (BaseShippingPoint)point).ToList() : null, // не меняю тип точек, чтобы после обновления у клиента не падал checkout 500 при конвертации типа из бд
                TariffId = sdekTariff.TariffId.ToString(),
                //tariffIsPvz,
                HideAddressBlock = tariffIsPvz,
                IsAvailablePaymentCashOnDelivery = insurePrice.HasValue && (cityTo?.PaymentLimit == null || cityTo.PaymentLimit.Value == -1f || cityTo.PaymentLimit.Value >= _totalPrice),
                CalculateOption = new SdekCalculateOption
                {
                    TariffId = sdekTariff.TariffId, 
                    WithInsure = _withInsure, 
                    AllowInspection = _allowInspection,
                    PartialDelivery = _partialDelivery,
                    TryingOn = _tryingOn
                }
            };

            if (tariffIsPvz)
                shippingOption.IsAvailablePaymentCashOnDelivery &= selectedPickpointId != null
                    ? points.Any(
                        point => (point.AvailableCashOnDelivery is true || point.AvailableCardOnDelivery is true) && point.Id == selectedPickpointId)
                    : points[0].AvailableCashOnDelivery is true || points[0].AvailableCardOnDelivery is true;
            return shippingOption;
        }

        private void SetMapData(SdekPointDeliveryMapOption option, float goodsWeight, float[] dimensionsInSm, string typePoints)
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
                    { "city", option.CityTo },
                    { "weight", goodsWeight.ToInvariantString() },
                    { "dimensions", string.Join("x", dimensionsInSm.Select(x => x.ToInvariantString())) },
                    { "typePoints", typePoints }
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null)
                return null;

            var city = data["city"].ToString().TryParseInt();
            var goodsWeight = data["weight"].ToString().TryParseFloat();
            var dimensionsInSm = data["dimensions"].ToString().Split('x').Select(x => x.TryParseFloat()).ToArray();
            var typePoints = (string)data["typePoints"];
            var points = typePoints.IsNotEmpty()
                ? PreparePoint(city, goodsWeight, dimensionsInSm).Where(x => x.Type.Equals(typePoints)).ToList()
                : null;

            return GetFeatureCollection(points ?? new List<SdekShippingPoint>());
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<SdekShippingPoint> points)
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
                                    p.TimeWorkStr,
                                    p.TimeWorkStr.IsNotEmpty() ? "<br>" : "",
                                    intId,
                                    p.Id),
                            BalloonContentFooter = _showAddressComment
                                ? p.Description
                                : null
                        }
                    };
                }).ToList()
            };
        }

        private List<SdekGoods> PrepareGoods(float[] dimensions, float weight)
        {
            var goods = new List<SdekGoods>();
        
            goods.Add(new SdekGoods
            {
                Length = Math.Ceiling(dimensions[0]).ToString(CultureInfo.InvariantCulture),
                Width = Math.Ceiling(dimensions[1]).ToString(CultureInfo.InvariantCulture),
                Height = Math.Ceiling(dimensions[2]).ToString(CultureInfo.InvariantCulture),
                Weight = weight.ToInvariantString(),
            });
        
            return goods;
        }

        private List<DeliveryPoint> GetCityPoints(int? sdekCityId)
        {
            return CacheManager.Get("Sdek_PreparePoint_" + sdekCityId, 60 * 24, () =>
            {
                var result = _sdekApiService20.GetDeliveryPoints(new DeliveryPointsFilter
                {
                    CityCode = sdekCityId,
                    Type = "ALL",
                    IsHandout = true
                });
                return result;
            });
        }

        private List<SdekShippingPoint> PreparePoint(int sdekCityToId, float goodsWeight, float[] dimensions)
        {
            var listShippingPoints = new List<SdekShippingPoint>();

            var sdekPvzList = GetCityPoints(sdekCityToId);

            if (sdekPvzList == null)
                return null;

            var currentCulture = Localization.Culture.GetCulture();
            foreach (var sdekPoint in sdekPvzList
                                     .Where(item =>
                                          ((!item.WeightMax.HasValue || item.WeightMax >= goodsWeight) &&
                                           (!item.WeightMin.HasValue || item.WeightMin < goodsWeight))
                                          && (item.Dimensions == null || item.Dimensions.Any(d =>
                                              dimensions[2] <= d.Height &&
                                              dimensions[0] <= d.Depth &&
                                              dimensions[1] <= d.Width))
                                      )
                                     .OrderBy(x => x.Location.Address))
            {
                listShippingPoints.Add(CastPoint(sdekPoint, currentCulture));
            }

            return listShippingPoints;
        }

        private Dictionary<string, object> ConfigWidget(List<SdekGoods> goods)
        {
            var widgetConfigData = new Dictionary<string, object>();

            widgetConfigData.Add("defaultCity", _calculationParameters.City);
            widgetConfigData.Add("cityFrom", _cityFrom);
            if (_calculationParameters.Country.IsNotEmpty())
                widgetConfigData.Add("country", _calculationParameters.Country);

            widgetConfigData.Add("goods", goods);
            widgetConfigData.Add("apikey", _yaMapsApiKey);

            return widgetConfigData;
        }


        public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude, float topLeftLongitude, float bottomRightLatitude,
            float bottomRightLongitude)
        {
            var totalWeightInKg = GetTotalWeight();
            var calcTarifs = SdekTariffs.Tariffs
                                        .Where(tariff => _tariffs.Contains(tariff.TariffId.ToString()))
                                        .Where(tariff => tariff.MaxWeight is null || tariff.MaxWeight.Value >= totalWeightInKg)
                                        .Where(tariff => tariff.MinWeight is null || tariff.MinWeight.Value <= totalWeightInKg)
                                        .ToList();
            
            var isAvailablePvz = calcTarifs.Any(x => x.Mode.EndsWith("-С"));
            var isAvailablePostamat = calcTarifs.Any(x => x.Mode.EndsWith("-П"));
            
            if (isAvailablePvz is false && isAvailablePostamat is false)
                return null;
            
            var dimensionsInInCentimeter = GetDimensions(rate:10);
            var currentCulture = Localization.Culture.GetCulture();

            return GetCityPoints(null)
                  .Where(point => topLeftLatitude > point.Location.Latitude)
                  .Where(point => topLeftLongitude < point.Location.Longitude)
                  .Where(point => bottomRightLatitude < point.Location.Latitude)
                  .Where(point => bottomRightLongitude > point.Location.Longitude)
                  .Where(point => !point.WeightMax.HasValue || point.WeightMax >= totalWeightInKg)
                  .Where(point => !point.WeightMin.HasValue || point.WeightMin < totalWeightInKg)
                  .Where(point => point.Dimensions == null || point.Dimensions.Any(d =>
                       dimensionsInInCentimeter[2] <= d.Height &&
                       dimensionsInInCentimeter[0] <= d.Depth &&
                       dimensionsInInCentimeter[1] <= d.Width))
                  .Where(point => isAvailablePostamat || point.Type != "POSTAMAT")
                  .Where(point => isAvailablePvz || point.Type != "PVZ")
                  .Select(point => CastPoint(point, currentCulture));
        }

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            var currentCulture = Localization.Culture.GetCulture();
            return GetCityPoints(null)
                  .Where(point => string.Equals(point.Code, pointId, StringComparison.OrdinalIgnoreCase))
                  .Select(point => CastPoint(point, currentCulture))
                  .FirstOrDefault();
        }
        
        private SdekShippingPoint CastPoint(DeliveryPoint point, CultureInfo currentCulture)
        {
            var firstDimensions = point.Dimensions?.FirstOrDefault();
            var baseShippingPoint2 = new SdekShippingPoint
            {
                Id = point.Code,
                Code = point.Code,
                Name = point.Name,
                Address = point.Location?.Address,
                Description = point.AddressComment,
                Longitude = point.Location?.Longitude,
                Latitude = point.Location?.Latitude,
                AvailableCashOnDelivery = point.AllowedCod && point.HaveCash,
                AvailableCardOnDelivery = point.AllowedCod && point.HaveCashless,
                TimeWorkStr = point.WorkTime,
                Phones = point.Phones?
                              .Select(x =>
                               {
                                   var additional = x.Additional.IsNotEmpty() ? $"({x.Additional})" : null;
                                   return $"{x.Number}{additional}";
                               })
                              .ToArray(),
                MaxWeightInGrams =
                    point.WeightMax.HasValue
                        ? MeasureUnits.ConvertWeight(point.WeightMax.Value, MeasureUnits.WeightUnit.Kilogramm, MeasureUnits.WeightUnit.Grams)
                        : (float?) null,
                MaxHeightInMillimeters =
                    firstDimensions != null
                        ? MeasureUnits.ConvertLength(firstDimensions.Height, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                        : (float?) null,
                MaxWidthInMillimeters =
                    firstDimensions != null
                        ? MeasureUnits.ConvertLength(firstDimensions.Width, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                        : (float?) null,
                MaxLengthInMillimeters =
                    firstDimensions != null
                        ? MeasureUnits.ConvertLength(firstDimensions.Depth, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                        : (float?) null,
                Type = point.Type,
            };
            SetTimeWork(baseShippingPoint2, point.WorkTimeList, currentCulture);

            return baseShippingPoint2;
        }

        private void SetTimeWork(SdekShippingPoint baseShippingPoint2, List<WorkTimeList> pointWorkTimeList, CultureInfo currentCulture)
        {
            if (pointWorkTimeList?.Count > 0 is false)
                return;

            baseShippingPoint2.TimeWork =
                pointWorkTimeList
                   .GroupBy(workTime => workTime.Time)
                   .Select(workTimeGroup =>
                    {
                        var timeParts = workTimeGroup.Key.Split('/');

                        if (timeParts.Length == 2
                            && TimeSpan.TryParse(timeParts[0], out var timeFrom)
                            && TimeSpan.TryParse(timeParts[1], out var timeTo))
                        {
                            var minDayOfWeek = workTimeGroup.Min(x => x.Day);
                            var maxDayOfWeek = workTimeGroup.Max(x => x.Day);
                            return new TimeWork
                            {
                                From = timeFrom,
                                To = timeTo,
                                Label = 
                                    minDayOfWeek == maxDayOfWeek
                                    ? currentCulture.DateTimeFormat.GetAbbreviatedDayName(maxDayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)maxDayOfWeek)
                                    : string.Format("{0}-{1}", 
                                        currentCulture.DateTimeFormat.GetAbbreviatedDayName(minDayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)minDayOfWeek),
                                        currentCulture.DateTimeFormat.GetAbbreviatedDayName(maxDayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)maxDayOfWeek))
                            };
                        }

                        return null;
                    })
                   .Where(timeWork => timeWork != null)
                   .ToList();

            if (baseShippingPoint2.TimeWork.Count == 0)
                baseShippingPoint2.TimeWork = null;
        }
    }
}