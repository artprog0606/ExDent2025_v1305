using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Booking.Affiliate
{
    public class AddUpdateAdditionalTimeModel : IValidatableObject
    {
        public int? AffiliateId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> Times { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AffiliateId.HasValue)
                yield return new ValidationResult("Не указан филиал");

            if (!Date.HasValue && (!StartDate.HasValue || !EndDate.HasValue))
                yield return new ValidationResult("Не указана дата");
        }
    }
}
