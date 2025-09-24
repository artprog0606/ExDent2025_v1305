using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Models.Coupons;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Coupons
{
    public sealed class AddCoupon : AbstractCommandHandler<CouponResponse>
    {
        private readonly string _code;

        public AddCoupon(string code)
        {
            _code = code;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_code))
                throw new BlException("Укажите код купона");
        }

        protected override CouponResponse Handle()
        {
            var appliedCoupon = CouponService.GetCustomerCoupon();
            if (appliedCoupon != null)
                throw new BlException(T("Checkout.CheckoutCart.CouponNotApplied"));
            
            var cert = GiftCertificateService.GetCertificateByCode(_code);
            
            if (SettingsCheckout.EnableGiftCertificateService && cert != null && cert.Paid && !cert.Used && cert.Enable)
            {
                GiftCertificateService.AddCustomerCertificate(cert.CertificateId);
                return new CouponResponse() { Result = true };
            }

            var currentCustomer = CustomerContext.CurrentCustomer;
            var customerGroup = currentCustomer.CustomerGroup;
            //if (customerGroup.CustomerGroupId != CustomerGroupService.DefaultCustomerGroup)
            //{
            //    return Json(new {result = false});
            //}

            // Нельзя применить купон при использовании бонусов
            var current = MyCheckout.Factory(currentCustomer.Id);
            if (current != null && current.Data.Bonus.UseIt && BonusSystem.ForbidOnCoupon)
            {
                throw new BlException(T("Checkout.CheckoutCart.CouponNotAppliedWithBonus"));
            }

            var coupon = CouponService.GetCouponByCode(_code);
            if (coupon != null && !coupon.CustomerGroupIds.Contains(customerGroup.CustomerGroupId))
                throw new BlException(T("Checkout.CheckoutCart.CouponNotApplied"));

            if (coupon != null && coupon.OnlyInMobileApp && !System.Web.HttpContext.Current.Request.IsMobileApp())
                throw new BlException(T("Coupon.CouponPost.CouponOnlyInMobileApp"));

            if (coupon != null && coupon.ForFirstOrderInMobileApp)
            {
                if (!System.Web.HttpContext.Current.Request.IsMobileApp())
                    throw new BlException(T("Coupon.CouponPost.CouponOnlyInMobileApp"));

                if (OrderService.IsCustomerHasConfirmedOrdersFromMobileApp(currentCustomer.Id))
                    throw new BlException(T("Coupon.CouponPost.CouponOnlyForFirstOrderInMobileApp"));
            }

            if (coupon != null && coupon.OnlyOnCustomerBirthday)
            {
                var error = CouponService.CheckCustomerCouponByBirthday(coupon, currentCustomer);
                if (error.IsNotEmpty())
                    throw new BlException(error);
            }

            var customerByEmail =
                !currentCustomer.RegistredUser && current != null && current.Data.User.Email.IsNotEmpty()
                    ? CustomerService.GetCustomerByEmail(current.Data.User.Email)
                    : null;

            if (coupon == null 
                || !CouponService.CanApplyCustomerCoupon(coupon) 
                || (customerByEmail != null && !CouponService.CanApplyCustomerCoupon(coupon, customerByEmail.Id)))
            {
                throw new BlException(T("Checkout.CheckoutCart.CouponNotApplied"));
            }

            CouponService.AddCustomerCoupon(coupon.CouponID);

            if (coupon.Type == CouponType.FixedOnGiftOffer)
            {
                if (!coupon.GiftOfferId.HasValue || !coupon.IsAppliedToCard(ShoppingCartService.CurrentShoppingCart))
                {
                    throw new BlException(T("Checkout.CheckoutCart.CouponNotApplied"));
                }
                
                var product = ProductService.GetProductByOfferId(coupon.GiftOfferId.Value);

                if (product == null || !product.Enabled || !product.CategoryEnabled)
                {
                    throw new BlException(T("Checkout.CheckoutCart.CouponNotApplied"));
                }

                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                {
                    OfferId = coupon.GiftOfferId.Value,
                    Amount = product.GetMinAmount(),
                    CustomPrice = coupon.GetRate(),
                    IsForbiddenChangeAmount = true,
                    IsByCoupon = true
                });

                if (current != null)
                {
                    current.Data.ShopCartHash = ShoppingCartService.CurrentShoppingCart.GetHashCode();
                    current.Update();
                }
            }

            ShoppingCartService.ResetHttpContentCard(ShoppingCartType.ShoppingCart);
            var cart = ShoppingCartService.CurrentShoppingCart;

            string msg = null;
            if (cart.Coupon == null || !cart.CouponCanBeApplied)
            {
                msg = T("Checkout.CheckoutCart.CouponNotApplied"); // result??
            }

            return new CouponResponse() { Result = true, Message = msg }; //Json(new { result = true, msg = msg });
        }
    }
}