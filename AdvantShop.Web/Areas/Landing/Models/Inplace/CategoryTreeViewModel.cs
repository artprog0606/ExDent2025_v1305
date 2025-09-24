using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models.Inplace
{
    public class CategoryTreeViewModel
    {
        public int CategoryId { get; set; }
        public List<string> Parents { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public  bool Opened { get; set; }
    }
}
