using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Orders;

namespace AdvantShop.Areas.Api.Model.Orders
{
    public class SetPaidModel : IValidatableObject
    {
        public int OrderId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderId == 0)
                yield return new ValidationResult("Укажите идентификатор заказа");

            Order order;
            if (OrderId != 0 && ((order = OrderService.GetOrder(OrderId)) == null || order.IsDraft))
                yield return new ValidationResult("Заказ не найден");
        }
    }
}