using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public List<Category> Children { get; set; }
    }
}
