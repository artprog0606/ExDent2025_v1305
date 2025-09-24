using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class AddUpdateAdditionalTimeModel : IValidatableObject
    {
        public int? AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public DateTime? Date { get; set; }
        public List<string> Times { get; set; }
        public bool UserConfirmed { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AffiliateId.HasValue)
                yield return new ValidationResult("Не указан филиал");

            if (!ReservationResourceId.HasValue)
                yield return new ValidationResult("Укажите ресурс");

            if (!Date.HasValue)
                yield return new ValidationResult("Не указана дата");
        }
    }
}
