using AdvantShop.Orders;

namespace AdvantShop.Areas.Api.Models.Cart
{
    public class CartItemMap
    {
        public int Index { get; set; }
        public ShoppingCartItem CartItem { get; set; } 
    }
}