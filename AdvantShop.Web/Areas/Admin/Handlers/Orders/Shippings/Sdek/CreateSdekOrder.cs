using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Sdek;
using AdvantShop.Shipping.Sdek.Api;
using AdvantShop.Taxes;
using AdvantShop.Web.Infrastructure.ActionResults;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek
{
    public class CreateSdekOrder
    {
        private readonly int _orderId;

        public CreateSdekOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() {Error = "Order is null"};
            
            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != ((ShippingKeyAttribute) typeof(Shipping.Sdek.Sdek).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                return new CommandResult() { Error = "Order shipping method is not 'Sdek' type" };
           
            try
            {
                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();
                var sdekMethod = new Shipping.Sdek.Sdek(shippingMethod, calculationParameters);
                var result = sdekMethod.UnloadOrder(order);
              
                if (!result.Success)
                    return new CommandResult() { Result = false, Error = result.Message ?? "Не удалось передать заказ" };
  
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);
                
                return new CommandResult {Result = true};
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать заказ: " + ex.Message };
            }
        }
    }
}
