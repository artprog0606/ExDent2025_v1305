using System.Collections.Generic;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            SendMessages = new Dictionary<string, bool>();
        }

        public Partner Partner { get; set; }

        public Dictionary<string, bool> SendMessages { get; set; }
    }
}