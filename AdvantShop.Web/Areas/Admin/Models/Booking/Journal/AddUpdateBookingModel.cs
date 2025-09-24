using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Payment;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class AddUpdateBookingModel : IValidatableObject
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public DateTime BeginDate { get; set; }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                //for 23:00-00:00, 23:45-00:00
                if (_endDate < BeginDate && _endDate.TimeOfDay == TimeSpan.Zero)
                    _endDate += new TimeSpan(1, 0, 0, 0);

                return _endDate;
            }
            set { _endDate = value; }
        }
        public BookingStatus Status { get; set; }
        public string StatusName { get; set; }

        //Customer Fields
        public Guid? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Organization { get; set; }
        public string Phone { get; set; }
        public long? StandardPhone { get; set; }
        public string EMail { get; set; }
        public DateTime? BirthDay { get; set; }
        public List<CustomerFieldWithValue> CustomerFields { get; set; }

        public BookingCustomerSocial Social { get; set; }

        public List<BookingItemModel> Items { get; set; }
        public bool UserConfirmed { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool CanBeEditing { get; set; }
        public int? ManagerId { get; set; }
        public int OrderSourceId { get; set; }
        public int? OrderId { get; set; }

        public bool Payed { get; set; }
        public BookingSummaryModel Summary { get; set; }

        public string AdminComment { get; set; }
        
        public CustomerType CustomerType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AffiliateId <= 0)
                yield return new ValidationResult("Неуказан филиал");
            if (Summary == null)
                yield return new ValidationResult("Summary is null");
        }
    }

    public class BookingSummaryModel
    {
        public BookingCurrency BookingCurrency { get; set; }
        public float ServicesCost { get; set; }
        public string ServicesCostStr
        {
            get { return ServicesCost.FormatPrice(BookingCurrency); }
        }
        public float BookingDiscount { get; set; }
        public float BookingDiscountValue { get; set; }
        public float DiscountCost { get; set; }
        public string DiscountCostStr
        {
            get { return DiscountCost.FormatPrice(BookingCurrency); }
        }
        public int? PaymentMethodId { get; set; }
        public string PaymentName { get; set; }
        public float PaymentCost { get; set; }
        public string PaymentCostStr
        {
            get { return PaymentCost.FormatPrice(BookingCurrency); }
        }
        public PaymentDetails PaymentDetails { get; set; }
        public string PaymentKey { get; set; }
        public bool ShowSendBillingLink { get; set; }
        public bool ShowCreateBillingLink { get; set; }
        public bool ShowInnPaymentDetails { get; set; }
        public bool ShowCompanyNamePaymentDetails { get; set; }
        //public bool ShowPrintPaymentDetails { get; set; }
        //public string PrintPaymentDetailsText { get; set; }
        //public string PrintPaymentDetailsLink { get; set; }
        public float Sum { get; set; }
        public string SumStr
        {
            get { return Sum.FormatPrice(BookingCurrency); }
        }
        public int? OrderId { get; set; }
        public CustomerType CustomerType { get; set; }
    }
}
