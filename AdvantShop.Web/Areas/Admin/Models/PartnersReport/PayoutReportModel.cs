using System;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.PartnersReport
{
    public class PayoutReportModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime PeriodTo { get; set; }
        public DateTime DateCreated { get; set; }

        public string DateCreatedFormatted
        {
            get { return Culture.ConvertDate(DateCreated); }
        }

        public string PeriodToFormatted
        {
            get { return PeriodTo.ToString("MMMM yyyy"); }
        }
    }
}
