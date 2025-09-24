using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.ApiShip;
using AdvantShop.Core.Services.Shipping.ApiShip.Api;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ApiShip;
using AdvantShop.Shipping.ApiShip.Api;
using AdvantShop.Web.Infrastructure.ActionResults;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.ApiShip
{
    public class ApiShipCreateOrder
    {
        private readonly int _orderId;

        public ApiShipCreateOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != ((ShippingKeyAttribute)typeof(Shipping.ApiShip.ApiShip).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                return new CommandResult() { Error = "Order shipping method is not 'ApiShip' type" };
            if (order.OrderCustomer == null)
                return new CommandResult() { Error = "Отсутствуют данные пользователя" };
            try
            {
                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                   .ByOrder(order)
                                   .FromAdminArea()
                                   .Build();
                var apiShipMethod = new Shipping.ApiShip.ApiShip(shippingMethod, calculationParameters);
                var result = apiShipMethod.UnloadOrder(order);
                        
                if (!result.Success)
                    return new CommandResult() { Result = false, Error = result.Message ?? "Не удалось передать заказ" };
  
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);

                return new CommandResult { Result = true, Message = result.Message };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать заказ: " + ex.Message };
            }
        }
    }
}
