using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class UpdateAfterDragBookingModel : IValidatableObject
    {
        public int Id { get; set; }
        public int? ReservationResourceId { get; set; }
        public DateTime BeginDate { get; set; }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                //for 23:00-00:00, 23:45-00:00
                if (_endDate < BeginDate)
                    _endDate += new TimeSpan(1, 0, 0, 0);

                return _endDate;
            }
            set { _endDate = value; }
        }
        public bool UserConfirmed { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id <= 0)
                yield return new ValidationResult("Неуказана бронь");
        }
    }
}
