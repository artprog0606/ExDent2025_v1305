using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class AddUpdateReservationResourceModel : IValidatableObject
    {
        public int Id { get; set; }
        public int? AffiliateId { get; set; }
        public int? ManagerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? BookingIntervalMinutes { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase PhotoFile { get; set; }
        public string PhotoSrc { get; set; }

        public List<string> MondayTimes { get; set; }
        public List<string> TuesdayTimes { get; set; }
        public List<string> WednesdayTimes { get; set; }
        public List<string> ThursdayTimes { get; set; }
        public List<string> FridayTimes { get; set; }
        public List<string> SaturdayTimes { get; set; }
        public List<string> SundayTimes { get; set; }

        public List<string> Tags { get; set; }
        public bool CanBeEditing { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (!AffiliateId.HasValue)
            //    yield return new ValidationResult("Не указан филиал");

            //if (!CustomerId.HasValue)
            //    yield return new ValidationResult("Укажите сотрудника");

            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Укажите название");
        }

        public static explicit operator AddUpdateReservationResourceModel(ReservationResource reservationResource)
        {
            return new AddUpdateReservationResourceModel()
            {
                Id = reservationResource.Id,
                ManagerId = reservationResource.ManagerId,
                Name = reservationResource.Name,
                Description = reservationResource.Description,
                SortOrder = reservationResource.SortOrder,
                Image = reservationResource.Image,
                PhotoSrc = reservationResource.ImageSrc,
                Enabled = reservationResource.Enabled,
                Tags = TagService.GetReservationResourceTags(reservationResource.Id).Select(x => x.Name).ToList()
            };
        }
    }
}
