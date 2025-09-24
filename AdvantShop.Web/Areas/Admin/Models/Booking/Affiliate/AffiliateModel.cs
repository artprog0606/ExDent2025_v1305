using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Booking.Affiliate
{
    public class AffiliateModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public long? StandardPhone { get; set; }
        public int SortOrder { get; set; }
        public int BookingIntervalMinutes { get; set; }
        public bool Enabled { get; set; }
        public bool AccessForAll { get; set; }
        public List<int> ManagerIds { get; set; }
        public bool AnalyticsAccessForAll { get; set; }
        public List<int> AnalyticManagerIds { get; set; }
        public bool AccessToViewBookingForResourceManagers { get; set; }
        public bool IsActiveSmsNotification { get; set; }
        public int? ForHowManyMinutesToSendSms { get; set; }
        public string SmsTemplateBeforeStartBooiking { get; set; }
        public int? CancelUnpaidViaMinutes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Укажите название");

            if (BookingIntervalMinutes <= 0)
                yield return new ValidationResult("Укажите интервал бронирования");

        }
    }
}
