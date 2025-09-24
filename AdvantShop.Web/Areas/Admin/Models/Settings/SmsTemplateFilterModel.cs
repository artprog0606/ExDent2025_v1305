using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SmsTemplateFilterModel :  BaseFilterModel
    {
        public string SmsText { get; set; }

        public bool? Enabled { get; set; }

        public int? OrderStatusId { get; set; }
    }
}
