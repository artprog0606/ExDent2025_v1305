using System;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public sealed class GetUserMeStatistics : AbstractCommandHandler<UserMeStatisticsResponse>
    {
        private Customer _customer;

        protected override void Validate()
        {
            _customer = CustomerContext.CurrentCustomer;
            
            if (!_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
        }

        protected override UserMeStatisticsResponse Handle()
        {
            var count = StatisticService.GetCustomerOrdersCount(_customer.Id);
            var sum = count > 0 ? StatisticService.GetCustomerOrdersSum(_customer.Id) : 0;

            return new UserMeStatisticsResponse()
            {
                OrdersSum = sum.FormatPrice(),
                OrdersCount = count,
                AverageCheck = (count > 0 ? (sum / count) : 0).FormatPrice(),

                DurationOfWorkWithClient = _customer.RegistrationDateTime.GetDurationString(DateTime.Now),
                AddressesCount = _customer.Contacts.Count,
                WishListCount = ShoppingCartService.CurrentWishlist.Count,
                ActiveOrdersCount = StatisticService.GetCustomerOrdersCountNotCompleteNotCancelled(_customer.Id)
            };
        }
    }
}