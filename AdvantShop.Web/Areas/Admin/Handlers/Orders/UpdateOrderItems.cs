using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class UpdateOrderItems
    {
        private readonly Order _order;
        private readonly bool _resetOrderCargoParams;

        public UpdateOrderItems(Order order, bool resetOrderCargoParams = false)
        {
            _order = order;
            _resetOrderCargoParams = resetOrderCargoParams;
        }

        public bool Execute()
        {
            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);
            var trackChanges = !_order.IsDraft;
            var oldOrderItems = OrderService.GetOrderItems(_order.OrderID);

            OrderService.AddUpdateOrderItems(_order.OrderItems, oldOrderItems, _order, changedBy, trackChanges);
            
            new UpdateOrderTotal(OrderService.GetOrder(_order.OrderID), _resetOrderCargoParams).Execute();
            
            return true;
        }
    }
}
