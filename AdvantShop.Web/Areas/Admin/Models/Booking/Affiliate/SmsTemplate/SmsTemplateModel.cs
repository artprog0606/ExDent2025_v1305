using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.Models.Booking.Affiliate.SmsTemplate
{
    public class SmsTemplateModel : IValidatableObject
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusName
        {
            get { return Status.Localize(); }
        }
        public string Text { get; set; }
        public bool Enabled { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AffiliateId <= 0)
                yield return new ValidationResult("Укажите филиал");

            if (string.IsNullOrWhiteSpace(Text))
                yield return new ValidationResult("Укажите шаблон смс");
        }
    }
}
