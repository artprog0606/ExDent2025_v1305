using System;
using System.Linq;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.Boxberry
{
    public partial class Boxberry : IUnloadOrder
    {
        public UnloadOrderResult UnloadOrder(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");
            
            var dimensions = GetDimensions(rate: 10).Select(x => (int)Math.Ceiling(x)).ToArray();
            var result = _boxberryApiService.ParselCreate(order, (int)GetTotalWeight(1000), dimensions, _withInsure, _method.ShippingCurrency);

            if (result is null)
                return UnloadOrderResult.CreateFailedResult("Не удалось создать черновик заказа.");
            
            if (!string.IsNullOrEmpty(result.Error))
                return UnloadOrderResult.CreateFailedResult(result.Error);

            if (!string.IsNullOrEmpty(result.TrackNumber))
            {
                order.TrackNumber = result.TrackNumber;
                OrderService.UpdateOrderMain(order);
            }

            return UnloadOrderResult.CreateSuccessResult(result.TrackNumber);
        }
    }
}