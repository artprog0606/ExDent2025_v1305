using System.Collections.Generic;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IApiSettings
    {
        IList<IApiSettingItem> GetSettings();
    }
    
    public interface IApiSettingItem
    {
        string Key { get; set; }
        object Value { get; set; }
    }
}