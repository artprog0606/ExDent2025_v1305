using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetPayments
    {
        private readonly int _orderId;
        private string _country;
        private string _city;
        private string _region;
        private string _district;
        private CustomerType _customerType;
        private readonly Order _order;

        public GetPayments(int orderId, string country, string city, string region, string district)
        {
            _orderId = orderId;
            _country = country;
            _city = city;
            _region = region;
            _district = district;
        }

        public GetPayments(Order order)
        {
            _order = order;
        }

        public List<BasePaymentOption> Execute()
        {
            var order = _order ?? OrderService.GetOrder(_orderId);
            if (order == null || order.OrderItems == null || order.OrderItems.Count == 0)
                return null;

            if (!order.IsDraft && order.OrderCustomer != null)
            {
                _country = order.OrderCustomer.Country;
                _region = order.OrderCustomer.Region;
                _district = order.OrderCustomer.District;
                _city = order.OrderCustomer.City;
            }

            var manager = new PaymentManager(
                config => config
                         .ByOrder(order, actualizeShippingAndPayment: true)
                         .WithCountry(_country ?? "")
                         .WithRegion(_region ?? "")
                         .WithDistrict(_district ?? "")
                         .WithCity(_city ?? "")
                         .Build());
            return manager.GetOptions();
        }
    }
}
