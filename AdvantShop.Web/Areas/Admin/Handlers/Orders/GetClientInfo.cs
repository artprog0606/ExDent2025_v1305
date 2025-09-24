using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetClientInfo
    {
        private readonly OrderModel _orderModel;

        public GetClientInfo(OrderModel orderModel)
        {
            _orderModel = orderModel;
        }

        public ClientInfoModel Execute()
        {
            var order = _orderModel.Order;

            var model = new ClientInfoModel()
            {
                CustomerGroup = order.GroupDiscountString,
            };

            var customer = _orderModel.Customer;

            if (_orderModel.Order.LinkedCustomerId != null)
            {
                var c = CustomerService.GetCustomer(_orderModel.Order.LinkedCustomerId.Value);
                if (c != null)
                    customer = c;
            }
            
            if (customer != null)
            {
                model.CustomerId = customer.Id.ToString();
                model.Customer = customer;
                model.OrderId = _orderModel.OrderId;
                model.Order = _orderModel.Order;
                model.InterestingCategories = StatisticService.GetCustomerInterestingCategories(customer.Id).Take(8).ToList();

                model.Statistic = new ClientStatistic
                {
                    RegistrationDate = Culture.ConvertDate(customer.RegistrationDateTime),
                    RegistrationDuration = customer.RegistrationDateTime.GetDurationString(DateTime.Now),
                    AdminCommentAboutCustomer = customer.AdminComment,
                    OrdersCount = OrderService.GetOrdersCountByCustomer(customer.Id)
                };
                var count = StatisticService.GetCustomerOrdersCount(customer.Id);
                var sum = count > 0 ? StatisticService.GetCustomerOrdersSum(customer.Id) : 0;

                model.Statistic.OrdersSum = sum.FormatPrice();
                model.Statistic.AverageCheck = (count > 0 ? (sum / count) : 0).FormatPrice();
            }

            return model;
        }
    }
}
