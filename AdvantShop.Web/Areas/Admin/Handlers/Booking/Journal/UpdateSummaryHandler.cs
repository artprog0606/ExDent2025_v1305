using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Booking.Journal;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class UpdateSummaryHandler
    {
        private readonly UpdateSummaryModel _model;

        public UpdateSummaryHandler(UpdateSummaryModel model)
        {
            _model = model;
            if (_model.CurrentItems == null)
                _model.CurrentItems = new List<BookingItemModel>();
            if (_model.Summary == null)
                _model.Summary = new BookingSummaryModel();

            _model.Summary.BookingCurrency = _model.BookingId > 0 
                ? BookingCurrencyService.Get(_model.BookingId) ?? CurrencyService.CurrentCurrency
                : (BookingCurrency)CurrencyService.CurrentCurrency;
        }

        public BookingSummaryModel Execute()
        {
            float totalPrice = 0;
            float totalItemsPrice = 0;
            float totalDiscount = 0;

            totalItemsPrice =
                _model.CurrentItems.Sum(item => item.Price * item.Amount);

            totalDiscount += _model.Summary.BookingDiscount > 0 ? (_model.Summary.BookingDiscount * totalItemsPrice / 100) : 0;
            totalDiscount += _model.Summary.BookingDiscountValue;

            totalDiscount = totalDiscount.RoundPrice(_model.Summary.BookingCurrency.CurrencyValue, _model.Summary.BookingCurrency);

            totalPrice = (totalItemsPrice - totalDiscount + _model.Summary.PaymentCost).RoundPrice(_model.Summary.BookingCurrency.CurrencyValue, _model.Summary.BookingCurrency);

            if (totalPrice < 0) totalPrice = 0;

            _model.Summary.ServicesCost = totalItemsPrice;
            _model.Summary.DiscountCost = totalDiscount;
            _model.Summary.Sum = totalPrice;

            if (_model.UpdateStatusBillingLink)
            {
                var booking = _model.BookingId > 0 ? BookingService.Get(_model.BookingId) : null;

                _model.Summary.ShowSendBillingLink = booking != null && booking.Customer != null &&
                                                     booking.OrderId.HasValue && !booking.Payed &&
                                                     booking.Status != BookingStatus.Cancel;

                _model.Summary.ShowCreateBillingLink = booking != null && booking.Customer != null &&
                                                       !booking.OrderId.HasValue &&
                                                       booking.Status != BookingStatus.Cancel;
            }

            return _model.Summary;
        }
    }
}
