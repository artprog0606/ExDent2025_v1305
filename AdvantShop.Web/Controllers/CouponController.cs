using AdvantShop.Core.Services.Catalog;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core;
using AdvantShop.Handlers.Coupons;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CouponController : BaseClientController
    {
        public JsonResult CouponJson()
        {
            var totalDiscount = ShoppingCartService.CurrentShoppingCart.TotalDiscount;
            var summary = new List<object>();
            if (ShoppingCartService.CurrentShoppingCart.Certificate != null)
            {
                summary.Add(
                    new
                    {
                        Key = T("Checkout.CheckoutCart.Certificate"),
                        Value = string.Format("-{0}<a class=\"cross\" data-cart-remove-cert=\"true\" title=\"{1}\"></a>",
                                  ShoppingCartService.CurrentShoppingCart.Certificate.Sum.FormatPrice(), 
                                  T("Checkout.CheckoutCart.DeleteCertificate"))
                    });
            }
            if (ShoppingCartService.CurrentShoppingCart.Coupon == null) 
                return Json(summary);
            if (totalDiscount == 0)
            {
                summary.Add(
                    new
                    {
                        Key = T("Checkout.CheckoutCart.Coupon"),
                        Value = string.Format("-{0} ({1}) <img src='images/question_mark.png' title='{3}'> <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                            0F.FormatPrice(), ShoppingCartService.CurrentShoppingCart.Coupon.Code,
                            T("Checkout.CheckoutCart.DeleteCoupon"),
                            T("Checkout.CheckoutCart.CouponNotApplied"))
                    });
            }
            else
            {
                switch (ShoppingCartService.CurrentShoppingCart.Coupon.Type)
                {
                    case CouponType.Fixed:
                        summary.Add(new
                        {
                            Key = T("Checkout.CheckoutCart.Coupon"),
                            Value = string.Format("-{0} ({1}) <a class=\"cross\" data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                totalDiscount.FormatPrice(), ShoppingCartService.CurrentShoppingCart.Coupon.Code,
                                T("Checkout.CheckoutCart.DeleteCoupon"))
                        });
                        break;
                    case CouponType.Percent:
                        summary.Add(new
                        {
                            Key = T("Checkout.CheckoutCart.Coupon"),
                            Value = string.Format("-{0} ({1}%) ({2}) <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{3}\"></a>",
                                totalDiscount.FormatPrice(),
                                ShoppingCartService.CurrentShoppingCart.Coupon.Value.FormatPrice(),
                                ShoppingCartService.CurrentShoppingCart.Coupon.Code, T("Checkout.CheckoutCart.DeleteCoupon"))
                        });
                        break;
                }
            }
            return Json(summary);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CouponPost(string code)
        {
            try
            {
                var result = new AddCoupon(code).Execute();
                return Json(new { result = result.Result, msg = result.Message }); 
            }
            catch (BlException e)
            {
                return Json(new { result = false, msg = e.Message });
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCoupon()
        {
            new RemoveCoupon().Execute();
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCertificate()
        {
            var cer = GiftCertificateService.GetCustomerCertificate();
            if (cer != null)
                GiftCertificateService.DeleteCustomerCertificate(cer.CertificateId);
            
            return Json(true);
        }
    }
}