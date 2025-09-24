using System.Collections.Generic;
using System.Web.Routing;

namespace AdvantShop.Core.Modules.Interfaces
{
    /// <summary>
    /// New admin settings
    /// </summary>
    public interface IAdminModuleSettings
    {
        List<ModuleSettingTab> AdminSettings { get; }

        bool IsMobileAdminReady { get; }
    }

    public class ModuleSettingTab
    {
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        //IsAdaptive - показывает адптирован ли к мобильной версии модуль
        public bool IsAdaptive { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public ModuleSettingTab()
        {
            RouteValues = new RouteValueDictionary() {{"area", ""}};
        }
    }
}
