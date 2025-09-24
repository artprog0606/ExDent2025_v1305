using System.Collections.Generic;
using AdvantShop.CMS;

namespace AdvantShop.Web.Admin.Models.Cms.Menus
{
    public class MenusTree
    {
        public string Id { get; set; }

        public EMenuType MenuType { get; set; }

        public int? SelectedId { get; set; }

        public bool ShowRoot { get; set; }
        public bool ShowActions { get; set; }

        public int? ExcludeId { get; set; }
        
        public bool LevelLimitation { get; set; }
    }

    public class AdminMenuTreeViewItem
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string name { get; set; }
        public Dictionary<string, string> li_attr { get; set; }
        public Dictionary<string, string> a_attr { get; set; }
        public bool children { get; set; }
        public AdminMenuTreeViewItemState state { get; set; }
    }

    public class AdminMenuTreeViewItemState
    {
        public bool opened { get; set; }
        public bool selected { get; set; }
    }
}
