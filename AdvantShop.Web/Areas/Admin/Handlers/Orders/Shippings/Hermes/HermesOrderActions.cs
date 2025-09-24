using System;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Hermes;
using AdvantShop.Web.Admin.Models.Orders.Hermes;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Hermes
{
    public class HermesOrderActions
    {
        private readonly int _orderId;

        public HermesOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var model = new OrderActionsModel()
            {
                OrderId = _orderId
            };

            if (string.IsNullOrEmpty(OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Hermes.Hermes.KeyNameBarcodeInOrderAdditionalData)))
            {
                var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
                if (orderPickPoint != null && orderPickPoint.AdditionalData.IsNotEmpty())
                {
                    HermesCalculateOption hermesCalculateOption = null;

                    try
                    {
                        hermesCalculateOption =
                            JsonConvert.DeserializeObject<HermesCalculateOption>(orderPickPoint.AdditionalData);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }

                    if (hermesCalculateOption != null && hermesCalculateOption.BusinessUnitCode.IsNotEmpty())
                    {
                        var order = OrderService.GetOrder(_orderId);
                        if (order != null)
                        {
                            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                            if (shippingMethod != null &&
                                shippingMethod.ShippingType ==
                                ((ShippingKeyAttribute)
                                    typeof(Shipping.Hermes.Hermes).GetCustomAttributes(
                                        typeof(ShippingKeyAttribute), false).First())
                                    .Value)
                            {
                                var hermesMethod = new Shipping.Hermes.Hermes(shippingMethod, null);

                                var businessUnit = hermesMethod.GetBusinessUnit(hermesCalculateOption.BusinessUnitCode);

                                if (businessUnit != null)
                                {
                                    //ToDo: нет обязательных данных для отправки
                                    //model.ShowSendParcelVSD = businessUnit.Services.Contains("DIRECT_DELIVERY") && businessUnit.Services.Contains("DIRECT_RETURN") && businessUnit.Services.Contains("ACCOMPANYING_DOCUMENTS_RETURN") && 
                                    //hermesCalculateOption.SourcePointIsDistributionCenter == true;

                                    model.ShowSendParcelStandart = businessUnit.Services.Contains("DIRECT_DELIVERY") && businessUnit.Services.Contains("DIRECT_RETURN") &&
                                        hermesCalculateOption.SourcePointIsDistributionCenter == true;
                                    model.ShowSendParcelDrop = businessUnit.Services.Contains("DROP_OFF_TO_TARGET_PARCELSHOP") &&
                                        hermesCalculateOption.SourcePointIsDistributionCenter == false && hermesCalculateOption.SourcePointCode == orderPickPoint.PickPointId;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                model.ShowDeleteParcel = true;
            }

            return model;
        }
    }
}
