using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Models;

namespace AdvantShop.ViewModel.Home
{
    public partial class MainPageProductsViewModel : BaseModel
    {
        public List<ProductViewModel> ProductLists { get; set; }
        
        public bool HideNewProductsLink { get; set; }

        public bool NewArrivals { get; set; }

        public MainPageProductsViewModel()
        {
            ProductLists = new List<ProductViewModel>();
        }
    }
}