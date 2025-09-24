using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Coupons
{
    public sealed class RemoveCoupon : AbstractCommandHandler
    {
        protected override void Handle()
        {
            var coupon = CouponService.GetCustomerCoupon();
            if (coupon == null)
                return;
            
            CouponService.DeleteCustomerCoupon(coupon.CouponID);
            
            if (coupon.Type == CouponType.FixedOnGiftOffer && coupon.GiftOfferId.HasValue && coupon.GiftOfferId.Value != 0)
            {
                var item = ShoppingCartService.CurrentShoppingCart.FirstOrDefault(x => x.IsByCoupon && x.OfferId == coupon.GiftOfferId.Value);
                if (item != null)
                    ShoppingCartService.DeleteShoppingCartItem(item);
                var current = MyCheckout.Factory(CustomerContext.CustomerId);
                current.Data.ShopCartHash = ShoppingCartService.CurrentShoppingCart.GetHashCode();
                current.Update();
            }
        }
    }
}