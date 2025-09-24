using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.News;

namespace AdvantShop.ViewModel.News
{
    public class NewsProductsViewModel
    {
        public ProductViewModel Products { get; set; }
        
        public NewsItem.ENewsProductsDisplayMode HowENewsProducts { get; set; }

        public bool HasItems
        {
            get { return Products != null && Products.Products.Any(); }
        }
    }
}