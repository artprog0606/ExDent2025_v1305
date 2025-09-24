using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class GetBookingByTimeMonthDaysDto
    {
        public GetBookingByTimeMonthDaysDto()
        {
            SelectedServices = new List<int>();
        }

        public int AffiliateId { get; set; }
        public int ResourceId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool LoadPrevMonth { get; set; }
        public bool LoadCurrentMonth { get; set; }
        public bool LoadNextMonth { get; set; }
        public List<int> SelectedServices { get; set; }
    }
}
