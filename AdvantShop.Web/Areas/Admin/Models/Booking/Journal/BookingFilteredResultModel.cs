using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingFilteredResultModel
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public string AffiliateName { get; set; }
        public string CustomerName { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusName {
            get { return Status.Localize(); }
        }
        public int? ReservationResourceId { get; set; }
        public string ReservationResourceName { get; set; }
        public float Sum { get; set; }
        public string SumFormatted
        {
            get
            {
                return PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);
            }
        }
        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid
        {
            get { return PaymentDate != null; }
        }
        public DateTime BeginDate { get; set; }
        public string BeginDateFormatted { get { return Culture.ConvertDate(BeginDate); } }
        public DateTime EndDate { get; set; }
        public string EndDateFormatted { get { return Culture.ConvertDate(EndDate); } }
        public DateTime DateAdded { get; set; }
        public string DateAddedFormatted { get { return Culture.ConvertDate(DateAdded); } }
    }
}
