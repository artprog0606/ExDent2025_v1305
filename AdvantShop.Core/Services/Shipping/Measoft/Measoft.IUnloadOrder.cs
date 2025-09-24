using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.Measoft
{
    public partial class Measoft : IUnloadOrder
    {
        public UnloadOrderResult UnloadOrder(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            if (order.ShippingMethodId != _method.ShippingMethodId)
                return UnloadOrderResult.CreateFailedResult("В заказе используется другая доставка.");

            if (order.OrderCustomer == null)
                return UnloadOrderResult.CreateFailedResult("Отсутствуют данные пользователя");
            
            if (OrderService.GetOrderAdditionalData(order.OrderID, TrackNumberOrderAdditionalDataName).IsNotEmpty())
                return UnloadOrderResult.CreateFailedResult("Заказ уже передан в систему доставки.");

            if (order.OrderPickPoint == null || order.OrderPickPoint.AdditionalData.IsNullOrEmpty())
                return UnloadOrderResult.CreateFailedResult("Нет данных о параметрах расчета доставки.");

            float weight = GetTotalWeight();
            var dimensionsInSm = GetDimensions(rate: 10).Select(x => (int)Math.Ceiling(x)).ToArray();

            var result = _apiService.CreateOrder(order, weight, dimensionsInSm, _paymentCodCardId);

            if (result != null && !string.IsNullOrEmpty(result.Error))
            {
                return UnloadOrderResult.CreateFailedResult(result.Error);
            }
            else if (result != null && !string.IsNullOrEmpty(result.Number))
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    TrackNumberOrderAdditionalDataName,
                    result.Number);
                order.TrackNumber = result.Number;
                OrderService.UpdateOrderMain(order);


                return UnloadOrderResult.CreateSuccessResult("Черновик заказа успешно создан.", result.Number);
            }

            return UnloadOrderResult.CreateFailedResult("Не удалось передать заказ.");
        }
    }
}