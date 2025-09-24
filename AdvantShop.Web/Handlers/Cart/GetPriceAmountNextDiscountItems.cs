using System.Collections.Generic;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Models.Cart;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Handlers.Cart
{
    public class GetPriceAmountNextDiscountItems
    {
        private readonly UrlHelper _urlHelper;
        
        public GetPriceAmountNextDiscountItems()
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }
        
        public List<PriceAmountNextDiscountItem> Execute()
        {
            var customer = CustomerContext.CurrentCustomer;

            var result = new List<PriceAmountNextDiscountItem>();
            
            foreach (var cartItem in ShoppingCartService.CurrentShoppingCart)
            {
                if (cartItem.IsGift || cartItem.FrozenAmount)
                    continue;
                
                var rule = PriceRuleService.GetNextPriceRule(cartItem.OfferId, cartItem.Amount, customer.CustomerGroupId);
                if (rule == null)
                    continue;

                var product = cartItem.Offer.Product;
                var offer = OfferService.GetOffer(cartItem.OfferId);

                var (oldPrice, finalPrice, finalDiscount, preparedPrice) = 
                    offer.GetOfferPricesWithPriceRule(rule, cartItem.AttributesXml, customer, null);

                result.Add(new PriceAmountNextDiscountItem()
                {
                    Url = _urlHelper.AbsoluteRouteUrl("Product", new {url = product.UrlPath}) + GetUrlPrefix(offer),
                    Name = product.Name,
                    ColorName = offer.Color?.ColorName,
                    SizeName = offer.Size?.SizeName,
                    Amount = rule.Amount,
                    Unit = product.Unit?.DisplayName ?? product.Unit?.Name ?? "шт",
                    Price = finalPrice.FormatPrice(),
                    Discount = finalDiscount.HasValue
                        ? finalDiscount.Type == DiscountType.Amount
                            ? finalDiscount.Amount.FormatPrice()
                            : finalDiscount.Percent + "%"
                        : ""
                });
            }

            return result;
        }

        private string GetUrlPrefix(Offer offer)
        {
            var result = "";

            if (offer.ColorID != null)
                result += "?color=" + offer.ColorID;
            
            if (offer.SizeID != null)
                result += (result != "" ? "&" : "?") +"size=" + offer.SizeID;
            
            return result;
        }
    }
}