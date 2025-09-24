using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Domain.Trackers.Google;
using AdvantShop.App.Landing.Domain.Trackers.YandexMetrika;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.SEO;

namespace AdvantShop.App.Landing.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class LandingTrackingController : LandingBaseController
    {
        #region Yandex metrika 

        [ChildActionOnly]
        public ActionResult YandexMetrikaScript()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();

            var script = new YaMetrikaService().YaMetrikaHeadScript(LSiteSettings.YandexCounterId, LSiteSettings.YandexCounterHtml);
            return Content(script);
        }

        [ChildActionOnly]
        public ActionResult YandexMetrikaCheckoutFinalStep(IOrder order)
        {
            var script = new YaMetrikaService().CheckoutFinalStepScript(LSiteSettings.YandexCounterId, LSiteSettings.YandexCounterHtml, order);
            return Content(script);
        }


        [HttpGet]
        public JsonResult GetProductByOfferId(int offerId)
        {
            if (offerId != 0)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer != null)
                    return Json(new { artno = offer.ArtNo, name = offer.Product.Name });
            }
            return Json(new { artno = "", name = "" });
        }

        [HttpGet]
        public JsonResult GetProductById(int productId, int offerId, int? cartId)
        {
            var cartItem = ShoppingCartService.CurrentShoppingCart.FirstOrDefault(x => x.ShoppingCartItemId == cartId);
            if (cartItem == null)
                return Json(new { artno = "", name = "" });

            if (offerId != 0)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer != null)
                    return YaMetrikaEcommerceOnProductAdd(offer.Product, offer, cartItem.PriceWithDiscount);
            }
            if (productId != 0)
            {
                var product = ProductService.GetProduct(productId);
                if (product != null && product.Offers.Count > 0)
                    return YaMetrikaEcommerceOnProductAdd(product, product.Offers[0], cartItem.PriceWithDiscount);
            }

            return Json(new {artno = "", name = ""});
        }

        private JsonResult YaMetrikaEcommerceOnProductAdd(Product product, Offer offer, float? price)
        {
            int? prices = (int?)price;
            if (prices == null && offer != null)
                prices = (int)offer.RoundedPrice;

            var brands = product.Brand != null ? product.Brand.Name : null;

            var categories = product.ProductCategories;
            if (categories.Count > 5)
                categories = categories.Skip(categories.Count - 5).ToList();

            var category = String.Join("/", categories.Select(x => x.Name));

            return Json(new
            {
                artno = offer.ArtNo,
                name = offer.Product.Name,
                price = prices,
                brand = brands,
                category = category,
                amount = offer.Product.GetMinAmount()
            });
        }

        #endregion

        #region GA

        [ChildActionOnly]
        public ActionResult GaScript()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();

            return Content(new GoogleAnalyticsString(LSiteSettings.GoogleCounterId, true).GetGoogleAnalyticsString());
        }

        [ChildActionOnly]
        public ActionResult GaCheckoutFinalStep(int orderId)
        {
            if (string.IsNullOrEmpty(LSiteSettings.GoogleCounterId))
                return new EmptyResult();
            
            var order = OrderService.GetOrder(orderId);

            return Content(new GoogleAnalyticsString(LSiteSettings.GoogleCounterId, true).GetForOrder(order, LpService.CurrentLanding.Name));
        }

        #endregion

        #region GTM

        [ChildActionOnly]
        public ActionResult GtmScript()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();

            var gtm = LpGtmContext.Current;
            if (gtm == null)
                return new EmptyResult();

            if (gtm.PageType == ePageType.other)
                gtm.PageType = ePageType.home;

            return Content(gtm.RenderCounter());
        }

        [ChildActionOnly]
        public ActionResult GtmCheckoutFinalStep(int orderId)
        {
            var gtm = LpGtmContext.Current;
            if (gtm == null)
                return new EmptyResult();

            var cart = ShoppingCartService.CurrentShoppingCart;

            gtm.PageType = ePageType.order;
            gtm.ProdIds = cart.Select(item => item.Offer.ArtNo).ToList();
            gtm.Products = cart.Select(x => new TransactionProduct()
            {
                SKU = x.Offer.ArtNo,
                Category = x.Offer.Product.MainCategory != null ? x.Offer.Product.MainCategory.Name : "",
                Name = x.Offer.Product.Name,
                Price = x.Price,
                Quantity = x.Amount
            }).ToList();
            gtm.TotalValue = cart.TotalPrice;
            
            return Content(gtm.RenderCounter());
        }

        #endregion
    }
}
