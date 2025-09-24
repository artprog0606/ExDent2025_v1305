using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Pec
{
    public class PecCreateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PecCreateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId, Shipping.Pec.Pec.KeyNameCargoCodeInOrderAdditionalData);
            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                Errors.Add("Заказ уже передан");
                return false;
            }

            var order = OrderService.GetOrder(_orderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.Pec.Pec).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
            if (orderPickPoint == null || orderPickPoint.AdditionalData.IsNullOrEmpty())
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var pecMethod = new Shipping.Pec.Pec(order.ShippingMethod, calculationParameters);
            var result = pecMethod.UnloadOrder(order);

            if (!result.Success)
            {
                if (result.Message.IsNotEmpty())
                    Errors.AddRange(result.Message.Split("\n"));
                else
                    Errors.Add("Не удалось передать заказ");
                
                return false;
            }
  
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);
                
            return true;
        }
    }
}
