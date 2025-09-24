using System;
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
    public class GrastinSendOrderForRussianPost
    {
        private readonly SendOrderForRussianPostModel _model;

        public List<string> Errors { get; set; }

        public GrastinSendOrderForRussianPost(SendOrderForRussianPostModel model)
        {
            _model = model;
        }

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

                    var russianpostOrder = new RussianPostOrder()
                    {
                        Number = string.Format("{0}{1}", grastinMethod.OrderPrefix, order.Number),
                        Comment = _model.Comment.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Buyer = _model.Buyer.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Phone = _model.Phone.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Email = _model.Email,
                        Index = _model.Index.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Region = _model.Region.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        District = _model.District.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        City = _model.City.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Address = _model.Address.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        AssessedCost = _model.AssessedCost,
                        Service = _model.Service.Value,
                        CashOnDelivery = _model.CashOnDelivery,
                        //IsTest = false,
                        TakeWarehouse = _model.TakeWarehouse,
                        DeiveryDate = _model.DeliveryDate.Value,
                        SiteName = SettingsMain.ShopName.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        CargoType = _model.CargoType.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        BarCode = _model.BarCode.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                    };

                    if (order.OrderItems != null && order.OrderItems.Count > 0)
                    {
                        var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                              .AcceptableDifference(0.1f)
                                              .GetItems();
                        russianpostOrder.Products = orderItems.Select(x => new GrastinProduct()
                        {
                            ArtNo = x.ArtNo.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Name = x.Name.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Price = x.Price,
                            Amount = x.Amount,
                            Vat = GetVatRate(x.TaxType)
                        }).ToList();

                        if (order.ShippingCostWithDiscount > 0)
                        {
                            russianpostOrder.Products.Add(new GrastinProduct()
                            {
                                ArtNo = "Доставка",
                                Name = "Доставка",
                                Price = order.ShippingCostWithDiscount,
                                Amount = 1,
                                Vat = GetVatRate(order.ShippingTaxType)
                            });
                        }

                        russianpostOrder.OrderSum = russianpostOrder.Products.Sum(x => x.Price*x.Amount);
                    }
                    else
                    {
                        russianpostOrder.OrderSum = order.Sum;
                    }

                    if (!_model.CashOnDelivery)
                        russianpostOrder.OrderSum = 0f;

                    var response = service.AddRussianPostOrder(new RussianPostOrderContainer() { Orders = new List<RussianPostOrder>() { russianpostOrder } });

                    if (response != null && response.Count == 1)
                    {
                        if (string.IsNullOrEmpty(response[0].Error))
                        {
                            OrderService.AddUpdateOrderAdditionalData(order.OrderID, Shipping.Grastin.Grastin.KeyNameIsSendOrderInOrderAdditionalData, true.ToString());

                            order.DeliveryDate = russianpostOrder.DeiveryDate;

                            var trackChanges = !order.IsDraft;

                            OrderService.UpdateOrderMain(order, updateModules: false, trackChanges: trackChanges);

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
