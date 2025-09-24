using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class GetBookingByTimeFreeDayDto
    {
        public GetBookingByTimeFreeDayDto()
        {
            SelectedServices = new List<int>();
        }

        public int AffiliateId { get; set; }
        public int ResourceId { get; set; }
        public List<int> SelectedServices { get; set; }
    }
}
