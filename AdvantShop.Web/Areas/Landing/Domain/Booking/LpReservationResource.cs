using AdvantShop.Core.Services.Booking;

namespace AdvantShop.App.Landing.Domain.Booking
{
    public class LpReservationResource
    {
        public int Id { get; set; }
        public int? ManagerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public string Image { get; set; }

        public static explicit operator LpReservationResource(ReservationResource resource)
        {
            if (resource == null)
                return null;
            return new LpReservationResource
            {
                Id = resource.Id,
                Name = resource.Name,
                ManagerId = resource.ManagerId,
                Description = resource.Description,
                ImageSrc = resource.ImageSrc,
                Image = resource.Image
            };
        }
    }
}
