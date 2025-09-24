using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.ViewModel.PreOrder;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Controllers
{
    public partial class PreOrderController : BaseClientController
    {
        public ActionResult Index(int offerId)
        {
            var product = ProductService.GetProductByOfferId(offerId);
            if (product == null || product.UrlPath.IsNullOrEmpty())
                return Error404();
            return RedirectToRoutePermanent("Product", new { url = product.UrlPath } );
        }

        public ActionResult Success(bool? isLanding)
        {
            SetMetaInformation(T("PreOrder.Success.Title"));
            SetNoFollowNoIndex(); 
            return isLanding is true ? View("Success", "_LayoutEmpty") : View("Success");
        }

        public ActionResult LinkByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return RedirectToRoute("Home");

            SetMetaInformation(T("PreOrder.LinkByCode.Header"));
            SetMobileTitle(T("PreOrder.Index.MobileTitle"));
            SetNoFollowNoIndex();

            var model = new LinkByCodeViewModel();

            // Если код правильный, и такого же товара нет в корзине - то всё ок.
            var orderByRequest = OrderByRequestService.GetOrderByRequest(code);
            if (orderByRequest == null)
            {
                model.Error = T("PreOrder.Index.OrderProductMessage");
                return View(model);
            }

            if (ShoppingCartService.CurrentShoppingCart.Any(p => p.Offer.OfferId == orderByRequest.OfferId))
            {
                var item = ShoppingCartService.CurrentShoppingCart.First(p => p.Offer.OfferId == orderByRequest.OfferId);
                if (item.Amount != orderByRequest.Quantity)
                {
                    item.Amount = orderByRequest.Quantity;
                    ShoppingCartService.UpdateShoppingCartItem(item);
                }
                return RedirectToRoute("Cart");
            }

            var offer = OfferService.GetOffer(orderByRequest.OfferId);
            if (orderByRequest.IsValidCode && ProductService.IsExists(orderByRequest.ProductId) && offer != null && offer.BasePrice > 0)
            {
                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                {
                    OfferId = orderByRequest.OfferId,
                    Amount = orderByRequest.Quantity,
                    ShoppingCartType = ShoppingCartType.ShoppingCart,
                    AttributesXml = orderByRequest.Options,
                    AddedByRequest = orderByRequest.Quantity > offer.Amount
                });

                return RedirectToRoute("Cart");
            }
            model.Error = T("PreOrder.Index.OrderProductMessage");

            return View(model);
        }
    }
}