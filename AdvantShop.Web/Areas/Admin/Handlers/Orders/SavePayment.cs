using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SavePayment
    {
        private readonly Order _order;
        private readonly string _country;
        private readonly string _city;
        private readonly string _district;
        private readonly string _region;
        private readonly BasePaymentOption _payment;

        public SavePayment(Order order, string country, string city, string district, string region, BasePaymentOption payment)
        {
            _order = order;
            _country = country;
            _city = city;
            _district = district;
            _region = region;
            _payment = payment;
        }

        public void Execute()
        {
            var trackChanges = !_order.IsDraft;
            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

            var customer = _order.OrderCustomer;

            if (_order.IsDraft && customer != null && (customer.Country != _country || customer.Region != _region || customer.City != _city || customer.District != _district))
            {
                customer.Country = _country ?? "";
                customer.Region = _region ?? "";
                customer.District = _district ?? "";
                customer.City = _city ?? "";

                OrderService.UpdateOrderCustomer(customer, changedBy, trackChanges);
            }

            _order.PaymentMethodId = _payment.Id;
            _order.ArchivedPaymentName = _payment.Name ?? "";
            _order.Sum -= _order.PaymentCost; // убираем старую наценку
            _order.PaymentCost = _payment.Rate;
            _order.Sum += _order.PaymentCost; // прибавляем новую наценку
            var paymentDetails = _payment.GetDetails(_order);
            
            if (paymentDetails != null)
                OrderService.UpdatePaymentDetails(_order.OrderID, paymentDetails, changedBy, trackChanges);

            new UpdateOrderTotal(_order).Execute();
        }
    }
}
