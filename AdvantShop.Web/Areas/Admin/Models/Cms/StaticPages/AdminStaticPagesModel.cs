using System;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Cms.StaticPages
{
    public partial class StaticPagesModel
    {
        public int StaticPageId { get; set; }
        public string PageName { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        
        public DateTime ModifyDate { get; set; }
        public string ModifyDateFormatted { get { return Culture.ConvertDate(ModifyDate); } }
    }
}
