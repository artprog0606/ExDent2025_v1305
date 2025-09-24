using AdvantShop.Configuration;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.ViewModel.Cart
{
    public class CartViewModel
    {
        public ShoppingCart Cart { get; set; }

        public bool IsDemo { get; set; }

        public bool ShowConfirmButton { get; set; }

        public bool ShowBuyOneClick { get; set; }

        public int PhotoWidth { get; set; }

        public bool ShowPriceAmountNextDiscountsInCart => 
            (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes) &&
            SettingsPriceRules.ShowPriceAmountNextDiscountsInCart;
    }
}