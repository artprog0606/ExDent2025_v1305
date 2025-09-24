using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public class GenerateTemplatesDocxModel
    {
        public int OrderId { get; set; }
        public List<int> TemplatesDocx { get; set; }
        public bool Attach { get; set; }
    }
}
