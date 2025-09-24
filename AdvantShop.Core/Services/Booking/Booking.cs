using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Booking
{
    public class Booking
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }

        private Affiliate _affiliate;

        public Affiliate Affiliate
        {
            get
            {
                return _affiliate ?? (_affiliate = AffiliateService.Get(AffiliateId));
            }
        }

        public int? ReservationResourceId { get; set; }

        private ReservationResource _reservationResource;

        public ReservationResource ReservationResource
        {
            get
            {
                return _reservationResource ?? (ReservationResourceId.HasValue ? _reservationResource = ReservationResourceService.Get(ReservationResourceId.Value) : null);
            }
        }

        public Guid? CustomerId { get; set; }

        private Customer _customer = null;

        public Customer Customer
        {
            get
            {
                if (_customer != null)
                    return _customer;

                _customer = CustomerId != null ? CustomerService.GetCustomer(CustomerId.Value) : null;

                return _customer;
            }
            set { _customer = value; }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Patronymic { get; set; }
        public long? StandardPhone { get; set; }

        [Compare("Core.Booking.Booking.BeginDate")]
        public DateTime BeginDate { get; set; }

        [Compare("Core.Booking.Booking.EndDate")]
        public DateTime EndDate { get; set; }
        public DateTime DateAdded { get; set; }

        [Compare("Core.Booking.Booking.Status")]
        public BookingStatus Status { get; set; }

        [Compare("Core.Booking.Booking.Sum")]
        public float Sum { get; set; }

        private BookingCurrency _bookingCurrency;
        public BookingCurrency BookingCurrency
        {
            get
            {
                if (_bookingCurrency != null)
                    return _bookingCurrency;

                return _bookingCurrency = BookingCurrencyService.Get(Id) ?? CurrencyService.CurrentCurrency;
            }
            set { _bookingCurrency = value; }
        }

        private List<BookingItem> _bookingItems;

        public List<BookingItem> BookingItems
        {
            get { return _bookingItems ?? (_bookingItems = BookingItemsService.GetList(Id)); }
            set { _bookingItems = value; }
        }

        public bool IsFromAdminArea { get; set; }

        [Compare("Core.Booking.Booking.Manager", ChangeHistoryParameterType.Manager)]
        public int? ManagerId { get; set; }

        private Manager _manager;
        public Manager Manager
        {
            get { return _manager ?? (_manager = ManagerId.HasValue ? ManagerService.GetManager(ManagerId.Value) : null); }
        }

        [Compare("Core.Booking.Booking.OrderSource", ChangeHistoryParameterType.OrderSource)]
        public int OrderSourceId { get; set; }

        [Compare("Заказ", ChangeHistoryParameterType.Order)]
        public int? OrderId { get; set; }

        private OrderSource _orderSource { get; set; }
        public OrderSource OrderSource { get { return _orderSource ?? OrderSourceService.GetOrderSource(OrderSourceId); } }

        /// <summary>
        /// Скидка (процент)
        /// </summary>
        [Compare("Core.Booking.Booking.BookingDiscount")]
        public float BookingDiscount { get; set; }

        /// <summary>
        /// Скидка (число)
        /// </summary>
        [Compare("Core.Booking.Booking.BookingDiscountValue")]
        public float BookingDiscountValue { get; set; }

        /// <summary>
        /// Total bookin discount
        /// </summary>
        public float DiscountCost { get; set; }

        public bool Payed
        {
            get { return PaymentDate != null; }
        }

        [Compare("Core.Booking.Booking.PaymentDate")]
        public DateTime? PaymentDate { get; set; }

        [Compare("Core.Booking.Booking.PaymentCost")]
        public float PaymentCost { get; set; }

        private int? _paymentMethodId;
        public int? PaymentMethodId
        {
            get { return _paymentMethodId; }
            set
            {
                if (_paymentMethodId != value)
                    _paymentMethod = null;

                _paymentMethodId = value;
            }
        }

        private PaymentMethod _paymentMethod;

        public PaymentMethod PaymentMethod
        {
            get
            {
                return _paymentMethod ?? (_paymentMethod = PaymentMethodId.HasValue ? PaymentService.GetPaymentMethod(PaymentMethodId.Value) : null);
            }
        }

        [Compare("Core.Booking.Booking.PaymentName")]
        public string ArchivedPaymentName { get; set; }

        public string PaymentMethodName
        {
            get
            {
                return PaymentMethod != null ? PaymentMethod.Name : ArchivedPaymentName;
            }
        }

        private PaymentDetails _paymentDetails;
        public PaymentDetails PaymentDetails
        {
            get { return _paymentDetails ?? (_paymentDetails = BookingPaymentDetailsService.GetPaymentDetails(Id)); }
            set { _paymentDetails = value; }
        }

        public string AdminComment { get; set; }
    }

    public enum BookingStatus
    {
        [Localize("Core.Booking.BookingStatus.New")]
        New = 0,
        [Localize("Core.Booking.BookingStatus.Confirmed")]
        Confirmed = 1,
        [Localize("Core.Booking.BookingStatus.Completed")]
        Completed = 2,
        [Localize("Core.Booking.BookingStatus.Cancel")]
        Cancel = 3
    }
}
