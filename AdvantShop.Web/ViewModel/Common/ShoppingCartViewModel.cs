using AdvantShop.Models;

namespace AdvantShop.ViewModel.Common
{
    public partial class ShoppingCartViewModel: BaseModel
    {
        public string Amount { get; set; }
        public string Type { get; set; }

        public float TotalItems { get; set; }
        
        public string TotalPrice { get; set; }
        public bool ShowPriceInMiniCart { get; set; }
        public bool EnableShoppingCartPopup { get; set; }
    }
}