using System;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnerActReportModel
    {
        public int Id { get; set; }

        private DateTime PeriodFrom { get; set; }
        public string PeriodFromFormatted { get { return Culture.ConvertDateWithoutHours(PeriodFrom); } }

        private DateTime PeriodTo { get; set; }
        public string PeriodToFormatted { get { return Culture.ConvertDateWithoutHours(PeriodTo); } }

        private DateTime DateCreated { get; set; }
        public string DateCreatedFormatted { get { return Culture.ConvertDate(DateCreated); } }
    }
}
