using System.Collections.Generic;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class ProductTabsViewModel : BaseProductViewModel
    {
        public ProductTabsViewModel()
        {
            Tabs = new List<BaseTab>();
        }

        public ProductDetailsViewModel ProductModel { get; set; }

        public List<BaseTab> Tabs { get; set; }

        public string AdditionalDescription { get; set; }

        public int ReviewsCount { get; set; }

        public int VideosCount { get; set; }

        public bool UseStandartReviews { get; set; }
    }
}
