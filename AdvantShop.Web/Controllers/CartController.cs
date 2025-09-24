using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Customers;
using AdvantShop.Handlers.Cart;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Cart;
using AdvantShop.ViewModel.Common;
using AdvantShop.Web.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Models.Cart;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Controllers;
using System.Web;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class CartController : BaseClientController
    {
        public ActionResult Index(string products, string coupon)
        {
            if (!string.IsNullOrWhiteSpace(products))
                ShoppingCartService.AddShoppingCartItems(products);

            if (!string.IsNullOrWhiteSpace(coupon))
            {
                var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

                if (customerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup)
                {
                    var userCoupon = CouponService.GetCouponByCode(coupon);
                    if (userCoupon != null && 
                        userCoupon.CustomerGroupIds.Contains(customerGroup.CustomerGroupId) && 
                        CouponService.CanApplyCustomerCoupon(userCoupon) &&
                        GiftCertificateService.GetCustomerCertificate() == null &&
                        !userCoupon.OnlyInMobileApp)
                    {
                        CouponService.AddCustomerCoupon(userCoupon.CouponID);
                    }
                }
            }

            var shpCart = ShoppingCartService.CurrentShoppingCart;

            var model = new CartViewModel()
            {
                Cart = shpCart,
                ShowConfirmButton = true,
                PhotoWidth = SettingsPictureSize.XSmallProductImageWidth
            };

            foreach (var module in AttachedModules.GetModules<IShoppingCart>())
            {
                var moduleObject = (IShoppingCart)Activator.CreateInstance(module, null);
                model.ShowConfirmButton &= moduleObject.ShowConfirmButtons;
            }

            model.ShowBuyOneClick = model.ShowConfirmButton && SettingsCheckout.BuyInOneClick;

            SetMetaInformation(T("Cart.Index.ShoppingCart"));
            SetNoFollowNoIndex();

            var tagManager = GoogleTagManagerContext.Current;
            if (tagManager.Enabled)
            {
                tagManager.PageType = ePageType.cart;
                tagManager.ProdIds = shpCart.Select(item => item.Offer.ArtNo).ToList();
                tagManager.Products = shpCart.Select(x => new TransactionProduct()
                {
                    Id = x.Offer.OfferId.ToString(),
                    SKU = x.Offer.ArtNo,
                    Category = x.Offer.Product.MainCategory != null ? x.Offer.Product.MainCategory.Name : string.Empty,
                    Name = x.Offer.Product.Name,
                    Price = x.Price,
                    Quantity = x.Amount
                }).ToList();
                tagManager.TotalValue = shpCart.TotalPrice;
            }

            WriteLog("", Url.AbsoluteRouteUrl("Cart"), ePageType.cart);

            SetNgController(NgControllers.NgControllersTypes.CartPageCtrl);

            return View(model);
        }


        [ChildActionOnly]
        public ActionResult ShoppingCart()
        {
            var itemsAmount = ShoppingCartService.CurrentShoppingCart.TotalItems;
            var amount = string.Format("{0} {1}", itemsAmount == 0 ? "" : itemsAmount.ToString(),
                                  Strings.Numerals(itemsAmount,
                                    T("Cart.Product0"), T("Cart.Product1"), T("Cart.Product2"), T("Cart.Product5")));
            var totalPrice = ShoppingCartService.CurrentShoppingCart.TotalPrice.FormatPrice();

            return PartialView("ShoppingCart", new ShoppingCartViewModel()
            {
                Amount = amount, 
                TotalItems = itemsAmount,
                TotalPrice = totalPrice,
                ShowPriceInMiniCart = SettingsDesign.ShowPriceInMiniCart,
                EnableShoppingCartPopup = SettingsDesign.EnableShoppingCartPopup
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCart()
        {
            return Json(new GetCartHandler().Get());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddToCart(CartItemAddingModel item)
        {
            return Json(new AddToCart(item, this).Execute());
        }

        public JsonResult AddCartItems(List<CartItemAddingModel> items, int payment = 0)
        {
            if (!items.Any())
                return Json(new { status = "fail" });

            foreach (var item in items)
            {
                AddToCart(item);
            }
            var cart = ShoppingCartService.CurrentShoppingCart;
            var mainCartItem = cart.FirstOrDefault(x => x.OfferId == items[0].OfferId) ?? cart.FirstOrDefault();
            return Json(new { cart.TotalItems, status = "success", cartId = mainCartItem != null ? mainCartItem.ShoppingCartItemId : 0 });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCart(Dictionary<int, float> items)
        {
            if (items == null)
                return Json(new { status = "fail" });

            var cart = ShoppingCartService.CurrentShoppingCart;

            foreach (var pair in items)
            {
                var cartItem = cart.Find(x => x.ShoppingCartItemId == pair.Key);

                if (cartItem == null || pair.Value <= 0)
                    return Json(new { status = "fail" });
                
                if (cartItem.FrozenAmount)
                    continue;

                if (ShoppingCartService.ShoppingCartHasProductsWithGifts())
                    ProductGiftService.UpdateGiftByOfferToCart(cart, cartItem.Offer, items);

                cartItem.Amount = pair.Value;
                ShoppingCartService.UpdateShoppingCartItem(cartItem);
            }

            return Json(new { ShoppingCartService.CurrentShoppingCart.TotalItems, status = "success" });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveFromCart(int itemId)
        {
            if (itemId == 0)
                return Json(new { status = "fail" });

            var cart = ShoppingCartService.CurrentShoppingCart;

            var cartItem = cart.Find(item => item.ShoppingCartItemId == itemId);
            if (cartItem != null)
            {
                if (cart.Any(x => x.IsGift))
                    ProductGiftService.RemoveGiftByOfferToCart(cart, itemId, cartItem.Offer);

                ShoppingCartService.DeleteShoppingCartItem(cartItem);
            }

            return Json(new
            {
                ShoppingCartService.CurrentShoppingCart.TotalItems,
                status = "success",
                offerId = cartItem != null ? cartItem.OfferId : 0
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ClearCart()
        {
            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

            // при полной очистке корзины удаляем примененый купон 04 12 18
            var coupon = CouponService.GetCustomerCoupon();
            if (coupon != null)
                CouponService.DeleteCustomerCoupon(coupon.CouponID);

            return Json(new { ShoppingCartService.CurrentShoppingCart.TotalItems, status = "success" });
        }

        [HttpGet]
        public JsonResult GetPriceAmountNextDiscountItems()
        {
            return Json(new GetPriceAmountNextDiscountItems().Execute());
        }

        [HttpGet]
        public JsonResult GetUrlShareCart()
        {
            var shareCart = new ShareShoppingCart();
            foreach (var cartItem in ShoppingCartService.CurrentShoppingCart)
            {
                if (cartItem.IsGift || cartItem.IsByCoupon || cartItem.IsForbiddenChangeAmount || cartItem.ModuleKey.IsNotEmpty())
                    continue;
                shareCart.Add(new ShareShoppingCartItem
                {
                    Amount = cartItem.Amount,
                    OfferId = cartItem.OfferId,
                    AttributesXml = cartItem.AttributesXml,
                    CustomerId = cartItem.CustomerId
                });
            }
            if (shareCart.Count == 0)
                return Json(false);

            var key = ShareShoppingCartService.AddShareShoppingCart(shareCart);
            if (key.IsNullOrEmpty())
                return Json(false);
            return Json(UrlService.GetUrl("cart/share/" + key));
        }

        public ActionResult Buy(string offerArtNo)
        {
            offerArtNo = HttpUtility.UrlDecode(offerArtNo);
            var offer = OfferService.GetOffer(offerArtNo);
            if (offer == null)
                return Error404();

            if (SettingsCheckout.ClearShoppingCartBeforeBuyByLink)
                ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
            {
                OfferId = offer.OfferId
            });
            if (LandingHelper.IsLandingDomain(HttpContext.Request.Url, out int lpId))
                return RedirectToAction("Lp", "Checkout", new { lpId });
            else
                return RedirectToAction("Index", "Checkout");
        }

        public ActionResult Share(string key)
        {
            var coupon = CouponService.GetCustomerCoupon();
            if (coupon != null)
                CouponService.DeleteCustomerCoupon(coupon.CouponID);

            var customer = CustomerContext.CurrentCustomer;
            var cartItems = ShareShoppingCartService.GetShareShoppingCartItems(key);
            foreach (var cartItem in cartItems)
            {
                var existsItem = ShoppingCartService.GetExistsShoppingCartItem(customer.Id, cartItem.OfferId, cartItem.AttributesXml, ShoppingCartType.ShoppingCart, false, null, false);
                if (existsItem == null)
                    ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                    {
                        OfferId = cartItem.OfferId,
                        AttributesXml = cartItem.AttributesXml,
                        Amount = cartItem.Amount
                    });
            }
            return RedirectToAction("Index", "Cart");
        }
    }
}