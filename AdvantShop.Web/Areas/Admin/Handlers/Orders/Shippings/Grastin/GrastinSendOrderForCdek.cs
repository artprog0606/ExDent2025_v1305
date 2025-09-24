using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using AdvantShop.Web.Admin.Models.Orders.Grastin;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Grastin
{
    public class GrastinSendOrderForCdek
    {
        private readonly SendOrderForCdekModel _model;

        public GrastinSendOrderForCdek(SendOrderForCdekModel model)
        {
            _model = model;
        }

        public List<string> Errors { get; set; }
       

        public bool Execute()
        {
            var order = OrderService.GetOrder(_model.OrderId);
            if (order != null)
            {
                var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                if (shippingMethod != null &&
                    shippingMethod.ShippingType ==
                    ((ShippingKeyAttribute)
                        typeof (Shipping.Grastin.Grastin).GetCustomAttributes(typeof (ShippingKeyAttribute), false)
                            .First())
                        .Value)
                {
                    var grastinMethod = new Shipping.Grastin.Grastin(shippingMethod, null);

                    var service = new GrastinApiService(grastinMethod.ApiKey);

                    var cdekOrder = new CdekOrder()
                    {
                        Number = string.Format("{0}{1}", grastinMethod.OrderPrefix, order.Number),
                        Buyer = _model.Buyer.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Phone = _model.Phone.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Phone2 = _model.Phone2.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Seats = _model.Seats,
                        //IsTest = false,
                        TakeWarehouse = _model.TakeWarehouse,
                        SiteName = SettingsMain.ShopName.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Email = _model.Email,
                        CargoType = _model.CargoType.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        PickupId = _model.Service == EnCdekService.PickPoint ? _model.PointId : null,
                        CityId = _model.Service == EnCdekService.Courier ? _model.CityId : null,
                        Address = _model.Service == EnCdekService.Courier ? _model.Address.RemoveInvalidXmlChars().RemoveEscapeXmlChars() : null,
                        CostDelivery = 0f,//order.ShippingCost,
                    };

                    if (order.OrderItems != null && order.OrderItems.Count > 0)
                    {
                        var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                              .AcceptableDifference(0.1f)
                                              .GetItems();
                        cdekOrder.Products = orderItems.Select(x => new GrastinProduct()
                        {
                            ArtNo = x.ArtNo.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Name = x.Name.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Price = x.Price,
                            Amount = x.Amount,
                            Vat = GetVatRate(x.TaxType)
                        }).ToList();

                        if (order.ShippingCostWithDiscount > 0)
                        {
                            cdekOrder.Products.Add(new GrastinProduct()
                            {
                                ArtNo = "Доставка",
                                Name = "Доставка",
                                Price = order.ShippingCostWithDiscount,
                                Amount = 1,
                                Vat = GetVatRate(order.ShippingTaxType)
                            });
                        }

                        cdekOrder.OrderSum = cdekOrder.Products.Sum(x => x.Price*x.Amount) + cdekOrder.CostDelivery;
                    }
                    else
                    {
                        cdekOrder.OrderSum = order.Sum;
                    }

                    cdekOrder.AssessedCost = _model.CashOnDelivery ? cdekOrder.OrderSum : _model.AssessedCost;

                    if (!_model.CashOnDelivery)
                    {
                        cdekOrder.OrderSum = 0f;
                        cdekOrder.CostDelivery = 0f;
                    }

                    var response = service.AddCdekOrder(new CdekOrderContainer() { Orders = new List<CdekOrder>() { cdekOrder } });

                    if (response != null && response.Count == 1)
                    {
                        if (string.IsNullOrEmpty(response[0].Error))
                        {
                            OrderService.AddUpdateOrderAdditionalData(order.OrderID, Shipping.Grastin.Grastin.KeyNameIsSendOrderInOrderAdditionalData, true.ToString());

                            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);

                            return true;
                        }

                        Errors = new List<string>() { response[0].Error };
                    }
                    else
                    {
                        Errors = service.LastActionErrors;
                    }
                }
            }

            return false;
        }
        
        private int? GetVatRate(TaxType? taxType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return null;

            if (taxType.Value == TaxType.Vat0)
                return 0;

            if (taxType.Value == TaxType.Vat10)
                return 10;

            if (taxType.Value == TaxType.Vat18)
                return 18;
            
            if (taxType.Value == TaxType.Vat20)
                return 20;

            return null;
        }
    }
}