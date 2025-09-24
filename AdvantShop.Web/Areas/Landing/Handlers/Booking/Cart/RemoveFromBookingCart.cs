using AdvantShop.Core.Services.Booking.Cart;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking.Cart
{
    public class RemoveFromBookingCart : AbstractCommandHandler<object>
    {
        private readonly int _itemId;

        public RemoveFromBookingCart(int itemId)
        {
            _itemId = itemId;
        }

        protected override object Handle()
        {
            if (_itemId == 0)
                return new { status = "fail" };

            var cart = ShoppingCartService.CurrentShoppingCart;

            var cartItem = cart.Find(item => item.ShoppingCartItemId == _itemId);
            if (cartItem != null)
            {
                ShoppingCartService.DeleteShoppingCartItem(cartItem);
            }

            return new { TotalItems = ShoppingCartService.CurrentShoppingCart.Count, status = "success" };
        }
    }
}
