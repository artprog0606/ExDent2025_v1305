using AdvantShop.Localization;
using System;

namespace AdvantShop.Web.Admin.Models.Marketing.Analytics
{
    public class SearchQueriesModel
    {
        public string Request { get; set; }
        public int ResultCount { get; set; }
        public DateTime Date { get; set; }
        public string DateFormatted
        {
            get { return Culture.ConvertDate(Date); }
        }
        public string SearchTerm { get; set; }
        public string Description { get; set; }
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
