using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class ProductGiftsViewModel : BaseProductViewModel
    {
        public Product Product { get; set; }
        public List<GiftModel> Gifts { get; set; }
        public bool ShowProductLink { get; set; }
    }
}