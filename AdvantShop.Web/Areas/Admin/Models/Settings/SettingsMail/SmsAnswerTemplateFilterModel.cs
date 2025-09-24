using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.SettingsMail
{
    public class SmsAnswerTemplateFilterModel : BaseFilterModel
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }        
        public string Text { get; set; }        
        public int SortOrder { get; set; }
        public bool? Active { get; set; }
    }
}
