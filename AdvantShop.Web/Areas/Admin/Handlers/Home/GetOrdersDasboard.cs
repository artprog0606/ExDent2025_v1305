using System.Data;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.ViewModels.Home;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetOrdersDasboard
    {
        public GetOrdersDasboard()
        {
        }

        public OrdersDasboardViewModel Execute()
        {
            var customer = CustomerContext.CurrentCustomer;

            var managerCondition = string.Empty;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if(manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.Assigned)
                    {
                        managerCondition = string.Format(" AND [Order].ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.AssignedAndFree)
                    {
                        managerCondition = string.Format(" AND ([Order].ManagerId = {0} or [Order].ManagerId is null)", manager.ManagerId);
                    }
                }
            }

            var statuses = SQLDataAccess.Query<OrderItemDasboardViewModel>(
                "SELECT OrderStatusID, StatusName, Color, " +
                "(Select Count(OrderID) From [Order].[Order] Where IsDraft = 0 AND [OrderStatusID] = OrderStatus.[OrderStatusID] "+ managerCondition + ") as OrdersCount " +
                "FROM [Order].OrderStatus " +
                "ORDER BY SortOrder")
                .ToList();
            
            var totalOrdersCount = SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(OrderID) FROM [Order].[Order] Where IsDraft = 0 " + managerCondition,
                CommandType.Text);

            statuses.Insert(0, new OrderItemDasboardViewModel()
            {
                Color = "000",
                StatusName = "Все заказы",
                OrdersCount = totalOrdersCount
            });

            return new OrdersDasboardViewModel() {OrderStatuses = statuses};
        }
    }
}
