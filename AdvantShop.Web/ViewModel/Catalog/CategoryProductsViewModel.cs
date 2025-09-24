using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.ViewModel.Catalog
{
    public class CategoryProductsViewModel : ProductViewModel
    {
        public CategoryProductsViewModel(List<ProductModel> products) : base(products)
        {
            DisplayComparison = false;
            DisplayPhotoPreviews = false;
        }

        public CategoryProductsViewModel(List<ProductModel> products, bool isMobile) : base(products, isMobile)
        {
            DisplayComparison = false;
            DisplayPhotoPreviews = false;
        }
        
        public string Url { get; set; }
        public int ProductsCount { get; set; }
    }
}