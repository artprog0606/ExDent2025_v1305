using System;

namespace AdvantShop.Web.Admin.Models.Cms.Menus
{
    [Serializable]
    public class AdminMenuReplaceModel
    {
        public string Module { get; set; }
        public string SalesChannel { get; set; }
        public AdminMenuModel MenuItem { get; set; }
    }
}