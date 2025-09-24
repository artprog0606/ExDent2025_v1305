using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsReservationResourceJournalFilterModel : IValidatableObject
    {
        public int? AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AffiliateId.HasValue)
                yield return new ValidationResult("Неуказан филиал");

            if (!ReservationResourceId.HasValue)
                yield return new ValidationResult("Неуказан ресурс");

            if (!StartDate.HasValue)
                yield return new ValidationResult("Неуказана стартовая дата");

            if (!EndDate.HasValue)
                yield return new ValidationResult("Неуказана конечная дата");

            if (ReservationResourceService.Get(ReservationResourceId.Value) == null)
                yield return new ValidationResult("Указанный ресурс не найден");
        }
    }
}
