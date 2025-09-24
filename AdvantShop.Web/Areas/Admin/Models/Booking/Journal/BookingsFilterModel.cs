using System;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsFilterModel : BaseFilterModel<int>
    {
        public int? BookingId { get; set; }
        public int? AffiliateFilterId { get; set; }

        public BookingStatus? Status { get; set; }
        public BookingStatus? NoStatus { get; set; }
        public int? ReservationResourceId { get; set; }
        public Guid? CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public bool? IsPaid { get; set; }
        public int? OrderSourceId { get; set; }

        public float? SumFrom { get; set; }
        public float? SumTo { get; set; }

        public DateTime? BeginDateFrom { get; set; }
        public DateTime? BeginDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }

        public DateTime? DateAddedFrom { get; set; }
        public DateTime? DateAddedTo { get; set; }
    }
}
