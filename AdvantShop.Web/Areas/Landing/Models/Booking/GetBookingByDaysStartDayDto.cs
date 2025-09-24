using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class GetBookingByDaysStartDayDto
    {
        public GetBookingByDaysStartDayDto()
        {
            SelectedServices = new List<int>();
        }

        public int AffiliateId { get; set; }
        public int ResourceId { get; set; }
        public List<int> SelectedServices { get; set; }
        public string TimeFrom { get; set; }
        public string TimeEnd { get; set; }
        public bool TimeEndAtNextDay { get; set; }
    }
}
