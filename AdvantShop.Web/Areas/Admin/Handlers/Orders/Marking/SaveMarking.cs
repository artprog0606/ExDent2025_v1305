using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders.Marking
{
    public class SaveMarking : AbstractCommandHandler<bool>
    {
        private readonly int _orderItemId;
        private readonly List<string> _codes;
        private OrderItem _orderItem;
        private int _amount;

        public SaveMarking(int orderItemId, List<string> codes)
        {
            _orderItemId = orderItemId;
            _codes = codes;
        }

        protected override void Load()
        {
            _orderItem = OrderService.GetOrderItem(_orderItemId);
        }

        protected override void Validate()
        {
            if (_orderItem == null)
                throw new BlException("Товар не найден");
            
            _amount = (int)Math.Round(_orderItem.Amount, MidpointRounding.AwayFromZero);
            
            if (_amount <= 0)
                throw new BlException("Кол-во товара должно быть больше 0");
        }

        protected override bool Handle()
        {
            MarkingOrderItemService.Delete(_orderItem.OrderItemID);

            foreach (var code in _codes.Where(x => !string.IsNullOrEmpty(x)))
            {
                MarkingOrderItemService.Add(new MarkingOrderItem(){OrderItemId = _orderItem.OrderItemID, Code = code});
            }

            ModulesExecuter.OrderItemUpdated(_orderItem);

            return true;
        }
    }
}
