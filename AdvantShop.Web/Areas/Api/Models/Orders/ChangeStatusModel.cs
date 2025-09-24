using AdvantShop.Orders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Areas.Api.Model.Orders
{
    public class ChangeStatusModel : IValidatableObject
    {
        public int OrderId { get; set; }
        public int StatusId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderId == 0)
                yield return new ValidationResult("Укажите идентификатор заказа");

            if (StatusId == 0)
                yield return new ValidationResult("Укажите идентификатор статуса");

            Order order;
            if (OrderId != 0 && ((order = OrderService.GetOrder(OrderId)) == null || order.IsDraft))
                yield return new ValidationResult("Заказ не найден");

            if (StatusId != 0 && OrderStatusService.GetOrderStatus(StatusId) == null)
                yield return new ValidationResult("Статус не найден");
        }
    }
}