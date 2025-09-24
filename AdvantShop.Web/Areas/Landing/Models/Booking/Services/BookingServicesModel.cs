using AdvantShop.Core.Services.Landing.Blocks;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models.Booking.Services
{
    public class BookingServicesModel
    {
        public LpBlock Block { get; set; }
        public LpBlockConfig Config { get; set; }
        public List<ServicesContentItemsModel> Services { get; set; }
    }
}
