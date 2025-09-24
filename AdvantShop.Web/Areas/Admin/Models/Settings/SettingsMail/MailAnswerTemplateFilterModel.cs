using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.SettingsMail
{
    public class MailAnswerTemplateFilterModel : BaseFilterModel
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }        
        public string Subject { get; set; }
        public string Body { get; set; }        
        public int SortOrder { get; set; }
        public bool? Active { get; set; }
    }
}
