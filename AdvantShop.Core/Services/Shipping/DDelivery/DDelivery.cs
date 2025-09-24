//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.DDelivery;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.DDelivery
{
    [ShippingKey("DDelivery")]
    public class DDelivery : BaseShippingWithCargo, IShippingSupportingPaymentCashOnDelivery
    {
        private readonly string _apiKey;
        private readonly string _shopId;
        private readonly string _token;
        //private readonly string _receptionCompanyId;
        //private readonly bool _createDraftOrder;
        private readonly bool _useWidget;

        private readonly DDeliveryApiService _dDeliveryApiService;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public DDelivery(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _apiKey = _method.Params.ElementOrDefault(DDeliveryTemplate.ApiKey);
            _shopId = _method.Params.ElementOrDefault(DDeliveryTemplate.ShopId);
            _token = _method.Params.ElementOrDefault(DDeliveryTemplate.Token);
            //_receptionCompanyId = _method.Params.ElementOrDefault(DDeliveryTemplate.ReceptionCompanyId);
            //_createDraftOrder = _method.Params.ElementOrDefault(DDeliveryTemplate.CreateDraftOrder).TryParseBool();
            _useWidget = _method.Params.ElementOrDefault(DDeliveryTemplate.UseWidget).TryParseBool();

            _dDeliveryApiService = new DDeliveryApiService(_apiKey, _shopId/*, _receptionCompanyId, _createDraftOrder*/, _useWidget);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var result = new List<BaseShippingOption>();

            var city = _dDeliveryApiService.GetCity(_calculationParameters.City);

            if (city == null)
                return result;
            
            var totalDimensions = GetDimensions(rate: 10);
            var totalWeight = GetTotalWeight();
            var totalCount = _items.Sum(item => item.Amount);

            var data = _dDeliveryApiService.CalculateDelivery(city.Id, totalDimensions, totalWeight, _items.Count, _totalPrice, false);
            //ToDo: почему только в виджете используется?
            var dataCash = _dDeliveryApiService.CalculateDelivery(city.Id, totalDimensions, totalWeight, _items.Count, _totalPrice, true);

            if (data == null || data.Data == null)
            {
                return result;
            }

            if (_useWidget)
            {
                var preorderOption = _calculationParameters.ShippingOption != null &&
                    _calculationParameters.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(DDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value &&
                    _calculationParameters.ShippingOption.GetType() == typeof(DDeliveryWidgetOption)
                        ? ((DDeliveryWidgetOption)_calculationParameters.ShippingOption)
                        : new DDeliveryWidgetOption();

                var widgetOption = GetDDeliveryWidgetOption(city.Name, totalDimensions, totalWeight, totalCount);
                if (widgetOption != null)
                {
                    var isCourier = false;
                    var isPost = false;
                    var isPickup = false;

                    if (preorderOption == null || preorderOption.PickpointAdditionalDataObj == null)
                    {
                        var costCourier = data.Data.Courier != null && data.Data.Courier.Delivery != null &&
                                          data.Data.Courier.Delivery.Count > 0
                            ? data.Data.Courier.Delivery.OrderBy(x => x.TotalPrice).First().TotalPrice
                            : float.MaxValue;

                        var costPost = data.Data.Post != null && data.Data.Post.Delivery != null &&
                                       data.Data.Post.Delivery.Count > 0
                            ? data.Data.Post.Delivery.OrderBy(x => x.TotalPrice).First().TotalPrice
                            : float.MaxValue;

                        var costPickup = data.Data.Pickup != null && data.Data.Pickup.Points != null &&
                                         data.Data.Pickup.Points.Count > 0
                            ? data.Data.Pickup.Points.OrderBy(x => x.PriceDelivery).First().PriceDelivery
                            : float.MaxValue;

                        var minCost = new float[] {costCourier, costPost, costPickup}.Min();

                        if (minCost != float.MaxValue)
                        {
                            if (minCost == costCourier)
                                isCourier = true;
                            else if (minCost == costPost)
                                isPost = true;
                            else if (minCost == costPickup)
                                isPickup = true;
                        }
                    }
                    else
                    {
                        isCourier = preorderOption.PickpointAdditionalDataObj.DeliveryType == 2 &&
                                    data.Data.Courier != null && data.Data.Courier.Delivery != null &&
                                    data.Data.Courier.Delivery.Count > 0;

                        isPost = preorderOption.PickpointAdditionalDataObj.DeliveryType == 3 &&
                                 data.Data.Post != null && data.Data.Post.Delivery != null &&
                                 data.Data.Post.Delivery.Count > 0;

                        isPickup = preorderOption.PickpointAdditionalDataObj.DeliveryType == 1 &&
                                   data.Data.Pickup != null && data.Data.Pickup.Points != null &&
                                   data.Data.Pickup.Points.Count > 0;
                    }

                    if (isCourier)
                    {
                        // Courier
                        var delivery = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                                ? data.Data.Courier.Delivery.FirstOrDefault(
                                    x => x.DeliveryCompanyId == preorderOption.PickpointAdditionalDataObj.DeliveryCompanyId &&
                                    x.PickupCompanyId == preorderOption.PickpointAdditionalDataObj.PickupCompanyId) ?? data.Data.Courier.Delivery.OrderBy(x => x.TotalPrice).First()
                                : data.Data.Courier.Delivery.OrderBy(x => x.TotalPrice).First();
                        var deliveryCash = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                                ? dataCash.Data.Courier.Delivery.FirstOrDefault(
                                    x => x.DeliveryCompanyId == preorderOption.PickpointAdditionalDataObj.DeliveryCompanyId &&
                                    x.PickupCompanyId == preorderOption.PickpointAdditionalDataObj.PickupCompanyId) ?? dataCash.Data.Courier.Delivery.OrderBy(x => x.TotalPrice).First()
                                : dataCash.Data.Courier.Delivery.OrderBy(x => x.TotalPrice).First();

                        widgetOption.Name += LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", delivery.DeliveryCompanyName);
                        widgetOption.Rate = delivery.TotalPrice;
                        widgetOption.BasePrice = delivery.TotalPrice;
                        widgetOption.PriceCash = deliveryCash.TotalPrice;
                        widgetOption.DeliveryTime = (delivery.DeliveryDays + _method.ExtraDeliveryTime) + "дн.";
                    }
                    else if (isPost)
                    {
                        // Post
                        var delivery = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                                ? data.Data.Post.Delivery.FirstOrDefault(
                                    x => x.DeliveryCompanyId == preorderOption.PickpointAdditionalDataObj.DeliveryCompanyId &&
                                    x.PickupCompanyId == preorderOption.PickpointAdditionalDataObj.PickupCompanyId) ?? data.Data.Post.Delivery.OrderBy(x => x.TotalPrice).First()
                                : data.Data.Post.Delivery.OrderBy(x => x.TotalPrice).First();
                        var deliveryCash = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                                ? dataCash.Data.Post.Delivery.FirstOrDefault(
                                    x => x.DeliveryCompanyId == preorderOption.PickpointAdditionalDataObj.DeliveryCompanyId &&
                                    x.PickupCompanyId == preorderOption.PickpointAdditionalDataObj.PickupCompanyId) ?? dataCash.Data.Post.Delivery.OrderBy(x => x.TotalPrice).First()
                                : dataCash.Data.Post.Delivery.OrderBy(x => x.TotalPrice).First();

                        widgetOption.Name += LocalizationService.GetResource("Core.Services.Shipping.RussianPostWithSpace");
                        widgetOption.Rate = delivery.TotalPrice;
                        widgetOption.BasePrice = delivery.TotalPrice;
                        widgetOption.PriceCash = deliveryCash.TotalPrice;
                        widgetOption.DeliveryTime = (delivery.DeliveryDays + _method.ExtraDeliveryTime) + "дн.";
                    }
                    else if (isPickup)
                    {
                        // Pickup

                        if (data.Data != null && data.Data.Pickup.Points != null && data.Data.Pickup.Points.Any())
                        {
                            var point = preorderOption != null && preorderOption.PickpointId != null
                                ? data.Data.Pickup.Points.FirstOrDefault(
                                    x => preorderOption.PickpointId == x.Id.ToString()) ?? data.Data.Pickup.Points.OrderBy(x => x.PriceDelivery).First()
                                : data.Data.Pickup.Points.OrderBy(x => x.PriceDelivery).First();
                            var pointCash = preorderOption != null && preorderOption.PickpointId != null
                                ? dataCash.Data.Pickup.Points.FirstOrDefault(
                                    x => preorderOption.PickpointId == x.Id.ToString()) ?? dataCash.Data.Pickup.Points.OrderBy(x => x.PriceDelivery).First()
                                : dataCash.Data.Pickup.Points.OrderBy(x => x.PriceDelivery).First();

                            widgetOption.Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams", widgetOption.Name, point.DeliveryCompanyName);
                            widgetOption.Rate = point.PriceDelivery;
                            widgetOption.BasePrice = point.PriceDelivery;
                            widgetOption.PriceCash = pointCash.PriceDelivery;
                        }
                    }

                    if (isCourier || isPost || isPickup)
                        result.Add(widgetOption);
                }
            }
            else
            {
                var pointOptions = GetDeliveryOptionsWithPoints(data);
                if (pointOptions != null && pointOptions.Count > 0)
                {
                    result.AddRange(pointOptions);
                }

                var courierOptions = GetDeliveryOptionsCourier(data);
                if (courierOptions != null && courierOptions.Count > 0)
                {
                    result.AddRange(courierOptions);
                }

                var postOptions = GetDeliveryOptionsPost(data);
                if (postOptions != null && postOptions.Count > 0)
                {
                    result.AddRange(postOptions);
                }
            }

            return result;
        }

        #region Get shipping options

        private List<DDeliveryPointOption> GetDeliveryOptionsWithPoints(DDeliveryObjectResponse<DDeliveryObjectCalculatorAnswer> deliveryData)
        {
            var options = new List<DDeliveryPointOption>();

            foreach (var deliveryCompany in deliveryData.Data.Pickup.Delivery)
            {
                var points = new List<DDeliveryPoint>();
                foreach (var ddeliveryPoint in deliveryData.Data.Pickup.Points.Where(item => item.DeliveryCompanyId == deliveryCompany.DeliveryCompanyId))
                {
                    points.Add(new DDeliveryPoint
                    {
                        Id = ddeliveryPoint.Id.ToString(),
                        Code = ddeliveryPoint.Id.ToString(),
                        Address = ddeliveryPoint.Adress,
                        Description = ddeliveryPoint.DescriptionIn + "<br/>" + ddeliveryPoint.DescriptionOut,
                        Rate = ddeliveryPoint.PriceDelivery,
                        DeliveryDate = ddeliveryPoint.DeliveryDate,
                        DeliveryCompanyId = ddeliveryPoint.DeliveryCompanyId,
                        DeliveryTypeId = (int)EDeliveryType.Pickup
                    });
                }

                var option = new DDeliveryPointOption(_method, _totalPrice)
                {
                    Name = deliveryCompany.DeliveryCompanyName,
                    DeliveryTime = (deliveryCompany.DeliveryDays + _method.ExtraDeliveryTime) + " дн.",
                    Rate = deliveryCompany.TotalPrice,
                    //DisplayIndex = false,
                    //HideAddressBlock = false,
                    ShippingPoints = points.OrderBy(item => item.Address).ToList(),
                    DeliveryTypeId = (int)EDeliveryType.Pickup,
                    IconName = ShippingIcons.GetShippingIcon(_method.ShippingType, _method.IconFileName.PhotoName, deliveryCompany.DeliveryCompanyName)
                };

                options.Add(option);
            }

            return options;
        }

        private List<DDeliveryOption> GetDeliveryOptionsCourier(DDeliveryObjectResponse<DDeliveryObjectCalculatorAnswer> deliveryData)
        {
            var options = new List<DDeliveryOption>();

            if (deliveryData.Data == null || deliveryData.Data.Courier == null)
                return options;

            foreach (var deliveryCompany in deliveryData.Data.Courier.Delivery)
            {
                var shippingOption = new DDeliveryOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", deliveryCompany.DeliveryCompanyName),
                    Rate = deliveryCompany.TotalPrice,
                    DeliveryTime = (deliveryCompany.DeliveryDays + _method.ExtraDeliveryTime) + " дн.",
                    //DisplayIndex = true,
                    //HideAddressBlock = false,
                    DeliveryCompanyId = deliveryCompany.DeliveryCompanyId,
                    DeliveryTypeId = (int)EDeliveryType.Courier,
                    IconName = ShippingIcons.GetShippingIcon(_method.ShippingType, _method.IconFileName.PhotoName, deliveryCompany.DeliveryCompanyName)
                };
                options.Add(shippingOption);
            }

            return options;
        }

        private List<DDeliveryOption> GetDeliveryOptionsPost(DDeliveryObjectResponse<DDeliveryObjectCalculatorAnswer> deliveryData)
        {
            var options = new List<DDeliveryOption>();

            foreach (var deliveryCompany in deliveryData.Data.Post.Delivery)
            {
                var shippingOption = new DDeliveryOption(_method, _totalPrice)
                {
                    Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", deliveryCompany.DeliveryCompanyName),
                    Rate = deliveryCompany.TotalPrice,
                    DeliveryTime = (deliveryCompany.DeliveryDays + _method.ExtraDeliveryTime) + " дн.",
                    //DisplayIndex = true,
                    //HideAddressBlock = false,
                    DeliveryTypeId = (int)EDeliveryType.Post,
                    DeliveryCompanyId = deliveryCompany.DeliveryCompanyId,
                    IconName = ShippingIcons.GetShippingIcon(_method.ShippingType, _method.IconFileName.PhotoName, deliveryCompany.DeliveryCompanyName)
                };
                options.Add(shippingOption);
            }

            return options;
        }

        private DDeliveryWidgetOption GetDDeliveryWidgetOption(string city, float[] totalDimensions, float totalWeight, float totalCount)
        {
            var products = new List<DDeliveryCartWidgetProduct>();
            /*
                products.name		Название товара.
                products.vendorCode		Артикул.
                products.barcode		Штрих-код.
                products.nds		НДС.
                Возможные значения: 0, 10, 18.
                По умолчанию: 0.
                products.price	Да	Стоимость единицы товара.
                Дробные значения не округляются. Допустимые значения: 0 – 999 999.
                products.discount		Размер скидки на товар (в рублях).
                Дробные значения округляются, не может быть меньше 0.
                products.count	Да	Количество товаров.
                Допустимые значения: 1 – 999.
             */

            foreach (var orderItem in _items)
            {
                products.Add(new DDeliveryCartWidgetProduct
                {
                    Name = orderItem.Name,
                    Count = Convert.ToInt32(orderItem.Amount),
                    Price = orderItem.Price,
                    VendorCode = orderItem.ArtNo
                });

                //products.Add(new DDeliveryObjectProduct
                //{
                //    Id = orderItem.Id.ToString(),
                //    Sku = orderItem.Id.ToString(),
                //    Name = orderItem.Name,
                //    Price = orderItem.Price,
                //    Weight = orderItem.Weight == 0 ? _defaultWeight : orderItem.Weight,
                //    Width = orderItem.Width == 0 ? _defaultWidth / 10 : orderItem.Width / 10,
                //    Height = orderItem.Height == 0 ? _defaultHeight / 10 : orderItem.Height / 10,
                //    Length = orderItem.Length == 0 ? _defaultLength / 10 : orderItem.Length / 10,
                //    Quantity = Convert.ToInt32(orderItem.Amount)
                //});
            }

            return new DDeliveryWidgetOption(_method, _totalPrice)
            {
                Name = _method.Name,
                WidgetConfigData = new DDeliveryCartWidgetObject
                {
                    RegionName = city,
                    Products = products,
                    Height = totalDimensions[0],
                    Width = totalDimensions[1],
                    Length = totalDimensions[2],
                    Weight = totalWeight,
                    ItemCount = Convert.ToInt32(totalCount),
                    PriceDeclared = Convert.ToInt32(_items.Sum(item => item.Price * item.Amount)),
                    NppOption = false,
                    //StopSubmit = true,
                    //UserFullName = "",
                    //UserPhone = ""
                }
            };
        }

        #endregion

        #region Api methods

        public DDeliveryObjectResponse<DDeliveryObjectNewOrder> CreateOrder(Order order)
        {
            var totalDimensions = GetDimensions(rate: 10);

            return _dDeliveryApiService.CreateOrder(order, totalDimensions, GetTotalWeight(), _method.ShippingCurrency);
        }

        public DDeliveryObjectResponse<DDeliveryObjectOrderInfo> GetOrderInfo(string ddeliveryOrderId)
        {
            return _dDeliveryApiService.GetOrderInfo(ddeliveryOrderId);
        }

        public DDeliveryObjectResponse<object> CanselOrder(string ddeliveryOrderId)
        {
            return _dDeliveryApiService.CanselOrder(ddeliveryOrderId);
        }

        #endregion
    }
}