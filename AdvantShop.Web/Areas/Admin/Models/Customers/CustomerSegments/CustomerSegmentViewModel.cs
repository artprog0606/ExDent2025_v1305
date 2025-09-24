using System;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerSegments
{
    public class CustomerSegmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted { get { return Culture.ConvertDate(CreatedDate); } }
        public int CustomersCount { get; set; }
    }
}
