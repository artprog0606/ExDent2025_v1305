
using System.Collections.Generic;
using AdvantShop.Configuration;

namespace AdvantShop.Web.Admin.ViewModels.Settings
{
    public class OtherSettings
    {
        public ETemplateSettingSection? Section { get; set; }
        public List<TemplateSettingSection> Settings { get; set; }
    }
}
