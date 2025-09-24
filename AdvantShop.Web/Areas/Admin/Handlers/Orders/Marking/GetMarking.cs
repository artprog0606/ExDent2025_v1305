using System;
using System.Collections.Generic;
using AdvantShop.Core;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.Marking;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders.Marking
{
    public class GetMarking : AbstractCommandHandler<MarkingOrderItemModel>
    {
        private readonly int _orderItemId;
        private OrderItem _orderItem;
        private int _amount;

        public GetMarking(int orderItemId)
        {
            _orderItemId = orderItemId;
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

        protected override MarkingOrderItemModel Handle()
        {
            var codes = new List<string>();
            var items = MarkingOrderItemService.GetMarkingItems(_orderItem.OrderItemID);
            
            for (var i = 0; i < _amount; i++)
            {
                codes.Add(items.Count > i ? items[i].Code : "");
            }

            return new MarkingOrderItemModel()
            {
                OrderItemId = _orderItem.OrderItemID,
                Name = _orderItem.Name,
                Codes = codes
            };
        }
    }
}
