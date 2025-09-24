using System;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Cms.StaticBlock
{
    public class StaticBlockFilterResultModel
    {
        public int StaticBlockId { get; set; }

        public string Key { get; set; }

        public string InnerName { get; set; }

        public string Content { get; set; }

        public DateTime Added { get; set; }
        public string AddedFormatted { get { return Culture.ConvertDate(Added); } }

        public DateTime Modified { get; set; }
        public string ModifiedFormatted { get { return Culture.ConvertDate(Modified); } }

        public bool Enabled { get; set; }
        
    }
}
