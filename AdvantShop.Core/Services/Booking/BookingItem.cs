using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.ChangeHistories;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingItem
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int? ServiceId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }

        [Compare("Core.Booking.BookingItem.Price", ChangeHistoryParameterType.BookingItemField)]
        public float Price { get; set; }

        [Compare("Core.Booking.BookingItem.Amount", ChangeHistoryParameterType.BookingItemField)]
        public float Amount { get; set; }

        private Service _service;
        public Service Service
        {
            get
            {
                if (ServiceId.HasValue)
                    return _service ?? (_service = ServiceService.Get(ServiceId.Value));
                return null;
            }
        }
    }
}
