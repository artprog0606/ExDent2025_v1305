using System.Collections.Generic;
using AdvantShop.Core.Services.Configuration;

namespace AdvantShop.App.Landing.Domain
{
    public class LpBlockListItem
    {
        public LpBlockListItem()
        {
            Blocks = new List<LpBlockItem>();
        }

        public string Category { get; set; }
        public string CategoryName { get; set; }
        public List<LpBlockItem> Blocks { get; set; } 
    }

    public class LpBlockItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public bool ExperimentalFeaturesBlock { get; set; }

        public EProviderSetting[] SettingValues { get; set; }
    }
}
