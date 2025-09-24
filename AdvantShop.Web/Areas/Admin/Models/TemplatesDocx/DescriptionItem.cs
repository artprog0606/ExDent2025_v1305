using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.TemplatesDocx
{
    public class DescriptionItem
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        public List<DescriptionItem> Childs { get; set; }
    }
}
