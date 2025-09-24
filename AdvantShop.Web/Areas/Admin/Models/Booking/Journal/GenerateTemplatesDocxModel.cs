using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class GenerateTemplatesDocxModel
    {
        public int BookingId { get; set; }
        public List<int> TemplatesDocx { get; set; }
        public bool Attach { get; set; }
    }
}
