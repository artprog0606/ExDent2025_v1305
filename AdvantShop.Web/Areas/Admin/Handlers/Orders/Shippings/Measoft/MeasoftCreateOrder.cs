using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;
using System;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Measoft
{
    public class MeasoftCreateOrder
    {
        private readonly int _orderId;

        public MeasoftCreateOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            string trackNumber = OrderService.GetOrderAdditionalData(_orderId, 
                Shipping.Measoft.Measoft.TrackNumberOrderAdditionalDataName);
            if (trackNumber.IsNotEmpty())
                return new CommandResult() { Error = "Заказ уже передан" };

            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Measoft")
                return new CommandResult() { Error = "Order shipping method is not 'Measoft' type" };

            if (order.OrderPickPoint == null || order.OrderPickPoint.AdditionalData.IsNullOrEmpty())
                return new CommandResult() { Error = "Нет данных о параметрах рассчета доставки" };

            try
            {
                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();
                
                var measoftShipping = new Shipping.Measoft.Measoft(shippingMethod, calculationParameters);
                var result = measoftShipping.UnloadOrder(order);
                        
                if (!result.Success)
                    return new CommandResult() { Result = false, Error = result.Message ?? "Не удалось передать заказ" };
  
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);

                return new CommandResult() { Result = true, Message = result.Message, Obj = result.Object };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать черновик заказа: " + ex.Message };
            }
        }
    }
}
