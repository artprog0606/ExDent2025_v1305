using System.Collections.Generic;

namespace AdvantShop.Web.Admin.ViewModels.Home
{
    public class BirthdayDashboardViewModel
    {
        public List<BirtdayItem> Birthday { get; set; }
    }

    public class BirtdayItem
    {
        public string SrcImage { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public int Sorting { get; set; }
        public bool IsToday { get; set; }
    }
}
