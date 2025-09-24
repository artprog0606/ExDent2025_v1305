using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping.Shiptor.Api;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Shiptor
{
    [ShippingKey("Shiptor")]
    public class Shiptor : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingSupportingPaymentCashOnDelivery, IShippingWithBackgroundMaintenance
    {
        #region Ctor

        private readonly string _apiKey;
        private readonly int? _paymentCodCardId;
        private readonly bool _withInsure;
        private readonly string _pickupName;
        private readonly bool _statusesSync;
        private readonly string _yaMapsApiKey;
        private readonly ShiptorCheckoutApiService _shiptorCheckoutApiService;

        public const string KeyNameOrderShiptorIdInOrderAdditionalData = "OrderShiptorId";

        public const string ServiceCostTypeShipping = "shipping";
        public const string ServiceCostTypeCashOnDelivery = "cod";
        public const string ServiceCostTypeDeclaredCost = "cost_declaring";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Shiptor(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _apiKey = _method.Params.ElementOrDefault(ShiptorTemplate.ApiKey);
            _paymentCodCardId = _method.Params.ElementOrDefault(ShiptorTemplate.PaymentCodCardId).TryParseInt(true);
            _withInsure = method.Params.ElementOrDefault(ShiptorTemplate.WithInsure).TryParseBool();
            _pickupName = _method.Params.ElementOrDefault(ShiptorTemplate.PickupName);
            _statusesSync = method.Params.ElementOrDefault(ShiptorTemplate.StatusesSync).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(ShiptorTemplate.YaMapsApiKey);

            _shiptorCheckoutApiService = new ShiptorCheckoutApiService(_apiKey);

            var statusesReference = method.Params.ElementOrDefault(ShiptorTemplate.StatusesReference);
            if (!string.IsNullOrEmpty(statusesReference))
            {
                string[] arr = null;
                StatusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToDictionary(x => (arr = x.Split(','))[0],
                                x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                StatusesReference = new Dictionary<string, int?>();
        }

        public ShiptorCheckoutApiService ShiptorCheckoutApiService
        {
            get { return _shiptorCheckoutApiService; }
        }

        public int? PaymentCodCardId { get { return _paymentCodCardId; } }

        public float DefaultWeight { get { return _defaultWeight; } }

        public float DefaultHeight { get { return _defaultHeight; } }

        public float DefaultLength { get { return _defaultLength; } }

        public float DefaultWidth { get { return _defaultWidth; } }

        public bool WithInsure { get { return _withInsure; } }
        
        #endregion

        #region Satuses

        public bool StatusesSync
        {
            get { return _statusesSync; }
        }

        public bool SyncByAllOrders => false;
        public void SyncStatusOfOrders(IEnumerable<Order> orders) => throw new NotImplementedException();

        public Dictionary<string, int?> StatusesReference { get; private set; }

        public void SyncStatusOfOrder(Order order)
        {
            var shiptorOrderId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderShiptorIdInOrderAdditionalData);
            if (shiptorOrderId.IsNotEmpty())
            {
                var package = _shiptorCheckoutApiService.GetPackage(new GetPackageParams { Id = shiptorOrderId.TryParseInt() }) ??
                              _shiptorCheckoutApiService.GetPackage(new GetPackageParams { OrderNumber = order.Number });
                if (package != null)
                {
                    var shiptorOrderStatus = StatusesReference.ContainsKey(package.Status)
                        ? StatusesReference[package.Status]
                        : null;

                    if (shiptorOrderStatus.HasValue &&
                        order.OrderStatusId != shiptorOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(shiptorOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date).FirstOrDefault();

                        var shiptorStatus = 
                            _shiptorCheckoutApiService.GetStatusList()
                                .Where(x => x.Value.Code == package.Status)
                                .Select(x => x.Value)
                                .FirstOrDefault();

                        var shiptorChangeStatus = shiptorStatus != null && package.History != null
                            ? package.History.FirstOrDefault(x => shiptorStatus.Name.Equals(x.Event, StringComparison.OrdinalIgnoreCase))
                            : null;

                        if (shiptorChangeStatus != null 
                            && (lastOrderStatusHistory == null 
                                || lastOrderStatusHistory.Date < shiptorChangeStatus.Date))
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                shiptorOrderStatus.Value, "Синхронизация статусов для Shiptor");
                        }
                    }

                    // подтягиваем трек-номер (при создании он не генерируется)
                    if (package.ExternalTrackingNumber.IsNotEmpty() && order.TrackNumber.IsNullOrEmpty())
                    {
                        order.TrackNumber = package.ExternalTrackingNumber;
                        OrderService.UpdateOrderMain(order,
                            changedBy: new OrderChangedBy("Синхронизация статусов для Shiptor"));
                    }
                }
            }
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
                var ordersAndShiptorId =
                    SQLDataAccess.ExecuteReadList(@"SELECT o.*, oad.[Value] as 'ShiptorId'
                    FROM [Order].[Order] AS o
                        INNER JOIN [Order].[OrderAdditionalData] AS oad ON o.[OrderID] = oad.[OrderID]
                        inner join [Order].[OrderStatus] os ON o.OrderStatusID = os.[OrderStatusID]
                    WHERE oad.[Name] = @KeyNameOrderShiptorId AND o.OrderDate >= @MinOrderDate AND os.[IsCanceled] = 0 
                        AND os.[IsCompleted] = 0 AND o.[ShippingMethodID] = @ShippingMethodID
                        AND (o.TrackNumber IS NULL OR o.TrackNumber = '') ",
                        CommandType.Text,
                        reader => new Tuple<Order, string>(OrderService.GetOrderFromReader(reader),
                            SQLDataHelper.GetString(reader, "ShiptorId")),
                        new SqlParameter("@ShippingMethodID", _method.ShippingMethodId),
                        new SqlParameter("@KeyNameOrderShiptorId", KeyNameOrderShiptorIdInOrderAdditionalData),
                        new SqlParameter("@MinOrderDate", DateTime.Today.AddMonths(-3)));

                foreach (var orderAndShiptorId in ordersAndShiptorId)
                    UpdateTrackNumberOrder(orderAndShiptorId.Item1, orderAndShiptorId.Item2);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        private void UpdateTrackNumberOrder(Order order, string shiptorOrderId)
        {
            if (shiptorOrderId.IsNotEmpty())
            {
                var package = _shiptorCheckoutApiService.GetPackage(new GetPackageParams { Id = shiptorOrderId.TryParseInt() }) ??
                              _shiptorCheckoutApiService.GetPackage(new GetPackageParams { OrderNumber = order.Number });
                if (package != null)
                {
                    // подтягиваем трек-номер (при создании он не генерируется)
                    if (package.ExternalTrackingNumber.IsNotEmpty() && order.TrackNumber.IsNullOrEmpty())
                    {
                        order.TrackNumber = package.ExternalTrackingNumber;
                        OrderService.UpdateOrderMain(order,
                            changedBy: new OrderChangedBy("Получение трек-номеров Shiptor"));
                    }
                }
            }
        }
        
        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var shippingOptions = new List<BaseShippingOption>();

            string countryIso2;
            var kladrKodCity = GetKladrKodCity(out countryIso2);

            if (kladrKodCity.IsNotEmpty())
            {
                var orderCost = _totalPrice;
                var weight = GetTotalWeight();
                var dimensions = GetDimensions(rate: 10);

                var calcParams = new SimpleCalculateParams
                {
                    Height = dimensions[0],
                    Width = dimensions[1],
                    Length = dimensions[2],
                    Weight = weight,
                    CashOnDelivery = 0,
                    DeclaredCost = _withInsure ? orderCost : 10f,
                    SettlementToKladrId = kladrKodCity,
                    Basket = _items.Select(x => new BasketItem
                    {
                        ArtNo = x.ArtNo,
                        Price = x.Price,
                        Amount = (int) x.Amount,
                        Length = x.Length / 10,
                        Height = x.Height / 10,
                        Width = x.Width / 10,
                        Weight = x.Weight
                    }).ToList()
                };

                var shippings = _shiptorCheckoutApiService.SimpleCalculate(calcParams);

                calcParams.CashOnDelivery = orderCost;
                calcParams.DeclaredCost = orderCost;
                var shippingsWithCod = _shiptorCheckoutApiService.SimpleCalculate(calcParams);

                if (shippings != null && shippings.Methods != null)
                {
                    ShiptorWidgetOption shiptorPickpoint = null;
                    var pickpointMethods = new List<CalculateMethod>();
                    var pickpointMethodsWithCod = new List<CalculateMethod>();

                    foreach (var shipMethod in shippings.Methods)
                    {
                        var shipMethodWithCod = shippingsWithCod != null && shippingsWithCod.Methods != null
                            ? shippingsWithCod.Methods.FirstOrDefault(cm => cm.Method.Id == shipMethod.Method.Id)
                            : null;

                        if (shipMethod.Method.Category == EnTypeCategory.DeliveryPoint ||
                            shipMethod.Method.Category == EnTypeCategory.DeliveryPointToDeliveryPoint ||
                            shipMethod.Method.Category == EnTypeCategory.DoorToDeliveryPoint)
                        {
                            if (!calculationVariants.HasFlag(CalculationVariants.PickPoint))
                                continue;
                            
                            if (shiptorPickpoint == null)
                                shiptorPickpoint = new ShiptorWidgetOption(_method, _totalPrice)
                                {
                                    DeliveryId = "Самовывоз".GetHashCode(),
                                    Name = string.Format("{1}", _method.Name, string.IsNullOrWhiteSpace(_pickupName) ? "Самовывоз" : _pickupName),
                                    PaymentCodCardId = _paymentCodCardId,
                                    CurrentKladrId = kladrKodCity
                                };

                            pickpointMethods.Add(shipMethod);

                            if (shipMethodWithCod != null)
                                pickpointMethodsWithCod.Add(shipMethodWithCod);

                            continue;
                        }
                        
                        if (!calculationVariants.HasFlag(CalculationVariants.Courier))
                            continue;

                        var rate = GetDeliverySum(shipMethod.Cost, withInsurance: true, cachOnDelivery: false);
                        var rateCash = shipMethodWithCod != null
                            ? GetDeliverySum(shipMethodWithCod.Cost, withInsurance: true, cachOnDelivery: true)
                            : rate;

                        var option = new ShiptorOption(_method, _totalPrice)
                        {
                            DeliveryId = string.Format("{0}_{1}_{2}", shipMethod.Method.Group, shipMethod.Method.Category, shipMethod.Method.Courier).GetHashCode(),
                            Name = string.Format("{1}", _method.Name, shipMethod.Method.Name),
                            Rate = rate,
                            BasePrice = rate,
                            PriceCash = rateCash,
                            DeliveryTime = shipMethod.Days,
                            IsAvailablePaymentCashOnDelivery = shipMethodWithCod != null && GetCashOnDeliveryAvailable(countryIso2, shipMethod),
                            CashOnDeliveryCardAvailable = shipMethodWithCod != null && GetCashOnDeliveryAvailable(countryIso2, shipMethod) && GetCashOnDeliveryCardAvailable(countryIso2, shipMethod),
                            PaymentCodCardId = _paymentCodCardId,
                        };
                        option.CalculateOption = new ShiptorEventWidgetData
                        {
                            Courier = shipMethod.Method.Courier,
                            KladrId = kladrKodCity,
                            ShippingMethod = shipMethod.Method.Id,
                            Cod = option.IsAvailablePaymentCashOnDelivery,
                            Card = option.CashOnDeliveryCardAvailable
                        };

                        shippingOptions.Add(option);
                    }

                    if (shiptorPickpoint != null)
                    {
                        var preorderOption = _calculationParameters.ShippingOption != null &&
                                             _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Shiptor).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value &&
                                             _calculationParameters.ShippingOption.GetType() == typeof(ShiptorWidgetOption)
                            ? ((ShiptorWidgetOption)_calculationParameters.ShippingOption)
                            : null;

                        // этот блок кода под вопросом. Может быть что часть доставок в ПВЗ поддерживают наложенный, а чать нет
                        shiptorPickpoint.IsAvailablePaymentCashOnDelivery = pickpointMethods.All(x => GetCashOnDeliveryAvailable(countryIso2, x));
                        shiptorPickpoint.CashOnDeliveryCardAvailable = shiptorPickpoint.IsAvailablePaymentCashOnDelivery && pickpointMethods.All(x => GetCashOnDeliveryCardAvailable(countryIso2, x));
                        //-----

                        CalculateMethod methodPoint = null;
                        CalculateMethod methodPointWithCod = null;
                        bool isLoadByPreorderOption = false;
                        if (preorderOption != null)
                        {
                            if (preorderOption.PickpointAdditionalDataObj != null && preorderOption.PickpointAdditionalDataObj.KladrId == kladrKodCity)
                            {
                                methodPoint = pickpointMethods.FirstOrDefault(
                                    x =>
                                        x.Method.Id == preorderOption.PickpointAdditionalDataObj.ShippingMethod);/* &&
                                        x.Method.Courier == preorderOption.PickpointAdditionalDataObj.Courier);*/

                                methodPointWithCod = methodPoint != null
                                    ? pickpointMethodsWithCod.FirstOrDefault(x => x.Method.Id == methodPoint.Method.Id)
                                    : null;

                                shiptorPickpoint.IsAvailablePaymentCashOnDelivery = methodPointWithCod != null && shiptorPickpoint.IsAvailablePaymentCashOnDelivery && preorderOption.PickpointAdditionalDataObj.Cod;
                                shiptorPickpoint.CashOnDeliveryCardAvailable = shiptorPickpoint.IsAvailablePaymentCashOnDelivery && shiptorPickpoint.CashOnDeliveryCardAvailable && preorderOption.PickpointAdditionalDataObj.Card;

                                isLoadByPreorderOption = methodPoint != null;
                            }
                        }

                        if (methodPoint == null)
                        {
                            methodPoint = pickpointMethods.OrderBy(x => x.Cost.Total.Sum).FirstOrDefault();

                            methodPointWithCod = methodPoint != null
                                ? pickpointMethodsWithCod.FirstOrDefault(x => x.Method.Id == methodPoint.Method.Id)
                                : null;
                        }

                        if (methodPoint != null)
                        {
                            var rate = GetDeliverySum(methodPoint.Cost, withInsurance: true, cachOnDelivery: false);
                            var rateCash = methodPointWithCod != null
                                ? GetDeliverySum(methodPointWithCod.Cost, withInsurance: true, cachOnDelivery: true)
                                : rate;

                            shiptorPickpoint.Rate = rate;
                            shiptorPickpoint.BasePrice = rate;
                            shiptorPickpoint.PriceCash = rateCash;
                            if (isLoadByPreorderOption)
                            {
                                shiptorPickpoint.PickpointCompany = methodPoint.Method.Name;
                                shiptorPickpoint.DeliveryTime = methodPoint.Days;
                            }

                        }

                        ConfigShiptorWidget(shiptorPickpoint, orderCost, weight, dimensions, pickpointMethods, kladrKodCity);
                        shippingOptions.Add(shiptorPickpoint);
                    }
                }
            }

            return shippingOptions;
        }

        private string GetKladrKodCity(out string countryIso2)
        {
            countryIso2 = null;
            if (_calculationParameters.Country.IsNotEmpty())
            {
                var country = CountryService.GetCountryByName(_calculationParameters.Country);
                if (country != null)
                    countryIso2 = country.Iso2;
            }

            var suggests = _shiptorCheckoutApiService.SuggestSettlement(string.Format("{0} {1}", _calculationParameters.Region, _calculationParameters.City), countryIso2);
            var suggestCity = suggests != null && suggests.Count > 0 ? suggests.FirstOrDefault(x => x.Name.IndexOf(_calculationParameters.City, StringComparison.OrdinalIgnoreCase) != -1) : null;

            return suggestCity != null ? suggestCity.KladrId : null;
        }

        private bool GetCashOnDeliveryAvailable(string countryIso2, CalculateMethod calculateMethod)
        {
            return countryIso2 == null || countryIso2.Equals("RU", StringComparison.OrdinalIgnoreCase) &&
                            !calculateMethod.Method.Group.Equals("dpd_economy_courier", StringComparison.OrdinalIgnoreCase);
        }

        private bool GetCashOnDeliveryCardAvailable(string countryIso2, CalculateMethod calculateMethod)
        {
            return (countryIso2 == null || countryIso2.Equals("RU", StringComparison.OrdinalIgnoreCase)) &&
                            !calculateMethod.Method.Group.Equals("russian_post", StringComparison.OrdinalIgnoreCase) &&
                            !calculateMethod.Method.Group.Equals("dpd_economy_courier", StringComparison.OrdinalIgnoreCase);
        }

        private void ConfigShiptorWidget(ShiptorWidgetOption shiptorPickpoint, float orderCost, float weight, float[] dimensions,
            List<CalculateMethod> pickpointMethods, string kladrKodCity)
        {
            var cod = false;
            var card = false;

            var checkoutInfo = MyCheckout.Factory(Customers.CustomerContext.CustomerId);
            if (checkoutInfo != null && checkoutInfo.Data != null && checkoutInfo.Data.SelectPayment?.GetDetails()?.IsCashOnDeliveryPayment is true)
            {
                if (checkoutInfo.Data.SelectPayment.Id == shiptorPickpoint.PaymentCodCardId)
                    card = true;
                else
                    cod = true;
            }

            var dimensionsObj = new WidgetConfigParamDimensions { height = dimensions[0], width = dimensions[1], length = dimensions[2] };
            //var courier = string.Join(",", pickpointMethods.Select(x => x.Method.Courier).Distinct());
            var round = CurrencyService.CurrentCurrency.EnablePriceRounding && Math.Round(CurrencyService.CurrentCurrency.RoundNumbers, 2) > 0.01d
                ? "math"
                : null;

            shiptorPickpoint.WidgetConfigData = new Dictionary<string, string>();
            shiptorPickpoint.WidgetConfigParams = new Dictionary<string, object>();

            shiptorPickpoint.WidgetConfigData.Add("data-mode", "inline");
            shiptorPickpoint.WidgetConfigData.Add("data-close-on-select", "0");
            shiptorPickpoint.WidgetConfigData.Add("data-weight", weight.ToInvariantString());
            shiptorPickpoint.WidgetConfigData.Add("data-dimensions", JsonConvert.SerializeObject(dimensionsObj));
            shiptorPickpoint.WidgetConfigData.Add("data-price", cod || card ? orderCost.ToInvariantString() : "0");
            shiptorPickpoint.WidgetConfigData.Add("data-cod", cod ? "1" : "0");
            shiptorPickpoint.WidgetConfigData.Add("data-card", card ? "1" : "0");
            //shiptorPickpoint.WidgetConfigData.Add("data-courier", courier);
            shiptorPickpoint.WidgetConfigData.Add("data-declaredCost", (_withInsure ? orderCost : 10f).ToInvariantString());
            shiptorPickpoint.WidgetConfigData.Add("data-kladr", kladrKodCity);

            //if (round.IsNotEmpty())
                shiptorPickpoint.WidgetConfigData.Add("data-round", round ?? string.Empty);

            shiptorPickpoint.WidgetConfigData.Add("data-wk", _apiKey);

            shiptorPickpoint.WidgetConfigParams.Add("location", new WidgetConfigParamLocation { kladr_id = kladrKodCity });
            shiptorPickpoint.WidgetConfigParams.Add("dimensions", dimensionsObj);
            shiptorPickpoint.WidgetConfigParams.Add("cod", cod);
            shiptorPickpoint.WidgetConfigParams.Add("card", card);
            //shiptorPickpoint.WidgetConfigParams.Add("courier", courier);
            shiptorPickpoint.WidgetConfigParams.Add("price", cod || card ? orderCost : 0f);
            shiptorPickpoint.WidgetConfigParams.Add("weight", weight);
            shiptorPickpoint.WidgetConfigParams.Add("declaredCost", (_withInsure ? orderCost : 10f));
            //if (_method.Extracharge > 0)
            //{
                /*
                if (_method.ExtrachargeType == Payment.ExtrachargeType.Percent)
                {
                    if (_method.ExtrachargeFromOrder)
                    {
                        shiptorPickpoint.WidgetConfigParams.Add("markup", Math.Round(_method.Extracharge * _totalPrice / 100, 2).ToInvariantString());
                        shiptorPickpoint.WidgetConfigData.Add("data-markup", Math.Round(_method.Extracharge * _totalPrice / 100, 2).ToInvariantString());
                    }
                    else
                    {
                        shiptorPickpoint.WidgetConfigParams.Add("markup", _method.Extracharge.ToInvariantString() + "%");
                        shiptorPickpoint.WidgetConfigData.Add("data-markup", _method.Extracharge.ToInvariantString() + "%");
                    }
                }
                else
                {
                    shiptorPickpoint.WidgetConfigParams.Add("markup", _method.Extracharge.ToInvariantString());
                    shiptorPickpoint.WidgetConfigData.Add("data-markup", _method.Extracharge.ToInvariantString());
                }
                */
                if (_method.ExtrachargeInPercents != 0 && _method.ExtrachargeInNumbers == 0)
                {
                    if (_method.ExtrachargeFromOrder)
                    {
                        shiptorPickpoint.WidgetConfigParams.Add("markup", Math.Round(_method.ExtrachargeInPercents * _totalPrice / 100, 2).ToInvariantString());
                        shiptorPickpoint.WidgetConfigData.Add("data-markup", Math.Round(_method.ExtrachargeInPercents * _totalPrice / 100, 2).ToInvariantString());
                    }
                    else
                    {
                        shiptorPickpoint.WidgetConfigParams.Add("markup", _method.ExtrachargeInPercents.ToInvariantString() + "%");
                        shiptorPickpoint.WidgetConfigData.Add("data-markup", _method.ExtrachargeInPercents.ToInvariantString() + "%");
                    }
                }
                else if (_method.ExtrachargeInPercents == 0 && _method.ExtrachargeInNumbers != 0)
                {
                    shiptorPickpoint.WidgetConfigParams.Add("markup", _method.ExtrachargeInNumbers.ToInvariantString());
                    shiptorPickpoint.WidgetConfigData.Add("data-markup", _method.ExtrachargeInNumbers.ToInvariantString());
                }
                else
                {
                    var markup = shiptorPickpoint.GetExtracharge();

                    shiptorPickpoint.WidgetConfigParams.Add("markup", markup.ToInvariantString());
                    shiptorPickpoint.WidgetConfigData.Add("data-markup", markup.ToInvariantString());
                }


                //}

                //if (round.IsNotEmpty())
                shiptorPickpoint.WidgetConfigParams.Add("round", round ?? string.Empty);

            //if (shiptorPickpoint.Rate > 0)
            //{
                //Если модуль устанавливает для доставки бесплатную цену, тогда передать это виджету

                var copyShiptorPickpoint = new ShiptorWidgetOption(_method, _totalPrice)
                {
                    HideAddressBlock = shiptorPickpoint.HideAddressBlock,
                    //Rate = shiptorPickpoint.Rate,
                    Rate = shiptorPickpoint.Rate > 0f ? shiptorPickpoint.Rate : 1f,//еденица чтобы отследить обнуление цены
                    BasePrice = shiptorPickpoint.BasePrice,
                    PriceCash = shiptorPickpoint.PriceCash,
                    DeliveryId = shiptorPickpoint.DeliveryId,
                    Name = shiptorPickpoint.Name,
                    DeliveryTime = shiptorPickpoint.DeliveryTime,
                    IsAvailablePaymentCashOnDelivery = shiptorPickpoint.IsAvailablePaymentCashOnDelivery,
                    CashOnDeliveryCardAvailable = shiptorPickpoint.CashOnDeliveryCardAvailable,
                    WidgetConfigData = shiptorPickpoint.WidgetConfigData,
                    WidgetConfigParams = shiptorPickpoint.WidgetConfigParams,
                    PickpointId = shiptorPickpoint.PickpointId,
                    PickpointAddress = shiptorPickpoint.PickpointAddress,
                    PickpointAdditionalData = shiptorPickpoint.PickpointAdditionalData,
                    PickpointAdditionalDataObj = shiptorPickpoint.PickpointAdditionalDataObj,
                };

                var items = new List<BaseShippingOption>() { copyShiptorPickpoint };
                var modules = Core.Modules.AttachedModules.GetModules<Core.Modules.Interfaces.IShippingCalculator>();
                foreach (var module in modules)
                {
                    if (module != null)
                    {
                        var classInstance = (Core.Modules.Interfaces.IShippingCalculator)Activator.CreateInstance(module);
                        classInstance.ProcessOptions(items, _items, _totalPrice);
                    }
                }

                if (/*shiptorPickpoint.Rate > 0 &&*/ items[0].Rate <= 0f)
                {
                    shiptorPickpoint.WidgetConfigData.Add("data-deliveryPriceText", _method.ZeroPriceMessage);
                    shiptorPickpoint.WidgetConfigParams.Add("textPrice", _method.ZeroPriceMessage);
                }
                else
                {
                    shiptorPickpoint.WidgetConfigData.Add("data-deliveryPriceText", string.Empty);
                    shiptorPickpoint.WidgetConfigParams.Add("textPrice", string.Empty);
                }
            //}

            if (_yaMapsApiKey.IsNotEmpty())
                shiptorPickpoint.WidgetConfigData.Add("data-yk", _yaMapsApiKey);

        }

        private float GetDeliverySum(CalculateMethodCost deliveryCost, bool withInsurance, bool cachOnDelivery = false)
        {
            var serviceCashOnDelivery = deliveryCost.Services != null ? deliveryCost.Services.FirstOrDefault(x => x.Service == ServiceCostTypeCashOnDelivery) : null;
            var serviceDeclaredCost = deliveryCost.Services != null ? deliveryCost.Services.FirstOrDefault(x => x.Service == ServiceCostTypeDeclaredCost) : null;
            return deliveryCost.Total.Sum -
                (withInsurance || serviceDeclaredCost == null ? 0F : serviceDeclaredCost.Sum) -
                (cachOnDelivery || serviceCashOnDelivery == null ? 0F : serviceCashOnDelivery.Sum);
        }
    }
}
