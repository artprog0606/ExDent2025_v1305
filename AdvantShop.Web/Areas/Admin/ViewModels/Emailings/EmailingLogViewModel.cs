using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.ViewModels.Emailings
{
    public class EmailingLogViewModel
    {
        public Guid EmailingId { get; set; }
        public string EmailSubject { get; set; }
        public int? TriggerId { get; set; }
        public List<KeyValuePair<string, string>> BreadCrumbs { get; set; }
        public string BackUrl { get; set; }
        public int? FormatId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Statuses { get; set; }
    }
}
