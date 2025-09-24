using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetPaymentsHandler
    {
        private readonly GetPayments _model;
        private readonly List<PaymentMethod> _listMethod;

        public GetPaymentsHandler(GetPayments model)
        {
            _model = model;
            if (_model.Summary == null)
                _model.Summary = new BookingSummaryModel();

            //_model.Summary.BookingCurrency = _model.BookingId > 0
            //    ? BookingCurrencyService.Get(_model.BookingId) ?? CurrencyService.CurrentCurrency
            //    : (BookingCurrency)CurrencyService.CurrentCurrency;
            _listMethod = PaymentService.GetAllPaymentMethods(true);
        }

        public List<BasePaymentOption> Execute()
        {
            var options = new List<BasePaymentOption>();
            var currentItems = _listMethod.Where(x => x.CustomerType == (int)_model.Summary.CustomerType || x.CustomerType == (int)CustomerType.All).ToList();

            var shipping = new BaseShippingOption() { Name = "Temp", Rate = 0f };

            foreach (var paymentMethod in currentItems)
            {
                if (paymentMethod is IPaymentGiftCertificate) continue;
                
                var servicesCostInPaymentCurrency =
                    _model.Summary.ServicesCost
                        .ConvertCurrency(_model.Summary.BookingCurrency ?? CurrencyService.CurrentCurrency,
                            paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency);

                if (paymentMethod is ICreditPaymentMethod creditPaymentMethod 
                    && creditPaymentMethod.ActiveCreditPayment 
                    && (creditPaymentMethod.MinimumPrice > servicesCostInPaymentCurrency
                        || creditPaymentMethod.MaximumPrice < servicesCostInPaymentCurrency)) continue;

                //if (ShippingMethodService.IsPaymentNotUsed(shipping.MethodId, item.PaymentMethodId))
                //    continue;

                options.Add(paymentMethod.GetOption(shipping, _model.Summary.Sum, _model.Summary.CustomerType));
            }
            options = options.Where(x => x != null).ToList();

            return options;
        }
    }
}
