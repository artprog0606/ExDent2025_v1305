using AdvantShop.Core.Services.Booking.Cart;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking.Cart
{
    public class ClearBookingCart : AbstractCommandHandler<object>
    {
        protected override object Handle()
        {
            ShoppingCartService.ClearShoppingCart();

            return new { TotalItems = ShoppingCartService.CurrentShoppingCart.Count, status = "success" };
        }
    }
}
