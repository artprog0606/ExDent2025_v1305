using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Handlers.Catalog;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Wishlist;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class WishlistController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index()
        {
            if (!SettingsDesign.WishListVisibility)
                RedirectToRoute("Home");

            var model = new WishListHandler().Get();

            SetMetaInformation(T("Wishlist.Index.WishListHeader"));
            SetNoFollowNoIndex();

            SetNgController(NgControllers.NgControllersTypes.WishlistPageCtrl);

            return View(model);
        }

        public ActionResult Wishlist()
        {
            var model = new WishListHandler().Get();
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult WishListBlock(WishListBlockViewModel wishlistModel)
        {
            if (!SettingsDesign.WishListVisibility)
                return new EmptyResult();

            return PartialView(wishlistModel);
        }

        public JsonResult WishListAddToBasket(int offerId)
        {
            var wishListItem = ShoppingCartService.CurrentWishlist.Find(item => item.OfferId == offerId);
            if (wishListItem != null)
            {
                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                {
                    OfferId = wishListItem.OfferId,
                    Amount = 1,
                    ShoppingCartType = ShoppingCartType.ShoppingCart,
                    AttributesXml = wishListItem.AttributesXml,
                });
                return Json(new { result = "success" });
            }
            return Json(new { result = "error" });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult WishListRemove(int offerId)
        {
            var wishListItem = ShoppingCartService.CurrentWishlist.Find(item => item.OfferId == offerId);

            if (wishListItem != null)
                ShoppingCartService.DeleteShoppingCartItem(wishListItem);
            
            var wishCount = ShoppingCartService.CurrentWishlist.Count;
            var wishlistCount = string.Format("{0} {1}",
              wishCount == 0 ? "" : wishCount.ToString(CultureInfo.InvariantCulture),
              Strings.Numerals(wishCount,
                  T("Common.TopPanel.WishList0"),
                  T("Common.TopPanel.WishList1"),
                  T("Common.TopPanel.WishList2"),
                  T("Common.TopPanel.WishList5")));

            return Json(new { Count = wishCount, CountString = wishlistCount });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult WishlistAdd(int offerId)
        {
            if (!SettingsDesign.WishListVisibility)
                return Json(new { Count = 0, CountString = T("Common.TopPanel.WishList0") });

            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return Json(new { Count = 0, CountString = T("Common.TopPanel.WishList0") });
            
            var item = new ShoppingCartItem
            {
                OfferId = offer.OfferId,
                ShoppingCartType = ShoppingCartType.Wishlist
            };

            var itemId = ShoppingCartService.AddShoppingCartItem(item);

            int wishCount = ShoppingCartService.CurrentWishlist.Count;
            string wishlistCount = string.Format("{0} {1}",
                wishCount == 0 ? "" : wishCount.ToString(CultureInfo.InvariantCulture),
                Strings.Numerals(wishCount,
                    T("Common.TopPanel.WishList0"),
                    T("Common.TopPanel.WishList1"),
                    T("Common.TopPanel.WishList2"),
                    T("Common.TopPanel.WishList5")));

            ModulesExecuter.AddToCompare(item, Url.AbsoluteRouteUrl("Product", new { url = offer.Product.UrlPath }));
            return Json(new { Count = wishCount, CountString = wishlistCount });
        }

        public JsonResult GetStatus(int offerId)
        {
            var result = false;

            if (offerId != 0)
                result = ShoppingCartService.CurrentWishlist.Any(item => item.OfferId == offerId);

            return Json(result);
        }
    }
}